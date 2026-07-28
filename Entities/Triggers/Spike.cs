using Fiourp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Unnamed
{
    public class Spike : Entity
    {
        public const int DefaultSize = 8;
        private Direction direction;
        public static Texture2D Texture = DataManager.GetTexture("Objects/Decals").CropTo(new Vector2(32, 56), new Vector2(8));

        public Spike(Vector2 position, Direction direction)
            : base(position)
        {
            Sprite sprite = (Sprite)AddComponent(new Sprite(Texture));

            this.direction = direction;
            float rotation = MathHelper.ToRadians(GetRotation(direction));

            Vector2 hPos = Vector2.Zero;
            int width = DefaultSize;
            int height = DefaultSize;
            switch (direction)
            {
                case Direction.Left:
                    hPos = new Vector2(width / 2, 0);
                    width /= 2;
                    break;
                case Direction.Right:
                    width /= 2;
                    break;
                case Direction.Up:
                    hPos = new Vector2(0, height / 2);
                    height /= 2;
                    break;
                case Direction.Down:
                    height /= 2;
                    break;
            }
            HurtBox h = new HurtBox(new AABBCollider(hPos, width, height));
            h.DeathConditions = Conditions;
            AddComponent(h);

            sprite.Rotation = rotation;
            sprite.Origin = new Vector2(width, height) / 2;
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

        public static float GetRotation(Direction direction)
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
