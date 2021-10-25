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
        private const float jumpForce = 1000;
        private const float constJumpTime = 0.4f;
        private float jumpTime;
        private bool isJumping;

        private bool onGround;
        

        public Player(Vector2 position, int width, int height) : base(position, width, height)
        {
            
        }

        public override void Update()
        {
            onGround = CollideAt(Platformer.Solids, Pos + new Vector2(0, 1));

            float movingSpeed = 0;
            if (Input.GetKey(Keys.Right))
                movingSpeed += speed * Platformer.Deltatime;
            if (Input.GetKey(Keys.Left))
                movingSpeed -= speed * Platformer.Deltatime;

            velocity.X += movingSpeed;

            if (Input.GetKeyDown(Keys.Space) && onGround || isJumping)
                Jump();

            if (onGround)
                jumpTime = constJumpTime;
            else
                Gravity(0.5f);

            Debug.Log(onGround);

            MoveX(velocity.X, null);
            MoveY(velocity.Y, null);
            velocity = Vector2.Zero;
        }
        
        void Jump()
        {
            isJumping = true;
            if (jumpTime > 0 && Input.GetKey(Keys.Space))
            {
                velocity.Y -= jumpForce * (jumpTime/constJumpTime) * Platformer.Deltatime;
                jumpTime -= Platformer.Deltatime;
            }
            else if(jumpTime <= 0)
                isJumping = false;
            else
            {
                velocity.Y -= jumpForce * (jumpTime / constJumpTime) * Platformer.Deltatime;
                jumpTime -= Platformer.Deltatime * 2;
            }
        }

        public override void Render()
        {
            Drawing.Draw(Sprites[SpriteStates.Idle], new Rectangle((int) Pos.X, (int)Pos.Y, Width, Height), Color.White);        }
    }
}
