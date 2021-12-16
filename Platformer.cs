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

        public static float Deltatime;
        public static float TimeScale = 1;
        public static Vector2 ScreenSize;

        public Player player;

        public static Map CurrentMap;
        public static Camera Cam;

        public Platformer()
        {
            instance = this;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 960;
            graphics.PreferredBackBufferHeight = 540;
            ScreenSize = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            CurrentMap = new Map(Vector2.Zero, 60, 60, new int[8, 20] {
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
            });
            CurrentMap.LoadMap();

            Cam = new Camera(ScreenSize / 2, 0, 0.8f);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Drawing.Init(spriteBatch, Content.Load<SpriteFont>("font"));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Input.UpdateState();

            Deltatime = (float) gameTime.ElapsedGameTime.TotalSeconds * TimeScale;

            if (Input.GetKeyDown(Keys.F3))
                Debug.DebugMode = !Debug.DebugMode;
            
            if(Input.GetKeyDown(Keys.C))
                Debug.Clear();

            if (Input.GetKeyDown(Keys.X))
                Cam.ZoomLevel -= 0.2f;

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

            spriteBatch.End();

            spriteBatch.Begin();
            Drawing.DebugString();
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}