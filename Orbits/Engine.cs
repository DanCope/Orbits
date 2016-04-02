using LilyPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Orbits.Domain;
using System;

namespace Orbits
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Engine : Game
    {
        public static int SCALE = 1500000;
        public static int SPEED = 5000;
        public static Vector2 DRAW_OFFSET;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        DrawBatch drawBatch;

        private Texture2D background;

        public Body Earth = new Body("Earth", 5.972e24F, 6371000);
        public Body Moon, MoonRedux;

        public Conic MoonZero;

        public Engine()
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
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 1024;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 768;   // set this value to the desired height of your window
            graphics.ApplyChanges();

            DRAW_OFFSET = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);


            //Moon = new Body("Moon", 7.35e22F, 3626000, Earth, new Vector2(r.X, r.Y), new Vector2(v.X, v.Y));
            Moon = new Body("Moon", 7.35e22F, 3626000 * 1.4f, Earth, new Vector2(0, -376671000), new Vector2(500, 500));
            MoonRedux = new ConicBody("Moon", 7.35e22F, 3626000, Earth, Moon.Conic);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            drawBatch = new DrawBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            //background = Content.Load<Texture2D>("Images/stars");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var dT = (float)gameTime.ElapsedGameTime.TotalSeconds * SPEED;
            var T = (float)gameTime.TotalGameTime.TotalSeconds * SPEED;

            Moon.Step(T, dT);
            MoonRedux.Step(T, dT);

            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            //spriteBatch.Draw(background, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);

            spriteBatch.End();

            drawBatch.Begin(DrawSortMode.Deferred);

            drawBatch.DrawConic(Color.Red, Moon.Conic, DRAW_OFFSET, SCALE);
            drawBatch.DrawConic(Color.Green, MoonRedux.Conic, DRAW_OFFSET, SCALE);

            drawBatch.DrawBody(Color.Red, Moon, DRAW_OFFSET, SCALE);
            drawBatch.DrawBody(Color.Green, MoonRedux, DRAW_OFFSET, SCALE);
            drawBatch.DrawBody(Color.SkyBlue, Earth, DRAW_OFFSET, SCALE);

            drawBatch.End();

            base.Draw(gameTime);
        }

        public Vector2 RotateRadians(Vector2 v, double radians)
        {
            var ca = Math.Cos(radians);
            var sa = Math.Sin(radians);
            return new Vector2((float)(ca * v.X - sa * v.Y), (float)(sa * v.X + ca * v.Y));
        }

        public Vector2 DrawPos(Vector3 Position)
        {
            return new Vector2(graphics.PreferredBackBufferWidth/2 + Position.X / Engine.SCALE, graphics.PreferredBackBufferHeight/2 + Position.Y / Engine.SCALE);
        }
    }

    public static class DrawBatchExtensions
    {
        public static void DrawConic(this DrawBatch drawBatch, Color color, Conic path, Vector2 offset, int scale = 0)
        {
            var angle = Math.Cos(path.Inclination) * path.ArgumentOfPeriapsis; //Might need to change
            var ellipseOffset = new Vector2((float)(path.FociToCenter * Math.Cos(angle)), (float)(path.FociToCenter * Math.Sin(angle)));
            var focus = (path.PrimeFocus.Position - ellipseOffset) / scale;

            drawBatch.DrawEllipse(new Pen(new SolidColorBrush(color)),
                focus + offset,
                (float)path.SemiMajorAxis / Engine.SCALE,
                (float)path.SemiMinorAxis / Engine.SCALE,
                (float)angle);
        }

        public static void DrawBody(this DrawBatch drawBatch, Color color, Body body, Vector2 offset, int scale = 0)
        {
            drawBatch.FillCircle(new SolidColorBrush(color), (body.Position / scale) + offset, body.Radius / Engine.SCALE);
        }
    }
}
