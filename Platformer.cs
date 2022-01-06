using Basic_platformer.Mapping;
using Basic_platformer.Solids;
using Basic_platformer.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Basic_platformer
{
    public class Platformer : Game
    {
        public static Platformer instance;
        public static GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static bool Paused;
        private bool previousPauseKeyPress;
        public static float Deltatime;
        public static float TimeScale = 1;

        public static Vector2 ScreenSize;
        public static Vector2 ScreenSizeX;
        public static Vector2 ScreenSizeY;

        public static Player player;

        public static Map CurrentMap;
        public static Camera Cam;

        public Platformer()
        {
            instance = this;
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 960;
            graphics.PreferredBackBufferHeight = 540;
            graphics.ApplyChanges();
            ScreenSize = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            ScreenSizeX = new Vector2(ScreenSize.X, 0);
            ScreenSizeY = new Vector2(0, ScreenSize.Y);
            
            CurrentMap = new Map(Vector2.Zero);

            Cam = new Camera(ScreenSize / 2, 0, 1);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Drawing.Init(spriteBatch, Content.Load<SpriteFont>("font"));
            
            player = (Player)CurrentMap.Instantiate(
                new Player(new Vector2(Platformer.ScreenSize.X / 2, Platformer.ScreenSize.Y - 300), 20, 30, Content.Load<Texture2D>("robot")));
            CurrentMap.Data.Actors.Add(player);

            CurrentMap.LoadMap();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Input.UpdateState();

            #region Pausing

            if (Input.GetKey(Keys.X) && !previousPauseKeyPress)
                Paused = !Paused;

            previousPauseKeyPress = Input.GetKey(Keys.X);

            if (Paused)
                return;

            #endregion

            Deltatime = (float) gameTime.ElapsedGameTime.TotalSeconds * TimeScale;

            if (Input.GetKeyDown(Keys.F3))
                Debug.DebugMode = !Debug.DebugMode;
            
            if(Input.GetKeyDown(Keys.C))
                Debug.Clear();

            if (Input.GetKeyDown(Keys.V))
            {
                Point mousePos = Mouse.GetState(Window).Position;
                player.Pos = Cam.ScreenToWorldPosition(new Vector2(mousePos.X, mousePos.Y));
            }

            CurrentMap.Update();

            Cam.Update();
            Input.UpdateOldState();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, Cam.ViewMatrix);
            
            CurrentMap.Render();

            Drawing.DebugPoint();
            Drawing.DebugEvents();

            spriteBatch.End();

            spriteBatch.Begin();
            Drawing.DebugString();
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}