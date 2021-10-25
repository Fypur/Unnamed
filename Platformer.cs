using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Basic_platformer
{
    public class Platformer : Game
    {
        public static Platformer instance;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static float Deltatime;

        public static List<Solid> Solids = new List<Solid>();
        Player player;

        public Platformer()
        {
            instance = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            player = new Player(new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight - 40 - 128), 32, 60);
            Solids.Add(new Platform(new Vector2(0, graphics.PreferredBackBufferHeight - 30), graphics.PreferredBackBufferWidth, 30, Color.White));
            Solids.Add(new Platform(new Vector2(graphics.PreferredBackBufferWidth - 30, 0), 30, graphics.PreferredBackBufferHeight, Color.White));
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Drawing.Init(spriteBatch, Content.Load<SpriteFont>("font"));
            Player.Sprites.Add(Player.SpriteStates.Idle, Content.Load<Texture2D>("idle"));
            
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Input.UpdateState();

            Deltatime = (float) gameTime.ElapsedGameTime.TotalSeconds;

            player.Update();

            Input.UpdateOldState();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            player.Render();
            foreach (Solid s in Solids)
                s.Render();

            Drawing.DebugString();
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
