using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace BrickBreaker
{
    class Slider
    {
        Rectangle boundingBox;
        SpriteBatch spriteBatch;
        Texture2D texture;

        /// <summary>
        /// To initialize slider in the main game
        /// </summary>
        /// <param name="sliderTexture"></param>
        /// <param name="spriteBatch">SpriteBatch for drawing</param>
        /// <param name="position">Initial position of the slider</param>
        public Slider(ref Texture2D sliderTexture,ref SpriteBatch spriteBatch, Vector2 position)
        {
            this.texture = sliderTexture;
            this.spriteBatch = spriteBatch;
            boundingBox = new Rectangle((int) position.X, (int) position.Y, texture.Width, texture.Height);
        }

        public Vector2 getPosition()
        {
            Vector2 position;
            position.X = boundingBox.X;
            position.Y = boundingBox.Y;
            return position;
        }
        

        public void setPosition(Vector2 position)
        {
            boundingBox.X = (int)position.X;
            boundingBox.Y = (int)position.Y;
        }

        public Texture2D getTexture()
        {
            return this.texture;
        }

        public float getX()
        {
            return boundingBox.X;
        }

        public float getY()
        {
            return boundingBox.Y;
        }

        public void Draw()
        {
            spriteBatch.Draw(texture, boundingBox, Color.White);

        }

        public bool checkCollision(Rectangle ballBoundingBox)
        {
            return (boundingBox.Intersects(ballBoundingBox));
        }

        /// <summary>
        /// Move slider left on keyboard left. Also handles explicit collision detection because of a bug with slider moving
        /// </summary>
        /// <param name="x">The value by which slider will move left in a single frame</param>
        /// <param name="ball">To detect collision with ball</param>
        /// <param name="brickManager">To detect collision with brick</param>
        /// <param name="gameFrame">Detect collision with screen edges</param>
        /// <param name="lifelost">Boolean variable to store if ball went below slider and a life was lost</param>
        /// <param name="hit">Sound when collision occurs</param>
        public void MoveLeft(int x, ref Ball ball, ref BricksManager brickManager, ref Rectangle gameFrame, ref bool lifelost, SoundEffectInstance hit)
        {
            //for every unit movement of the slider to the left
            for (int i = 0; i < x; i++)
            {
                boundingBox.X--;
                ball.UpdatePosition(brickManager, gameFrame, this, ref lifelost, hit, x); //note slow down factor, since UpdatePosition is called 'x' times

                //this was an explicit bug fix. Slider used to cross over the ball
                if (checkCollision(ball.getBoundingBox()))
                {
                    boundingBox.X+=3;
                    
                }
            }
        }

        /// <summary>
        /// Move slider right on keyboard right. Also handles explicit collision detection because of a bug with slider moving
        /// </summary>
        /// <param name="x">The value by which slider will move right in a single frame</param>
        /// <param name="ball">To detect collision with ball</param>
        /// <param name="brickManager">To detect collision with brick</param>
        /// <param name="gameFrame">Detect collision with screen edges</param>
        /// <param name="lifelost">Boolean variable to store if ball went below slider and a life was lost</param>
        /// <param name="hit">Sound when collision occurs</param>
        public void MoveRight(int x, ref Ball ball, ref BricksManager brickManager, ref Rectangle gameFrame, ref bool lifelost, SoundEffectInstance hit)
        {
            //for every unit movement of the slider to the right
            for (int i = 0; i < x; i++)
            {
                boundingBox.X++;
                ball.UpdatePosition(brickManager, gameFrame, this, ref lifelost,hit,x); //note slow down factor, since UpdatePosition is called 'x' times

                //this was an explicit bug fix. Slider used to cross over the ball
                if (checkCollision(ball.getBoundingBox()))
                {
                    boundingBox.X-=3;
                }
            }
        }
    }
}
