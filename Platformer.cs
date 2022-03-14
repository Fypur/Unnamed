using FMOD.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Fiourp;
using LDtk;

namespace Basic_platformer
{
    public class Platformer : Game
    {
        public static Platformer instance;
        public static GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static RenderTarget2D RenderTarget => Engine.RenderTarget;

        private static bool Paused;
        public static PauseMenu PauseMenu;
        private static Input.State PreviousPauseOldState;

        public static LDtkWorld World;

        public static Player player;

        public static Camera Cam { get => Engine.Cam; set => Engine.Cam = value; }

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

            Cam = new Camera(new Vector2(Engine.RenderTarget.Width / 2, Engine.RenderTarget.Height / 2), 0, 1);

            World = LDtkWorld.LoadWorld("ldtk/world.ldtk");

            base.Initialize();

            PauseMenu = new PauseMenu();
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

            map.LoadMap(new Level(Levels.GetLevelData(0, Vector2.Zero)));
            Cam.SetBoundaries(Engine.CurrentMap.CurrentLevel.Pos, Engine.CurrentMap.CurrentLevel.Size);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Input.UpdateState();
            Engine.Deltatime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Input.GetKeyDown(Keys.X))
            {
                Paused = !Paused;
                if (Paused)
                    PreviousPauseOldState = Input.OldState;
                else
                    Input.OldState = PreviousPauseOldState;
            }

            if (Paused)
                PauseMenu.Update();

            if (!Paused)
                Engine.CurrentMap.Update();

#if DEBUG
            if (Input.GetKeyDown(Keys.F3))
                Debug.DebugMode = !Debug.DebugMode;
            
            if(Input.GetKeyDown(Keys.C))
                Debug.Clear();
            
            if (Input.GetKeyDown(Keys.V))
            {
                player.Pos = Input.MousePos;
            }

            if (Input.GetKeyDown(Keys.W))
                Cam.Move(Vector2.UnitX * -100, 0.2f);

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
            Drawing.DebugPoint((int)Engine.ScreenSize.X / Engine.RenderTarget.Width);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, null, null, null, Cam.ViewMatrix);

            Engine.CurrentMap.UIRender();

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, null, null, null, null);

            if (Paused)
                PauseMenu.Render();

            spriteBatch.End();

            spriteBatch.Begin();
            
            Drawing.DebugString();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static void PauseOrUnpause()
        {
            if (!Paused)
                Pause();
            else
                Unpause();
        }

        public static void Pause()
        {
            if (Paused)
                return;

            Paused = true;
            if (PreviousPauseOldState == null)
                PreviousPauseOldState = Input.OldState;
        }

        public static void Unpause()
        {
            if (!Paused)
                return;

            Paused = false;
            Input.OldState = PreviousPauseOldState;
            PreviousPauseOldState = null;
        }

        protected override void EndRun()
        {
            Audio.Finish();
            base.EndRun();
        }
    }
}