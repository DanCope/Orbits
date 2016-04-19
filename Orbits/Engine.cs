using LilyPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Orbits.Domain;
using Orbits.Extensions;
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

        public Body System;
        public Body Selected;
        public Body Highlighted;

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

            var Sun = new StationaryBody("Sun", 1.988e30F, 696.342e6F, new TimeSpan(25, 1, 12, 0));
            var Earth = new ConicBody("Earth", 5.972e24F, 6.371e6F, new TimeSpan(23, 56, 4), 149.60e9, 0.0167, 102.94719, 100.46435, Sun);
            var Moon = new ConicBody("Moon", 7.35e22F, 1.737e6F, new TimeSpan(27, 7, 43, 14), 3.844e8, 0.0554, 5.527, 2.3609, Earth);

            System = Sun;
            Selected = Earth;

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

            System.Step(T, dT);

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

            var selectedOffset = DRAW_OFFSET - (Selected.SystemPosition / SCALE);

            drawBatch.DrawBody(Color.SkyBlue, System, selectedOffset, SCALE);
            foreach (var s in System.GetAllOrbits())
            {
                drawBatch.DrawConic(Color.LightGray, s.Conic, selectedOffset, SCALE);
                drawBatch.DrawBody(Color.Red, s, selectedOffset, SCALE);
            }

            drawBatch.DrawCursor();
            drawBatch.End();

            base.Draw(gameTime);
        }

        public Vector2 RotateRadians(Vector2 v, double radians)
        {
            var ca = Math.Cos(radians);
            var sa = Math.Sin(radians);
            return new Vector2((float)(ca * v.X - sa * v.Y), (float)(sa * v.X + ca * v.Y));
        }
    }

    
}


   /*
    * TODO:
    * DONE - Mouse Cursor
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
