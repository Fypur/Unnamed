using Fiourp;
using Microsoft.Xna.Framework;

namespace Unnamed
{
    public class SpikeRow : Entity
    {
        public int Width;
        public int Height;

        public const int defaultSize = 8;
        private Direction pointingTowards;
        private Vector2 move;
        private int spikeNb;
        private float rotation;

        public SpikeRow(Vector2 position, Direction direction, int length, Direction pointingTowards)
            : base(GetBaseParameter(direction, length, position, out int width, out int height))
        {
            Width = width;
            Height = height;
            spikeNb = length / Spike.DefaultSize;
            this.pointingTowards = pointingTowards;

            switch (direction)
            {
                case Direction.Up:
                    move = new Vector2(0, -Spike.DefaultSize);
                    break;
                case Direction.Down:
                    move = new Vector2(0, Spike.DefaultSize);
                    break;
                case Direction.Left:
                    move = new Vector2(-Spike.DefaultSize, 0);
                    break;
                case Direction.Right:
                    move = new Vector2(Spike.DefaultSize, 0);
                    break;
            }

            Vector2 hPos = Vector2.Zero;
            int hWidth = Width;
            int hHeight = Height;
            switch (pointingTowards)
            {
                case Direction.Left:
                    hPos = new Vector2(Width / 2, 0);
                    hWidth /= 2;
                    break;
                case Direction.Right:
                    hWidth /= 2;
                    break;
                case Direction.Up:
                    hPos = new Vector2(0, Height / 2);
                    hHeight /= 2;
                    break;
                case Direction.Down:
                    hHeight /= 2;
                    break;
            }

            HurtBox h = new HurtBox(new AABBCollider(hPos, hWidth, hHeight));
            h.DeathConditions = Conditions;
            AddComponent(h);

            rotation = MathHelper.ToRadians(Spike.GetRotation(pointingTowards));
        }

        public static Vector2 GetBaseParameter(Direction direction, int length, Vector2 position, out int width, out int height)
        {
            width = defaultSize;
            height = defaultSize;

            if (direction == Direction.Up || direction == Direction.Down)
                height = length;
            else
                width = length;

            return position;
        }

        public override void Render()
        {
            base.Render();

            for (int i = 0; i < spikeNb; i++)
            {
                Drawing.Draw(Spike.Texture, Pos + move * i + new Vector2(Spike.Texture.Width, Spike.Texture.Height) / 2, null, Color.White, rotation, new Vector2(Spike.Texture.Width, Spike.Texture.Height) / 2, Vector2.One);
            }
        }

        private bool Conditions(Player player)
        {
            if (pointingTowards == Direction.Up)
                return player.Velocity.Y >= 0;
            else if (pointingTowards == Direction.Down)
                return player.Velocity.Y <= 0;
            else if (pointingTowards == Direction.Left)
                return player.Velocity.X >= 0;
            else
                return player.Velocity.X <= 0;
        }
    }
}
