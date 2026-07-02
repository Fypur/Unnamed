using Fiourp;
using Microsoft.Xna.Framework;

namespace Unnamed
{
    public class Fire : Kinematic
    {
        private const float coefFire = 0.05f;
        private readonly ParticleType fireParticle = Particles.Fire.Copy();
        private Direction direction;

        public Fire(Vector2 position, Vector2 size, Direction direction) : base(position, new AABBCollider(Vector2.Zero, (int)size.X, (int)size.Y), null)
        {
            Collider.Collidable = false;
            AddHitbox((int)size.X, (int)size.Y, direction);
        }

        public override void Awake()
        {
            base.Awake();

            AddComponent(new Sound3D("SFX/Fire/FirePatch"));
        }

        private void AddHitbox(int width, int height, Direction direction)
        {
            Rectangle emitRect;
            int amountEmitted;
            int amountMoved = 0;
            this.direction = direction;

            switch (direction)
            {
                default:
                case Direction.Up:
                    amountEmitted = (int)(width * coefFire);
                    fireParticle.Direction = -90;
                    emitRect = new Rectangle(amountMoved, height - 1, width - amountMoved * 2, 1);
                    //FireParticle.Acceleration = -Vector2.UnitY * Accel;
                    break;
                case Direction.Down:
                    amountEmitted = (int)(width * coefFire);
                    fireParticle.Direction = 90;
                    emitRect = new Rectangle(amountMoved, 0, width - amountMoved * 2, 1);
                    //FireParticle.Acceleration = Vector2.UnitY * Accel;
                    break;
                case Direction.Left:
                    amountEmitted = (int)(height * coefFire);
                    fireParticle.Direction = 180;
                    emitRect = new Rectangle(width - 1, amountMoved, 1, height - amountMoved * 2);
                    //FireParticle.Acceleration = -Vector2.UnitX * Accel;
                    break;
                case Direction.Right:
                    amountEmitted = (int)(height * coefFire);
                    fireParticle.Direction = 0;
                    emitRect = new Rectangle(0, amountMoved, 1, height - amountMoved * 2);
                    //FireParticle.Acceleration = Vector2.UnitX * Accel;
                    break;
            }

            Vector2 hPos = Vector2.Zero;
            int hWidth = width;
            int hHeight = height;
            switch (direction)
            {
                case Direction.Left:
                    hPos -= Vector2.UnitX;
                    hWidth *= 2;
                    break;
                case Direction.Right:
                    hWidth *= 2;
                    break;
                case Direction.Up:
                    hPos -= Vector2.UnitY;
                    hHeight *= 2;
                    break;
                case Direction.Down:
                    hHeight *= 2;
                    break;
            }

            HurtBox h = new HurtBox(new AABBCollider(hPos, hWidth, hHeight));
            h.DeathConditions = Conditions;
            h.InstaDeath = true;
            AddComponent(h);

            AddComponent(new ParticleEmitter(Engine.CurrentMap.MiddlegroundSystem, fireParticle, emitRect, amountEmitted));
        }

        private bool Conditions(Player player)
        {
            if (direction == Direction.Up)
                return player.Velocity.Y >= 0;
            else if (direction == Direction.Down)
                return player.Velocity.Y <= 0;
            else if (direction == Direction.Left)
                return player.Velocity.X >= 0;
            else
                return player.Velocity.X <= 0;
        }
    }
}
