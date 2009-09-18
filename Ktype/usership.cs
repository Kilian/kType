using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Ktype
{
    class usership : Sprite
    {

        public override Vector2 direction
        {
            get
            {
                //move the ship with the keyboard
                Vector2 inputDirection = Vector2.Zero;
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    inputDirection.X -= 1;
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    inputDirection.X += 1;
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    inputDirection.Y -= 1;
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    inputDirection.Y += 1;
                return inputDirection * speed;
            }
        }
        

        public usership(Texture2D textureImage, Vector2 position, Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed, int scoreValue)
        : base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, 0)
        {

        }
        public usership(Texture2D textureImage, Vector2 position, Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed, int millisecondsPerFrame, int scoreValue)
        : base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, millisecondsPerFrame, 0)
        {

        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            // Move the sprite according to the direction property
            position += direction;

            // If the sprite is off the screen, put it back in play
            if (position.X < 0)
                position.X = 0;
            if (position.Y < 56)
                position.Y = 56;
            if (position.X > clientBounds.Width - frameSize.X)
                position.X = clientBounds.Width - frameSize.X;
            if (position.Y > clientBounds.Height - frameSize.Y)
                position.Y = clientBounds.Height - frameSize.Y;
            base.Update(gameTime, clientBounds);
        }
    }
}
