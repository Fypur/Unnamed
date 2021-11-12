using Basic_platformer.Solids;
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

        public Player player;

        public static Map map;
        public static Camera cam;
        public static List<Solid> Solids = new List<Solid>();
        public static List<Entity> Entities = new List<Entity>();
        public static List<Entity> EntitiesToAdd = new List<Entity>();
        public static List<Entity> EntitiesToRemove = new List<Entity>();
        public static Dictionary<Type, List<Entity>> EntitiesByType = new Dictionary<Type, List<Entity>>();

        public Platformer()
        {
            instance = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            player = (Player)Instantiate(new Player(new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight - 300), 32, 32));
            Instantiate(new Goomba(new Vector2(graphics.PreferredBackBufferWidth / 2 + 200, graphics.PreferredBackBufferHeight - 100), 30, 30));

            cam = new Camera(Vector2.Zero, 0, 1);

            base.Initialize();

            map = new Map(Vector2.Zero, graphics.PreferredBackBufferWidth / 13, graphics.PreferredBackBufferHeight / 10, new int[10, 13] {
                {1,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1}
            });

            foreach (Solid s in map.solidTiles)
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

            Deltatime = (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (Input.GetKeyDown(Keys.F3))
                Debug.DebugMode = !Debug.DebugMode;

            if(Input.GetKeyDown(Keys.A))
                Instantiate(new Goomba(new Vector2(graphics.PreferredBackBufferWidth / 2 + 200, graphics.PreferredBackBufferHeight - 100), 30, 30));

            foreach (Entity e in Entities)
                e.Update();

            foreach (Entity e in EntitiesToAdd)
                Entities.Add(e);
            EntitiesToAdd.Clear();

            foreach (Entity e in EntitiesToRemove)
                Entities.Remove(e);
            EntitiesToRemove.Clear();

            Input.UpdateOldState();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, cam.ViewMatrix);

            map.Render();

            foreach (Entity e in Entities)
                e.Render();
            foreach (Solid s in Solids)
                s.Render();

            Drawing.DebugString();
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static Entity Instantiate(Entity entity)
        {
            EntitiesToAdd.Add(entity);
            return entity;
        }

        public static void Destroy(Entity entity)
        {
            EntitiesToRemove.Add(entity);
            Type t = entity.GetType();
            EntitiesByType[t].Remove(entity);
            if (EntitiesByType[t].Count == 0)
                EntitiesByType.Remove(t);
        }
    }
}