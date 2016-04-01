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
        public static int SPEED = 50000; 

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        DrawBatch drawBatch;

        private Texture2D background;

        public Body Earth = new Body("Earth", 5.972e24F, 6371000);
        //public Body SpaceStation = new Body("Space Station", 2000, 5, null, new Vector2(0, 6.771e7F), new Vector2(8500, 0));
        public Body Moon = new Body("Moon", 7.35e22F, 3626000, null, new Vector2(0, 376671000), new Vector2(500, -1.538058f));

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

            Moon.Parent = Earth;
            MoonZero = new Conic(Moon.Position, Moon.Velocity, Earth);

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

            Moon.Step(dT);

            //From http://www.castor2.ca/05_OD/01_Gauss/14_Kepler/index.html
            //var test = new Conic(new Vector3(5052458.7f, 1056271.3f, 5011636.6f), new Vector3(3858.9872f, 4276.3114f, -4807.0493f), Earth);

            //var test2 = new Conic(73108163f, 0.0159858f, 2.40429914663f, 3.68759754395f, 1.24002508663f, 6.1954373041f, Earth);
            //Vector3 r, v;
            //test2.ToCartesian(0, out r, out v);

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
            var dT = (float)gameTime.TotalGameTime.TotalSeconds * SPEED;
            MoonZero.ToCartesian(dT, out r, out v);

            //Need to figure out why it needs to be rotated
            var mzr = RotateRadians(new Vector2(r.X, r.Y), Math.PI);
            var mzv = RotateRadians(new Vector2(v.X, v.Y), Math.PI);
            var moonZeroConic = new Conic(mzr, mzv, Earth);


            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            //spriteBatch.Draw(background, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);

            spriteBatch.End();

            drawBatch.Begin(DrawSortMode.Deferred);

            drawBatch.DrawEllipse(new Pen(new SolidColorBrush(Color.Red)), DrawPos(Earth.Position - new Vector2((float)(conic.FociToCenter * Math.Cos(conic.ArgumentOfPeriapsis)), (float)(conic.FociToCenter * Math.Sin(conic.ArgumentOfPeriapsis)))), (float)conic.SemiMajorAxis / Engine.SCALE, (float)conic.SemiMinorAxis / Engine.SCALE, (float)conic.ArgumentOfPeriapsis);
            drawBatch.DrawEllipse(new Pen(new SolidColorBrush(Color.Green)), 
                DrawPos(Earth.Position - new Vector2((float)(moonZeroConic.FociToCenter * Math.Cos(moonZeroConic.ArgumentOfPeriapsis)), (float)(moonZeroConic.FociToCenter * Math.Sin(moonZeroConic.ArgumentOfPeriapsis)))),
                (float)moonZeroConic.SemiMajorAxis / Engine.SCALE, 
                (float)moonZeroConic.SemiMinorAxis / Engine.SCALE, 
                (float)moonZeroConic.ArgumentOfPeriapsis);

            drawBatch.FillCircle(new SolidColorBrush(Color.DeepSkyBlue), DrawPos(Earth.Position), Earth.Radius / Engine.SCALE);
            drawBatch.FillCircle(new SolidColorBrush(Color.WhiteSmoke), DrawPos(Moon.Position), Math.Max(Moon.Radius / Engine.SCALE, 1));

            drawBatch.FillCircle(new SolidColorBrush(Color.YellowGreen), DrawPos(mzr), Moon.Radius / Engine.SCALE);

            drawBatch.End();

            base.Draw(gameTime);
        }

        public Vector2 RotateRadians(Vector2 v, double radians)
        {
            var ca = Math.Cos(radians);
            var sa = Math.Sin(radians);
            return new Vector2((float)(ca * v.X - sa * v.Y), (float)(sa * v.X + ca * v.Y));
        }

        public Vector2 DrawPos(Vector2 Position)
        {
            return new Vector2(graphics.PreferredBackBufferWidth/2 + Position.X / Engine.SCALE, graphics.PreferredBackBufferHeight/2 + Position.Y / Engine.SCALE);
        }
    }
}
