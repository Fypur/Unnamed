using Basic_platformer.Components;
using Basic_platformer.Solids;
using Basic_platformer.Utility;
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
        #region constants

        private enum States { Idle, Running, Jumping, Falling, Dashing, Swinging, WallSliding, Pulling }

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

        private const float maxGrappleDist = 1000f;
        #endregion

        #region variables
        private readonly StateMachine<States> stateMachine;

        private int xMoving;
        private int yMoving;
        private int facing = 1;
        private bool normalMouvement = true;
        private bool invicible;
        private bool hasDashed;
        private bool isUnsticking;

        private float distanceToGrapplingPoint;
        private Vector2 grapplingPos;
        private bool isAtSwingEnd;

        private Vector2 respawnPoint;

        #endregion

        #region check variables
        private bool onGround;
        private bool onWall;
        private bool onRightWall;
        private bool collisionX;
        private bool collisionY;

        #endregion

        public Player(Vector2 position, int width, int height) : base(position, width, height, constGravityScale) 
        {
            stateMachine = new StateMachine<States>(States.Idle);
            stateMachine.RegisterStateFunctions(States.Jumping, null, () => { if (Velocity.Y > 0) stateMachine.Switch(States.Falling); }, null);
            AddComponent(stateMachine);
            respawnPoint = position;
        }

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

            #region calulating the moving direction and the facing direction
            {
                if (Input.GetKey(Keys.Right) || Input.GetKey(Keys.D))
                    xMoving = 1;
                if (Input.GetKey(Keys.Left) || Input.GetKey(Keys.Q))
                    xMoving = -1;
                if (!((Input.GetKey(Keys.Right) || Input.GetKey(Keys.D)) ^ (Input.GetKey(Keys.Left) || Input.GetKey(Keys.Q))))
                    xMoving = 0;

                if (Input.GetKey(Keys.Up) || Input.GetKey(Keys.Z))
                    yMoving = -1;
                if (Input.GetKey(Keys.Down) || Input.GetKey(Keys.S))
                    yMoving = 1;
                if (!((Input.GetKey(Keys.Up) || Input.GetKey(Keys.Z)) ^ (Input.GetKey(Keys.Down) || Input.GetKey(Keys.S))))
                    yMoving = 0;

                if (xMoving != 0)
                    facing = xMoving;
            }

            #endregion

            #region Horizontal

            #region Velocity calculation and clamping
            {
                if (normalMouvement && onGround)
                    Velocity.X += xMoving * acceleration - friction * Velocity.X;
                else if (normalMouvement && isAtSwingEnd)
                    Velocity.X += xMoving * swingAcceleration;
                else if (normalMouvement && !onWall)
                    Velocity.X += xMoving * airAcceleration - airFriction * Velocity.X;

                if (normalMouvement && !stateMachine.Is(States.Swinging))
                    Velocity.X = Math.Clamp(Velocity.X, -maxSpeed, maxSpeed);
                if (Velocity.X <= 1 && Velocity.X >= -1)
                    Velocity.X = 0;
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
                    if (Velocity.Y > 0)
                        Velocity.Y = 0;
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
                    ThrowRope();
                }
                if (Input.GetKey(Keys.A) && stateMachine.Is(States.Swinging))
                    Swing(grapplingPos, distanceToGrapplingPoint);
                else if(Input.GetKeyUp(Keys.A))
                {
                    Velocity.X *= 1.5f;
                    if (Velocity.Y < 0)
                        Velocity.Y *= 1.4f;
                    else
                        Velocity.Y *= 0.7f;
                    stateMachine.Switch(States.Jumping);
                    isAtSwingEnd = false;
                }
            }
            #endregion

            #region Entity Collisions
            if (CollideAt(Platformer.Map.Data.Solids, Pos))
                Death();

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

            if (onGround && xMoving == 0 && normalMouvement && !stateMachine.Is(States.Swinging))
                stateMachine.Switch(States.Idle);
            else if (onGround && !stateMachine.Is(States.Swinging) && normalMouvement)
                stateMachine.Switch(States.Running);

            collisionX = collisionY = false;
            MoveX(Velocity.X * Platformer.Deltatime, CollisionX);
            MoveY(Velocity.Y * Platformer.Deltatime, CollisionY);
        }

        private void ThrowRope()
        {
            #region Determining grappling point
            Solid grappledSolid = null;
            Solid reserveGrappledSolid = null;
            float distance = maxGrappleDist;
            float reserveDistance = maxGrappleDist;

            foreach(Solid g in Platformer.Map.Data.GrapplingSolids)
            {
                float d = Vector2.Distance(Pos, g.Pos);

                if (d < distance)
                {
                    Vector2 dir = g.Pos - Pos;
                    bool onRightDir = true;

                    if (!(((Math.Sign(dir.X) == xMoving || Math.Sign(dir.X) == 0) && (Math.Sign(dir.Y) == yMoving || Math.Sign(dir.Y) == 0)) || (xMoving == 0 && yMoving == 0)))
                    {
                        if (d > reserveDistance)
                            continue;
                        onRightDir = false;
                    }

                    if (g is GrapplingTrigger trigger && !trigger.Active)
                        continue;

                    Raycast ray = new Raycast(Pos + new Vector2(Width / 2, Height / 2), g.Pos + new Vector2(g.Width, g.Height));
                    if (!ray.hit)
                    {
                        if (onRightDir)
                        {
                            distance = d;
                            grappledSolid = g;
                        }
                        else
                        {
                            reserveGrappledSolid = g;
                            reserveDistance = d;
                        }
                    }
                }
            }

            if (grappledSolid == null)
            {
                if (reserveGrappledSolid != null)
                {
                    grappledSolid = reserveGrappledSolid;
                    distance = reserveDistance;
                }
                else
                    return;
            }

            #endregion

            #region Acting Accordingly depending on Grappled Object
            if (grappledSolid.GetType() == typeof(GrapplingPoint))
            {
                stateMachine.Switch(States.Swinging);
                distanceToGrapplingPoint = distance;
                grapplingPos = grappledSolid.Pos;

                grapplingPos = grappledSolid.Pos +
                new Vector2(grappledSolid.Width / 2, grappledSolid.Height / 2);

                AddComponent(new LineRenderer(Pos, grappledSolid.Pos, 4, Color.Blue,
                        (line) => { if (!stateMachine.Is(States.Swinging)) RemoveComponent(line); },
                    (line) => {
                        line.StartPos = Pos + new Vector2(Width / 2, Height / 2);
                        line.EndPos = grappledSolid.Pos + new Vector2(grappledSolid.Width / 2, grappledSolid.Height / 2);
                    }));
            }
            else if(grappledSolid is GrapplingTrigger trigger)
            {
                stateMachine.Switch(States.Pulling);
                trigger.Active = false;
                bool deactivateLine = false;

                AddComponent(new Timer(0.1f, true, 
                    (timer) => {
                        Velocity.Y *= 0.5f;
                    },
                    () => {
                    trigger.Pulled();
                    deactivateLine = true;
                    stateMachine.Switch(States.Jumping);
                    Velocity.Y = -500; }));

                AddComponent(new LineRenderer(Pos, grappledSolid.Pos, 4, Color.Blue, 
                    (line) => { if (deactivateLine) RemoveComponent(line); },
                    (line) => {
                        line.StartPos = Pos + new Vector2(Width / 2, Height / 2);
                        line.EndPos = grappledSolid.Pos;
                    }));
            }

            #endregion
        }

        private void Swing(Vector2 grapplePos, float ropeLength)
        {
            Vector2 testPos = Pos + Velocity * Platformer.Deltatime;

            if ((grapplePos - testPos).Length() > ropeLength)
            {
                testPos = grapplePos + Vector2.Normalize(testPos - grapplePos) * ropeLength;
                Velocity = (testPos - Pos) / Platformer.Deltatime;
                isAtSwingEnd = true;
            }
            else
                isAtSwingEnd = false;
        }

        public void Death()
        {
            Pos = respawnPoint;
            Velocity = Vector2.Zero;
        }

        private void Jump()
        {
            stateMachine.Switch(States.Jumping);
            AddComponent(new Timer(maxJumpTime, true, (timer) =>
            {
                if (collisionY || hasDashed)
                    timer.End();

                if (!Input.GetKey(Keys.Space))
                    timer.TimeScale = 10;

                Velocity.Y = -jumpForce * (timer.Value / maxJumpTime);
            }, () => { if (stateMachine.Is(States.Jumping)) stateMachine.Switch(States.Falling); } ));
        }

        private void Dash()
        {
            hasDashed = true;
            stateMachine.Switch(States.Dashing);

            int dir = facing;
            Velocity.X += dashSpeed * dir;

            AddComponent(new Timer(dashTime, true, (timer) => {
                if (collisionX)
                    timer.End();
                else
                {
                    Velocity.Y = 0;
                    normalMouvement = false;
                }
            } 
            , () => normalMouvement = true));
        }

        private void WallJump()
        {
            stateMachine.Switch(States.Jumping);
            int wallJumpingDirection = onRightWall ? -1 : 1;
            facing = -facing;
            Velocity.X = wallJumpSideForce * wallJumpingDirection;
            Velocity.Y -= jumpForce * 0.5f;

            AddComponent(new Timer(maxJumpTime, true, (timer) =>
            {
                if (collisionY || hasDashed)
                    timer.End();

                if (!Input.GetKey(Keys.Space))
                    timer.TimeScale = 2;

                if(!collisionY)
                    Velocity.Y = -jumpForce * timer.Value / maxJumpTime;

            }, () => { if (stateMachine.Is(States.Jumping)) stateMachine.Switch(States.Falling); } ));
        }

        private void WallSlide()
        {
            stateMachine.Switch(States.WallSliding);
            if(Velocity.Y > 0)
                gravityScale = 0.5f * constGravityScale;

            if (!isUnsticking && xMoving != 0 && (xMoving < 0) == onRightWall)
            {
                isUnsticking = true;
                AddComponent(new Timer(unstickTime, true, (timer) =>
                {
                    if (xMoving != 0 && (xMoving < 0) != onRightWall || !onWall)
                    {
                        isUnsticking = false;
                        RemoveComponent(timer);
                    }
                }, () => { Velocity.X += xMoving * 4; isUnsticking = false; stateMachine.Switch(States.Falling); }));
            }
        }

        void CollisionX()
        {
            Velocity.X = 0;
            collisionX = true;
        }

        void CollisionY()
        {
            Velocity.Y = 0;
            collisionY = true;
        }

        public void Damage(int direction)
        {
            /*Platformer.Instantiate(new Player(new Vector2(Platformer.graphics.PreferredBackBufferWidth / 2, Platformer.graphics.PreferredBackBufferHeight - 100), 32, 60));
            Platformer.Destroy(this);*/ //Player death code (could also just move to position)
            Velocity.X += 300 * direction;
            Velocity.Y -= 200;
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