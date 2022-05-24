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

        public SpikeRow(Vector2 position, Direction direction, int length, Direction pointingTowards)
            : base(GetBaseParameter(direction, length, position, out int width, out int height), width, height, null)
        {
            int spikeNb = length / Spike.DefaultSize;
            Vector2 move = Vector2.Zero;
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
                AddChild(new Spike(position + move * i, pointingTowards));
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
    }
}
