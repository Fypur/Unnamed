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
        public enum Direction { Up, Down, Left, Right };

        public SpikeRow(Vector2 position, Direction direction, int length, Direction pointingTowards)
            : base(GetBaseParameter(direction, length, position, out int width, out int height), width, height, null)
        {
            int spikeNb = length / Spike.size;

            Vector2 move = Vector2.Zero;

            switch (direction)
            {
                case Direction.Up:
                    move = new Vector2(0, -Spike.size);
                    break;
                case Direction.Down:
                    move = new Vector2(0, Spike.size);
                    break;
                case Direction.Left:
                    move = new Vector2(-Spike.size, 0);
                    break;
                case Direction.Right:
                    move = new Vector2(Spike.size, 0);
                    break;
            }

            float rotation = 0;
            switch (pointingTowards)
            {
                case Direction.Up:
                    rotation = 0;
                    break;
                case Direction.Down:
                    rotation = 180;
                    break;
                case Direction.Left:
                    rotation = 270;
                    break;
                case Direction.Right:
                    rotation = 90;
                    break;
            }
            for (int i = 0; i < spikeNb; i++)
                AddChild(new Spike(position + move * i, rotation));
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
