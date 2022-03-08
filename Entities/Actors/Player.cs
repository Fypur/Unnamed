using Fiourp;
using Fiourp;
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

        private const float maxSpeed = 250;
        private const float maxFallingSpeed = 300;
        private const float dashSpeed = 200;
        
        private const float acceleration = 70f;
        private const float airAcceleration = 15f;
        private const float swingAcceleration = 3f;
        private const float friction = 0.4f;
        private const float airFriction = 0.1f;
        
        private const float wallJumpSideForce = 200f;
        private const float jumpForce = 200f;
        private const float constGravityScale = 1.2f;
        
        private const float maxJumpTime = 0.4f;
        private const float dashTime = 0.2f;
        private const float invinciblityTime = 1.5f;
        private const float unstickTime = 0.1f;
        
        private const float maxGrappleDist = 2000f;

        private readonly Texture2D idleTexture;
        #endregion

        #region variables
        private readonly StateMachine<States> stateMachine;

        public bool canMove = true;

        private int xMoving;
        private int yMoving;
        private int facing = 1;
        private bool normalMouvement = true;
        //private bool invicible;
        private bool hasDashed;
        private bool isUnsticking;

        private float totalRopeLength;
        private Solid grappledSolid;
        private List<Vector2> grapplePositions = new List<Vector2>();
        private List<int> grapplePositionsSign = new List<int>() { 0 };
        private bool isAtSwingEnd;

        public Vector2 respawnPoint;

        #endregion

        #region check variables
        private bool onGround;
        private bool onWall;
        private bool onRightWall;
        private bool collisionX;
        private bool collisionY;

        #endregion
        
        public Player(Vector2 position, int width, int height, Texture2D idleTexture) : base(position, width, height, constGravityScale, new Sprite(Color.Red)) 
        {
            Engine.Player = this;
            stateMachine = new StateMachine<States>(States.Idle);
            stateMachine.RegisterStateFunctions(States.Jumping, null, () => { if (Velocity.Y > 0) stateMachine.Switch(States.Falling); }, null);
            AddComponent(stateMachine);

            this.idleTexture = idleTexture;
            respawnPoint = position;
        }

        public override void Update()
        {
            if (!canMove) return;
            Debug.LogUpdate(respawnPoint);
            #region Checks for Ground and Wall
            onGround = Collider.CollideAt(Pos + new Vector2(0, 1));
            onRightWall = Collider.CollideAt(Pos + new Vector2(1, 0));
            onWall = Collider.CollideAt(Pos + new Vector2(-1, 0)) || onRightWall;
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

            #endregion

            #region Vertical
            {
                if (onWall && !onGround && !stateMachine.Is(States.Swinging))
                    WallSlide();
                else
                    gravityScale = constGravityScale;

                if (Input.GetKeyDown(Keys.Space) && onGround)
                    Jump();
                else if (Input.GetKeyDown(Keys.Space) && onWall)
                    WallJump();

                if (onGround)
                {
                    if (Velocity.Y > 0)
                        Velocity.Y = 0;
                }
                else
                    Gravity();

                Velocity.Y = Math.Min(Velocity.Y, maxFallingSpeed);
            }
            #endregion

            #region Horizontal and Vertical
            {
                if (Input.GetKeyDown(Keys.E) && !hasDashed)
                    Dash();

                if(onGround || onWall)
                    hasDashed = false;

                if (Input.GetKeyDown(Keys.A))
                    ThrowRope();
                if (Input.GetKey(Keys.A) && stateMachine.Is(States.Swinging))
                    Swing();
                else if(Input.GetKeyUp(Keys.A))
                {
                    Velocity.X *= 1.2f;
                    if (Velocity.Y < 0)
                        Velocity.Y *= 1.1f;
                    else
                        Velocity.Y *= 0.7f;

                    grapplePositions.Clear();
                    grapplePositionsSign = new List<int> { 0 };
                    stateMachine.Switch(States.Jumping);
                    isAtSwingEnd = false;
                }
            }
            #endregion

            #region Entity Collisions
            /*if (CollideAt(Engine.CurrentMap.Data.Solids, Pos))
                Death();

            if (CollidedWithEntityOfType(Pos + new Vector2(0, 7), out Goomba goomba) &&
                !CollidedWithEntity(goomba, Pos + new Vector2(1, 0)) &&
                !CollidedWithEntity(goomba, Pos + new Vector2(-1, 0)))
            {
                Engine.CurrentMap.Destroy(goomba);
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
            }*/
            #endregion

            if (onGround && xMoving == 0 && normalMouvement && !stateMachine.Is(States.Swinging))
                stateMachine.Switch(States.Idle);
            else if (onGround && !stateMachine.Is(States.Swinging) && normalMouvement)
                stateMachine.Switch(States.Running);

            collisionX = collisionY = false;
            Debug.LogUpdate(LiftSpeed);
            MoveX(Velocity.X * Engine.Deltatime, CollisionX);
            MoveY(Velocity.Y * Engine.Deltatime, CollisionY);
        }

        public override bool IsRiding(Solid solid)
            => (base.IsRiding(solid)
                || ((Collider.CollideAt(solid, Pos + new Vector2(1, 0))
            || Collider.CollideAt(solid, Pos + new Vector2(-1, 0))) && stateMachine.Is(States.WallSliding))) && !stateMachine.Is(States.Jumping);

        public override void Squish()
            => Death();

        private void ThrowRope()
        {
            #region Determining grappling point
            Fiourp.Solid determinedGrappledSolid = null;
            Fiourp.Solid reserveGrappledSolid = null;
            float distance = maxGrappleDist;
            float reserveDistance = maxGrappleDist;

            foreach(Fiourp.Solid g in GrapplingPoint.GrapplingSolids)
            {
                float d = Vector2.Distance(Pos + new Vector2(Width / 2, Height / 2), g.Pos);

                if (d < distance)
                {
                    Vector2 dir = g.Pos - Pos;
                    bool onRightDir = true;
                    int signX = Math.Sign(dir.X), signY = Math.Sign(dir.Y);

                    if ( !( (xMoving == signX || xMoving == 0 ) && (yMoving == signY || yMoving == 0 ) ) )
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
                            determinedGrappledSolid = g;
                        }
                        else
                        {
                            reserveGrappledSolid = g;
                            reserveDistance = d;
                        }
                    }
                }
            }

            if (determinedGrappledSolid == null)
            {
                if (reserveGrappledSolid != null)
                {
                    determinedGrappledSolid = reserveGrappledSolid;
                    distance = reserveDistance;
                }
                else
                    return;
            }

            grappledSolid = determinedGrappledSolid;

            #endregion

            #region Acting Accordingly depending on Grappled Object
            if (determinedGrappledSolid is ISwinged swinged)
            {
                swinged.OnGrapple(this);
                stateMachine.Switch(States.Swinging);
                totalRopeLength = distance;

                Vector2 grapplingPos = determinedGrappledSolid.Pos;

                grapplingPos = determinedGrappledSolid.Pos +
                new Vector2(determinedGrappledSolid.Width / 2, determinedGrappledSolid.Height / 2);

                grapplePositions.Add(grapplingPos);

                AddComponent(new LineRenderer(new List<Vector2> { Pos, grapplingPos }, 2, Color.Blue,
                        (line) => { if (!stateMachine.Is(States.Swinging)) RemoveComponent(line); },
                    (line) => {

                        List<Vector2> linePositions = new List<Vector2>() { Pos + new Vector2(Width / 2, Height / 2) };
                        List<Vector2> reversedPositions = new List<Vector2>(grapplePositions);
                        reversedPositions.Reverse();
                        linePositions.AddRange(reversedPositions);
                        line.Positions = linePositions;

                    }));
            }
            else if(determinedGrappledSolid is GrapplingTrigger trigger)
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

                AddComponent(new LineRenderer(Pos, determinedGrappledSolid.Pos, 2, Color.Blue, 
                    (line) => { if (deactivateLine) RemoveComponent(line); },
                    (line) => {
                        line.Positions[0] = Pos + new Vector2(Width / 2, Height / 2);
                        line.Positions[1] = determinedGrappledSolid.Pos;
                    }));
            }

            #endregion
        }

        private void Swing()
        {
            #region Swinging

            Vector2 grapplePos = grapplePositions[grapplePositions.Count - 1];

            float ropeLength = totalRopeLength;
            for (int i = 0; i < grapplePositions.Count - 1; i++)
                ropeLength -= Vector2.Distance(grapplePositions[i], grapplePositions[i + 1]);

            Vector2 testPos = ExactPos + HalfSize + Velocity * Engine.Deltatime;
            
            if ((grapplePos - testPos).Length() > ropeLength)
            {
                testPos = grapplePos + Vector2.Normalize(testPos - grapplePos) * ropeLength;
                Velocity = (testPos - ExactPos - HalfSize) / Engine.Deltatime;
                isAtSwingEnd = true;
            }
            else
                isAtSwingEnd = false;
            
            #endregion
            
            #region Determining the right position to Swing to (Rope colliding with terrain)

            grapplePositions[0] = grappledSolid.Pos + new Vector2(grappledSolid.Width / 2, grappledSolid.Height / 2);

            RemoveGrapplingPoints();

            List<Vector2> cornersToCheck = new List<Vector2>(Engine.CurrentMap.CurrentLevel.Corners);
            foreach (Vector2 point in grapplePositions)
                cornersToCheck.Remove(point);

            AddGrapplingPoints(cornersToCheck, grapplePositions[grapplePositions.Count - 1]);
            #endregion

            #region Grappling Methods

            void AddGrapplingPoints(List<Vector2> cornersToCheck, Vector2 checkingFrom)
            {
                float angle = VectorHelper.GetAngle(checkingFrom - Pos - HalfSize, checkingFrom - Pos - HalfSize - Velocity * Engine.Deltatime);

                if (angle == 0)
                    return;

                float distanceFromPoint = Vector2.Distance(Pos + HalfSize + Velocity * Engine.Deltatime, checkingFrom);

                float closestAngle = angle;
                Vector2? closestPoint = null;

                List<Vector2> nextCorners = new List<Vector2>();

                foreach (Vector2 corner in cornersToCheck)
                {
                    float cornerDistance = Vector2.Distance(checkingFrom, corner);
                    if (cornerDistance > distanceFromPoint)
                    {
                        continue;
                    }

                    float pointAngle = VectorHelper.GetAngle(checkingFrom - Pos - HalfSize, checkingFrom - corner);

                    if (pointAngle * Math.Sign(angle) >= 0 && pointAngle * Math.Sign(angle) <= angle * Math.Sign(angle))
                    {
                        if (pointAngle * Math.Sign(closestAngle) <= closestAngle * Math.Sign(closestAngle))
                        {
                            closestAngle = pointAngle;
                            closestPoint = corner;
                        }

                        nextCorners.Add(corner);
                    }
                }

                if (closestPoint is Vector2 foundCorner)
                {
                    grapplePositions.Add(foundCorner);
                    grapplePositionsSign.Add(Math.Sign(angle));
                    if (foundCorner == new Vector2(660, 840))
                    {
                        //Debug.Pause();
                    }

                    nextCorners.Remove(foundCorner);

                    if (nextCorners.Count > 0)
                        AddGrapplingPoints(nextCorners, foundCorner);
                }
            }

            void RemoveGrapplingPoints()
            {
                for (int i = grapplePositions.Count - 1; i >= 1; i--)
                {
                    float grappleAngle = VectorHelper.GetAngle(grapplePositions[i - 1] - Pos - HalfSize, grapplePositions[i - 1] - grapplePositions[i]);

                    if (Math.Sign(grappleAngle) == grapplePositionsSign[i])
                    {
                        grapplePositions.RemoveAt(i);
                        grapplePositionsSign.RemoveAt(i);
                    }
                    else
                        break;
                }
            }

            #endregion
        }

        public void Death()
        {
            Pos = respawnPoint;
            Velocity = Vector2.Zero;
            stateMachine.Switch(States.Idle);
        }

        private void Jump()
        {
            stateMachine.Switch(States.Jumping);
            Velocity.X += LiftBoost.X;
            AddComponent(new Timer(maxJumpTime, true, (timer) =>
            {
                if (collisionY || hasDashed)
                    timer.End();

                if (!Input.GetKey(Keys.Space))
                    timer.TimeScale = 10;

                Velocity.Y = (-jumpForce) * (timer.Value / maxJumpTime) + LiftBoost.Y;
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

        /*public void Damage(int direction)
        {
            Death();
            Velocity.X += 300 * direction;
            Velocity.Y -= 200;
            invicible = true;
            Timer timer = new Timer(invinciblityTime, true, null, () => invicible = false);
            AddComponent(timer);
        }*/

        public override void Render()
        {
            //Renderer components
            base.Render();

            Drawing.Draw(new Rectangle((int)Pos.X, (int)Pos.Y, Width, Height), Color.Red);

            /*Drawing.Draw(idleTexture, Pos + new Vector2(Width / 2, Height / 2), null, Color.White, 0, new Vector2(idleTexture.Width / 2, idleTexture.Height / 2),
                Vector2.One * 1.5f, facing == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);*/
        }

        public Vector2 LiftBoost
        {
            get
            {
                Vector2 boost = LiftSpeed;
                if(boost.Y < 0)
                    boost.Y = 0;

                return boost;
            }
        }
    }
}