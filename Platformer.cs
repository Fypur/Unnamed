using FMOD.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Fiourp;

namespace Basic_platformer
{
    public class Platformer : Game
    {
        public static Platformer instance;
        public static GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static RenderTarget2D RenderTarget => Engine.RenderTarget;

        public static bool Paused;
        private bool previousPauseKeyPress;

        public static Player player;

        public static Camera Cam;

        private static EventInstance music;

        public Platformer()
        {
            instance = this;
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Engine.Initialize(graphics, Content, 1280, 720, new RenderTarget2D(GraphicsDevice, 320, 180, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24)); ;

            Cam = new Camera(Engine.ScreenSize / 2, 0, 1);
            base.Initialize();

            /*CurrentMap.Instantiate(new TextBox("Le gaming ou quoi donde cuando je mange des pates a l'aide de mes deux bras gauches " +
                "car bon nous on est pas des zemmouriens tu vois ce que je dire lol des barres zfhbeqrfy tgiustg ozerihuierg", "Pixel", 0.01f, Vector2.One * 40, 500, 200));
            */
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Drawing.Init(spriteBatch, Content.Load<SpriteFont>("font"));

            var map = new Map(Vector2.Zero);
            Engine.CurrentMap = map;

            player = (Player)Engine.CurrentMap.Instantiate(
                new Player(new Vector2(RenderTarget.Width / 2, RenderTarget.Height - 300), 7, 10, Content.Load<Texture2D>("Graphics/robot")));
            

            map.LoadMap(new Level(Levels.GetLevelData(3, Vector2.Zero)));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Engine.Update(gameTime);
            
            #region Pausing

            if (Input.GetKey(Keys.X) && !previousPauseKeyPress)
                Paused = !Paused;

            previousPauseKeyPress = Input.GetKey(Keys.X);

            if (Paused)
                return;

            #endregion

#if DEBUG
            
            if (Input.GetKeyDown(Keys.F3))
                Debug.DebugMode = !Debug.DebugMode;
            
            if(Input.GetKeyDown(Keys.C))
                Debug.Clear();

            if (Input.GetKeyDown(Keys.V))
            {
                player.Pos = Input.MousePos;
            }

            if (Input.GetKeyDown(Keys.Z))
                music.setParameterByName("Parameter 1", 0.2f);

            if (Input.GetKeyDown(Keys.M))
                music.setParameterByName("Parameter 1", 1);
#endif

            Cam.Update();
            Input.UpdateOldState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            GraphicsDevice.SetRenderTarget(RenderTarget);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, Cam.ViewMatrix);

            Engine.CurrentMap.Render();

            Drawing.DebugEvents();
            
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);

            spriteBatch.Draw(RenderTarget, new Rectangle(new Point(0, 0), Engine.ScreenSize.ToPoint()), Color.White);
            Drawing.DebugPoint(4);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, null, null, null, Cam.ViewMatrix);

            Engine.CurrentMap.UIRender();

            spriteBatch.End();


            spriteBatch.Begin();
            
            Drawing.DebugString();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static void Pause()
            => Paused = true;

        protected override void EndRun()
        {
            Audio.Finish();
            base.EndRun();
        }
    }
}