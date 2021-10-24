using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class Player : Entity
    {
        public enum SpriteStates { Idle, Running, Jumping, Falling }
        public static Dictionary<SpriteStates, Texture2D> Sprites = new Dictionary<SpriteStates, Texture2D>();

        private readonly float speed = 300f; //in Pixel Per Second
        private readonly float jumpForce = 4000;

        private bool onGround;
        

        public Player(Vector2 position, int width, int height) : base(position, width, height)
        {
            
        }

        public override void Update()
        {
            Gravity(3);

            KeyboardState kbState = Keyboard.GetState();
            if (kbState.IsKeyDown(Keys.Right))
                velocity.X += speed;
            if (kbState.IsKeyDown(Keys.Left))
                velocity.X -= speed;
            if (kbState.IsKeyDown(Keys.Space))
                velocity.Y -= jumpForce;

            base.Update();
        }

        public override void Render()
        {
            Drawing.Draw(Sprites[SpriteStates.Idle], new Rectangle((int) Pos.X, (int)Pos.Y, Width, Height), Color.White);
        }
    }
}
