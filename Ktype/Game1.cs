using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Ktype
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        spritemanager spriteManager;
        ScrollingBackground scrollingBackground;
        Texture2D backgroundTexture;        

        public Random rnd { get; private set; }
        int currentScore = 0;
        SpriteFont scoreFont;

        public int currentLives = 3;
        public int currentLevel = 1;

        enum GameState { Start, InGame, GameOver };
        GameState currentGameState = GameState.Start;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //new random number generator
            rnd = new Random();
            // set width and height
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1024;
        }

        protected override void Initialize()
        {
            //set up scrolling background, hide it
            scrollingBackground = new ScrollingBackground(this);
            Components.Add(scrollingBackground);
            scrollingBackground.Enabled = false;
            scrollingBackground.Visible = false;
            
            //set up all the game components, hide them
            spriteManager = new spritemanager(this);
            Components.Add(spriteManager);
            spriteManager.Enabled = false;
            spriteManager.Visible = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //load the font for the score/lives etc and the static background
            scoreFont = Content.Load<SpriteFont>(@"fonts\arial");
            backgroundTexture = Content.Load<Texture2D>(@"images\bg1");
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            // Only perform certain actions based on
            // the current game state
            switch (currentGameState)
            {
                case GameState.Start:
                    if (Keyboard.GetState().GetPressedKeys().Length > 0)
                    {
                        // start game
                        currentGameState = GameState.InGame;
                        spriteManager.Enabled = true;
                        spriteManager.Visible = true;
                        scrollingBackground.Enabled = true;
                        scrollingBackground.Visible = true;
                    }
                    break;
                case GameState.InGame:
                    
                    if (currentLives < 1) // if dead, go to game over
                    {
                        currentGameState = GameState.GameOver;
                        //kill all enemies, reset powerup time, hide game background and sprites
                        spriteManager.UnloadShips();
                        spriteManager.powerupTime = 0;
                        spriteManager.Enabled = false;
                        spriteManager.Visible = false;
                        scrollingBackground.Enabled = false;
                        scrollingBackground.Visible = false;
                    }
                    break;
                case GameState.GameOver:
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        //reset statistics, start game
                        currentLevel = 1;
                        currentScore = 0;
                        currentLives = 3;
                        currentGameState = GameState.InGame;
                        spriteManager.Enabled = true;
                        spriteManager.Visible = true;
                        scrollingBackground.Initialize();
                        scrollingBackground.Enabled = true;
                        scrollingBackground.Visible = true;
                    }
                    break;
            }

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            base.Update(gameTime);

        }
        protected override void Draw(GameTime gameTime)
        {
            switch (currentGameState)
            {
                case GameState.Start:
                    GraphicsDevice.Clear(Color.Black);
                    // Draw text for intro splash screen
                    spriteBatch.Begin();
                        string text = "Shoot anything that moves!";
                        spriteBatch.DrawString(scoreFont, text,new Vector2((Window.ClientBounds.Width / 2) - (scoreFont.MeasureString(text).X / 2), (Window.ClientBounds.Height / 2)
                            - (scoreFont.MeasureString(text).Y / 2)),
                            Color.LightGray);

                        text = "(Press any key to begin, arrows to move and space to shoot)";
                        spriteBatch.DrawString(scoreFont, text,new Vector2((Window.ClientBounds.Width / 2) - (scoreFont.MeasureString(text).X / 2), (Window.ClientBounds.Height / 2)
                            - (scoreFont.MeasureString(text).Y / 2) + 30),
                            Color.LightGray);
                    spriteBatch.End();
                    break;

                case GameState.InGame:
                    GraphicsDevice.Clear(Color.Black);
                    spriteBatch.Begin();
                        //draw background
                        spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                        //draw user statistics
                        spriteBatch.DrawString(scoreFont, "Score: " + currentScore, new Vector2(Window.ClientBounds.Width - 150, 5), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                        spriteBatch.DrawString(scoreFont, "Level: " + currentLevel, new Vector2(10, 5), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                        spriteBatch.DrawString(scoreFont, "Lives: " + currentLives, new Vector2(10, 7+scoreFont.MeasureString("Lives").Y), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                        if (spriteManager.powerupTime > 0)
                        {   
                            //time +1 so it countd down to 1 second, not zero.
                            spriteBatch.DrawString(scoreFont, "Powerup: " + ((int)spriteManager.powerupTime / 1000 + 1) + " seconds", new Vector2(Window.ClientBounds.Width - 225, 7 + scoreFont.MeasureString("Powerup").Y), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                        }
                    spriteBatch.End();
                    break;

                case GameState.GameOver:
                    GraphicsDevice.Clear(Color.Black);
                    // Draw text for game over screen
                    spriteBatch.Begin();
                        spriteBatch.DrawString(scoreFont, "Score: " + currentScore, new Vector2(Window.ClientBounds.Width - 150, 5), Color.LightGray, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                        spriteBatch.DrawString(scoreFont, "Level: " + currentLevel, new Vector2(10, 5), Color.LightGray, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                                            
                        //display game over text
                        string text2 = "Game Over";
                        spriteBatch.DrawString(scoreFont, text2, new Vector2((Window.ClientBounds.Width / 2) - (scoreFont.MeasureString(text2).X / 2), (Window.ClientBounds.Height / 2) - (scoreFont.MeasureString(text2).Y / 2)), Color.LightGray);

                        text2 = "(Press enter to start over)";
                        spriteBatch.DrawString(scoreFont, text2, new Vector2((Window.ClientBounds.Width / 2) - (scoreFont.MeasureString(text2).X / 2), (Window.ClientBounds.Height / 2) - (scoreFont.MeasureString(text2).Y / 2) + 30), Color.LightGray);
                    spriteBatch.End();
                    break;
            }
            base.Draw(gameTime);
        }

        public void addScore(int scoreValue)
        {
            // add the score of the enemy just shot
            currentScore += scoreValue;
        }

    }
}
