using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platformer
{
    public class SpikeRow : Entity
    {
        public const int defaultSize = 8;
        public Direction Direction;

        public SpikeRow(Vector2 position, Direction direction, int length, Direction pointingTowards)
            : base(GetBaseParameter(direction, length, position, out int width, out int height), width, height, null)
        {
            int spikeNb = length / Spike.DefaultSize;
            Vector2 move = Vector2.Zero;
            Direction = pointingTowards;

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

            for (int i = 0; i < spikeNb; i++)
            {
                Spike s = new Spike(position + move * i, pointingTowards);
                AddChild(s);
                s.Active = false;
            }

            Vector2 hPos = Vector2.Zero;
            int hWidth = Width;
            int hHeight = Height;
            switch (pointingTowards)
            {
                case Direction.Left:
                    hPos = Size.OnlyX() / 2;
                    hWidth /= 2;
                    break;
                case Direction.Right:
                    hWidth /= 2;
                    break;
                case Direction.Up:
                    hPos = Size.OnlyY() / 2;
                    hHeight /= 2;
                    break;
                case Direction.Down:
                    hHeight /= 2;
                    break;
            }

            HurtBox h = new HurtBox(hPos, hWidth, hHeight);
            h.DeathConditions = Conditions;
            AddComponent(h);
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

        private bool Conditions(Player player)
        {
            if (Direction == Direction.Up)
                return player.Velocity.Y >= 0;
            else if (Direction == Direction.Down)
                return player.Velocity.Y <= 0;
            else if (Direction == Direction.Left)
                return player.Velocity.X >= 0;
            else
                return player.Velocity.X <= 0;
        }
    }
}
