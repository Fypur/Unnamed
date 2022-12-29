using FMOD.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using Fiourp;
using LDtk;

namespace Platformer
{
    public class Platformer : Game
    {
        public static Platformer instance;
        public static GraphicsDeviceManager GraphicsManager;
        public static RenderTarget2D RenderTarget => Engine.RenderTarget;

        private static bool Paused;
        public static PauseMenu PauseMenu;
        private static Input.State PreviousPauseOldState;

        public static LDtkFile LDtkFile;
        public static LDtkWorld World;

        public static Player player => (Player)Engine.Player;

        public static Camera Cam { get => Engine.Cam; set => Engine.Cam = value; }

        public static EventInstance music;
        public static Tile BackgroundTile;

#if DEBUG
        public static string InitLevel = "85";
        public static int InitWorld = 0;
        private FileSystemWatcher watcher;
        private bool waitRefresh;
#endif

#if RELEASE
        private const string InitLevel = "60";
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

            LDtkFile = LDtkFile.FromFile("Content/World.ldtk");
            World = LDtkFile.LoadWorld(LDtkTypes.Worlds.World.Iid);

            base.Initialize();

            Cam = new Camera(Vector2.Zero, 0, 1f);

#if DEBUG
            StartGame();

            string currentDir = Environment.CurrentDirectory;
            currentDir = currentDir.Replace('\\', '/');
            watcher = new FileSystemWatcher(currentDir.Substring(0, currentDir.LastIndexOf("Platformer/") + 11) + "Content");
            watcher.NotifyFilter = NotifyFilters.LastWrite;                 
            watcher.Filter = "*.ldtk";
            watcher.Changed += new FileSystemEventHandler((ev, eve) => RefreshLDtk());
            watcher.EnableRaisingEvents = true;
#endif
        }

        protected override void LoadContent()
        {
            Drawing.Init(new SpriteBatch(GraphicsDevice), Content.Load<SpriteFont>("font"));

            Engine.CurrentMap = new Map(Vector2.Zero);

            Engine.CurrentMap.Instantiate(new MainMenu());

        }

        protected override void Update(GameTime gameTime)
        {
            Input.UpdateState();
            
#if DEBUG
            /*if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Input.GetKeyDown(Keys.Escape)))
                Exit();*/
#endif

            Engine.Deltatime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Input.GetKeyDown(Keys.D1))
                PauseOrUnpause();

            if (Input.GetKeyDown(Keys.F11))
                Options.FullScreen();

            if (!Paused)
            {
                Engine.CurrentMap.Update();
                Cam.Update();
            }

            PauseMenu?.Update();

            if (BackgroundTile != null)
                BackgroundTile.Update();

#if DEBUG
            //Debug.LogUpdate(Input.MousePos);

            /*if(Engine.Deltatime != 0)
                Debug.LogUpdate(1 / Engine.Deltatime);*/
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
            
            if (Input.GetKeyDown(Keys.D2))
                RefreshLDtk();



            /*if (Input.GetKey(Keys.W))
            {
                ParticleType PT = new ParticleType()
                {
                    Color = Color.LightSkyBlue,
                    Size = 2,
                    SizeRange = 0,
                    LifeMin = 0.5f,
                    LifeMax = 4f,
                    SpeedMin = 65,
                    SpeedMax = 70,
                    Direction = 0,
                    DirectionRange = 10,
                    Acceleration = Vector2.UnitY * 100,
                    Friction = 0.1f,
                    FadeMode = ParticleType.FadeModes.EndLinear,
                    SizeChange = ParticleType.FadeModes.EndSmooth,
                };

                Engine.CurrentMap.BackgroundSystem.Emit(PT, Input.MousePos, 3);
                IsMouseVisible = false;
                Debug.LogUpdate(Engine.CurrentMap.BackgroundSystem.Particles.Count);
            }
            else
                IsMouseVisible = true;*/
#endif

            Input.UpdateOldState();

            Audio.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(RenderTarget);
            GraphicsDevice.Clear(new Color(3, 11, 28));

            Drawing.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, null);

            if(BackgroundTile != null)
                BackgroundTile.Render();

            Drawing.End();

            Drawing.BeginPrimitives();

            Drawing.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, Cam.ViewMatrix);

            Engine.CurrentMap.Render();

            Drawing.End();


            Drawing.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, Cam.ViewMatrix);

            Drawing.DebugPoint(1, 1);

            Drawing.DebugEvents();

            Drawing.End();

            Drawing.EndPrimitives();

            GraphicsDevice.SetRenderTarget(null);

            DataManager.PixelShaders["Test"].Parameters["extent"].SetValue(0.3f);
            DataManager.PixelShaders["Test"].Parameters["strength"].SetValue(30f);
            Drawing.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, DataManager.PixelShaders["Test"], null);

            Drawing.Draw(RenderTarget, new Rectangle(new Point(0, 0), Engine.ScreenSize.ToPoint()), Color.White);

            Drawing.End();
            Drawing.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Cam.ViewMatrix);

            Engine.CurrentMap.UIRender();

            Drawing.End();
            

            Drawing.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            Engine.CurrentMap.UIOverlayRender();

            PauseMenu?.Render();
            PauseMenu?.UIChildRender();

            Drawing.End();
            Drawing.Begin();

            Drawing.DebugString();

            Drawing.End();

            //Drawing.DrawCircle(Vector2.Zero, 10, 0.01f, Color.White);

            base.Draw(gameTime);
        }

        public static void StartGame()
        {
            var map = new Map(Vector2.Zero);
            Engine.CurrentMap = map;
            Engine.Cam.RenderTargetMode = true;

            int worldDepth = 0;

            var level = Levels.GetLdtkLevel(InitLevel);
            worldDepth = level.WorldDepth;
            Engine.CurrentMap.CurrentLevel = new Level(Levels.GetLevelData(level));
            
            Levels.LoadWorldGrid(World, worldDepth);
            Engine.CurrentMap.CurrentLevel.LoadNoAutoTile();

            BackgroundTile = new Tile(Vector2.Zero, Engine.RenderTarget.Width, Engine.RenderTarget.Height,  new Sprite(DataManager.Textures["bg/bg1"]));

#if RELEASE
            if (World.Iid == LDtkTypes.Worlds.World.Iid)
                player.CanJetpack = false;
#endif

            Vector2 lvlSize = Engine.CurrentMap.CurrentLevel.Size;
            if (lvlSize.Y == 184)
                lvlSize.Y = 180;
            Cam.SetBoundaries(Engine.CurrentMap.CurrentLevel.Pos, lvlSize);
            //Cam.Pos = player.Pos;
            Cam.FollowsPlayer = true;

            PauseMenu = new PauseMenu();

            //music = Audio.PlayEvent(AudioData.MUSIC);
        }

        public static void EndGame()
        {
            foreach (Entity entity in Engine.CurrentMap.Data.Entities)
                entity.OnDestroy();

            Engine.CurrentMap = new Map(Vector2.Zero);
            Engine.Cam.SetBoundaries(Rectangle.Empty);
            Engine.Cam.Pos = Vector2.Zero;
            Engine.Player = null;


            Audio.StopEvent(music);
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
            if (Paused || Engine.CurrentMap.Data.UIElements.Exists((element) => element is MainMenu))
                return;

            Paused = true;
            PauseMenu.Show();
            if (PreviousPauseOldState == null)
                PreviousPauseOldState = Input.OldState;
        }

        public static void Unpause()
        {
            if (!Paused)
                return;

            PauseMenu.Children = new();
            Paused = false;
            Input.OldState = PreviousPauseOldState;
            PreviousPauseOldState = null;
        }

        protected override void EndRun()
        {
            Audio.Finish();
            base.EndRun();
        }
        
        #if DEBUG
        private void RefreshLDtk()
        {
            if(waitRefresh)
                return;
            
            waitRefresh = true;

            Tile t = (Tile)Engine.CurrentMap.Instantiate(new Tile(Vector2.Zero, 0, 0, Sprite.None));
            t.AddComponent(new Timer(2, true, null, () =>
            {
                waitRefresh = false;

                string currentDir = Environment.CurrentDirectory;
                currentDir = currentDir.Replace('\\', '/');

                File.Copy(currentDir.Substring(0, currentDir.LastIndexOf("Platformer/") + 11) + "Content/World.ldtk", currentDir + "/Content/World.ldtk", true);
            
                World = LDtkFile.FromFile("Content/World.ldtk").LoadWorld(LDtkTypes.Worlds.World.Iid);
                Engine.CurrentMap.CurrentLevel.Unload();
                LDtkLevel lvl = World.LoadLevel(Levels.LastLDtkLevel.Iid);
                new Level(Levels.GetLevelData(lvl)).LoadNoAutoTile();
            
                Engine.CurrentMap.Data.EntitiesByType[typeof(Grid)][0].SelfDestroy();
                Levels.LoadWorldGrid(World, lvl.WorldDepth);

                Vector2 lvlSize = Engine.CurrentMap.CurrentLevel.Size;
                if (lvlSize.Y == 184)
                    lvlSize.Y = 180;
                Cam.SetBoundaries(Engine.CurrentMap.CurrentLevel.Pos, lvlSize);

                if(Engine.Player != null)
                    player.InstaDeath();
                t.SelfDestroy();
            }));
            
        }
        #endif
    }
}
