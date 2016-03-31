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
        public static int SCALE = 2500000; //1 = 25,000m
        public static int SPEED = 100; 

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        DrawBatch drawBatch;

        private Texture2D background;

        public Body Earth = new Body("Earth", 5.972e24F, 6371000);
        //public Body SpaceStation = new Body("Space Station", 2000, 5, null, new Vector2(0, 6.771e7F), new Vector2(8500, 0));
        public Body Moon = new Body("Moon", 7.35e22F, 362600000, null, new Vector2(0, 376671000), new Vector2(1022, 0));

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

            Moon.Parent = Earth;

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
            background = Content.Load<Texture2D>("Images/stars");
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

            Moon.Step(dT);

            //var test = new Conic(new Vector3(5052.4587f, 1056.2713f, 5011.6366f), new Vector3(3.8589872f, 4.2763114f, -4.8070493f), Earth);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            var conic = new Conic(Moon.Position, Moon.Velocity, Earth);

            //Test
            Vector3 r, v;
            var dT = (float)gameTime.ElapsedGameTime.TotalSeconds * SPEED;
            conic.ToCartesian(dT, out r, out v);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.Draw(background, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);

            spriteBatch.End();

            drawBatch.Begin(DrawSortMode.Deferred);

            drawBatch.DrawEllipse(new Pen(new SolidColorBrush(Color.Red)), DrawPos(Earth.Position - new Vector2(conic.FociToCenter * (float)Math.Cos(conic.ArgumentOfPeriapsis), conic.FociToCenter * (float)Math.Sin(conic.ArgumentOfPeriapsis))), conic.SemiMajorAxis / Engine.SCALE, conic.SemiMinorAxis / Engine.SCALE, conic.ArgumentOfPeriapsis);

            drawBatch.FillCircle(new SolidColorBrush(Color.DeepSkyBlue), DrawPos(Earth.Position), 6371000 / Engine.SCALE);
            drawBatch.FillCircle(new SolidColorBrush(Color.WhiteSmoke), DrawPos(Moon.Position), Math.Max(1737000 / Engine.SCALE, 1));

            
            drawBatch.End();

            base.Draw(gameTime);
        }

        public Vector2 DrawPos(Vector2 Position)
        {
            return new Vector2(graphics.PreferredBackBufferWidth/2 + Position.X / Engine.SCALE, graphics.PreferredBackBufferHeight/2 + Position.Y / Engine.SCALE);
        }
    }
}
