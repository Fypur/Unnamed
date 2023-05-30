using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class Boss : Actor
    {
        private const float walkingSpeed = 30f;
        private const float walkingTimeCycle = 0.4f;
        private const float jumpChargeTime = 1f;
        //private const float jumpForce = 500f;
        private const float jumpTime = 0.4f;
        private const float constGravityScale = 1.7f;


        public StateMachine<States> StateMachine;
        public enum States { Idle, Walking, Jumping, Missiling, WallMissiling, JumpDown, Swipe, MachineGun }


        private Player player;
        private bool onGround;
        private int counter;

        private IdentifierTrigger[] zones;
        private bool DownZone => zones[0].PlayerIn;
        private bool LeftZone => zones[1].PlayerIn;
        private bool RightZone => zones[2].PlayerIn;

        public Boss(Vector2 position) : base(position, 32, 32, constGravityScale, new Sprite(Color.Red))
        {
            player = (Player)Engine.Player;

            StateMachine = new StateMachine<States>(States.Walking);

            StateMachine.RegisterStateFunctions(States.Walking, () => Walking(null), null, () => Velocity.X = 0);
            StateMachine.RegisterStateFunctions(States.Jumping, JumpUp, null, null);
            StateMachine.RegisterStateFunctions(States.Missiling, Missiling, null, null);
            StateMachine.RegisterStateFunctions(States.WallMissiling, Missiling, null, null);
            StateMachine.RegisterStateFunctions(States.JumpDown, JumpDown, null, null);
            StateMachine.RegisterStateFunctions(States.MachineGun, () => AddComponent(new Coroutine(MachineGun())), null, null);
            StateMachine.RegisterStateFunctions(States.Swipe, () => AddComponent(new Coroutine(Swipe())), null, null);

            AddComponent(StateMachine);
        }

        public override void Awake()
        {
            base.Awake();

            zones = Engine.CurrentMap.Data.GetEntities<IdentifierTrigger>().ToArray();
            zones = zones.OrderBy((trig) => trig.Id).ToArray();
        }

        public override void Update()
        {
            base.Update();

            Debug.LogUpdate(StateMachine.CurrentState);

            onGround = Collider.CollideAt(new List<Entity>(Engine.CurrentMap.Data.Platforms), Pos + new Vector2(0, 1));

            if (!onGround)
                Gravity();
            else if (Velocity.Y > 0)
                Velocity.Y = 0;

            if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
                StateMachine.Switch(States.MachineGun);
            if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.Q))
                StateMachine.Switch(States.Swipe);


            MoveX(Velocity.X * Engine.Deltatime, CollisionX);
            MoveY(Velocity.Y * Engine.Deltatime, CollisionY);
        }

        public void Walking(bool? d)
        {
            bool walkingRight;
            if (d == null)
                walkingRight = Rand.NextDouble() < 0.5f;
            else
                walkingRight = d.Value;

            AddComponent(new Timer(walkingTimeCycle, true, (timer) =>
            {
                if (!StateMachine.Is(States.Walking))
                {
                    RemoveComponent(timer);
                    return;
                }

                Velocity.X = walkingSpeed * (walkingRight ? 1 : -1);

                Debug.LogUpdate(walkingRight, Pos.X > zones[3].Pos.X);

                if (Vector2.DistanceSquared(zones[3].Pos, Pos + Velocity) > 64 * 64)
                {
                    if(!(walkingRight == Pos.X < zones[3].Pos.X))
                    {
                        Walking(!walkingRight);
                        RemoveComponent(timer);
                    }

                }

            }, () =>
            {
                counter++;

                Switch();

                if (StateMachine.Is(States.Walking))
                    Walking(null);
            }));
        }

        public void JumpUp()
        {
            //Charge jump

            Vector2 p = player.Pos;

            Vector2 aimed;
            if(LeftZone)
                aimed = new Vector2(zones[1].Pos.X, p.Y);
            else
                aimed = new Vector2(zones[2].Pos.X + zones[2].Width, p.Y);

            if (aimed.Y - zones[2].Pos.Y < 24)
                aimed.Y = zones[2].Pos.Y + 24;
            

            AddComponent(new Timer(jumpChargeTime, true, (timer) => 
            {
                Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, new Rectangle((int)Pos.X, (int)Pos.Y + Height - 3, Width, 3), 3);
            }, () =>
            {
                gravityScale = 0;

                Vector2 init = Pos;
                Velocity.X = (aimed.X - Pos.X) / jumpTime;

                Engine.Cam.LightShake();
                //Velocity.Y = (aimed.Y - Pos.Y) / jumpTime - gravityVector.Y * gravityScale * jumpTime;

                AddComponent(new Timer(jumpTime, true, (timer) =>
                {
                    Pos.Y = Vector2.Lerp(init, aimed, Ease.CubicOut(timer.AmountCompleted())).Y;
                }, () =>
                {
                    Pos.Y = aimed.Y;
                    Velocity = Vector2.Zero;
                    Engine.Cam.LightShake();

                    if (Rand.NextDouble() > 0.5f)
                        StateMachine.Switch(States.JumpDown);
                    else
                        StateMachine.Switch(States.Missiling);
                }));
            }));
        }

        public void Missiling()
        {
            Engine.CurrentMap.Instantiate(new HomingMissile(MiddlePos, -45));

            AddComponent(new Timer(2f, true, null, () =>
            {
                if (zones[1].Collider.Collide((BoxCollider)Collider) || zones[2].Collider.Collide((BoxCollider)Collider))
                    StateMachine.Switch(States.JumpDown);
                else
                    StateMachine.Switch(States.Walking);
            }));
            //StateMachine.Switch(States.Walking);
        }

        public void JumpDown()
        {
            Vector2 aimed;
            Rectangle rect;
            if (zones[1].Collider.Collide((BoxCollider)Collider)) //left
            {
                rect = new Rectangle((int)Pos.X, (int)Pos.Y, 3, Height);
                aimed = zones[3].Pos - Vector2.UnitX * Rand.NextDouble() * 64;
            }
            else
            {
                rect = new Rectangle((int)Pos.X + Width - 3, (int)Pos.Y, 3, Height);
                aimed = zones[3].Pos + Vector2.UnitX * Rand.NextDouble() * 64;
            }


            AddComponent(new Timer(jumpChargeTime, true, (timer) =>
            {
                Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, rect, 3);
            }, () =>
            {
                gravityScale = 0;

                Vector2 init = Pos;
                Velocity.X = (aimed.X - Pos.X) / jumpTime;

                Engine.Cam.LightShake();
                //Velocity.Y = (aimed.Y - Pos.Y) / jumpTime - gravityVector.Y * gravityScale * jumpTime;

                AddComponent(new Timer(jumpTime, true, (timer) =>
                {
                    Pos.Y = Vector2.Lerp(init, aimed, Ease.CubicIn(timer.AmountCompleted())).Y;
                }, () =>
                {
                    Pos.Y = aimed.Y;
                    Velocity = Vector2.Zero;
                    Engine.Cam.LightShake();

                    gravityScale = constGravityScale;

                    StateMachine.Switch(States.Walking);
                }));
            }));
        }

        public IEnumerator MachineGun()
        {
            int numBullets = 15;
            int range = 90;
            bool right = Engine.Player.MiddlePos.X > MiddlePos.X;

            for(int i = 0; i < numBullets; i++)
            {
                Engine.CurrentMap.Instantiate(new MachineGunBullet(MiddlePos, right ? -i * range / numBullets : i * range / numBullets + 180));
                Engine.Cam.Shake(0.3f, 0.7f);
                yield return new Coroutine.WaitForSeconds(0.03f);
            }

            StateMachine.Switch(States.Walking);
        }

        public IEnumerator Swipe()
        {
            Sprite.Color = Color.Green;

            bool right = Pos.X > zones[3].Pos.X;

            float backJumpTime = 0.5f;

            Velocity.Y -= 250;
            if (right)
                Velocity.X = (zones[0].Pos.X + zones[0].Width - Pos.X - Width) / backJumpTime;
            else
                Velocity.X = (zones[0].Pos.X - Pos.X) / backJumpTime;

            Engine.Cam.LightShake();

            yield return new Coroutine.WaitForSeconds(backJumpTime);

            Engine.Cam.LightShake();

            Velocity.X = 0;

            yield return new Coroutine.WaitForSeconds(1f);

            if (right)
                Pos.X = zones[0].Pos.X;
            else
                Pos.X = zones[0].Pos.X + zones[0].Width - Width;

            Sprite.Color = Color.Red;

            if (zones[0].Contains(player))
                player.InstaDeath();

            yield return new Coroutine.WaitForSeconds(1f);

            StateMachine.Switch(States.Walking);
        }

        public void Switch()
        {

            if (StateMachine.Is(States.Walking) && counter < 3)
                return; //we do nothing and keep walkin

            counter = 0;

            if (LeftZone || RightZone)
            {
                StateMachine.Switch(States.Jumping);
                return;
            }
            
            StateMachine.Switch(States.Missiling);
        }

        private void CollisionX()
        {

        }

        private void CollisionY()
        {

        }
    }
}
