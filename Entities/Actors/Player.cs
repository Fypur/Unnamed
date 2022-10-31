using Fiourp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using FMOD.Studio;

namespace Platformer
{
    public class Player : Actor
    {
        #region constants

        public enum States { Idle, Running, Jumping, Ascending, Falling, Dashing, Swinging, WallSliding, Pulling, Jetpack, Dead }

        private const float maxSpeed = 100;
        private const float maxFallingSpeed = 260;
        private const float maxFallSlidingSpeed = 150; //Max Falling Speed while Wall Sliding
        private const float dashSpeed = 200;

        private const float jetpackPowerX = 20;
        private const float jetpackPowerY = 20;
        private const float maxJetpackSpeedX = 170;
        private const float maxJetpackSpeedY = 120;
        private const float maxJetpackTime = 0.5f;
        
        private const float acceleration = 50f;
        private const float airAcceleration = 15f;
        private const float swingAcceleration = 3f; //Swing accel is very low since friction isn't applied
        private const float friction = 0.8f;
        private const float airFriction = 0.1f;
        
        private const float wallJumpSideForce = 160f;
        private const float jumpForce = 200f;
        private const float constGravityScale = 1.2f;
        
        private const float maxJumpTime = 0.4f;
        private const float dashTime = 0.2f;
        private const float unstickTime = 0.1f;
        private const float safeTime = 1.0f;

        private const float coyoteTime = 0.07f;
        private const float jumpGraceTime = 0.1f;
        //Can wall jump if wall is this far away from the wall in pixels
        private const float wallJumpPixelGap = 4;
        
        public static readonly ParticleType Dust = Particles.Dust.Copy();

        private readonly ParticleType JetpackParticle = new ParticleType()
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

        private ParticleType ExplosionParticle = new ParticleType()
        {
            LifeMin = 0.2f,
            LifeMax = 1,
            Color = Color.White,
            Color2 = Color.Orange,
            Size = 3,
            SizeRange = 2,
            SizeChange = ParticleType.FadeModes.Linear,
            Direction = 0,
            DirectionRange = 360,
            SpeedMin = 4,
            SpeedMax = 100,
            Acceleration = Vector2.UnitY * 10,
        };

        #endregion

        #region variables

        private readonly StateMachine<States> stateMachine;

        public bool CanMove = true;
        public bool Safe = true;
        public float SafePercentage = 0;
        public bool Jetpacking;
        public bool CanJetpack = true;
        public bool CanWallJump = true;
        public Vector2 JetpackPowerCoef = Vector2.One;
        public Vector2 JetpackDirectionalPowerCoef = Vector2.Zero;
        public Vector2 RespawnPoint;
        public event Action OnDeath = delegate { };

        private bool onGround;
        private bool previousOnGround = true;
        private bool onWall;
        private bool onFarWall;
        private bool onRightWall;
        private bool onRightFarWall;
        private bool collisionX;
        private bool collisionY;

        private float xMoving;
        private float yMoving;
        private int xMovingRaw;
        private int yMovingRaw;
        public int Facing = 1;

        private bool normalMouvement = true;
        private bool cancelJump;
        private bool hasDashed;
        private bool isUnsticking;
        private bool canUnstick;
        private bool inCoyoteTime;
        private float potentialFallingSpeed;

        private float jetpackTime;
        private BoostBar boostBar;

        private float totalRopeLength;
        private Solid grappledSolid;
        private List<Vector2> swingPositions = new List<Vector2>();
        private List<int> swingPositionsSign = new List<int>() { 0 };
        private bool isAtSwingEnd;

        //Controls
        public static ControlList LeftControls = Input.LeftControls;
        public static ControlList RightControls = Input.RightControls;
        public static ControlList UpControls = Input.UpControls;
        public static ControlList DownControls = Input.DownControls;
        public static ControlList JumpControls = new ControlList(Keys.C, Keys.I, Keys.Space, Buttons.A);
        public static ControlList JetpackControls = new ControlList(Keys.X, Keys.O, MouseButton.Right, Buttons.X);
        public static ControlList SwingControls = new ControlList(Keys.W, Keys.P, MouseButton.Middle, Buttons.LeftTrigger, Buttons.RightTrigger);

        private EventInstance jetpackAudio;

        #endregion


        public Player(Vector2 position) : base(position, 8, 13, constGravityScale, new Sprite(Color.White))
        {
            Engine.Player = this;

            Sprite.Add(Sprite.AllAnimData["Player"]);

            Sprite.Play("idle");
            Sprite.Offset = new Vector2(-3, -3);
            //Sprite.Origin = HalfSize;

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
                    Sprite.Play("fall");
                    if (grappledSolid is ISwinged swinged)
                        swinged.OnGrapple(this, () => isAtSwingEnd); }, null,
                        () =>
                {
                    if (grappledSolid is ISwinged swinged)
                        swinged.OnStopGrapple(this);
                    ResetSwing();
                });

            AddComponent(stateMachine);

            RespawnPoint = position;
            Layer = 2;

            boostBar = (BoostBar)AddChild(new BoostBar(Pos + new Vector2(1, -5), Width - 1, 1, 0.5f));
        }

        public override void Update()
        {
            if (!CanMove)
            {
                swingPositions.Clear();
                swingPositionsSign = new List<int> { 0 };
                if (stateMachine.Is(States.Swinging))
                    stateMachine.Switch(States.Ascending);
                isAtSwingEnd = false;

                Jetpacking = false;

                if(jetpackAudio.isValid())
                    Audio.StopEvent(jetpackAudio);
            }
            
            onGround = OnGroundCheck(Pos + new Vector2(0, 1), out Entity onGroundEntity);
            onRightWall = Collider.CollideAt(Pos + new Vector2(1, 0));
            onWall = Collider.CollideAt(Pos + new Vector2(-1, 0)) || onRightWall;
            if (onWall)
            {
                onFarWall = true;
                onRightFarWall = onRightWall;
            }
            else
            {
                onRightFarWall = Collider.CollideAt(Pos + new Vector2(wallJumpPixelGap, 0));
                onFarWall = Collider.CollideAt(Pos + new Vector2(-wallJumpPixelGap, 0)) || onRightFarWall;
            }

            if (!CanWallJump)
            {
                onWall = false;
                onFarWall = false;
            }

            base.Update();

            if (!CanMove)
                return;

            {
                xMoving = yMoving = 0;
                if (RightControls.Is() && !Input.GetButton(Buttons.LeftThumbstickRight)) xMoving = 1;
                if (LeftControls.Is() && !Input.GetButton(Buttons.LeftThumbstickLeft)) xMoving = -1;
                if (!((RightControls.Is() && !Input.GetButton(Buttons.LeftThumbstickRight)) ^ LeftControls.Is() && !Input.GetButton(Buttons.LeftThumbstickLeft))) xMoving = 0; //Using ^ which means XOR

                if (UpControls.Is() && !Input.GetButton(Buttons.LeftThumbstickUp)) yMoving = -1;
                if (DownControls.Is() && !Input.GetButton(Buttons.LeftThumbstickDown)) yMoving = 1;
                if (!((UpControls.Is() && !Input.GetButton(Buttons.LeftThumbstickUp)) ^ (DownControls.Is() && !Input.GetButton(Buttons.LeftThumbstickDown)))) yMoving = 0;

                if (Input.GamePadConnected)
                {
                    if (xMoving == 0)
                        xMoving = Input.GetLeftThumbstick().X;
                    if(yMoving == 0)
                        yMoving = -Input.GetLeftThumbstick().Y;
                    xMovingRaw = xMoving > 0.3f ? 1 : xMoving <  -0.3f ? -1 : 0;
                    yMovingRaw = yMoving > 0.4f ? 1 : yMoving <  -0.2f ? -1 : 0;
                }
                else
                {
                    xMovingRaw = (int)xMoving;
                    yMovingRaw = (int)yMoving;
                }
            }

            if (onGround && xMoving == 0 && normalMouvement && !stateMachine.Is(States.Swinging) && !stateMachine.Is(States.Jumping) && !stateMachine.Is(States.Jetpack))
                stateMachine.Switch(States.Idle);
            else if (onGround && !stateMachine.Is(States.Swinging) && !stateMachine.Is(States.Jumping) && !stateMachine.Is(States.Jetpack) && normalMouvement)
                stateMachine.Switch(States.Running);

            //Velocity calculation and clamping

            if (normalMouvement && onGround)
                Velocity.X = VelocityApproach(acceleration, friction);
            else if (normalMouvement && isAtSwingEnd)
                Velocity.X += xMoving * swingAcceleration;
            else if (normalMouvement && !onWall)
                Velocity.X = VelocityApproach(airAcceleration, airFriction);

            float VelocityApproach(float acceleration, float friction)
            {
                if (Velocity.X == 0 || xMoving != 0)
                    return Approach(Velocity.X, (xMoving > 0.3f ? 1 : xMoving < -0.3f ? -1 : xMoving) * maxSpeed, acceleration);
                return Approach(Velocity.X, 0, friction * Math.Abs(Velocity.X));
            }

            float Approach(float value, float approached, float move)
            {
                if (value < approached)
                    return Math.Min(value + move, approached);
                return Math.Max(value - move, approached);
            }

            //Celeste code this is inspired from
            /*if (Math.Abs(Speed.X) > max && Math.Sign(Speed.X) == moveX)
                Speed.X = Calc.Approach(Speed.X, max * moveX, RunReduce * mult * Engine.DeltaTime);  //Reduce back from beyond the max speed
            else
                Speed.X = Calc.Approach(Speed.X, max * moveX, RunAccel * mult * Engine.DeltaTime);   //Approach the max speed*/

            /*if (normalMouvement && !stateMachine.Is(States.Swinging) && !Jetpacking)
                Velocity.X = Math.Clamp(Velocity.X, -maxSpeed, maxSpeed);*/

            if (Velocity.X <= 1 && Velocity.X >= -1)
                Velocity.X = 0;

            if (!Safe && SafePercentage == 0)
            {
                SafePercentage = 0.01f;
                if (onGround && !(onGroundEntity is JumpThru) && xMoving == 0)
                    AddComponent(new Timer(safeTime, true, (timer) =>
                    {
                        SafePercentage = Ease.Reverse(timer.Value / timer.MaxValue);
                        if (!(onGround && !(onGroundEntity is JumpThru)) || xMoving != 0)
                        {
                            SafePercentage = 0;
                            RemoveComponent(timer);
                        }
                    },
                    () => { SafePercentage = 1; Safe = true; })); ;
            }

            if (!onGround || xMoving != 0)
            {
                Safe = false;
                SafePercentage = 0;
            }

            {
                if (previousOnGround && !onGround)
                {
                    inCoyoteTime = true;
                    AddComponent(new Timer(coyoteTime, true, null, () => inCoyoteTime = false));
                }


                if (onWall && !onGround && !stateMachine.Is(States.Swinging))
                    WallSlide();
                else
                {
                    gravityScale = constGravityScale;
                    isUnsticking = false;
                }

                if(JumpControls.IsDown())
                {
                    bool TestJump()
                    {
                        if(!CanMove)
                            return false;

                        if (onGround || inCoyoteTime)
                        {
                            Jump();
                            return true;
                        }
                        else if (onFarWall)
                        {
                            if (!onWall)
                                onRightWall = onRightFarWall;

                            WallJump();
                            return true;
                        }
                        
                        return false;
                    }

                    //Take the jump if on the ground, else wait for jump grace period
                    if(!TestJump())
                    {
                        AddComponent(new Timer(jumpGraceTime, true, (timer) =>
                        {
                            if (TestJump())
                                RemoveComponent(timer);
                        }, () =>
                        {
                            TestJump();
                        }));
                    }
                }

                if (onGround)
                {
                    if (Velocity.Y > 0)
                        Velocity.Y = 0;
                }
                else
                    Gravity();

                if(stateMachine.Is(States.WallSliding))
                    Velocity.Y = Math.Min(Velocity.Y, maxFallSlidingSpeed);
                else if (!stateMachine.Is(States.Swinging))
                {
                    Velocity.Y = Math.Min(Velocity.Y, maxFallingSpeed);
                }
            }

            {
                if (!onGround && CanJetpack && jetpackTime > 0 && JetpackControls.Is())
                {
                    Jetpack();
                }
                else
                {
                    Jetpacking = false;
                    if (jetpackAudio.isValid())
                        Audio.StopEvent(jetpackAudio);
                }

                if (onGround)
                {
                    jetpackTime = maxJetpackTime;
                    boostBar.Visible = false;
                }

                boostBar.Value = jetpackTime / maxJetpackTime;

                if(onGround || onWall)
                    cancelJump = false;

                if (Velocity.Y >= maxFallingSpeed)
                {
                    potentialFallingSpeed += gravityVector.Y * gravityScale;
                    potentialFallingSpeed = Math.Min(potentialFallingSpeed, 400);
                }
                else
                    potentialFallingSpeed = Velocity.Y;

                if (SwingControls.IsDown())
                    ThrowRope();
                if (SwingControls.Is() && stateMachine.Is(States.Swinging))
                    Swing();
                else if(SwingControls.IsUp() && stateMachine.Is(States.Swinging))
                {
                    Velocity.X *= 1.2f;
                    if (Velocity.Y < 0)
                        Velocity.Y *= 1.1f;
                    else
                        Velocity.Y *= 0.7f;

                    stateMachine.Switch(States.Ascending);
                }
            }

            if ((stateMachine.Is(States.Running) || stateMachine.Is(States.Idle)) && !OnGroundCheck(Pos + Velocity * Engine.Deltatime + new Vector2(0, 1)))
                stateMachine.Switch(States.Ascending);

            if (xMoving != 0 && !isUnsticking)
                Facing = Math.Sign(xMoving);

            Dust.Acceleration = Velocity;

            collisionX = collisionY = false;
            previousOnGround = onGround;

            MoveX(Velocity.X * Engine.Deltatime, CollisionX);
            MoveY(Velocity.Y * Engine.Deltatime, new List<Entity>(Engine.CurrentMap.Data.Platforms), CollisionY);

            UpdateChildrenPos();
        }

        public override void Squish()
            => Death();

        private void ThrowRope()
        {
            #region Determining grappling point
            Fiourp.Solid determinedGrappledSolid = null;
            Fiourp.Solid reserveGrappledSolid = null;
            float distance = float.PositiveInfinity;
            float reserveDistance = float.PositiveInfinity;

            foreach (Solid g in SwingingPoint.SwingingPoints)
            {
                float d = Vector2.Distance(MiddleExactPos, g.MiddleExactPos);

                if (d < distance && d < ((ISwinged)g).MaxSwingDistance)
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

                    Raycast ray = new Raycast(Raycast.RayTypes.MapTiles, MiddleExactPos, g.MiddleExactPos);
                    if (!ray.Hit)
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

            Velocity.Y = potentialFallingSpeed;

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
            cornersToCheck.Remove(swingPositions[swingPositions.Count - 1]);

            AddGrapplingPoints(cornersToCheck, swingPositions[swingPositions.Count - 1]);
            #endregion

            #region Grappling Methods

            void AddGrapplingPoints(List<Vector2> cornersToCheck, Vector2 checkingFrom)
            {
                float angle = VectorHelper.GetAngle(checkingFrom - ExactPos - HalfSize, checkingFrom - ExactPos - HalfSize - Velocity * Engine.Deltatime);

                if (angle == 0)
                    return;

                float distanceFromPoint = Vector2.Distance(ExactPos + HalfSize + Velocity * Engine.Deltatime, checkingFrom);

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

                    float pointAngle = VectorHelper.GetAngle(checkingFrom - ExactPos - HalfSize, checkingFrom - corner);

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

                    nextCorners.Remove(foundCorner);

                    if (nextCorners.Count > 0)
                        AddGrapplingPoints(nextCorners, foundCorner);
                }
            }

            void RemoveGrapplingPoints()
            {
                for (int i = swingPositions.Count - 1; i >= 1; i--)
                {
                    float grappleAngle = VectorHelper.GetAngle(swingPositions[i - 1] - ExactPos - HalfSize, swingPositions[i - 1] - swingPositions[i]); ;

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
            previousOnGround = false;

            var j = Audio.PlayEvent("Jump");
            PlayerStats.JumpCount++;

            Engine.CurrentMap.MiddlegroundSystem.Emit(Dust, 7, new Rectangle((Pos + new Vector2(0, Height - 3)).ToPoint(), new Point(Width, 3)), null, xMoving == 1 ? 0 : xMoving == 0 ? -90 : 180, Dust.Color);

            AddComponent(new Timer(maxJumpTime, true, (timer) =>
            {
                if (collisionY || hasDashed || cancelJump)
                {
                    timer.End();
                    return;
                }

                float JumpTimeScale = 1;
                if (timer.Value < maxJumpTime - 0.07f && !JumpControls.Is())
                    JumpTimeScale = 10;

                /*if (Jetpacking && Velocity.Y < -100)
                    timer.TimeScale = 3;
                else
                    timer.TimeScale = 1;*/

                timer.TimeScale = JumpTimeScale;

                if (Jetpacking && -jumpForce * (timer.Value / maxJumpTime) + LiftBoost.Y > Velocity.Y)
                    return;

                Velocity.Y = -jumpForce * (timer.Value / maxJumpTime) + LiftBoost.Y;

            }, () => {
                cancelJump = false;
                LiftSpeed = new Vector2(LiftSpeed.X, 0);
                if (stateMachine.Is(States.Jumping))
                    stateMachine.Switch(States.Ascending); 
            }));
        }

        private void WallJump()
        {
            stateMachine.Switch(States.Jumping);

            Audio.PlayEvent("Jump");
            PlayerStats.JumpCount++;

            int wallJumpingDirection = onRightWall ? -1 : 1;
            float coef = 1;

            if (Velocity.Y < -250)
                coef += 0.5f;

            Facing = -Facing;
            Velocity.X = wallJumpSideForce * wallJumpingDirection * coef;
            Velocity.Y -= jumpForce * 0.5f;

            if (onRightWall)
                Engine.CurrentMap.MiddlegroundSystem.Emit(Dust, new Rectangle((int)Pos.X + Width - 1, (int)Pos.Y, 1, Height), 7);
            else
                Engine.CurrentMap.MiddlegroundSystem.Emit(Dust, new Rectangle((int)Pos.X, (int)Pos.Y, 1, Height), 7);

            AddComponent(new Timer(maxJumpTime + (coef == 1 ? 0 : 0.1f), true, (timer) =>
            {
                if (collisionY || hasDashed || cancelJump)
                {
                    timer.End();
                    return;
                }

                float JumpScale = 0;
                if (!JumpControls.Is())
                    JumpScale = 2;

                /*if (Jetpacking && Velocity.Y < -100)
                    timer.TimeScale = 1.3f;
                else
                    timer.TimeScale = 1;*/

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

            if(onRightWall)
                Engine.CurrentMap.MiddlegroundSystem.Emit(Dust, new Rectangle((int)Pos.X + Width - 1, (int)Pos.Y, 1, Height), 2);
            else
                Engine.CurrentMap.MiddlegroundSystem.Emit(Dust, new Rectangle((int)Pos.X, (int)Pos.Y, 1, Height), 2);
            

            Action OnUnstick = () => {

                if (xMovingRaw == 0)
                {
                    canUnstick = true;
                    return;
                }

                MoveX(xMovingRaw);
                isUnsticking = false;
                canUnstick = false;
                gravityScale = constGravityScale;
                if (stateMachine.Is(States.WallSliding))
                    stateMachine.Switch(States.Ascending);
            };

            if (canUnstick && (xMovingRaw < 0) == onRightWall)
                OnUnstick();
            else if (!isUnsticking && (xMovingRaw == 0 || (xMovingRaw < 0) == onRightWall))
            {
                isUnsticking = true;
                AddComponent(new Timer(unstickTime, true, (timer) =>
                {
                    if (!(xMovingRaw == 0 || (xMovingRaw < 0) == onRightWall) || !onWall)
                    {
                        isUnsticking = false;
                        RemoveComponent(timer);
                    }
                },
                OnUnstick));
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

            int dir = Facing;
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

        private void Jetpack()
        {
            Vector2 dir;
            if (xMoving == 0 && yMoving == 0)
                return;
            else
                dir = new Vector2(Math.Sign(xMoving), Math.Sign(yMoving));

            boostBar.Visible = true;

            if (!Jetpacking)
                jetpackAudio = Audio.PlayEvent("Jetpack");

            dir.X = dir.Normalized().X;

            jetpackTime -= Engine.Deltatime;
            if (jetpackTime <= 0)
                return;

            Vector2 coef = Vector2.One;

            if (((dir.Y < 0 && Velocity.Y > -10) || (dir.Y > 0 && Velocity.Y < 10)))
            {
                coef.Y += 1.5f;
            }

            /*
            if (Velocity.X * dir.X < 150)
            {
                if (!nerfedJetpack)
                    coef.X += 2;
                else
                    coef.X += 0.7f;
            }*/

            coef *= JetpackPowerCoef;

            if (JetpackDirectionalPowerCoef.X != 0 && Math.Sign(Velocity.X) == Math.Sign(JetpackDirectionalPowerCoef.X))
                coef.X *= Math.Abs(JetpackDirectionalPowerCoef.X);
            if (JetpackDirectionalPowerCoef.Y != 0 && Math.Sign(Velocity.Y) == -Math.Sign(JetpackDirectionalPowerCoef.Y))
                coef.Y *= Math.Abs(JetpackDirectionalPowerCoef.Y);

            //Velocity.Y = Math.Clamp(Velocity.Y, -140, 140);

            Vector2 d = dir * coef *new Vector2(jetpackPowerX, jetpackPowerY);
            d = Vector2.Clamp(d + Velocity, new Vector2(-maxJetpackSpeedX,  -maxJetpackSpeedY), new Vector2(maxJetpackSpeedX, maxFallingSpeed + maxJetpackSpeedY)) - Velocity;

            if (Math.Sign(d.X) != Math.Sign(xMoving))
                d.X = 0;
            if (Math.Sign(d.Y) != Math.Sign(yMoving))
                d.Y = 0;

            if (isAtSwingEnd && Math.Abs(d.X) < 10)
                d.X = xMovingRaw * 10;
            if (isAtSwingEnd && Math.Abs(d.Y) < 10)
                d.Y = yMovingRaw * 10;

            Velocity += d;

            Jetpacking = true;

            Engine.CurrentMap.MiddlegroundSystem.Emit(JetpackParticle, MiddlePos);
            jetpackAudio.setParameterByName("JetpackTime", jetpackTime / maxJetpackTime);
        }

        public void Death()
        {
            stateMachine.Switch(States.Dead);
            CanMove = false;

            Sprite.Play("ascend");

            ResetJetpack();
            ResetSwing();

            

            PlayerStats.DeathCount++;

            foreach (Trigger trig in Engine.CurrentMap.Data.Triggers)
            {
                if (trig.Contains(this))
                {
                    trig.OnTriggerExit(this);
                    trig.OnTriggerEnter(this);
                }
            }

            Engine.Cam.Shake(0.2f, 2);

            Audio.PlayEvent("DeathInit");

            Vector2 startPos = Pos;
            Vector2 endPos = Pos - Velocity.Normalized() * 15;

            //Engine.CurrentMap.Instantiate(new ScreenFlash(0.5f, Ease.QuintOut));

            //Engine.CurrentMap.MiddlegroundSystem.Emit(ExplosionParticle, Bounds, 10);

            HitStop(0.05f, () =>
            {
                CanMove = false;
                AddComponent(new Timer(0.2f, true, (timer) =>
                {
                    Pos = Vector2.Lerp(startPos, endPos, Ease.QuintOut(Ease.Reverse(timer.Value / timer.MaxValue)));
                    Sprite.Color.A = (byte)((float)Math.Sin(timer.Value * 14) * 255); //Blinking
                    Sprite.Color.B = (byte)((float)Math.Sin(timer.Value * 14) * 255);
                    Sprite.Color.G = (byte)((float)Math.Sin(timer.Value * 14) * 255);
                },
                () =>
                {
                    Sprite.Color = Color.White;
                    InstaDeath();
                }));
            });
        }

        public void InstaDeath()
        {
            stateMachine.Switch(States.Dead);
            CanMove = false;

            Sprite.Play("ascend");

            ResetJetpack();
            ResetSwing();

            Visible = false;

            OnDeath?.Invoke();
            OnDeath = delegate { };

            Engine.CurrentMap.MiddlegroundSystem.Emit(ExplosionParticle, Bounds, 100);
            Engine.Cam.Shake(0.4f, 1);

            //Audio.PlayEvent("DeathExplosion");

            Velocity = Vector2.Zero;
            Engine.CurrentMap.Instantiate(new ScreenWipe(1, Color.Black, () =>
            {
                stateMachine.Switch(States.Idle);
                Sprite.Play("idle");
                onGround = true;

                Vector2 groundedRespawnPos = RespawnPoint;
                for (int i = 0; i < 100; i++)
                    if (!OnGroundCheck(groundedRespawnPos + new Vector2(0, 1)))
                        groundedRespawnPos += new Vector2(0, 1);
                    else
                        break;

                ExactPos = groundedRespawnPos;

                bool changedCamPos = false;
                foreach (CameraLock camLock in Engine.CurrentMap.Data.GetEntities<CameraLock>())
                    if (camLock.Contains(this))
                    {
                        Engine.Cam.NoBoundsPos = Engine.Cam.InBoundsPos(Pos, camLock.Bounds);
                        changedCamPos = true;
                    }

                if (!changedCamPos)
                    Engine.Cam.CenteredPos = ExactPos;

                Vector2 offset = Vector2.Zero;
                foreach (CameraOffset camOffset in Engine.CurrentMap.Data.GetEntities<CameraOffset>())
                    Engine.Cam.CenteredPos += camOffset.Offset;
                
                

                Levels.ReloadLastLevelFetched();
                Active = true;
                Visible = true;
            }, () => CanMove = true));
        }

        private void CollisionX(Entity collided)
        {
            if (collided is GlassWall gl && gl.Break(this, Velocity, true))
                return;

            Velocity.X = 0;
            collisionX = true;
        }

        private void CollisionY(Entity collided)
        {
            if (collided is JumpThru)
                Pos.Y = collided.Pos.Y - Height;

            if (collided is GlassWall gl && gl.Break(this, Velocity, false))
                return;

            if (Velocity.Y > 0)
                Land();

            Velocity.Y = 0;
            collisionY = true;
        }

        private bool OnGroundCheck(Vector2 position, out Entity groundedEntity)
            => Collider.CollideAt(new List<Entity>(Engine.CurrentMap.Data.Platforms), position, out groundedEntity);
        private bool OnGroundCheck(Vector2 position)
            => Collider.CollideAt(new List<Entity>(Engine.CurrentMap.Data.Platforms), position);

        void Land()
        {
            ParticleType oldDust = Dust.Copy();
            Dust.LifeMax = 0.4f;
            Dust.SpeedMax = 10; 
            Engine.CurrentMap.MiddlegroundSystem.Emit(Dust, 4, new Rectangle((Pos + new Vector2(0, Height - 3)).ToPoint(), new Point(Width, 3)), null, xMoving == 1 ? 0 : xMoving == 0 ? -90 : 180, Dust.Color);

            Dust.CopyFrom(oldDust);
        }

        public override void Render()
        {
            if (stateMachine.Is(States.WallSliding))
            {
                if (!onRightWall)
                    Sprite.Effect = SpriteEffects.None;
                else
                    Sprite.Effect = SpriteEffects.FlipHorizontally;
            }
            else
            {
                if (Facing == 1)
                    Sprite.Effect = SpriteEffects.None;
                else if (Facing == -1)
                    Sprite.Effect = SpriteEffects.FlipHorizontally;
            }

            if(!stateMachine.Is(States.Dead))
                Sprite.Color = Color.Lerp(Color.OrangeRed, Color.White, jetpackTime / maxJetpackTime);

            //Renderer components
            base.Render();
        }

        public void RefillJetpack()
            => jetpackTime = maxJetpackTime;

        private void ResetJetpack()
        {
            jetpackTime = maxJetpackTime;
            JetpackPowerCoef = Vector2.One;
        }

        private void ResetSwing()
        {
            swingPositions.Clear();
            swingPositionsSign = new List<int>() { 0 };
            isAtSwingEnd = false;
            if (stateMachine.Is(States.Swinging))
                stateMachine.Switch(States.Ascending);
        }

        public void LimitJetpackY(float coef, float time, Func<bool> stopLimitting)
        {
            AddComponent(new Timer(time, true, (timer) =>
            {
                if (stopLimitting())
                { timer.End(); return; }

                JetpackPowerCoef.Y = coef;
            }, 
            () => 
            JetpackPowerCoef.Y = 1));
        }

        public bool Is(States state)
            => stateMachine.Is(state);

        public void HitStop(float time, Action OnEnd = null)
        {
            CanMove = false;
            AddComponent(new Timer(time, true, null, () => { CanMove = true; OnEnd?.Invoke(); }));
        }
    }
}
