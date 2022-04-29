using Fiourp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platformer
{
    public class Player : Actor
    {
        #region constants

        private enum States { Idle, Running, Jumping, Ascending, Falling, Dashing, Swinging, WallSliding, Pulling, Jetpack }

        private const float maxSpeed = 100;
        private const float maxFallingSpeed = 300;
        private const float dashSpeed = 200;

        private const float jetpackPowerX = 9;
        private const float jetpackPowerY = 15;
        private const float maxJetpackTime = 0.5f;
        
        private const float acceleration = 70f;
        private const float airAcceleration = 15f;
        private const float swingAcceleration = 3f;
        private const float friction = 0.4f;
        private const float airFriction = 0.07f;
        
        private const float wallJumpSideForce = 200f;
        private const float jumpForce = 200f;
        private const float constGravityScale = 1.2f;
        
        private const float maxJumpTime = 0.4f;
        private const float dashTime = 0.2f;
        private const float invinciblityTime = 1.5f;
        private const float unstickTime = 0.1f;
        
        private const float maxGrappleDist = 1000f;

        public static readonly ParticleType Dust = new ParticleType()
        {
            Color = Color.White,
            Size = 2,
            SizeRange = 1,
            LifeMin = 0.05f,
            LifeMax = 0.4f,
            SpeedMin = 5,
            SpeedMax = 30,
            Direction = -90,
            DirectionRange = 45,
            FadeMode = ParticleType.FadeModes.EndLinear,
            SizeChange = ParticleType.FadeModes.EndSmooth
        };

        private readonly ParticleType JetpackParticle;

        #endregion

        #region variables

        private readonly StateMachine<States> stateMachine;

        public bool canMove = true;
        public bool Jetpacking;
        public bool canJetpack = true;
        public Vector2 jetpackPowerCoef = Vector2.One;
        public Vector2 respawnPoint;

        private bool onGround;
        private bool onWall;
        private bool onRightWall;
        private bool collisionX;
        private bool collisionY;
        private bool previousOnGround = true;

        private float xMoving;
        private float yMoving;
        private int xMovingRaw;
        private int yMovingRaw;
        private int facing = 1;

        private bool normalMouvement = true;
        private bool cancelJump;
        private bool hasDashed;
        private bool isUnsticking;
        private float jetpackTime;

        private Vector2 AddedJetpackSpeed;

        private float totalRopeLength;
        private Solid grappledSolid;
        private List<Vector2> swingPositions = new List<Vector2>();
        private List<int> swingPositionsSign = new List<int>() { 0 };
        private bool isAtSwingEnd;

        #endregion


        public Player(Vector2 position) : base(position, 8, 14, constGravityScale, new Sprite(Color.White))
        {
            Engine.Player = this;

            #region Sprite Animations
            Sprite.Rect = Rectangle.Empty;

            Sprite.Add(Sprite.AllAnimData["Player"]);

            Sprite.Play("idle");
            Sprite.Offset = new Vector2(-3, -2);
            //Sprite.Origin = HalfSize;

            #endregion

            #region StateMachine

            stateMachine = new StateMachine<States>(States.Idle);

            stateMachine.RegisterStateFunctions(States.Running, () => Sprite.Play("run"), null, null);
            stateMachine.RegisterStateFunctions(States.Jumping, () => Sprite.Play("jump"), null, null);
            stateMachine.RegisterStateFunctions(States.Ascending, () => Sprite.Play("ascend"), () => { if (Velocity.Y >= 0) stateMachine.Switch(States.Falling); }, null);
            stateMachine.RegisterStateFunctions(States.Falling, () => Sprite.Play("fall"), null, null);
            stateMachine.RegisterStateFunctions(States.Idle, () => Sprite.Play("idle"), null, null);
            stateMachine.RegisterStateFunctions(States.WallSliding, () => Sprite.Play("wallSlide"), () => { if (!onWall) stateMachine.Switch(States.Ascending); }, null);
            stateMachine.RegisterStateFunctions(States.Jetpack, () => Sprite.Play("ascend"), () => { if (Velocity.Y >= 0) stateMachine.Switch(States.Falling); }, null);

            stateMachine.RegisterStateFunctions(States.Swinging, () =>
                {
                    if (grappledSolid is ISwinged swinged)
                        swinged.OnGrapple(this, () => isAtSwingEnd); }, null,
                        () =>
                {
                    if (grappledSolid is ISwinged swinged)
                        swinged.OnStopGrapple(this);
                });

            AddComponent(stateMachine);

            #endregion

            JetpackParticle = new ParticleType()
            {
                LifeMin = 0.2f,
                LifeMax = 0.4f,
                Color = Color.Orange,
                Color2 = Color.Yellow,
                Size = 4,
                SizeRange = 3,
                SizeChange = ParticleType.FadeModes.Linear,
                Direction = 180,
                SpeedMin = 2,
                SpeedMax = 5
            };

            respawnPoint = position;
        }

        public override void Update()
        {

            if (!canMove)
            {
                swingPositions.Clear();
                swingPositionsSign = new List<int> { 0 };
                if (stateMachine.Is(States.Swinging))
                    stateMachine.Switch(States.Ascending);
                isAtSwingEnd = false;
                return; 
            }

            onGround = Collider.CollideAt(Pos + new Vector2(0, 1));
            onRightWall = Collider.CollideAt(Pos + new Vector2(1, 0));
            onWall = Collider.CollideAt(Pos + new Vector2(-1, 0)) || onRightWall;

            base.Update();

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

                if (Input.GamePadConnected)
                {
                    if (xMoving == 0)
                        xMoving = Input.GetLeftThumbstick().X;
                    if(yMoving == 0)
                        yMoving = -Input.GetLeftThumbstick().Y;
                    xMovingRaw = xMoving > 0.2 ? 1 : xMoving <  -0.2 ? -1 : 0;
                    yMovingRaw = yMoving > 0.2 ? 1 : yMoving <  -0.2 ? -1 : 0;
                }
                else
                {
                    xMovingRaw = (int)xMoving;
                    yMovingRaw = (int)yMoving;
                }
            }

            if (onGround && xMovingRaw == 0 && normalMouvement && !stateMachine.Is(States.Swinging) && !stateMachine.Is(States.Jumping) && !stateMachine.Is(States.Jetpack))
                stateMachine.Switch(States.Idle);
            else if (onGround && !stateMachine.Is(States.Swinging) && !stateMachine.Is(States.Jumping) && !stateMachine.Is(States.Jetpack) && normalMouvement)
                stateMachine.Switch(States.Running);

            //Velocity calculation and clamping

            if (normalMouvement && onGround)
                Velocity.X += xMoving * acceleration - friction * Velocity.X;
            else if (normalMouvement && isAtSwingEnd)
                Velocity.X += xMoving * swingAcceleration;
            else if (normalMouvement && !onWall)
                Velocity.X += xMoving * airAcceleration - airFriction * Velocity.X;

            if (normalMouvement && !stateMachine.Is(States.Swinging) && !stateMachine.Is(States.Jetpack))
                Velocity.X = Math.Clamp(Velocity.X, -maxSpeed, maxSpeed);            

            if (Velocity.X <= 1 && Velocity.X >= -1)
                Velocity.X = 0;
            
            AddedJetpackSpeed.X = Math.Clamp(AddedJetpackSpeed.X, -jetpackPowerX * 5, jetpackPowerX * 5);

            if (AddedJetpackSpeed.X <= 1 && AddedJetpackSpeed.X >= -1)
                AddedJetpackSpeed.X = 0;


            {
                if (onWall && !onGround && !stateMachine.Is(States.Swinging))
                    WallSlide();
                else
                    gravityScale = constGravityScale;

                if(Input.GetKeyDown(Keys.Space) || Input.GetKeyDown(Keys.C) || Input.GetButtonDown(Buttons.A))
                {
                    if (onGround)
                        Jump();
                    else if(onWall)
                        WallJump();
                }

                if (onGround)
                {
                    if (Velocity.Y > 0)
                        Velocity.Y = 0;
                }
                else
                    Gravity();

                Velocity.Y = Math.Min(Velocity.Y, maxFallingSpeed);
            }

            {
                if ((!onGround || onWall) && canJetpack && (Input.GetKey(Keys.X) || Input.GetButton(Buttons.X)) && jetpackTime > 0)
                    Jetpack();
                else
                {
                    AddedJetpackSpeed = Vector2.Zero;
                    Jetpacking = false;
                }

                if (onGround)
                {
                    jetpackTime = maxJetpackTime;
                    AddedJetpackSpeed.X = 0;

                    if (!previousOnGround)
                        Land();
                }

                if(onGround || onWall)
                    cancelJump = false;

                if (Input.GetKeyDown(Keys.A) || Input.GetButtonDown(Buttons.LeftTrigger) || Input.GetButtonDown(Buttons.RightTrigger))
                    ThrowRope();
                if ((Input.GetKey(Keys.A) || Input.GetButton(Buttons.LeftTrigger) || Input.GetButton(Buttons.RightTrigger)) && stateMachine.Is(States.Swinging))
                    Swing();
                else if(Input.GetKeyUp(Keys.A) || Input.GetButtonUp(Buttons.LeftTrigger) || Input.GetButtonUp(Buttons.RightTrigger))
                {
                    Velocity.X *= 1.2f;
                    if (Velocity.Y < 0)
                        Velocity.Y *= 1.1f;
                    else
                        Velocity.Y *= 0.7f;

                    swingPositions.Clear();
                    swingPositionsSign = new List<int> { 0 };
                    stateMachine.Switch(States.Ascending);
                    isAtSwingEnd = false;
                }
            }

            if ((stateMachine.Is(States.Running) || stateMachine.Is(States.Idle)) && !Collider.CollideAt(Pos + Velocity * Engine.Deltatime + new Vector2(0, 1)))
                stateMachine.Switch(States.Ascending);

            if (xMovingRaw != 0 && !isUnsticking)
                facing = xMovingRaw;

            Dust.Acceleration = Velocity;

            /*Dust.LifeMin = 0.3f;
            Dust.LifeMax = 0.4f;
            Dust.Color = Color.White;
            Dust.Size = 1.5f;
            Dust.SizeRange = 0.2f;
            Dust.SizeChange = ParticleType.FadeModes.Linear;
            Dust.Direction = 0;
            Dust.DirectionRange = 0.5f;
            Dust.SpeedMin = 5;
            Dust.SpeedMax = 15;*/

            /*if (Input.GetKeyDown(Keys.C))
                Platformer.pS.Emit(Dust, -Vector2.One * 5, this, 1000);*/

            Velocity += AddedJetpackSpeed;

            previousOnGround = onGround;
            collisionX = collisionY = false;

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

            foreach (Fiourp.Solid g in SwingingPoint.SwingingPoints)
            {
                float d = Vector2.Distance(MiddleExactPos, g.MiddleExactPos);

                if (d < distance)
                {
                    Vector2 dir = g.Pos - Pos;
                    bool onRightDir = true;
                    int signX = Math.Sign(dir.X), signY = Math.Sign(dir.Y);

                    if (!((xMovingRaw == signX || xMovingRaw == 0) && (yMovingRaw == signY || yMovingRaw == 0)))
                    {
                        if (d > reserveDistance)
                            continue;
                        onRightDir = false;
                    }

                    if (g is GrapplingTrigger trigger && !trigger.Active)
                        continue;

                    Raycast ray = new Raycast(MiddleExactPos, g.MiddleExactPos);
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
                stateMachine.Switch(States.Swinging);
                totalRopeLength = distance;

                Vector2 grapplingPos = determinedGrappledSolid.MiddleExactPos;
                swingPositions.Add(grapplingPos);

                AddComponent(new LineRenderer(new List<Vector2> { Pos, grapplingPos }, 2, Color.Blue,
                        (line) => { if (!stateMachine.Is(States.Swinging)) RemoveComponent(line); },
                    (line) => {

                        List<Vector2> linePositions = new List<Vector2>() { MiddlePos };
                        List<Vector2> reversedPositions = new List<Vector2>(swingPositions);
                        reversedPositions.Reverse();
                        linePositions.AddRange(reversedPositions);
                        line.Positions = linePositions;

                    }));
            }
            else if (determinedGrappledSolid is GrapplingTrigger trigger)
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
                        stateMachine.Switch(States.Ascending);
                        Velocity.Y = -500;
                    }));

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

            Vector2 grapplePos = swingPositions[swingPositions.Count - 1];

            float ropeLength = totalRopeLength;
            for (int i = 0; i < swingPositions.Count - 1; i++)
                ropeLength -= Vector2.Distance(swingPositions[i], swingPositions[i + 1]);

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

            swingPositions[0] = grappledSolid.Pos + new Vector2(grappledSolid.Width / 2, grappledSolid.Height / 2);

            RemoveGrapplingPoints();

            List<Vector2> cornersToCheck = new List<Vector2>(Engine.CurrentMap.CurrentLevel.Corners);
            foreach (Vector2 point in swingPositions)
                cornersToCheck.Remove(point);

            AddGrapplingPoints(cornersToCheck, swingPositions[swingPositions.Count - 1]);
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
                    swingPositions.Add(foundCorner);
                    swingPositionsSign.Add(Math.Sign(angle));
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
                for (int i = swingPositions.Count - 1; i >= 1; i--)
                {
                    float grappleAngle = VectorHelper.GetAngle(swingPositions[i - 1] - Pos - HalfSize, swingPositions[i - 1] - swingPositions[i]);

                    if (Math.Sign(grappleAngle) == swingPositionsSign[i])
                    {
                        swingPositions.RemoveAt(i);
                        swingPositionsSign.RemoveAt(i);
                    }
                    else
                        break;
                }
            }

            #endregion
        }

        private void Jump()
        {
            stateMachine.Switch(States.Jumping);
            Velocity.X += LiftBoost.X;

            Debug.Log();
            Platformer.pS.Emit(Dust, 7, new Rectangle((Pos + new Vector2(0, Height - 3)).ToPoint(), new Point(Width, 3)), null, xMoving == 1 ? 0 : xMoving == 0 ? -90 : 180, Dust.Color);

            AddComponent(new Timer(maxJumpTime, true, (timer) =>
            {
                if (collisionY || hasDashed || cancelJump)
                {
                    timer.End();
                    return;
                }

                float JumpScale = 0;
                if (!(Input.GetKey(Keys.Space) || Input.GetKey(Keys.C) || Input.GetButton(Buttons.A)))
                    JumpScale = 10;

                if (Jetpacking && AddedJetpackSpeed.Y < -10)
                    timer.TimeScale = 3;
                else
                    timer.TimeScale = 1;

                timer.TimeScale += JumpScale;

                Velocity.Y = -jumpForce * (timer.Value / maxJumpTime) + LiftBoost.Y;
            }, () => {
                cancelJump = false;
                AddedJetpackSpeed.X += 0.1f;
                if (stateMachine.Is(States.Jumping))
                    stateMachine.Switch(States.Ascending); 
            }));
        }

        private void WallJump()
        {
            stateMachine.Switch(States.Jumping);
            int wallJumpingDirection = onRightWall ? -1 : 1;
            float coef = 1;

            if (Velocity.Y < -250)
                coef += 0.5f;

            facing = -facing;
            Velocity.X = wallJumpSideForce * wallJumpingDirection * coef;
            Velocity.Y -= jumpForce * 0.5f;

            AddComponent(new Timer(maxJumpTime + (coef == 1 ? 0 : 0.1f), true, (timer) =>
            {
                if (collisionY || hasDashed || cancelJump)
                {
                    timer.End();
                    return;
                }

                float JumpScale = 0;
                if (!(Input.GetKey(Keys.Space) || Input.GetKey(Keys.C) || Input.GetButton(Buttons.A)))
                    JumpScale = 2;

                if (Jetpacking && AddedJetpackSpeed.Y < -10)
                    timer.TimeScale = 1.3f;
                else
                    timer.TimeScale = 1;

                timer.TimeScale += JumpScale;

                if (!collisionY)
                    Velocity.Y = -jumpForce * coef * timer.Value / timer.MaxValue;

            }, () => { cancelJump = false;  if (stateMachine.Is(States.Jumping)) stateMachine.Switch(States.Ascending); } ));
        }

        public void CancelJump()
            => cancelJump = true;

        private void WallSlide()
        {
            stateMachine.Switch(States.WallSliding);
            if(Velocity.Y > 0)
                gravityScale = 0.5f * constGravityScale;

            Platformer.pS.Emit(Dust.Create(this, new Vector2(0, 0)), 1000);

            if (!isUnsticking && xMovingRaw != 0 && (xMovingRaw < 0) == onRightWall)
            {
                isUnsticking = true;
                AddComponent(new Timer(unstickTime, true, (timer) =>
                {
                    if (xMovingRaw != 0 && (xMovingRaw < 0) != onRightWall || !onWall)
                    {
                        isUnsticking = false;
                        RemoveComponent(timer);
                    }
                }, () => { Velocity.X += xMovingRaw * 4; isUnsticking = false; }));
            }
        }

        public Vector2 LiftBoost
        {
            get
            {
                Vector2 boost = LiftSpeed;
                if (boost.Y > 0)
                    boost.Y = 0;

                return boost;
            }
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

        public void Jetpack()
        {
            Vector2 dir;
            if(xMoving == 0 && yMoving == 0)
                dir = -Vector2.UnitY;
            else
                dir = new Vector2(xMovingRaw, yMovingRaw);

            jetpackTime -= Engine.Deltatime;
            if (jetpackTime <= 0)
                return;

            //stateMachine.Switch(States.Jetpack);
            if (normalMouvement && onGround)
                AddedJetpackSpeed.X += dir.X * jetpackPowerX * jetpackPowerCoef.X - friction * AddedJetpackSpeed.X;
            else if (normalMouvement && isAtSwingEnd)
                AddedJetpackSpeed.X += dir.X * swingAcceleration * jetpackPowerCoef.X;
            else if (normalMouvement && !onWall)
                AddedJetpackSpeed.X += dir.X * jetpackPowerX * jetpackPowerCoef.X - airFriction * AddedJetpackSpeed.X;

            Vector2 coef = Vector2.One;
            if (Velocity.Y > -15 && dir.Y < 0)
                coef += Vector2.One;

            coef *= jetpackPowerCoef;

            if (stateMachine.Is(States.Jumping))
                AddedJetpackSpeed.Y += dir.Y * jetpackPowerY * 0.5f * coef.Y;
            else
                AddedJetpackSpeed.Y = dir.Y * jetpackPowerY * coef.Y;

            Jetpacking = true;

            Platformer.pS.Emit(JetpackParticle, MiddlePos);
        }

        public void Death()
        {
            Velocity = Vector2.Zero;
            Active = false;
            stateMachine.Switch(States.Idle);

            Engine.CurrentMap.Instantiate(new ScreenWipe(1, () =>
            {
                ExactPos = respawnPoint;
                Engine.Cam.Pos = ExactPos;
                Levels.ReloadLastLevelFetched();
                Active = true;
            }));
        }

        private void CollisionX(Entity collided)
        {
            if (collided is GlassWall gl && gl.DestroyOnX && Math.Abs(Velocity.X) >= gl.BreakVelocity)
            {
                gl.Break(Velocity);
                return;
            }

            Velocity.X = 0;
            collisionX = true;
        }

        private void CollisionY(Entity collided)
        {
            if (collided is GlassWall gl && !gl.DestroyOnX && Math.Abs(Velocity.Y) >= gl.BreakVelocity)
            {
                gl.Break(Velocity);
                return;
            }

            Velocity.Y = 0;
            collisionY = true;
        }

        void Land()
        {
            ParticleType oldDust = Dust.Copy();
            Dust.LifeMax = 0.4f;
            Dust.SpeedMax = 10; 
            Platformer.pS.Emit(Dust, 4, new Rectangle((Pos + new Vector2(0, Height - 3)).ToPoint(), new Point(Width, 3)), null, xMoving == 1 ? 0 : xMoving == 0 ? -90 : 180, Dust.Color);

            Dust.CopyFrom(oldDust);
        }

        public override void Render()
        {
            if (stateMachine.Is(States.WallSliding))
            {
                if (facing == -1)
                    Sprite.Effect = SpriteEffects.None;
                else if (facing == 1)
                    Sprite.Effect = SpriteEffects.FlipHorizontally;
            }
            else
            {
                if (facing == 1)
                    Sprite.Effect = SpriteEffects.None;
                else if (facing == -1)
                    Sprite.Effect = SpriteEffects.FlipHorizontally;
            }

            //Renderer components
            base.Render();
        }

        public void ResetJetpack()
        {
            jetpackTime = maxJetpackTime;
            AddedJetpackSpeed = Vector2.Zero;
        }

        public void LimitJetpackY(float coef, float time, Func<bool> stopLimitting)
        {
            AddComponent(new Timer(time, true, (timer) =>
            {
                if (stopLimitting())
                    timer.Destroy();

                jetpackPowerCoef.Y = coef;
            }, () => jetpackPowerCoef.Y = 1));
        }
    }
}