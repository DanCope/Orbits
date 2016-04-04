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
        public static int SCALE = 1000000;
        public static int SPEED = 50000;
        public static Vector2 DRAW_OFFSET;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        DrawBatch drawBatch;

        //private Texture2D background;

        public StationaryBody Earth = new StationaryBody("Earth", 5.972e24F, 6371000, new TimeSpan(23, 56, 4));
        public Body Moon;

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
            graphics.PreferredBackBufferWidth = 1920;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 1080;   // set this value to the desired height of your window
            graphics.ApplyChanges();

            DRAW_OFFSET = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);

            Moon = new ConicBody("Moon", 7.35e22F, 3626000, new TimeSpan(27, 7, 43, 14), Earth,
                new Conic(384400000, 0.0554, 5.527, 0 /*2.1830*/, 0/*0.0901*/, 2.3609, Earth));

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

            Earth.Step(T, dT);
            Moon.Step(T, dT);

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

            drawBatch.DrawBody(Color.Red, Moon, DRAW_OFFSET, SCALE);
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
            return new Vector2(graphics.PreferredBackBufferWidth / 2 + Position.X / Engine.SCALE, graphics.PreferredBackBufferHeight / 2 + Position.Y / Engine.SCALE);
        }
    }

    public static class DrawBatchExtensions
    {
        public static void DrawConic(this DrawBatch drawBatch, Color color, Conic path, Vector2 offset, int scale = 0)
        {
            var angle = Math.Cos(path.Inclination) * path.ArgumentOfPeriapsis; //Might need to change
            var ellipseOffset = new Vector2((float)(path.FociToCenter * Math.Cos(angle)), (float)(path.FociToCenter * Math.Sin(angle)));
            var focus = (path.PrimeFocus.Position - ellipseOffset) / scale;

            drawBatch.DrawPrimitiveEllipse(new Pen(new SolidColorBrush(color)),
                focus + offset,
                (float)path.SemiMajorAxis / Engine.SCALE,
                (float)path.SemiMinorAxis / Engine.SCALE,
                (float)angle);
        }

        public static void DrawBody(this DrawBatch drawBatch, Color color, Body body, Vector2 offset, int scale = 0)
        {
            //TODO : Check body is actually on screen
            var drawPosition = (body.Position / scale) + offset;
            var drawRadius = body.Radius / Engine.SCALE;
            var drawEdgePosition = new Vector2(
                (float)(drawPosition.X + drawRadius * Math.Cos(body.Rotation)),
                (float)(drawPosition.Y + drawRadius * Math.Sin(body.Rotation)));

            drawBatch.FillCircle(new SolidColorBrush(color), drawPosition, (float)drawRadius);
            drawBatch.DrawPrimitiveLine(new Pen(new SolidColorBrush(Color.HotPink)), drawPosition, drawEdgePosition);
        }
    }
}


   /*
    * TODO:
    * Mouse Cursor
    * Selecting planets/Lock on
    * Zooming
    * Cycling between planets
    * Loading larger sol
    * Loading from file
    * System generation
    * ShipBody - Cartesian vectors(prograde, retrograde etc)
    * ShipBody - Movement
    * Conics - Interaction/Mouse over highlight
    * Conics - Maneuver nodes (probably automatic)
    * 
    * 
    * 
    */
