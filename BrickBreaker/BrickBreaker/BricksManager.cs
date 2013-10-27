using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace BrickBreaker
{
    class BricksManager
    {
        Texture2D brickTexture;
        List<Brick> bricks;
        //used for drawing
        SpriteBatch spriteBatch;
        //used to calculate the number of bricks based on screen resolution
        GraphicsDeviceManager graphics;
        //a count of the maximum number of bricks possible in width
        int widthCount;

        ///<summary>
        ///To initiliaze new BrickManager. Should be called insde main game
        ///</summary>
        ///<param name="graphics">Used to calculate the maximum number of bricks possible</param>
        public BricksManager(ref SpriteBatch spriteBatch,ref Texture2D brickTexture,ref GraphicsDeviceManager graphics)
        {
            this.brickTexture = brickTexture;
            this.spriteBatch = spriteBatch;
            this.graphics = graphics;
            bricks = new List<Brick>();
            widthCount = calculateWidthCount();

            Console.WriteLine(widthCount + "bricks to be rendered");
        }

        private int calculateWidthCount()
        {
            return graphics.GraphicsDevice.Viewport.Width / brickTexture.Width;
        }

        public void addBrick(Brick brick)
        {
            bricks.Add(brick);
        }

        public void removeBrick(Brick brick)
        {
            bricks.Remove(brick);
        }

        /// <summary>
        /// Called before restarting the game from titlescreen or gameover screen.
        /// Warning : Calling in the middle of the game is not recommended.
        /// </summary>
        public void clearBricks()
        {
            for (int i = bricks.Count - 1; i >= 0; i--)
                bricks.RemoveAt(i);
        }

        /// <summary>
        /// Called when initializing bricks in a new game, or when restarting the game.
        /// Note : This does not clear the bricks. clearBricks() will need to be called separately.
        /// </summary>
        public void generateBricks()
        {
            //add first brick at the top-left corner of the screen
            if(bricks.Count == 0)
                addBrick(new Brick(ref spriteBatch,ref brickTexture, Vector2.Zero));
                
            //for all other bricks
            while (bricks.Count < widthCount)
            {
                //create 2 rows of bricks

                //bricks at even indexes go to the top row
                if (bricks.Count % 2 == 0)
                {
                    Vector2 newBrickVector = bricks[bricks.Count - 1].getPos();
                    newBrickVector.X += brickTexture.Width;
                    newBrickVector.Y -= brickTexture.Height;
                    addBrick(new Brick(ref spriteBatch, ref brickTexture, newBrickVector));
                }

                //bricks at odd indexes go to the lower row
                else
                {
                    Vector2 newBrickVector = bricks[bricks.Count - 1].getPos();
                    newBrickVector.X += brickTexture.Width;
                    newBrickVector.Y += brickTexture.Height;
                    addBrick(new Brick(ref spriteBatch, ref brickTexture, newBrickVector));
                }
            }
        }

        public void Draw()
        {
            foreach (Brick brick in bricks)
                    brick.Draw();
        }

        /// <summary>
        /// To check if the supplied boundingBox (preferably the ball) has collided with a brick. If yes, remove that brick from the game and return true.
        /// </summary>
        /// <param name="boundingBox">Usually the boundingBox of the ball</param>
        /// <returns>True if collision occurs. False if there is no collision</returns>
        public bool DetectCollision(Rectangle boundingBox)
        {
            foreach (Brick brick in bricks)
                if (brick.getBoundingBox().Intersects(boundingBox))
                {
                    removeBrick(brick);
                    return true;
                }
            return false;
        }

        public int getBrickCount()
        {
            return bricks.Count;
        }
    }
}
