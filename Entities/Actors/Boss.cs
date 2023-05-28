using Fiourp;
using Microsoft.Xna.Framework;
using System;
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


        public StateMachine<States> StateMachine;
        public enum States { Idle, Walking, Jumping, Missiling, WallMissiling, JumpDown }


        private Player player;
        private bool onGround;
        private int counter;

        private IdentifierTrigger[] triggers;
        private bool DownZone => triggers[0].PlayerIn;
        private bool LeftZone => triggers[1].PlayerIn;
        private bool RightZone => triggers[2].PlayerIn;

        public Boss(Vector2 position) : base(position, 32, 32, 1.7f, new Sprite(Color.Red))
        {
            player = (Player)Engine.Player;

            StateMachine = new StateMachine<States>(States.Walking);

            StateMachine.RegisterStateFunctions(States.Walking, () => EnterWalking(null), null, () => Velocity.X = 0);
            StateMachine.RegisterStateFunctions(States.Jumping, EnterJump, null, null);
            StateMachine.RegisterStateFunctions(States.Missiling, EnterMissiling, null, null);
            StateMachine.RegisterStateFunctions(States.WallMissiling, EnterMissiling, null, null);
            StateMachine.RegisterStateFunctions(States.JumpDown, EnterJumpDown, null, null);

            AddComponent(StateMachine);
        }

        public override void Awake()
        {
            base.Awake();

            triggers = Engine.CurrentMap.Data.GetEntities<IdentifierTrigger>().ToArray();
            triggers = triggers.OrderBy((trig) => trig.Id).ToArray();
        }

        public override void Update()
        {
            base.Update();

            Debug.LogUpdate(StateMachine.CurrentState);

            onGround = Collider.CollideAt(new List<Entity>(Engine.CurrentMap.Data.Platforms), Pos + new Vector2(0, 1));


            /*if (!onGround)
                Gravity();*/

            if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.P))
                StateMachine.Switch(States.Walking);
            if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.O))
                StateMachine.Switch(States.Idle);
            if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.I))
                StateMachine.Switch(States.Jumping);
            if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.U))
                StateMachine.Switch(States.JumpDown);


            MoveX(Velocity.X * Engine.Deltatime, CollisionX);
            MoveY(Velocity.Y * Engine.Deltatime, CollisionY);
        }

        public void EnterWalking(bool? d)
        {
            bool direction;
            if (d == null)
                direction = Rand.NextDouble() < 0.5f;
            else
                direction = d.Value;

            AddComponent(new Timer(walkingTimeCycle, true, (timer) =>
            {
                if (!StateMachine.Is(States.Walking))
                {
                    RemoveComponent(timer);
                    return;
                }

                Velocity.X = walkingSpeed * (direction ? 1 : -1);

                if (Vector2.DistanceSquared(triggers[3].Pos, Pos + Velocity) > 64 * 64)
                {
                    EnterWalking(!direction);
                    RemoveComponent(timer);
                }

            }, () =>
            {
                counter++;

                Switch();

                if (StateMachine.Is(States.Walking))
                    EnterWalking(null);
            }));
        }

        public void EnterJump()
        {
            //Charge jump

            Vector2 p = player.Pos;

            Vector2 aimed;
            if(LeftZone)
                aimed = new Vector2(triggers[1].Pos.X, p.Y);
            else
                aimed = new Vector2(triggers[2].Pos.X + triggers[2].Width, p.Y);

            if (aimed.Y - triggers[2].Pos.Y < 24)
                aimed.Y = triggers[2].Pos.Y + 24;

            Debug.Point(aimed);

            

            AddComponent(new Timer(jumpChargeTime, true, (timer) => 
            {
                Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, new Rectangle((int)Pos.X, (int)Pos.Y + Height - 3, Width, 3), 3);
            }, () =>
            {
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

                }));
            }));

            //Actually Jumping

            //LandImpact
        }

        public void EnterMissiling()
        {

        }

        public void EnterJumpDown()
        {
            Vector2 aimed;
            Rectangle rect;
            if (triggers[1].Collider.Collide((BoxCollider)Collider)) //left
            {
                rect = new Rectangle((int)Pos.X, (int)Pos.Y, 3, Height);
                aimed = triggers[3].Pos - Vector2.UnitX * Rand.NextDouble() * 64;
            }
            else
            {
                rect = new Rectangle((int)Pos.X + Width - 3, (int)Pos.Y, 3, Height);
                aimed = triggers[3].Pos + Vector2.UnitX * Rand.NextDouble() * 64;
            }


            AddComponent(new Timer(jumpChargeTime, true, (timer) =>
            {
                Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, rect, 3);
            }, () =>
            {
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

                }));
            }));
        }

        public void EnterWallMissiling()
        {

        }

        public void Shoot()
        {

        }

        public void Switch()
        {

            if (StateMachine.Is(States.Walking) && counter < 3)
                return; //we do nothing and keep walkin

            counter = 0;

            if (LeftZone || RightZone)
                StateMachine.Switch(States.Jumping);
        }

        private void CollisionX()
        {

        }

        private void CollisionY()
        {

        }
    }
}
