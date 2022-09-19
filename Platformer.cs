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
        private SpriteBatch spriteBatch;

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
        public static string InitLevel = "Test";
        public static int InitWorld = 0;
        private FileSystemWatcher watcher;
        private bool waitRefresh;
#endif

#if RELEASE
        private const string InitLevel = "0";
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

            LDtkFile = LDtkFile.FromFile("Content/First.ldtk");
            World = LDtkFile.LoadWorld(LDtkTypes.Worlds.World.Iid);

            base.Initialize();

            Cam = new Camera(Vector2.Zero, 0, 1f);

#if DEBUG
            StartGame();

            watcher = new FileSystemWatcher("C:/Users/Administrateur/Documents/Monogame/Platformer/Content");
            //watcher.Path = "/home/f/Documents/Platformer/Content";
            watcher.NotifyFilter = NotifyFilters.LastWrite;
                                   
            watcher.Filter = "*.ldtk";
            //watcher.Deleted += new FileSystemEventHandler((ev, eve) => RefreshLDtk());
            //watcher.Renamed += new FileSystemEventHandler((ev, eve) => RefreshLDtk());
            //watcher.Disposed += new FileSystemEventHandler((ev, eve) => RefreshLDtk());
            //watcher.Error += new FileSystemEventHandler((ev, eve) => RefreshLDtk());
            //watcher.Created += new FileSystemEventHandler((ev, eve) => RefreshLDtk());
            watcher.Changed += new FileSystemEventHandler((ev, eve) => RefreshLDtk());

            watcher.EnableRaisingEvents = true;
            //Engine.CurrentMap.Instantiate(new MainMenu());

#endif

        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Drawing.Init(spriteBatch, Content.Load<SpriteFont>("font"));

            Engine.CurrentMap = new Map(Vector2.Zero);

            Engine.CurrentMap.Instantiate(new MainMenu());
#if RELEASE
#endif

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

            Input.UpdateOldState();

            Audio.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(RenderTarget);
            GraphicsDevice.Clear(new Color(3, 11, 28));


            Drawing.BeginPrimitives();


            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, null);

            if(BackgroundTile != null)
                BackgroundTile.Render();

            spriteBatch.End();


            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, Cam.ViewMatrix);

            Engine.CurrentMap.Render();

            spriteBatch.End();


            Drawing.EndPrimitives();


            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, Cam.ViewMatrix);

            Drawing.DebugPoint(1, 1);

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

            PauseMenu?.Render();
            PauseMenu?.UIChildRender();

            spriteBatch.End();
            spriteBatch.Begin();

            Drawing.DebugString();

            spriteBatch.End();

            //Drawing.DrawCircle(Vector2.Zero, 10, 0.01f, Color.White);

            base.Draw(gameTime);
        }

        public static void StartGame()
        {
            var map = new Map(Vector2.Zero);
            Engine.CurrentMap = map;
            Engine.Cam.RenderTargetMode = true;
            
            Levels.LoadWorldGrid(World);

            if (int.TryParse(InitLevel, out int lvl))
                map.LoadMapNoAutoTile(new Level(Levels.GetLevelData(lvl, Vector2.Zero)));
            else
                map.LoadMapNoAutoTile(new Level(Levels.GetLevelData(InitLevel, Vector2.Zero)));


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

           music = Audio.PlayEvent(AudioData.MUSIC);
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
                File.Copy("C:\\Users\\Administrateur\\Documents\\Monogame\\Platformer\\Content\\First.ldtk", "C:\\Users\\Administrateur\\Documents\\Monogame\\Platformer\\bin\\x64\\Debug\\net6.0\\Content\\First.ldtk", true);
            
                World = LDtkFile.FromFile("Content/First.ldtk").LoadWorld(LDtkTypes.Worlds.World.Iid);
                Engine.CurrentMap.CurrentLevel.Unload();
                LDtkLevel lvl = World.LoadLevel(Levels.LastLDtkLevel.Iid);
                new Level(Levels.GetLevelData(lvl)).LoadNoAutoTile();
            
                Engine.CurrentMap.Data.EntitiesByType[typeof(Grid)][0].SelfDestroy();
                Levels.LoadWorldGrid(World);
            
                player.Death();
                t.SelfDestroy();
            }));
            
        }
        #endif
    }
}
