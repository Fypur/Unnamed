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
        public static GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static RenderTarget2D RenderTarget => Engine.RenderTarget;

        private static bool Paused;
        public static PauseMenu PauseMenu;
        private static Input.State PreviousPauseOldState;

        public static LDtkWorld World;

        public static Player player => (Player)Engine.Player;

        public static Camera Cam { get => Engine.Cam; set => Engine.Cam = value; }
        private const string initLevel = "8";

        public Platformer()
        {
            instance = this;
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Engine.Initialize(graphics, Content, 1280, 720, new RenderTarget2D(GraphicsDevice, 320, 180, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24), "Utility/SpriteData.xml");

            Cam = new Camera(new Vector2(Engine.RenderTarget.Width / 2, Engine.RenderTarget.Height / 2), 0, 1);

            World = LDtkWorld.LoadWorld("Content/world.ldtk");

            base.Initialize();

            PauseMenu = new PauseMenu();
            
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Drawing.Init(spriteBatch, Content.Load<SpriteFont>("font"));

            var map = new Map(Vector2.Zero);
            Engine.CurrentMap = map;

            //player = (Player)Engine.CurrentMap.Instantiate(
            //new Player(new Vector2(RenderTarget.Width / 2, RenderTarget.Height - 300), 9, 18));
            if(int.TryParse(initLevel, out int lvl))
                map.LoadMapNoAutoTile(new Level(Levels.GetLevelData(lvl, Vector2.Zero)));
            else
                map.LoadMapNoAutoTile(new Level(Levels.GetLevelData(initLevel, Vector2.Zero)));
            Cam.SetBoundaries(Engine.CurrentMap.CurrentLevel.Pos, Engine.CurrentMap.CurrentLevel.Size);
            Cam.FollowsPlayer = true;

            //TODO: Main Menu
            //TODO: Chase Sequence
            //TODO: Re-enable this depending on which world you load
            //player.canJetpack = false;
        }

        protected override void Update(GameTime gameTime)
        {
            Input.UpdateState();

            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Input.GetKeyDown(Keys.Escape)))
                Exit();

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

            if (!Paused)
                Engine.CurrentMap.Update();

#if DEBUG
            if (Input.GetKeyDown(Keys.F3))
                Debug.DebugMode = !Debug.DebugMode;
            
            if(Input.GetKeyDown(Keys.N))
                Debug.Clear();

            if (Input.GetKeyDown(Keys.V))
                player.Pos = Input.MousePos;

                if (Input.GetKeyDown(Keys.R))
            {
                Levels.ReloadLastLevelFetched();
            }

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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(RenderTarget);
            GraphicsDevice.Clear(new Color(3, 11, 28));

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, Cam.ViewMatrix);

            Engine.CurrentMap.Render();
            //Drawing.Draw(DataManager.Textures["zed"], player.Pos);

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