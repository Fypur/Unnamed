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
        private const float wallJumpSideForce = 620f;
        private const float jumpForce = 600;
        private const float constJumpTime = 0.4f;
        private const float invinciblityTime = 1.5f;

        private int facing = 1;
        bool invicible;

        private int movingDir;
        private float jumpTime;
        private bool isJumping;
        private bool isWallJumping;
        private bool wallJumpingDirection;

        private bool onGround;
        private bool onWall;
        private bool onRightWall;

        float otherSpeed;

        public Player(Vector2 position, int width, int height) : base(position, width, height, gravityScale)
        {

        }

        public override void Update()
        {
            onGround = CollideAt(Platformer.Solids, Pos + new Vector2(0, 1));
            onWall = CollideAt(Platformer.Solids, Pos + new Vector2(1, 0)) ||
                CollideAt(Platformer.Solids, Pos + new Vector2(-1, 0));
            onRightWall = CollideAt(Platformer.Solids, Pos + new Vector2(Width + 1, 0));

            base.Update();

            if (Input.GetKey(Keys.Right))
                movingDir = 1;
            if (Input.GetKey(Keys.Left))
                movingDir = -1;
            if(!Input.GetKey(Keys.Right) && !Input.GetKey(Keys.Left))
                movingDir = 0;

            if (movingDir != 0)
                facing = movingDir;

            velocity.X = movingDir * speed + otherSpeed;
            otherSpeed *= 0.95f;
            if (otherSpeed <= 1 && otherSpeed >= -1)
                otherSpeed = 0;

            if (Input.GetKeyDown(Keys.Space) && onGround || isJumping)
                Jump();
            else if (Input.GetKeyDown(Keys.Space) && onWall || isWallJumping)
                WallJump();

            if (Input.GetKeyDown(Keys.C))
                Debug.Clear();

            if((onGround | onWall) &&  !isJumping && !isWallJumping)
                jumpTime = constJumpTime;

            if (onGround)
            {
                if (velocity.Y > 0)
                    velocity.Y = 0;
            }
            else
                Gravity();

            if (CollidedWithEntityOfType(Pos + new Vector2(0, 7), out Goomba goomba) &&
                !CollidedWithEntity(goomba, Pos + new Vector2(1, 0)) &&
                !CollidedWithEntity(goomba, Pos + new Vector2(-1, 0)))
            {
                Platformer.Destroy(goomba);
                jumpTime = constJumpTime;
                Jump();
            }
            else if (!invicible)
            {
                if (CollidedWithEntityOfType(Pos + new Vector2(1, 0), out goomba) &&
                    !CollidedWithEntity(goomba, Pos + new Vector2(-1, 0)))
                    Damage(-1);
                else if (CollidedWithEntityOfType<Goomba>(Pos + new Vector2(-1, 0)))
                    Damage(1);
            }

            Debug.LogUpdate(invicible);
            MoveX(velocity.X * Platformer.Deltatime, CollisionX);
            MoveY(velocity.Y * Platformer.Deltatime, CollisionY);
        }

        void CollisionX()
        {
            velocity.X = 0;
        }

        void CollisionY()
        {
            velocity.Y = 0;
        }

        public void Damage(int direction)
        {
            /*Platformer.Instantiate(new Player(new Vector2(Platformer.graphics.PreferredBackBufferWidth / 2, Platformer.graphics.PreferredBackBufferHeight - 100), 32, 60));
            Platformer.Destroy(this);*/
            otherSpeed += 300 * direction;
            velocity.Y -= 100;
            invicible = true;
            Timer timer = new Timer(invinciblityTime, true,() => invicible = false);
            AddComponent(timer);
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
                jumpTime -= Platformer.Deltatime * 2;
            }
        }

        public override void Render()
        {
            Drawing.Draw(Sprites[SpriteStates.Idle], Pos, null, Color.White, 0, Vector2.Zero,
                new Vector2(Width / Sprites[SpriteStates.Idle].Width, Height / Sprites[SpriteStates.Idle].Height), facing == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            Drawing.DrawEdge(new Rectangle((int)Pos.X, (int)Pos.Y, Width, Height), 1, Color.Red);
        }
    }
}