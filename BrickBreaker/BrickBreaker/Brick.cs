using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace BrickBreaker
{
    class Brick
    {
        Texture2D texture;
        //for drawing
        SpriteBatch spriteBatch;
        //used for collision detection & position
        Rectangle boundingBox;

		///<summary>
        ///To initiliaze new Brick. Should be called insde BrickManager
		///</summary>
        ///<param name="spriteBatch">For drawing</param>
        ///<param name="texture">Brick Texture</param>
        ///<param name="vector">Position of top-left corner of brick</param>
        public Brick(ref SpriteBatch spriteBatch, ref Texture2D texture, Vector2 vector)
        {
            this.spriteBatch = spriteBatch;
            this.texture = texture;
            boundingBox = new Rectangle((int)vector.X, (int)vector.Y, texture.Width, texture.Height);
        }

		public Vector2 getPos()
        {
            Vector2 position;
            position.X = boundingBox.X;
            position.Y = boundingBox.Y;

            return position;
        }

        public Rectangle getBoundingBox()
        {
            return boundingBox;
        }
        
        public void Draw()
        {
            spriteBatch.Draw(texture, boundingBox , Color.White);
        }
    }
}
