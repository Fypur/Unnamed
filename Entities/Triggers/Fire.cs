using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiourp;
using Microsoft.Xna.Framework;

namespace Unnamed
{
    public class Fire : DeathTrigger
    {
        private const float coefFire = 0.05f;
        private readonly ParticleType fireParticle = Particles.Fire.Copy();
        private Direction direction;

        public Fire(Rectangle bounds, Direction direction) : base(bounds)
        {
            DetermineStats(direction);
        }

        public Fire(Vector2 position, Vector2 size, Direction direction) : base(position, size)
        {
            DetermineStats(direction);
        }

        public Fire(Vector2 position, int width, int height, Direction direction) : base(position, width, height)
        {
            DetermineStats(direction);
        }

        public override void Awake()
        {
            base.Awake();

            AddComponent(new Sound3D("SFX/Fire/FirePatch"));
        }

        private void DetermineStats(Direction direction)
        {
            Rectangle emitRect;
            int amountEmitted;
            int amountMoved = 0;
            this.direction = direction;

            switch (direction)
            {
                default:
                case Direction.Up:
                    amountEmitted = (int)(Size.X * coefFire);
                    fireParticle.Direction = -90;
                    emitRect = new Rectangle(amountMoved, Bounds.Height - 1, Bounds.Width - amountMoved * 2, 1);
                    //FireParticle.Acceleration = -Vector2.UnitY * Accel;
                    break;
                case Direction.Down:
                    amountEmitted = (int)(Size.X * coefFire);
                    fireParticle.Direction = 90;
                    emitRect = new Rectangle(amountMoved, 0, Bounds.Width - amountMoved * 2, 1);
                    //FireParticle.Acceleration = Vector2.UnitY * Accel;
                    break;
                case Direction.Left:
                    amountEmitted = (int)(Size.Y * coefFire);
                    fireParticle.Direction = 180;
                    emitRect = new Rectangle(Bounds.Width - 1, amountMoved, 1, Bounds.Height - amountMoved * 2);
                    //FireParticle.Acceleration = -Vector2.UnitX * Accel;
                    break;
                case Direction.Right:
                    amountEmitted = (int)(Size.Y * coefFire);
                    fireParticle.Direction = 0;
                    emitRect = new Rectangle(0, amountMoved, 1, Bounds.Height - amountMoved * 2);
                    //FireParticle.Acceleration = Vector2.UnitX * Accel;
                    break;
            }

            Vector2 hPos = Vector2.Zero;
            int hWidth = Width;
            int hHeight = Height;
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

            HurtBox h = new HurtBox(hPos, hWidth, hHeight);
            h.DeathConditions = Conditions;
            h.InstaDeath = true;
            AddComponent(h);

            AddComponent(new ParticleEmitter(Engine.CurrentMap.MiddlegroundSystem, fireParticle, emitRect, amountEmitted));
        }

        private new bool Conditions(Player player)
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
