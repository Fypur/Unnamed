using Basic_platformer.Solids;
using Basic_platformer.Static_Classes;
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

        public static Map map;
        public static Camera cam;
        public static List<Solid> Solids = new List<Solid>();
        public static List<Actor> Entities = new List<Actor>();
        public static List<Actor> EntitiesToAdd = new List<Actor>();
        public static List<Actor> EntitiesToRemove = new List<Actor>();
        public static Dictionary<Type, List<Actor>> EntitiesByType = new Dictionary<Type, List<Actor>>();

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
            player = (Player)Instantiate(new Player(new Vector2(ScreenSize.X / 2, ScreenSize.Y - 300), 32, 32));
            Debug.Log(player.Pos);
            //Instantiate(new Goomba(new Vector2(graphics.PreferredBackBufferWidth / 2 + 200, graphics.PreferredBackBufferHeight - 100), 30, 30));

            cam = new Camera(ScreenSize / 2, 0, 1f);
            base.Initialize();

            map = new Map(Vector2.Zero, 60, 60, new int[8, 20] {
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
            });

            foreach (Solid s in map.data.solids)
                Solids.Add(s);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Drawing.Init(spriteBatch, Content.Load<SpriteFont>("font"));
            Player.Sprites.Add(Player.SpriteStates.Idle, Content.Load<Texture2D>("idle"));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Input.UpdateState();

            Deltatime = (float) gameTime.ElapsedGameTime.TotalSeconds * TimeScale;

            if (Input.GetKeyDown(Keys.F3))
                Debug.DebugMode = !Debug.DebugMode;

            foreach (Actor e in Entities)
                e.Update();

            foreach (Actor e in EntitiesToAdd)
                Entities.Add(e);
            EntitiesToAdd.Clear();

            foreach (Actor e in EntitiesToRemove)
                Entities.Remove(e);
            EntitiesToRemove.Clear();

            cam.Update();

            Input.UpdateOldState();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, cam.ViewMatrix);
            map.Render();

            foreach (Actor e in Entities)
                e.Render();
            foreach (Solid s in Solids)
                s.Render();

            
            spriteBatch.End();

            spriteBatch.Begin();
            Drawing.DebugString();
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static Actor Instantiate(Actor entity)
        {
            EntitiesToAdd.Add(entity);
            return entity;
        }

        public static void Destroy(Actor entity)
        {
            EntitiesToRemove.Add(entity);
            Type t = entity.GetType();
            EntitiesByType[t].Remove(entity);
            if (EntitiesByType[t].Count == 0)
                EntitiesByType.Remove(t);
        }
    }
}