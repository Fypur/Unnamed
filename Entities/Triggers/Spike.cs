using Fiourp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platformer
{
    public class Spike : Solid
    {
        public const int DefaultSize = 8;
        private Direction direction;

        public Spike(Vector2 position, Direction direction)
            : base(position, DefaultSize, DefaultSize, new Sprite(DataManager.GetTexture("SpikeTest")))
        {
            this.direction = direction;
            float rotation = MathHelper.ToRadians(GetRotation(direction));
            Collider.Collidable = false;

            var h = new HurtBox(Vector2.Zero, Width, Height);
            h.DeathConditions = Conditions;
            AddComponent(h);

            Sprite.Bounds = Bounds;
            Sprite.Rotation = rotation;
            Sprite.Origin = HalfSize;
            Sprite.Centered = true;
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

        private static float GetRotation(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return 0;
                case Direction.Down:
                    return 180;
                case Direction.Left:
                    return 270;
                default:
                    return 90;
            }
        }

        private static Direction GetDirection(float rotation)
        {
            switch (rotation)
            {
                case 0:
                    return Direction.Up;
                case 90:
                    return Direction.Right;
                case 180:
                    return Direction.Down;
                default:
                    return Direction.Left;
            }
        }
    }
}
