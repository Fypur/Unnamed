using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiourp;
using Microsoft.Xna.Framework;

namespace Platformer
{
    public class Fire : DeathTrigger
    {
        private const float coefFire = 0.05f;

        public ParticleType FireParticle = new ParticleType() 
        {
            LifeMin = 0.4f,
            LifeMax = 3f,
            Size = 5,
            SizeRange = 2,
            SizeChange = ParticleType.FadeModes.Linear,
            Color = Color.Red,
            Color2 = Color.Yellow,
            SpeedMin = 5,
            SpeedMax = 10
        };

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

        private void DetermineStats(Direction direction)
        {
            Rectangle emitRect;
            int amountEmitted;
            int amountMoved = 0;

            switch (direction)
            {
                default:
                case Direction.Up:
                    amountEmitted = (int)(Size.X * coefFire);
                    FireParticle.Direction = -90;
                    emitRect = new Rectangle(amountMoved, Bounds.Height - 1, Bounds.Width - amountMoved * 2, 1);
                    //FireParticle.Acceleration = -Vector2.UnitY * Accel;
                    break;
                case Direction.Down:
                    amountEmitted = (int)(Size.X * coefFire);
                    FireParticle.Direction = 90;
                    emitRect = new Rectangle(amountMoved, 0, Bounds.Width - amountMoved * 2, 1);
                    //FireParticle.Acceleration = Vector2.UnitY * Accel;
                    break;
                case Direction.Left:
                    amountEmitted = (int)(Size.Y * coefFire);
                    FireParticle.Direction = 180;
                    emitRect = new Rectangle(Bounds.Width - 1, amountMoved, 1, Bounds.Height - amountMoved * 2);
                    //FireParticle.Acceleration = -Vector2.UnitX * Accel;
                    break;
                case Direction.Right:
                    amountEmitted = (int)(Size.Y * coefFire);
                    FireParticle.Direction = 0;
                    emitRect = new Rectangle(0, amountMoved, 1, Bounds.Height - amountMoved * 2);
                    //FireParticle.Acceleration = Vector2.UnitX * Accel;
                    break;
            }

            AddComponent(new ParticleEmitter(Engine.CurrentMap.MiddlegroundSystem, FireParticle, emitRect, amountEmitted));
        }
    }
}
