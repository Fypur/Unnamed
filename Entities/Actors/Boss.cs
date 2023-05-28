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
        private const float jumpForce = 500f;


        public StateMachine<States> StateMachine;
        public enum States { Idling, Walking, Jumping, Missiling }


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

            StateMachine.RegisterStateFunctions(States.Walking, () => EnterWalking(null), null, null);
            StateMachine.RegisterStateFunctions(States.Jumping, EnterJump, null, null);
            StateMachine.RegisterStateFunctions(States.Missiling, EnterMissiling, null, null);

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

            Debug.LogUpdate(DownZone, LeftZone, RightZone);

            onGround = Collider.CollideAt(new List<Entity>(Engine.CurrentMap.Data.Platforms), Pos + new Vector2(0, 1));


            if (!onGround)
                Gravity();

            if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.P))
                StateMachine.Switch(States.Walking);
            if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.O))
                StateMachine.Switch(States.Idling);
            if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.I))
                StateMachine.Switch(States.Idling);


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
                Velocity.X = walkingSpeed * (direction ? 1 : -1);

                if (Vector2.DistanceSquared(triggers[3].Pos, Pos + Velocity) > 64 * 64)
                {
                    EnterWalking(!direction);
                    RemoveComponent(timer);
                }

            }, () =>
            {
                counter++;

                //if(counter < 3)
                    EnterWalking(null);
                /*else
                {
                    Switch();
                }*/
            }));
        }

        public void EnterJump()
        {
            //Charge jump
            AddComponent(new Timer(jumpChargeTime, true, (timer) => 
            {
                Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, new Rectangle((int)Pos.X, (int)Pos.Y + Height - 3, Width, 3), 3);
            }, () =>
            {





            }));

            //Actually Jumping

            //LandImpact
        }

        public void EnterMissiling()
        {

        }

        public void Switch()
        {
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
