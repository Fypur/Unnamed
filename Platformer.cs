using FMOD.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Fiourp;
using LDtk;

namespace Platformer
{
    public class Platformer : Game
    {
        public static Platformer instance;
        public static GraphicsDeviceManager GraphicsManager;
        private SpriteBatch spriteBatch;

        public static RenderTarget2D RenderTarget => Engine.RenderTarget;

        private static bool Paused;
        public static PauseMenu PauseMenu;
        private static Input.State PreviousPauseOldState;

        public static LDtkWorld World;

        public static Player player => (Player)Engine.Player;

        public static Camera Cam { get => Engine.Cam; set => Engine.Cam = value; }

        public static EventInstance music;

#if DEBUG
        private const string initLevel = "5";
#endif

#if RELEASE
        private const string initLevel = "0";
#endif

        public Platformer()
        {
            instance = this;
            GraphicsManager = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Engine.Initialize(GraphicsManager, Content, 1280, 720, new RenderTarget2D(GraphicsDevice, 320, 180, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24), "Utility/SpriteData.xml");

            Options.CurrentResolution = Engine.ScreenSize;

            World = LDtkFile.FromFile("Content/world.ldtk").LoadWorld(LDtkTypes.Worlds.World.Iid);

            base.Initialize();

            Cam = new Camera(Vector2.Zero, 0, 1);

            PauseMenu = new PauseMenu();
            
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Drawing.Init(spriteBatch, Content.Load<SpriteFont>("font"));

            Engine.CurrentMap = new Map(Vector2.Zero);
            Engine.CurrentMap.Instantiate(new MainMenu());
        }

        protected override void Update(GameTime gameTime)
        {
            Input.UpdateState();

            /*if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Input.GetKeyDown(Keys.Escape)))
                Exit();*/

            Engine.Deltatime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Input.GetKeyDown(Keys.D1))
            {
                Paused = !Paused;
                if (Paused)
                    PreviousPauseOldState = Input.OldState;
                else
                    Input.OldState = PreviousPauseOldState;
            }

            if (Paused)
                PauseMenu.Update();

            if (Input.GetKeyDown(Keys.F11))
                Options.FullScreen();

            if (!Paused)
                Engine.CurrentMap.Update();

#if DEBUG
            if (Input.GetKeyDown(Keys.F3))
                Debug.DebugMode = !Debug.DebugMode;
            
            if(Input.GetKeyDown(Keys.B))
                Debug.Clear();

            if (Input.GetKeyDown(Keys.V) && player != null)
                player.ExactPos = Input.MousePos;

            if (Input.GetKeyDown(Keys.R))
                Levels.ReloadLastLevelFetched();

            if (Input.GetKeyDown(Keys.NumPad1))
                music.setParameterByName("Pitch", 0);
            if (Input.GetKeyDown(Keys.NumPad2))
                music.setParameterByName("Pitch", 0.5f);
            if (Input.GetKeyDown(Keys.NumPad3))
                music.setParameterByName("Pitch", 1);

            /*if (Input.GetKey(Keys.W))
            {
                var PT = new ParticleType();
                PT.Color = Color.White;
                PT.Size = 2;
                PT.SizeRange = 1;
                PT.LifeMin = 0.1f;
                PT.LifeMax = 0.3f;
                PT.SpeedMin = 20;
                PT.SpeedMax = 50;
                PT.Direction = -90;
                PT.DirectionRange = 45;
                //PT.Acceleration = Vector2.UnitY * 100;
                //PT.Friction = 0.1f;
                PT.FadeMode = ParticleType.FadeModes.EndLinear;
                PT.SizeChange = ParticleType.FadeModes.EndSmooth;
                pS.Emit(PT, 10, new Rectangle((Input.MousePos - Vector2.One * 3).ToPoint(), (Vector2.One * 6).ToPoint()), null, -90, Color.White);
            }*/
#endif
            
            Cam.Update();

            Input.UpdateOldState();

            Audio.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(RenderTarget);
            GraphicsDevice.Clear(new Color(3, 11, 28));

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, Cam.ViewMatrix);

            Engine.CurrentMap.Render();

            Drawing.DebugPoint(1);

            Drawing.DebugEvents();

            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            spriteBatch.Draw(RenderTarget, new Rectangle(new Point(0, 0), Engine.ScreenSize.ToPoint()), Color.White);


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Cam.ViewMatrix);

            Engine.CurrentMap.UIRender();


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            Engine.CurrentMap.UIOverlayRender();

            if (Paused)
            {
                PauseMenu.Render();
                PauseMenu.UIChildRender();
            }

            spriteBatch.End();
            spriteBatch.Begin();

            Drawing.DebugString();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static void StartGame()
        {
            var map = new Map(Vector2.Zero);
            Engine.CurrentMap = map;
            Engine.Cam.RenderTargetMode = true;

            if (int.TryParse(initLevel, out int lvl))
                map.LoadMapNoAutoTile(new Level(Levels.GetLevelData(lvl, Vector2.Zero)));
            else
                map.LoadMapNoAutoTile(new Level(Levels.GetLevelData(initLevel, Vector2.Zero)));

#if RELEASE
            if (World.Iid == LDtkTypes.Worlds.World.Iid)
                player.CanJetpack = false;
#endif

            Cam.SetBoundaries(Engine.CurrentMap.CurrentLevel.Pos, Engine.CurrentMap.CurrentLevel.Size);
            Cam.FollowsPlayer = true;
        }

        public static void EndGame()
        {
            Engine.CurrentMap = new Map(Vector2.Zero);
            Engine.Cam.SetBoundaries(Rectangle.Empty);
            Engine.Cam.Pos = Vector2.Zero;
            Engine.Player = null;
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