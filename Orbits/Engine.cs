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
        public static int SCALE = 25000; //1 = 25,000m
        public static int SPEED = 100; 

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        DrawBatch drawBatch;

        private Texture2D background;

        Body Earth = new Body { Name = "Earth", Position = new Vector2(0, 0), Mass = 5.972e24F, Velocity = new Vector2(0, 0)};
        Body Shuttle = new Body { Name = "Shuttle", Position = new Vector2(0, 6771000), Mass = 2000, Velocity = new Vector2(8500, 0) };

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

            // Orbit logic
            Shuttle.Velocity += Shuttle.Acceleration * (dT / 2);
            Shuttle.Position += Shuttle.Velocity * dT;

            //Force should be determined against all bodies and result in a total net force
            var force = Shuttle.Force(Earth);
            Shuttle.Acceleration = force / Shuttle.Mass;

            Shuttle.Velocity += Shuttle.Acceleration * (dT / 2);

            //var test = new Conic(new Vector3(5052.4587f, 1056.2713f, 5011.6366f), new Vector3(3.8589872f, 4.2763114f, -4.8070493f), Earth);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            var conic = new Conic(Shuttle.Position, Shuttle.Velocity, Earth);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.Draw(background, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);

            spriteBatch.End();

            drawBatch.Begin(DrawSortMode.Deferred);

            drawBatch.DrawEllipse(new Pen(new SolidColorBrush(Color.Red)), DrawPos(Earth.Position - new Vector2(conic.FociToCenter * (float)Math.Cos(conic.ArgumentOfPeriapsis), conic.FociToCenter * (float)Math.Sin(conic.ArgumentOfPeriapsis))), conic.SemiMajorAxis / Engine.SCALE, conic.SemiMinorAxis / Engine.SCALE, conic.ArgumentOfPeriapsis);

            drawBatch.FillCircle(new SolidColorBrush(Color.SkyBlue), DrawPos(Earth.Position), 6371000 / Engine.SCALE);
            drawBatch.FillCircle(new SolidColorBrush(Color.WhiteSmoke), DrawPos(Shuttle.Position), 5);

            
            drawBatch.End();

            base.Draw(gameTime);
        }

        public Vector2 DrawPos(Vector2 Position)
        {
            return new Vector2(graphics.PreferredBackBufferWidth/2 + Position.X / Engine.SCALE, graphics.PreferredBackBufferHeight/2 + Position.Y / Engine.SCALE);
        }
    }
}
