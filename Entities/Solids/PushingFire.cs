using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    internal class PushingFire : CameraBlock
    {
        public float Speed = 80;
        public Direction Direction;

        private Vector2 dirVec;
        private ParticleType fire = Particles.Fire.Copy();
        public PushingFire(Vector2 position, float speed = 64, Direction direction = Direction.Right) : base(position, 8, 184)
        {
            Direction = direction;

            dirVec = direction switch
            {
                Direction.Left => -Vector2.UnitX,
                Direction.Right => Vector2.UnitX,
                Direction.Up => -Vector2.UnitY,
                Direction.Down => Vector2.UnitY,
                _ => throw new Exception("Direction can't be null");
            };

            make this work

            fire.LifeMin = 1f;
            fire.Direction = 0;
            fire.SpeedMin = speed;


            fire.Acceleration = Vector2.UnitX * (speed + 25);

            Speed = speed;

            HurtBox h = (HurtBox)AddComponent(new HurtBox(-Vector2.UnitX * 10, Width, Height));
            h.InstaDeath = true;
        }

        public override void Awake()
        {
            base.Awake();

            Engine.CurrentMap.CurrentLevel.DontDestroyOnUnload(this);

            ((Player)Engine.Player).OnDeathTransition += ResetPos;
        }

        public override void Update()
        {
            base.Update();

            Move(Speed * Engine.Deltatime, 0);

            if (Engine.Cam.Pos.X > Pos.X + Width)
            {
                Pos.X = Engine.Cam.Pos.X - Width;
            }

            Pos.Y = Engine.Cam.Pos.Y;

            /*if (Collider.Collide(Engine.Player))
                ((Player)Engine.Player).InstaDeath();*/

            Engine.CurrentMap.MiddlegroundSystem.Emit(fire, new Rectangle(Bounds.X + ((Speed == 0) ? 0 : Width), Bounds.Y, Bounds.Width, Bounds.Height), 5);
            Debug.LogUpdate(Engine.CurrentMap.MiddlegroundSystem.Particles.Count);
        }

        public void ResetPos()
        {
            Pos = Engine.CurrentMap.CurrentLevel.Pos - new Vector2(Speed * Engine.Deltatime * 1.5f, 0);

            float speed = Speed;
            Speed = 0;

            AddComponent(new Coroutine(Coroutine.WaitUntil(() => Platformer.player.Velocity != Vector2.Zero), Coroutine.Do(() => Speed = speed)));

            /*Speed = 0.3f * Speed;

            AddComponent(new Timer(0.35f, true, null, () => Speed = speed));*/
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            ((Player)Engine.Player).OnDeathTransition -= ResetPos;
        }
    }
}
