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
    public class spritemanager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        //variables
        SpriteBatch spriteBatch;
        usership player;
        List<Sprite> spriteList = new List<Sprite>();
        List<Sprite> bulletList = new List<Sprite>();
        List<Sprite> enemyBulletList = new List<Sprite>();
        List<Sprite> powerUpList = new List<Sprite>();
        
        //enemy variables
        int enemySpawnMinMilliseconds = 2000;
        int enemySpawnMaxMilliseconds = 4000;
        int enemyMinSpeed = 2;
        int enemyMaxSpeed = 4;
        int enemyNumber = 0;

        //timing variables
        int nextSpawnTime = 0;
        int playerNextBulletTime = 0;
        int enemyNextBulletTime = 0;
        int staticEnemyNextBulletTime = 1500;

        //powerup time
        public int powerupTime = 0;

        public spritemanager(Game game)
            : base(game)
        {
        }
        
        public override void Initialize()
        {

            //initialise spawntime
            ResetSpawnTime();

            //initialise bullet time for players and enemies
            PlayerResetBulletTime();
            EnemyResetBulletTime();
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            // check for levelup
            levelLogic();

            // hittest logic etc
            updateSprites(gameTime);

            // spawn new enemies
            nextSpawnTime -= gameTime.ElapsedGameTime.Milliseconds;
            if (nextSpawnTime < 0)
            {
                SpawnEnemy();
                ResetSpawnTime();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.FrontToBack, SaveStateMode.None);
                // Draw the player
                player.Draw(gameTime, spriteBatch);

                // Draw all ships
                foreach (Sprite s in spriteList)
                {
                    s.Draw(gameTime, spriteBatch);
                }
                // Draw all player bullets
                foreach (Sprite s in bulletList)
                {
                    s.Draw(gameTime, spriteBatch);
                }
                // Draw all enemy bullets
                foreach (Sprite s in enemyBulletList)
                {
                    s.Draw(gameTime, spriteBatch);
                }
                // Draw all powerups
                foreach (Sprite s in powerUpList)
                {
                    s.Draw(gameTime, spriteBatch);
                }
            spriteBatch.End();
            
            base.Draw(gameTime);

        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            // load the player
            player = new usership(Game.Content.Load<Texture2D>(@"Images/player"),new Vector2(40,(Game.GraphicsDevice.Viewport.Height/2)-37), new Point(75, 75), 15, new Point(0, 0),new Point(6, 2), new Vector2(6, 6),0);
            
            base.LoadContent();
        }
        public void UnloadShips()
        {
            //on game over, get rid of all enemies, bullets and powerups
            spriteList.Clear();
            bulletList.Clear();
            enemyBulletList.Clear();
            powerUpList.Clear();
        }
        protected void updateSprites(GameTime gameTime)
        {

            // Update player
            player.Update(gameTime, Game.Window.ClientBounds);
            if (player.position.Y > Game.Window.ClientBounds.Height-121) // don't hit the moon!
                livesLogic(-1);

            // update player bullettimer
            playerShootBullet(gameTime, player.position);

            //update enemy bullets, check for collision with player
            updateEnemyBullets(gameTime);

            // update ship and check for collision with enemies
            updateShip(gameTime);

            // update player bullet/enemy ship collision
            PlayerBulletCollision(gameTime);

            // update powerups;
            updatePowerUps(gameTime);
        }

        // SPAWNING ENEMIES
        private void ResetSpawnTime()
        {
            // get a random spawn time
            nextSpawnTime = ((Game1)Game).rnd.Next(
                enemySpawnMinMilliseconds,
                enemySpawnMaxMilliseconds);
        }
        private void SpawnEnemy()
        {
            //count all enemies for the levelling
            enemyNumber += 3;

            // get variables for positions
            Vector2 speed = Vector2.Zero;
            Vector2 position = Vector2.Zero;
            Point frameSize = new Point(75, 75);
            

            //random enemy placement and speed
            position = new Vector2(
                Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                ((Game1)Game).rnd.Next(60,Game.GraphicsDevice.PresentationParameters.BackBufferHeight - (frameSize.Y*3)-45));// -45 to make sure they don't float over the moon

            speed = new Vector2(-((Game1)Game).rnd.Next(enemyMinSpeed, enemyMaxSpeed), 0); //random speed

            //add a group of three enemies
            if (((Game1)Game).rnd.Next(0, 20) < 1*((Game1)Game).currentLevel) // sometimes you get an enemy that shoots two bullets. The chance increases with the levels
            {
                spriteList.Add(
                    new movingSprite(Game.Content.Load<Texture2D>(@"images\enemy2"),
                    position, new Point(75, 75), 15, new Point(0, 0), new Point(6, 2), speed, 200));
                spriteList.Add(
                     new movingSprite(Game.Content.Load<Texture2D>(@"images\enemy2"),
                        new Vector2(position.X - (frameSize.X / 2), position.Y + frameSize.Y), new Point(75, 75), 15, new Point(0, 0), new Point(6, 2), speed, 200));
                spriteList.Add(
                     new movingSprite(Game.Content.Load<Texture2D>(@"images\enemy2"),
                        new Vector2(position.X, position.Y + frameSize.Y * 2), new Point(75, 75), 15, new Point(0, 0), new Point(6, 2), speed, 200));
            }
            else
            {
                spriteList.Add(
                    new movingSprite(Game.Content.Load<Texture2D>(@"images\enemy"),
                    position, new Point(75, 75), 15, new Point(0, 0), new Point(6, 2), speed, 100));
                spriteList.Add(
                     new movingSprite(Game.Content.Load<Texture2D>(@"images\enemy"),
                        new Vector2(position.X - (frameSize.X / 2), position.Y + frameSize.Y), new Point(75, 75), 15, new Point(0, 0), new Point(6, 2), speed, 100));
                spriteList.Add(
                     new movingSprite(Game.Content.Load<Texture2D>(@"images\enemy"),
                         new Vector2(position.X, position.Y + frameSize.Y*2), new Point(75, 75), 15, new Point(0, 0), new Point(6, 2), speed, 100));
            }


        }


        // GAME LOGIC
        private void levelLogic()
        {
            // you need to beat 15*level enemies per level to progress (seamless) to the next level
            // spawn time and enemy speed keep increasing
            if (enemyNumber > ((Game1)Game).currentLevel * 15)
            {
                //reset enemy number, proceed to next level, reset lives
                enemyNumber = 0;
                ((Game1)Game).currentLevel += 1;
                ((Game1)Game).currentLives = 3;

                // make enemies faster early in the game, then slower on higher levels, then remain static once they spawn every half second
                // make them shoot more often as well
                if (enemySpawnMaxMilliseconds > 500)
                {
                    if (enemySpawnMaxMilliseconds > 1000)
                    {
                        enemySpawnMaxMilliseconds -= 100;
                        enemySpawnMinMilliseconds -= 100;
                        staticEnemyNextBulletTime -= 50;
                    }
                    else
                    {
                        enemySpawnMaxMilliseconds -= 10;
                        enemySpawnMinMilliseconds -= 10;
                        staticEnemyNextBulletTime -= 5;
                    }
                }
                enemyMaxSpeed += 1;
            }
        }
        public void livesLogic(int change)
        {
            // if the player gets killed, substract a live and move the player back to the initial position
            ((Game1)Game).currentLives += change;
            Vector2 newPosition = new Vector2(40, (Game.GraphicsDevice.Viewport.Height / 2) - 37);
            player.position = newPosition;
        }

        // SHOOTING, PLAYER
        private void playerShootBullet(GameTime gameTime, Vector2 position) 
        {
            // substract current time
            playerNextBulletTime -= gameTime.ElapsedGameTime.Milliseconds;

            // only shoot when allowed and space is pressed
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && playerNextBulletTime < 0)
            {
                Vector2 speed = new Vector2(4, 0);
                Point frameSize = new Point(75, 75);

                //if you have a powerup, you shoot two bullets at once
                if (powerupTime > 0)
                {
                    bulletList.Add(
                        new movingSprite(Game.Content.Load<Texture2D>(@"images\playerbullet"),
                        new Vector2(position.X, position.Y-20), new Point(75, 75), 20, new Point(0, 0), new Point(6, 2), speed, 10));
                    bulletList.Add(
                        new movingSprite(Game.Content.Load<Texture2D>(@"images\playerbullet"),
                        new Vector2(position.X, position.Y+20), new Point(75, 75), 20, new Point(0, 0), new Point(6, 2), speed, 10));
                }
                else //you shoot one bullet
                {
                    bulletList.Add(
                    new movingSprite(Game.Content.Load<Texture2D>(@"images\playerbullet"),
                    new Vector2(position.X, position.Y), new Point(75, 75), 20, new Point(0, 0), new Point(6, 2), speed, 10));
                }
                //reset the timer
                PlayerResetBulletTime();
            }
        }

        private void PlayerResetBulletTime()
        {
            // 1/4th of a second between bullets
            playerNextBulletTime = 250;
        }

        // SHOOTING, ENEMY
        private void enemyShootBullet(GameTime gameTime, Vector2 position, Vector2 speed, int scoreValue)
        {
            //randomise chance of enemy shooting
            int random = ((Game1)Game).rnd.Next(0, 100);
            if (random < 30)
            {
                //make it move a little faster than the ship thats shooting
                speed.X -= 2;

                int horizontalSpace = (int)position.X - (int)player.position.X;
                int verticalSpace = (int)position.Y - (int)player.position.Y;

                // only aim if player is in front of the ship
                if (horizontalSpace > verticalSpace)
                {
                    //aim at player
                    int horizontalSteps = (int)horizontalSpace/(int)speed.X;
                    int verticalSteps = (int)verticalSpace;

                    speed.Y = (float)verticalSteps / (float)horizontalSteps;
                    // ...but not if the bullet would go too fast, then just shoot forward
                    if (speed.Y > 4 || speed.Y < -4)
                        speed.Y = 0;
                }

                Point frameSize = new Point(75, 75);
                // if it's a stronger type of enemy, it shoots two bullets
                if (scoreValue > 100)
                {
                    enemyBulletList.Add(
                        new movingSprite(Game.Content.Load<Texture2D>(@"images\enemybullet"),
                        new Vector2(position.X, position.Y-20), new Point(75, 75), 20, new Point(0, 0), new Point(6, 2), speed, 10));
                    enemyBulletList.Add(
                        new movingSprite(Game.Content.Load<Texture2D>(@"images\enemybullet"),
                        new Vector2(position.X, position.Y+20), new Point(75, 75), 20, new Point(0, 0), new Point(6, 2), speed, 10));
                }
                else // else just one
                {
                    enemyBulletList.Add(
                        new movingSprite(Game.Content.Load<Texture2D>(@"images\enemybullet"),
                        new Vector2(position.X, position.Y), new Point(75, 75), 20, new Point(0, 0), new Point(6, 2), speed, 10));
                }


            }
        }
        private void EnemyResetBulletTime()
        {
            // reset enemy bullet time
            enemyNextBulletTime = staticEnemyNextBulletTime;
        }
 
        // COLLISION
        private void updateShip(GameTime gameTime)
        {

            //substract time for bullet
            enemyNextBulletTime -= gameTime.ElapsedGameTime.Milliseconds;

            // Update all enemy ships
            for (int i = 0; i < spriteList.Count; ++i)
            {
                Sprite s = spriteList[i];
                s.Update(gameTime, Game.Window.ClientBounds);
                
                if (enemyNextBulletTime < 0)
                {
                    //shoot bullet when possible
                    enemyShootBullet(gameTime, s.position, s.speed, s.scoreValue);
                }

                // Check for collisions or out of bound and remove enemy
                if (s.collisionRect.Intersects(player.collisionRect) || s.IsOutOfBounds(Game.Window.ClientBounds))
                {
                    if (s.collisionRect.Intersects(player.collisionRect))
                    {
                        // update score
                        ((Game1)Game).addScore(spriteList[i].scoreValue);
                        // substract live
                        livesLogic(-1);
                    }

                    //remove enemy ship
                    spriteList.RemoveAt(i);
                    --i;
                }
            }
            if (enemyNextBulletTime < 0)
                EnemyResetBulletTime();
        }

        private void PlayerBulletCollision(GameTime gameTime)
        {
            // Update all player bullets
            for (int i = 0; i < bulletList.Count; ++i)
            {
                Sprite b = bulletList[i];
                b.Update(gameTime, Game.Window.ClientBounds);

                // Check for out of bound and remove bullet
                if (b.IsOutOfBounds(Game.Window.ClientBounds))
                {
                    //remove bullet
                    bulletList.RemoveAt(i);
                    --i;
                }
                else
                {
                    // Check for collisions of player bullet with enemy
                    for (int j = 0; j < spriteList.Count; ++j)
                    {
                        Sprite s = spriteList[j];
                        if (b.collisionRect.Intersects(s.collisionRect))
                        {
                            // update score
                            ((Game1)Game).addScore(spriteList[j].scoreValue);

                            // drop a powerup (chance of)
                            dropPowerup(gameTime, s.position, s.speed);

                            //remove enemy ship and bullet if they're still there
                            if (i < bulletList.Count && j < spriteList.Count)
                            {
                                bulletList.RemoveAt(i);
                                --i;
                                spriteList.RemoveAt(j);
                                --j;
                            }
                        }
                    }
                }
            }
        }
        private void updateEnemyBullets(GameTime gameTime)
        {
            // Update all enemy bullets
            for (int i = 0; i < enemyBulletList.Count; ++i)
            {
                Sprite s = enemyBulletList[i];
                s.Update(gameTime, Game.Window.ClientBounds);

                // Check for collisions, substract a live and remove bullet
                if (s.collisionRect.Intersects(player.collisionRect))
                {
                    // substract live
                    livesLogic(-1);

                    //remove enemy bullet
                    enemyBulletList.RemoveAt(i);
                    --i;
                }
            }
        }
        private void updatePowerUps(GameTime gameTime)
        {
            // countdown timer for the powerup
            powerupTime -= gameTime.ElapsedGameTime.Milliseconds;
            if (powerupTime < 0) //make sure it doesnt go below zero
                powerupTime = 0;

            // Update all powerUps
            for (int i = 0; i < powerUpList.Count; ++i)
            {
                Sprite s = powerUpList[i];
                s.Update(gameTime, Game.Window.ClientBounds);

                // Check for collisions with player, then remove and activate power up
                if (s.collisionRect.Intersects(player.collisionRect))
                {
                    activatePowerUpTimeout();

                    //remove powerup
                    powerUpList.RemoveAt(i);
                    --i;
                }
            }
        }

        // POWERUPS
        private void dropPowerup(GameTime gameTime, Vector2 position, Vector2 speed)
        {
            //randomise chance of powerup falling
            int random = ((Game1)Game).rnd.Next(0, 100);
            if (random < 8)
            {
                //make it move a little slower than the ship it came from
                speed.X += 1;

                Point frameSize = new Point(75, 75);
                powerUpList.Add(new movingSprite(Game.Content.Load<Texture2D>(@"Images/powerup"),new Vector2(position.X, position.Y), new Point(75, 75), 20, new Point(0, 0), new Point(6, 2), speed,0));
            }
        }
        private void activatePowerUpTimeout()
        {
            // set powerup time to add 5 seconds;
            powerupTime += 5000;
        }
    }
}