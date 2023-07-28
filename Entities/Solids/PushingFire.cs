using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    internal class PushingFire : CameraBlock
    {
        public float Speed { get; private set; } = 80;
        private Direction direction;

        private Vector2 dirVec;
        private ParticleType fire = Particles.Fire.Copy();
        private ParticleType bigFire = Particles.Fire.Copy();
        public PushingFire(Vector2 position, float speed = 64, Direction direction = Direction.Right) : base(position, GetStats(direction, out int height), height)
        {
            this.direction = direction;

            dirVec = direction switch
            {
                Direction.Left => -Vector2.UnitX,
                Direction.Right => Vector2.UnitX,
                Direction.Up => -Vector2.UnitY,
                Direction.Down => Vector2.UnitY,
                _ => throw new Exception("Direction can't be null")
            };

            fire.LifeMin = 1f;
            fire.Direction = 0;
            fire.SpeedMin = speed;
            fire.SpeedMax = speed * 2;



            this.Speed = speed;

            HurtBox h = new HurtBox(-dirVec * 10, Width, Height);
            AddComponent(h);
            h.InstaDeath = true;

            ChangeDirection(direction);
        }

        public override void Awake()
        {
            base.Awake();
            ResetPos();

            Engine.CurrentMap.CurrentLevel.DontDestroyOnUnload(this);

            ((Player)Engine.Player).OnDeathTransition += ResetPos;
        }

        public override void Update()
        {
            base.Update();

            Move(dirVec * Speed * Engine.Deltatime);

            switch (direction)
            {
                case Direction.Right:
                    Engine.CurrentMap.MiddlegroundSystem.Emit(fire, Pos, Pos + Size.OnlyY(), 5, fire.Direction);
                    Pos.Y = Engine.Cam.Pos.Y;
                    if (Engine.Cam.Pos.X > Pos.X + Width)
                        Pos.X = Engine.Cam.Pos.X - Width;
                    break;
                case Direction.Left:
                    Engine.CurrentMap.MiddlegroundSystem.Emit(fire, Pos + Size.OnlyX(), Pos + Size, 5, fire.Direction);
                    Pos.Y = Engine.Cam.Pos.Y;
                    if (Engine.Cam.Pos.X + Engine.Cam.Width < Pos.X)
                        Pos.X = Engine.Cam.Pos.X + Engine.Cam.Width;
                    break;
                case Direction.Up:
                    Engine.CurrentMap.MiddlegroundSystem.Emit(fire, Pos + Size.OnlyY(), Pos + Size, 5, fire.Direction);
                    Pos.X = Engine.Cam.Pos.X;
                    if (Engine.Cam.Pos.Y + Engine.Cam.Height < Pos.Y)
                        Pos.Y = Engine.Cam.Pos.Y + Engine.Cam.Height;
                    break;
                case Direction.Down:
                    Engine.CurrentMap.MiddlegroundSystem.Emit(fire, Pos, Pos + Size.OnlyX(), 5, fire.Direction);
                    Pos.X = Engine.Cam.Pos.X;
                    if (Engine.Cam.Pos.Y > Pos.Y + Height)
                        Pos.Y = Engine.Cam.Pos.Y;
                    break;
            }

            /*bigFire = fire.Copy();
            bigFire.Size = 20;

            Rectangle rect = new Rectangle((int)(Pos.X - Engine.Cam.Size.X), (int)Pos.Y, (int)(Engine.Cam.Size.X + 10), Height);
            Engine.CurrentMap.MiddlegroundSystem.Emit(bigFire, rect, 20);*/
        }

        public override void Render()
        {
            base.Render();

            /*Drawing.DrawQuad(Pos - new Vector2(Engine.Cam.Size.X, 0), new Color(Color.Red, 200), Pos - new Vector2(Engine.Cam.Size.X, 0) + Size.OnlyY(), new Color(Color.Red, 200), Pos + Size.OnlyY() + dirVec * 4, new Color(Color.Orange, 200), Pos + dirVec * 4, new Color(Color.Orange, 200));*/
        }

        public void ResetPos()
        {
            switch (direction)
            {
                case Direction.Right:
                    Pos = Engine.CurrentMap.CurrentLevel.Pos - new Vector2(this.Speed * Engine.Deltatime * 1.5f, 0);
                    break;
                case Direction.Left:
                    Pos = Engine.CurrentMap.CurrentLevel.Pos + Engine.CurrentMap.CurrentLevel.Size * new Vector2(1, 0) + new Vector2(this.Speed * Engine.Deltatime * 1.5f, 0);
                    break;
                case Direction.Up:
                    Pos = Engine.CurrentMap.CurrentLevel.Pos + Engine.CurrentMap.CurrentLevel.Size * new Vector2(0, 1) + new Vector2(this.Speed * Engine.Deltatime * 1.5f, 0);
                    break;
                case Direction.Down:
                    Pos = Engine.CurrentMap.CurrentLevel.Pos + Engine.CurrentMap.CurrentLevel.Size * new Vector2(0, 0) - new Vector2(this.Speed * Engine.Deltatime * 1.5f, 0);
                    break;
            }

            Pos = Engine.CurrentMap.CurrentLevel.Pos + Engine.CurrentMap.CurrentLevel.Size / 2 - dirVec * Engine.CurrentMap.CurrentLevel.Size / 2 + dirVec * new Vector2(this.Speed * Engine.Deltatime * 1.5f, 0);

            float speed = this.Speed;
            this.Speed = 0;

            AddComponent(new Coroutine(Coroutine.WaitUntil(() => Platformer.player.Velocity != Vector2.Zero), Coroutine.Do(() => this.Speed = speed)));

            /*Speed = 0.3f * Speed;

            AddComponent(new Timer(0.35f, true, null, () => Speed = speed));*/
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            ((Player)Engine.Player).OnDeathTransition -= ResetPos;
        }

        public void ChangeSpeed(float speed)
        {
            this.Speed = speed;
            fire.SpeedMin = speed;
            fire.SpeedMax = speed * 2;
            fire.Acceleration = dirVec * (speed + 25);
        }

        public void ChangeDirection(Direction direction)
        {
            this.direction = direction;

            dirVec = direction switch
            {
                Direction.Left => -Vector2.UnitX,
                Direction.Right => Vector2.UnitX,
                Direction.Up => -Vector2.UnitY,
                Direction.Down => Vector2.UnitY,
                _ => throw new Exception("Direction can't be null")
            };

            fire.Direction = dirVec.ToAngleDegrees(); /*direction switch
            {
                Direction.Left => 180,
                Direction.Right => 0,
                Direction.Up => 90,
                Direction.Down => 270,
                _ => throw new Exception("Direction can't be null")
            };*/

            fire.Acceleration = dirVec * (Speed + 25);

            Width = GetStats(direction, out int height);
            Height = height;

            HurtBox h = GetComponent<HurtBox>();
            h.Trigger.Width = Width;
            h.Trigger.Height = Height;
        }

        private static int GetStats(Direction direction, out int height)
        {
            switch (direction)
            {
                case Direction.Left:
                    height = 184;
                    return 8;
                case Direction.Right:
                    height = 184;
                    return 8;
                case Direction.Up:
                    height = 8;
                    return 320;
                default:
                    height = 8;
                    return 320;

            }
        }
    }
}
