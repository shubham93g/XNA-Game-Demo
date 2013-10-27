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
using System.Diagnostics;

namespace BrickBreaker
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D brickTexture, ballTexture, sliderTexture;
        //boundingBox for the game screen (to detect collision with screen edges)
        Rectangle gameFrame;
        BricksManager brickManager;
        Ball ball;
        Slider slider;
        SpriteFont font;
        bool lifelost;
        int lives;
        DateTime start,end;
        SoundEffect fantomenk;
        SoundEffectInstance backgroundMusic;
        SoundEffect hit;
        GameState gameState;
        const int totalLives = 3;
        int timeDivideFactor = 20;
        const int sliderSpeed = 10;
        Vector2 speed = new Vector2(6,8);
        Vector2 ballPos,sliderPos;

        /// <summary>
        /// For GameState management
        /// </summary>
        enum GameState
        {
            TitleScreen,
            GameRunning,
            GameOver
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            gameState = GameState.TitleScreen;
            lives = 3;
            lifelost = false;

            font = Content.Load<SpriteFont>("SpriteFont");

            //sounds/music
            fantomenk = Content.Load<SoundEffect>("FantomenK");
            backgroundMusic = fantomenk.CreateInstance();
            hit = Content.Load<SoundEffect>("hit");
            
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //boundingBox for collision detection with screen edges
            gameFrame = new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);

            //bricks
            brickTexture = Content.Load<Texture2D>("brick");
            brickManager = new BricksManager(ref spriteBatch,ref brickTexture,ref graphics);
            brickManager.generateBricks();

            //ball
            ballTexture = Content.Load<Texture2D>("ball");
            ballPos = new Vector2(
                (
                graphics.GraphicsDevice.Viewport.Width - ballTexture.Width) / 2, //centre of the screen
                graphics.GraphicsDevice.Viewport.Height * 3 / 4 - (ballTexture.Height + 10)
                );
            ball = new Ball(ref spriteBatch,
                ref ballTexture,
                ballPos,
                new Vector2(0, 0), //velocity vector
                timeDivideFactor);

            //slider
            sliderTexture = Content.Load<Texture2D>("slider");
            sliderPos = new Vector2(
                (
                graphics.GraphicsDevice.Viewport.Width - sliderTexture.Width) / 2, //centre of the screen
                graphics.GraphicsDevice.Viewport.Height*3 / 4
                );
            slider = new Slider(ref sliderTexture,ref spriteBatch, sliderPos);
            
        }


        private void ResetGame()
        {
            lives = 3;
            ball.setPosition(ballPos);
            slider.setPosition(new Vector2(
            (
            graphics.GraphicsDevice.Viewport.Width - sliderTexture.Width) / 2, //centre of the screen
            graphics.GraphicsDevice.Viewport.Height * 3 / 4
            ));
            brickManager.clearBricks();
            brickManager.generateBricks();
            ball.setVelocity(Vector2.Zero);
            start = DateTime.Now;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            
            switch (gameState)
            {
                case GameState.TitleScreen: UpdateTitleScreen();
                    break;
                case GameState.GameRunning: UpdateGameRunning(gameTime);
                    break;
                case GameState.GameOver: UpdateGameOver();
                    break;
            }
            
        }

        private void UpdateGameRunning(GameTime gameTime)
        {
            backgroundMusic.Play();
            ball.refGameTime(ref gameTime);
            KeyboardState keyState = Keyboard.GetState();
            //if the game just started or has lost his life, the ball is stationary and the player has not yet pressed enter
            if (ball.getVelocity() == Vector2.Zero)
            {
                //moving slider left and keep it inside the gameFrame
                if (keyState.IsKeyDown(Keys.Left) && keyState.IsKeyUp(Keys.Right) && slider.getX() > 0)
                {
                    slider.MoveLeft(sliderSpeed, ref ball, ref brickManager, ref gameFrame, ref lifelost, hit.CreateInstance());
                    Vector2 position = ball.getPosition();
                    position.X -= sliderSpeed;
                    //move ball with slider
                    ball.setPosition(position);
                }

                //moving the slider right and keep it inside the gameFrame
                if (keyState.IsKeyDown(Keys.Right) && keyState.IsKeyUp(Keys.Left) && slider.getX() + slider.getTexture().Width < GraphicsDevice.Viewport.Width)
                {
                    slider.MoveRight(sliderSpeed, ref ball, ref brickManager, ref gameFrame, ref lifelost, hit.CreateInstance());
                    Vector2 position = ball.getPosition();
                    position.X += sliderSpeed;
                    //move ball with slider
                    ball.setPosition(position);
                }

                //on enter, set the ball moving
                if (keyState.IsKeyDown(Keys.Enter))
                    ball.setVelocity(speed);

                if(keyState.IsKeyDown(Keys.Escape))
                    gameState = GameState.GameOver;
            }

            //else if the ball is moving
            else
            {
                if (lifelost)
                {
                    lives--;
                    ball.setPosition(
                new Vector2(
                (slider.getPosition().X + slider.getTexture().Width/2 - ballTexture.Width/2),
                graphics.GraphicsDevice.Viewport.Height * 3 / 4 - ballTexture.Height-10
                )); //place the ball right above the slider when your life is lost

                    //will force program to the (if) statement of this update function in game loop iteration
                    ball.setVelocity(Vector2.Zero);

                    //reset boolean variable
                    lifelost = false;
                }

                //to exit from game at will OR exit game if bricks are over OR when you have 0 lives
                if (keyState.IsKeyDown(Keys.Escape) || (int)brickManager.getBrickCount() == 0 || lives == 0)
                {
                    end = DateTime.Now;
                    backgroundMusic.Stop();
                    gameState = GameState.GameOver;
                }

                //slider moving and collision detection with update of ball position
                //moving the slider left and keeping it inside the frame
                if (keyState.IsKeyDown(Keys.Left) && keyState.IsKeyUp(Keys.Right) && slider.getX() > 0)
                    slider.MoveLeft(sliderSpeed, ref ball, ref brickManager, ref gameFrame, ref lifelost, hit.CreateInstance());

                //moving the slider right and keeping it inside the frame
                else if (keyState.IsKeyDown(Keys.Right) && keyState.IsKeyUp(Keys.Left) && slider.getX() + slider.getTexture().Width < GraphicsDevice.Viewport.Width)
                    slider.MoveRight(sliderSpeed, ref ball, ref brickManager, ref gameFrame, ref lifelost, hit.CreateInstance());

                //updating ball position when slider is not moving
                else
                    ball.UpdatePosition(brickManager, gameFrame, slider, ref lifelost, hit.CreateInstance()); //note : no slow down factor, since UpdatePosition is called only once
            }
        }

        private void UpdateTitleScreen()
        {
          KeyboardState keyState = Keyboard.GetState();
          if (keyState.IsKeyDown(Keys.Space))
          {
              start = DateTime.Now;
              gameState = GameState.GameRunning;
          }

            if (keyState.IsKeyDown(Keys.Escape))
                this.Exit();
        }

        private void UpdateGameOver()
        {
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Space))
            {
                ResetGame();
                gameState = GameState.GameRunning;
            }

            if (keyState.IsKeyDown(Keys.Escape))
            {
                this.Exit(); 
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            switch (gameState)
            {
                case GameState.TitleScreen: DrawTitleScreen();
                    break;
                case GameState.GameRunning: DrawGameRunning();
                    break;
                case GameState.GameOver: DrawGameOver();
                    break;
            } 
        }

        private void DrawGameRunning()
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            //stats display
            spriteBatch.DrawString(font, "Lives Left: " + lives + "\n" 
                + DateTime.Now.Subtract(start).ToString() 
                + "\nBricks Left : " + brickManager.getBrickCount()
                + "\nControls : Left right arrow keys. Enter to start ball.", new Vector2(10, graphics.GraphicsDevice.Viewport.Height - 100), Color.Black);
            brickManager.Draw();
            ball.Draw();
            slider.Draw();
            spriteBatch.End();
        }

        private void DrawTitleScreen()
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Welcome to Shubham's game demo.\nPress Spacebar to continue\nEsc to exit", new Vector2(10, 10), Color.Black);
            spriteBatch.End();
        }

        private void DrawGameOver()
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Gameover\nLives lost : "+(totalLives-lives)+
                "\nBricks left : " +brickManager.getBrickCount()+
                "\nTime takes : " + end.Subtract(start)+
                "\nPress Spacebar to retry\nEsc to exit", new Vector2(10, 10), Color.Black);
            spriteBatch.End();
        }
    }
}
