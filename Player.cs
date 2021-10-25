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

        private const float gravityScale = 200;
        private const float speed = 300f; //in Pixel Per Second
        private const float wallJumpSpeed = 100f;
        private const float wallJumpSideForce = 600f;
        private const float jumpForce = 600;
        private const float constJumpTime = 0.4f;

        private int facing = 1;

        private int movingDir;
        private float jumpTime;
        private bool isJumping;
        private bool isWallJumping;
        private bool wallJumpingDirection;

        private bool onGround;
        private bool onWall;
        private bool onRightWall;

        public Player(Vector2 position, int width, int height) : base(position, width, height, gravityScale)
        {

        }

        public override void Update()
        {
            onGround = CollideAt(Platformer.Solids, Pos + new Vector2(0, 1));
            onWall = CollideAt(Platformer.Solids, Pos + new Vector2(1, 0)) ||
                CollideAt(Platformer.Solids, Pos + new Vector2(-1, 0));
            onRightWall = CollideAt(Platformer.Solids, Pos + new Vector2(Width + 1, 0));

            if (Input.GetKey(Keys.Right))
                movingDir = 1;
            if (Input.GetKey(Keys.Left))
                movingDir = -1;
            if(!Input.GetKey(Keys.Right) && !Input.GetKey(Keys.Left))
                movingDir = 0;

            if (movingDir != 0)
                facing = movingDir;

            velocity.X = movingDir * speed;

            if (Input.GetKeyDown(Keys.Space) && onGround || isJumping)
                Jump();
            else if (Input.GetKeyDown(Keys.Space) && onWall || isWallJumping)
                WallJump();

            if((onGround | onWall) &&  !isJumping && !isWallJumping)
                jumpTime = constJumpTime;

            if (onGround)
            {
                if (velocity.Y > 0)
                    velocity.Y = 0;
            }
            else
                Gravity();

            Debug.Log(facing);

            MoveX(velocity.X * Platformer.Deltatime, null);
            MoveY(velocity.Y * Platformer.Deltatime, null);
        }

        void Jump()
        {
            isJumping = true;
            if (jumpTime > 0 && Input.GetKey(Keys.Space))
            {
                velocity.Y = -jumpForce * (jumpTime / constJumpTime);
                jumpTime -= Platformer.Deltatime;
            }
            else if (jumpTime <= 0)
            {
                isJumping = false;
            }   
            else
            {
                velocity.Y = -jumpForce * (jumpTime / constJumpTime);
                jumpTime -= Platformer.Deltatime * 10;
            }
        }

        private void WallJump()
        {
            isWallJumping = true;
            if (jumpTime == constJumpTime)
                wallJumpingDirection = onRightWall;

            if (jumpTime > 0 && Input.GetKey(Keys.Space))
            {
                velocity.Y = -jumpForce * (jumpTime / constJumpTime);
                velocity.X = wallJumpSideForce * (wallJumpingDirection ? -1 : 1) * (jumpTime / constJumpTime) + wallJumpSpeed * movingDir;
                jumpTime -= Platformer.Deltatime;
            }
            else if (jumpTime <= 0)
                isWallJumping = false;
            else
            {
                velocity.Y = -jumpForce * (jumpTime / constJumpTime);
                velocity.X = wallJumpSideForce * (wallJumpingDirection ? -1 : 1) * (jumpTime / constJumpTime) + wallJumpSpeed * movingDir;
                jumpTime -= Platformer.Deltatime * 10;
            }
        }

        public override void Render()
        {
            Rectangle playerRect = new Rectangle((int)Pos.X, (int)Pos.Y, Width, Height);
            Drawing.Draw(Sprites[SpriteStates.Idle], Pos, null, Color.White, 0, Vector2.Zero,
                new Vector2(Width / Sprites[SpriteStates.Idle].Width, Height / Sprites[SpriteStates.Idle].Height), facing == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
        }
    }
}
