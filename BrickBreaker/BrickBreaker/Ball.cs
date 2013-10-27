using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace BrickBreaker
{
    //need to implement gametime in position update (MoveAhead methods)
    class Ball
    {
        Velocity velocity;
        SpriteBatch spriteBatch;
        Texture2D texture;
        Rectangle boundingBox;
        Vector2 position;
        //to avoid passing gameTime in all functions. Stored as a reference
        GameTime gameTime;
        //a factor to normalize the gameTime.ElapsedGameTime.TotalMilliSeconds
        int timeDivideFactor;

        public Ball( ref SpriteBatch spriteBatch, ref Texture2D texture, Vector2 position, Vector2 velocity, int timeDivideFactor)
        {
            this.spriteBatch = spriteBatch;
            this.texture = texture;
            this.position = position;
            boundingBox = new Rectangle((int) position.X, (int) position.Y, texture.Width,  texture.Width);
            this.velocity = new Velocity(velocity);
            this.timeDivideFactor = timeDivideFactor;
        }

        public void refGameTime(ref GameTime gameTime)
        {
            this.gameTime = gameTime;
        }

        public Rectangle getBoundingBox()
        {
            return this.boundingBox;
        }

        public void Draw()
        {
            spriteBatch.Draw(texture, boundingBox, Color.White);
        }

        public Vector2 getPosition()
        {
            return position;
        }

        public void setPosition(Vector2 newPosition)
        {
            this.position = newPosition;
            boundingBox.X = (int)newPosition.X;
            boundingBox.Y = (int)newPosition.Y;
        }

        public Vector2 getVelocity()
        {
            return velocity.get();
        }

        public void setVelocity(Vector2 speed)
        {
            velocity = new Velocity(speed);
        }

        /// <summary>
        /// Move the X position of ball by a single unit. For example, if X velocity, is 8, this function will move by 1 unit only.
        /// </summary>
        private void xMoveAheadByUnit(int extraTimeDivide)
        {
            position.X += (velocity.get().X / Math.Abs(velocity.get().X)) * (float)gameTime.ElapsedGameTime.TotalMilliseconds / (timeDivideFactor*extraTimeDivide);
            boundingBox.X = (int)position.X;
        }

        /// <summary>
        /// Move the Y position of ball by a single unit. For example, if Y velocity, is 5, this function will move by 1 unit only.
        /// </summary>
        private void yMoveAheadByUnit(int extraTimeDivide)
        {
            position.Y += (velocity.get().Y / Math.Abs(velocity.get().Y)) * (float)gameTime.ElapsedGameTime.TotalMilliseconds / (timeDivideFactor*extraTimeDivide);
            boundingBox.Y = (int)position.Y;
        }

        /// <summary>
        /// Function to move the ball, while checking for collisions with bricks, slider and screen edges(gameFrame)
        /// </summary>
        /// <param name="brickManager">BrickManager to check collision with all bricks</param>
        /// <param name="gameFrame">BoundingBox of the gamescreen to detect collision with screen edges</param>
        /// <param name="slider">Slider to detect collission</param>
        /// <param name="lifelost">Boolean variable to see if the ball goes below the slider</param>
        /// <param name="hit">SoundEffect when the ball collides</param>
        /// <param name="extraTimeDivide">Used ONLY when we need to slow down movement of ball. Used in this game when the function is called by Slider</param>
        public void UpdatePosition(BricksManager brickManager, Rectangle gameFrame, Slider slider, ref bool lifelost, SoundEffectInstance hit, int extraTimeDivide = 1)
        {
            //check for collision with every unit of movement for X, then for Y
            for (int i = 0; i < Math.Abs(velocity.get().X); i++)
            {
                xMoveAheadByUnit(extraTimeDivide);
                if (slider.checkCollision(this.boundingBox) || brickManager.DetectCollision(this.boundingBox) || !gameFrame.Contains(this.boundingBox))
                {
                    hit.Play();
                    collideX(extraTimeDivide);
                }
            }

            for (int i = 0; i < Math.Abs(velocity.get().Y); i++)
            {
                yMoveAheadByUnit(extraTimeDivide);
                if (slider.checkCollision(this.boundingBox) || brickManager.DetectCollision(this.boundingBox) || !gameFrame.Contains(this.boundingBox))
                {
                    hit.Play();
                    collideY(extraTimeDivide);
                }

                //when ball goes below the slider
                if (gameFrame.Bottom - this.getPosition().Y < 50)
                {
                    lifelost = true;
                }
            } 
        }

       

        public void collideX(int extraTimeDivide)
        {
            velocity.reverseX();
            xMoveAheadByUnit(extraTimeDivide);
        }

        public void collideY(int extraTimeDivide)
        {
            velocity.reverseY();
            yMoveAheadByUnit(extraTimeDivide);
        }
    }
}
