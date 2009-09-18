using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    class ScrollingBackground : Microsoft.Xna.Framework.DrawableGameComponent
    {
        //variables
        SpriteBatch spriteBatch;
        Texture2D background;
        Rectangle position1;
        Rectangle position2;
        Rectangle position3;

        public ScrollingBackground(Game game)
            : base(game)
        { 
        }
        public override void Initialize()
        {
            //get the initial position for scrolling divs
            position1 = new Rectangle(0, 0, Game.Window.ClientBounds.Width,Game.Window.ClientBounds.Height);
            position2 = new Rectangle(Game.Window.ClientBounds.Width, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height);
            position3 = new Rectangle(Game.Window.ClientBounds.Width*2, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height);            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            //load in the scrolling background image
            background = Game.Content.Load<Texture2D>(@"Images/movingstars");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // decrement the position with 1;
            position1.X -= 1;
            position2.X -= 1;
            position3.X -= 1;
            

            //move the background back to behind the last background to make it look continuous
            if (position2.X == 0)
                position1.X = Game.Window.ClientBounds.Right * 2 - 1000;
            

            if (position3.X == 0)
                position2.X = Game.Window.ClientBounds.Right * 2 - 1000;
            

            if (position1.X == 0)
                position3.X = Game.Window.ClientBounds.Right * 2 - 1000;
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.FrontToBack, SaveStateMode.None);
                //draw backgrounds
                spriteBatch.Draw(background, position1, Color.White);
                spriteBatch.Draw(background, position2, Color.White);
                spriteBatch.Draw(background, position3, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
