using Basic_platformer.Components;
using Basic_platformer.Solids;
using Basic_platformer.Static_Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class Player : Actor
    {
        public enum SpriteStates { Idle, Running, Jumping, Falling }
        public static Dictionary<SpriteStates, Texture2D> Sprites = new Dictionary<SpriteStates, Texture2D>();

        #region constants

        private const float maxSpeed = 600f; //in Pixel Per Second
        private const float dashSpeed = 800f;

        private const float acceleration = 200f;
        private const float airAcceleration = 50f;
        private const float swingAcceleration = 10f;
        private const float friction = 0.4f;
        private const float airFriction = 0.1f;
        private const float constGravityScale = 4f;

        private const float wallJumpSideForce = 620f;
        private const float jumpForce = 600f;

        private const float maxJumpTime = 0.4f;
        private const float dashTime = 0.2f;
        private const float invinciblityTime = 1.5f;
        private const float unstickTime = 0.1f;

        private const float maxGrappleDist = 20000f;

        #endregion

        #region variables
        private int movingDir;
        private int facing = 1;
        private bool normalMouvement = true;
        private bool invicible;
        private bool hasDashed;
        private bool isUnsticking;
        private float distanceToGrapplingPoint;
        private bool isSwinging;
        private bool isAtSwingEnd;

        #endregion

        #region check variables
        private bool onGround;
        private bool onWall;
        private bool onRightWall;
        private bool collisionX;
        private bool collisionY;

        #endregion

        public Player(Vector2 position, int width, int height) : base(position, width, height, constGravityScale) { }

        public override void Update()
        {
            #region Checks for Ground and Wall
            onGround = CollideAt(Platformer.Solids, Pos + new Vector2(0, 1));
            onWall = CollideAt(Platformer.Solids, Pos + new Vector2(1, 0)) ||
                CollideAt(Platformer.Solids, Pos + new Vector2(-1, 0));
            onRightWall = CollideAt(Platformer.Solids, Pos + new Vector2(Width + 1, 0));
            #endregion

            #region Component Update
            base.Update();
            #endregion

            #region Horizontal

            #region calulating the moving direction and the facing direction
            {
                if (Input.GetKey(Keys.Right) || Input.GetKey(Keys.D))
                    movingDir = 1;
                if (Input.GetKey(Keys.Left) || Input.GetKey(Keys.Q))
                    movingDir = -1;
                if (!((Input.GetKey(Keys.Right) || Input.GetKey(Keys.D)) ^ (Input.GetKey(Keys.Left) || Input.GetKey(Keys.Q))))
                    movingDir = 0;

                if (movingDir != 0)
                    facing = movingDir;
            }
            
            #endregion

            #region Velocity calculation and clamping
            {
                if (normalMouvement && onGround)
                    velocity.X += movingDir * acceleration - friction * velocity.X;
                else if (normalMouvement && isAtSwingEnd)
                    velocity.X += movingDir * swingAcceleration;
                else if (normalMouvement && !onWall)
                    velocity.X += movingDir * airAcceleration - airFriction * velocity.X;

                if (normalMouvement && !isSwinging)
                    velocity.X = Math.Clamp(velocity.X, -maxSpeed, maxSpeed);
                if (velocity.X <= 1 && velocity.X >= -1)
                    velocity.X = 0;
            }
            #endregion

            if (onWall && !onGround)
                WallSlide();
            else
                gravityScale = constGravityScale;

            #endregion

            #region Vertical
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
            #endregion

            #region Horizontal and Vertical
            {
                if (Input.GetKeyDown(Keys.E) && !hasDashed)
                    Dash();

                if(onGround || onWall)
                    hasDashed = false;

                if (Input.GetKeyDown(Keys.A))
                {
                    GrapplingPoint grapplingPoint = null;
                    ThrowRope(out grapplingPoint, out distanceToGrapplingPoint);

                     if(grapplingPoint != null)
                    {
                        isSwinging = true;

                        Vector2 middlePoint = grapplingPoint.Pos +
                        new Vector2(grapplingPoint.Width / 2, grapplingPoint.Height / 2);

                        AddComponent(new LineRenderer(Pos, middlePoint, 4, Color.Blue,
                            (line) => { if (!isSwinging) RemoveComponent(line); },
                        (line) => {
                            line.StartPos = Pos + new Vector2(Width / 2, Height / 2);
                            line.EndPos = middlePoint;
                        }));
                    }
                }
                if (Input.GetKey(Keys.A) && isSwinging)
                    Swing(Platformer.Map.data.grapplingPoints[0].Pos, distanceToGrapplingPoint);
                else if(Input.GetKeyUp(Keys.A))
                {
                    velocity.X *= 1.5f;
                    if (velocity.Y < 0)
                        velocity.Y *= 1.4f;
                    else
                        velocity.Y *= 0.7f;
                    isSwinging = false;
                    isAtSwingEnd = false;
                }
            }
            #endregion

            #region Entity Collisions
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
            #endregion

            collisionX = collisionY = false;
            MoveX(velocity.X * Platformer.Deltatime, CollisionX);
            MoveY(velocity.Y * Platformer.Deltatime, CollisionY);
        }

        private void ThrowRope(out GrapplingPoint grapplingPoint, out float distance)
        {
            grapplingPoint = null;
            distance = maxGrappleDist;

            foreach(GrapplingPoint g in Platformer.Map.data.grapplingPoints)
            {
                float d = Vector2.Distance(Pos, g.Pos);
                if (d < distance)
                {
                    distance = d;
                    grapplingPoint = g;
                }
            }

            //raycast to check if there is collision in between
        }
        
        private void Swing(Vector2 grapplePos, float ropeLength)
        {
            Vector2 testPos = Pos + velocity * Platformer.Deltatime;

            if ((grapplePos - testPos).Length() > ropeLength)
            {
                testPos = grapplePos + Vector2.Normalize(testPos - grapplePos) * ropeLength;
                velocity = (testPos - Pos) / Platformer.Deltatime;
                isAtSwingEnd = true;
            }
            else
                isAtSwingEnd = false;
        }

        private void Jump()
        {
            AddComponent(new Timer(maxJumpTime, true, (timer) =>
            {
                if (collisionY || hasDashed)
                    timer.End();

                if (!Input.GetKey(Keys.Space))
                    timer.TimeScale = 10;

                velocity.Y = -jumpForce * (timer.Value / maxJumpTime);
            }));
        }

        private void Dash()
        {
            hasDashed = true;

            int dir = facing;
            velocity.X += dashSpeed * dir;

            AddComponent(new Timer(dashTime, true, (timer) => {
                if (collisionX)
                    timer.End();
                else
                {
                    velocity.Y = 0;
                    normalMouvement = false;
                }
            }
            , () => normalMouvement = true));
        }

        private void WallJump()
        {
            int wallJumpingDirection = onRightWall ? -1 : 1;
            facing = -facing;
            velocity.X = wallJumpSideForce * wallJumpingDirection;
            velocity.Y -= jumpForce * 0.5f;

            AddComponent(new Timer(maxJumpTime, true, (timer) =>
            {
                if (hasDashed)
                    timer.End();

                if (!Input.GetKey(Keys.Space))
                    timer.TimeScale = 2;

                if(!collisionY)
                    velocity.Y = -jumpForce * timer.Value / maxJumpTime;

            }, null));
        }

        private void WallSlide()
        {
            if(velocity.Y > 0)
                gravityScale = 0.5f * constGravityScale;

            if (!isUnsticking && movingDir != 0 && (movingDir < 0) == onRightWall)
            {
                isUnsticking = true;
                AddComponent(new Timer(unstickTime, true, (timer) =>
                {
                    if (movingDir != 0 && (movingDir < 0) != onRightWall || !onWall)
                    {
                        isUnsticking = false;
                        RemoveComponent(timer);
                    }
                }, () => { velocity.X += movingDir * 4; isUnsticking = false; }));
            }
        }

        void CollisionX()
        {
            velocity.X = 0;
            collisionX = true;
        }

        void CollisionY()
        {
            velocity.Y = 0;
            collisionY = true;
        }

        public void Damage(int direction)
        {
            /*Platformer.Instantiate(new Player(new Vector2(Platformer.graphics.PreferredBackBufferWidth / 2, Platformer.graphics.PreferredBackBufferHeight - 100), 32, 60));
            Platformer.Destroy(this);*/ //Player death code (could also just move to position)
            velocity.X += 300 * direction;
            velocity.Y -= 200;
            invicible = true;
            Timer timer = new Timer(invinciblityTime, true, null, () => invicible = false);
            AddComponent(timer);
        }

        public override void Render()
        {
            //Renderer components
            base.Render();

            Drawing.Draw(new Rectangle((int)Pos.X, (int)Pos.Y, Width, Height), Color.Red);
            //Drawing.Draw(Sprites[SpriteStates.Idle], Pos, null, Color.White, 0, Vector2.Zero,
            //new Vector2(Width / Sprites[SpriteStates.Idle].Width, Height / Sprites[SpriteStates.Idle].Height), facing == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            
            if(Debug.DebugMode)
                Drawing.DrawEdge(new Rectangle((int)Pos.X, (int)Pos.Y, Width, Height), 1, Color.Red);
        }
    }
}