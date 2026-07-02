using Fiourp;
using Microsoft.Xna.Framework;
using System;

namespace Unnamed
{
    internal class PushingFire : CameraBlock
    {
        public float Speed { get; private set; } = 80;
        public Direction Direction;

        private Vector2 dirVec;
        private ParticleType fire = Particles.Fire.Copy();
        private ParticleType bigFire = Particles.Fire.Copy();
        public PushingFire(Vector2 position, float speed = 64, Direction direction = Direction.Right) : base(position, GetStats(direction, out int height), height)
        {
            this.Direction = direction;

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

            UpdateBigFire();

            AddComponent(new Coroutine(Coroutine.WaitUntil(() => Platformer.Player.Velocity != Vector2.Zero), Coroutine.Do(() =>
            this.Speed = speed)));

            HurtBox h = new HurtBox(-dirVec * 10, Width, Height);
            AddComponent(h);
            h.InstaDeath = true;

            ChangeDirection(direction);
        }

        public override void Awake()
        {
            base.Awake();
            ResetPos();

            AddComponent(new Sound3D("SFX/Fire/PushingFire"));

            Engine.CurrentMap.CurrentLevel.DontDestroyOnUnload(this);

            AddComponent(new CircleLight(MiddlePos, 200, new Color(Color.Orange, 200), Color.Transparent));

            Platformer.Player.OnDeathTransition += ResetPos;
        }

        public override void Update()
        {
            base.Update();

            Move(dirVec * Speed * Engine.Deltatime);

            Debug.LogUpdate(MiddlePos);
            Debug.LogUpdate(Input.MousePos);

            Rectangle rect;
            Vector2 particlePos;
            Vector2 particleSize;
            switch (Direction)
            {
                case Direction.Right:
                    rect = new Rectangle((int)(Pos.X - Platformer.GameCam.Camera.Width), (int)Pos.Y, (int)(Platformer.GameCam.Camera.Width + 10), Height);
                    particlePos = Pos;
                    particleSize = Pos + Size.OnlyY();
                    Pos.Y = Platformer.GameCam.Camera.Pos.Y;
                    if (Platformer.GameCam.Camera.Pos.X > Pos.X + Width && Speed != 0)
                        Pos.X = Platformer.GameCam.Camera.Pos.X - Width;
                    break;
                case Direction.Left:
                    rect = new Rectangle((int)Pos.X, (int)Pos.Y, (int)(Platformer.GameCam.Camera.Width + 10), Height);
                    particlePos = Pos + Size.OnlyX();
                    particleSize = Pos + Size;
                    Pos.Y = Platformer.GameCam.Camera.Pos.Y;
                    if (Platformer.GameCam.Camera.Pos.X + Platformer.GameCam.Camera.Width < Pos.X && Speed != 0)
                        Pos.X = Platformer.GameCam.Camera.Pos.X + Platformer.GameCam.Camera.Width;
                    break;
                case Direction.Up:
                    rect = new Rectangle((int)Pos.X, (int)Pos.Y, Width, (int)(Platformer.GameCam.Camera.Height + 10));
                    particlePos = Pos + Size.OnlyY();
                    particleSize = Pos + Size;
                    Pos.X = Platformer.GameCam.Camera.Pos.X;
                    if (Platformer.GameCam.Camera.Pos.Y + Platformer.GameCam.Camera.Height < Pos.Y && Speed != 0)
                        Pos.Y = Platformer.GameCam.Camera.Pos.Y + Platformer.GameCam.Camera.Height;
                    break;
                default: //Down
                    rect = new Rectangle((int)Pos.X, (int)(Pos.Y - Platformer.GameCam.Camera.Height), Width, (int)(Platformer.GameCam.Camera.Height + 10));
                    particlePos = Pos;
                    particleSize = Pos + Size.OnlyX();
                    Pos.X = Platformer.GameCam.Camera.Pos.X;
                    if (Platformer.GameCam.Camera.Pos.Y > Pos.Y + Height && Speed != 0)
                        Pos.Y = Platformer.GameCam.Camera.Pos.Y;
                    break;
            }

            Engine.CurrentMap.MiddlegroundSystem.Emit(fire, particlePos, particleSize, (int)(200 * Engine.Deltatime), fire.Direction);
            Engine.CurrentMap.MiddlegroundSystem.Emit(bigFire, rect, (int)(300 * Engine.Deltatime));
        }

        private void UpdateBigFire()
        {
            bigFire = fire.Copy();
            bigFire.Size = 20;
            bigFire.LifeMax = 1.5f;
        }

        public override void Render()
        {
            base.Render();

            //Drawing.DrawQuad(Pos - new Vector2(Platformer.GameCam.Camera.Width, 0), new Color(Color.Red, 200), Pos - new Vector2(Platformer.GameCam.Camera.Width, 0) + Size.OnlyY(), new Color(Color.Red, 200), Pos + Size.OnlyY() + dirVec * 4, new Color(Color.Orange, 200), Pos + dirVec * 4, new Color(Color.Orange, 200));
        }

        public void ResetPos()
        {
            switch (Direction)
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

            //Pos = Engine.CurrentMap.CurrentLevel.Pos + Engine.CurrentMap.CurrentLevel.Size / 2 - dirVec * Engine.CurrentMap.CurrentLevel.Size / 2 + dirVec * new Vector2(this.Speed * Engine.Deltatime * 1.5f, 0);

            float speed = this.Speed;
            this.Speed = 0;

            AddComponent(new Coroutine(Coroutine.WaitUntil(() => Platformer.Player.Velocity != Vector2.Zero), Coroutine.Do(() => this.Speed = speed)));

            /*Speed = 0.3f * Speed;

            AddComponent(new Timer(0.35f, null, () => Speed = speed));*/
        }

        public override bool CollidingConditions(Collider other)
        {
            if (Speed == 0)
                return false;

            return base.CollidingConditions(other);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Platformer.Player.OnDeathTransition -= ResetPos;
        }

        public void ChangeSpeed(float speed)
        {
            this.Speed = speed;
            fire.SpeedMin = speed;
            fire.SpeedMax = speed * 2;
            fire.Acceleration = dirVec * (speed + 25);

            UpdateBigFire();
        }

        public void ChangeDirection(Direction direction)
        {
            this.Direction = direction;

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

            UpdateBigFire();

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
