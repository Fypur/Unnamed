using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class SpikeRow : Entity
    {
        public const int defaultSize = 8;

        public SpikeRow(Vector2 position, Spike.Direction direction, int length, Spike.Direction pointingTowards)
            : base(GetBaseParameter(direction, length, position, out int width, out int height), width, height, null)
        {
            int spikeNb = length / Spike.size;
            Vector2 move = Vector2.Zero;
            switch (direction)
            {
                case Spike.Direction.Up:
                    move = new Vector2(0, -Spike.size);
                    break;
                case Spike.Direction.Down:
                    move = new Vector2(0, Spike.size);
                    break;
                case Spike.Direction.Left:
                    move = new Vector2(-Spike.size, 0);
                    break;
                case Spike.Direction.Right:
                    move = new Vector2(Spike.size, 0);
                    break;
            }

            for (int i = 0; i < spikeNb; i++)
                AddChild(new Spike(position + move * i, pointingTowards));
        }

        public static Vector2 GetBaseParameter(Spike.Direction direction, int length, Vector2 position, out int width, out int height)
        {
            width = defaultSize;
            height = defaultSize;

            if (direction == Spike.Direction.Up || direction == Spike.Direction.Down)
                height = length;
            else
                width = length;

            return position;
        }
    }
}
