using Fiourp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Unnamed
{
    public class Spike : Solid
    {
        public const int DefaultSize = 8;
        private Direction direction;
        private static Texture2D texture = DataManager.GetTexture("Objects/Decals").CropTo(new Vector2(32, 56), new Vector2(8));

        public Spike(Vector2 position, Direction direction)
            : base(position, DefaultSize, DefaultSize, new Sprite(texture))
        {
            this.direction = direction;
            float rotation = MathHelper.ToRadians(GetRotation(direction));
            Collider.Collidable = false;

            Vector2 hPos = Vector2.Zero;
            int width = Width;
            int height = Height;
            switch (direction)
            {
                case Direction.Left:
                    hPos = Size.OnlyX() / 2;
                    width /= 2;
                    break;
                case Direction.Right:
                    width /= 2;
                    break;
                case Direction.Up:
                    hPos = Size.OnlyY() / 2;
                    height /= 2;
                    break;
                case Direction.Down:
                    height /= 2;
                    break;
            }
            HurtBox h = new HurtBox(hPos, width, height);
            h.DeathConditions = Conditions;
            AddComponent(h);

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
