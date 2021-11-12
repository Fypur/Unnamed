using Basic_platformer.Static_Classes;
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
        private const float airSpeed = 250f;
        private const float wallJumpSpeed = 100f;
        private const float wallJumpSideForce = 620f;
        private const float jumpForce = 600;
        private const float maxJumpTime = 0.4f;
        private const float dashTime = 0.1f;
        private const float dashSpeed = 1700;
        private const float invinciblityTime = 1.5f;

        private int movingDir;
        private int facing = 1;
        private bool normalMouvement = true;
        private bool invicible;

        private bool onGround;
        private bool onWall;
        private bool onRightWall;

        float otherSpeed;

        public Player(Vector2 position, int width, int height) : base(position, width, height, gravityScale) { }

        public override void Update()
        {
            //Checks for Ground and Wall
            {
                onGround = CollideAt(Platformer.Solids, Pos + new Vector2(0, 1));
                onWall = CollideAt(Platformer.Solids, Pos + new Vector2(1, 0)) ||
                    CollideAt(Platformer.Solids, Pos + new Vector2(-1, 0));
                onRightWall = CollideAt(Platformer.Solids, Pos + new Vector2(Width + 1, 0));
            }

            //Components Update
            base.Update();

            //Horizontal
            {
                if (Input.GetKey(Keys.Right) || Input.GetKey(Keys.D))
                    movingDir = 1;
                if (Input.GetKey(Keys.Left) || Input.GetKey(Keys.Q))
                    movingDir = -1;
                if (!(Input.GetKey(Keys.Right) || Input.GetKey(Keys.D)) && !(Input.GetKey(Keys.Left) || Input.GetKey(Keys.Q)))
                    movingDir = 0;

                if (movingDir != 0)
                    facing = movingDir;

                if (normalMouvement && onGround)
                    velocity.X = movingDir * speed;

                if (otherSpeed <= 1 && otherSpeed >= -1)
                    otherSpeed = 0;
                if (Math.Sign(movingDir) != Math.Sign(otherSpeed))
                    otherSpeed *= 0.8f;
                else
                    otherSpeed *= 0.95f;

                if (Input.GetKeyDown(Keys.E))
                    Dash();

                if (normalMouvement && !onGround)
                    velocity.X = movingDir * airSpeed + otherSpeed;
            }

            //Vertical
            {
                if (Input.GetKeyDown(Keys.Space) && onGround)
                    Jump();
                else if (Input.GetKeyDown(Keys.Space) && onWall)
                    WallJump();

                if (Input.GetKeyDown(Keys.C))
                    Debug.Clear();

                if (onGround)
                {
                    if (velocity.Y > 0)
                        velocity.Y = 0;
                }
                else
                    Gravity();
            }

            //Entity Collisions
            {
                if (CollidedWithEntityOfType(Pos + new Vector2(0, 7), out Goomba goomba) &&
                !CollidedWithEntity(goomba, Pos + new Vector2(1, 0)) &&
                !CollidedWithEntity(goomba, Pos + new Vector2(-1, 0)))
                {
                    Platformer.Destroy(goomba);
                    Jump();
                }
                else if (!invicible)
                {
                    if (CollidedWithEntityOfType(Pos + new Vector2(1, 0), out goomba))
                    {
                        if (!CollidedWithEntity(goomba, Pos + new Vector2(-1, 0)))
                            Damage(-1);
                        else
                        {
                            if (Pos.X - goomba.Pos.X < 0)
                                Damage(-1);
                            else
                                Damage(1);
                        }
                    }
                    else if (CollidedWithEntityOfType<Goomba>(Pos + new Vector2(-1, 0)))
                        Damage(1);
                }
            }

            MoveX(velocity.X * Platformer.Deltatime, CollisionX);
            MoveY(velocity.Y * Platformer.Deltatime, CollisionY);
        }
        
        private void Dash()
        {
            normalMouvement = false;
            int dir = facing;
            otherSpeed += dashSpeed * dir;
            AddComponent(new Timer(dashTime, true, (timer) => {
                velocity.X = dir * dashSpeed * Ease.QuintOut(Ease.Reverse(timer.Value / timer.MaxValue));
                velocity.Y = 0;
            }
            , () => {
                normalMouvement = true;
            }
            ));
        }

        private void Jump()
        {
            AddComponent(new Timer(maxJumpTime, true, (timer) =>
            {
                if (!Input.GetKey(Keys.Space))
                    timer.TimeScale = 10;

                velocity.Y = -jumpForce * (timer.Value / maxJumpTime);
            }));
        }

        private void WallJump()
        {
            normalMouvement = false;
            int wallJumpingDirection = onRightWall ? -1 : 1;

            AddComponent(new Timer(maxJumpTime, true, (timer) =>
            {
                if (!Input.GetKey(Keys.Space))
                    timer.TimeScale = 2;

                velocity.Y = -jumpForce * (timer.Value / maxJumpTime);
                velocity.X = wallJumpSideForce * wallJumpingDirection * (timer.Value / maxJumpTime) + wallJumpSpeed * movingDir;
            }, () => normalMouvement = true));
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
            Platformer.Destroy(this);*/ //Player death code (could also just move to position)
            otherSpeed += 300 * direction;
            velocity.Y -= 200;
            invicible = true;
            Timer timer = new Timer(invinciblityTime, true, null, () => invicible = false);
            AddComponent(timer);
        }

        public override void Render()
        {
            Drawing.Draw(new Rectangle((int)Pos.X, (int)Pos.Y, Width, Height), Color.White);
            //Drawing.Draw(Sprites[SpriteStates.Idle], Pos, null, Color.White, 0, Vector2.Zero,
            //new Vector2(Width / Sprites[SpriteStates.Idle].Width, Height / Sprites[SpriteStates.Idle].Height), facing == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            
            if(Debug.DebugMode)
                Drawing.DrawEdge(new Rectangle((int)Pos.X, (int)Pos.Y, Width, Height), 1, Color.Red);
        }
    }
}