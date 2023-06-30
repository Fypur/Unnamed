using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiourp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer
{
    public class JetpackBooster : PlayerTrigger
    {
        private float boostX = 2.4f;
        private float boostY = 2.4f;
        private Vector2 boostingDir;

        public JetpackBooster(Rectangle bounds, Direction direction) : this(bounds.Location.ToVector2(), bounds.Width, bounds.Height, direction)
        { }

        public JetpackBooster(Vector2 position, Vector2 size, Direction direction) : this(position, (int)size.X, (int)size.Y, direction)
        { }

        public JetpackBooster(Vector2 position, int width, int height, Direction direction) : base(position, width, height, new Sprite())
        {
            /*switch (direction)
            {
                case Direction.Left: Sprite.NineSliceSettings = nineSliceLeft; break;
                case Direction.Right: Sprite.NineSliceSettings = nineSliceRight; break;
                case Direction.Up: Sprite.NineSliceSettings = nineSliceUp; break;
                case Direction.Down: Sprite.NineSliceSettings = nineSliceDown; break;
            }
            */

            Sprite.Add(Sprite.AllAnimData["JetpackBooster"]);
            Sprite.Play("right");
            Sprite.Color.A = 170;
            boostingDir = DirectionToVector2(direction);
        }

        public override void Render()
        {
            Sprite.Color.A = 220;
            base.Render();
            for (int i = 0; i < Width / Sprite.Width; i++)
            {
                for (int j = 0; j < Height / Sprite.Height; j++)
                {
                    Sprite.Offset = new Vector2(Sprite.Width * i, Sprite.Height * j);
                    Sprite.Render();
                }
            }
        }

        public override void OnTriggerEnter(Player player)
        {
            player.JetpackDirectionalPowerCoef += boostingDir * new Vector2(boostX, boostY);
        }

        public override void OnTriggerExit(Player player)
        {
            player.JetpackDirectionalPowerCoef -= boostingDir * new Vector2(boostX, boostY);
        }

        public Vector2 DirectionToVector2(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return new Vector2(0, 1);
                case Direction.Down:
                    return new Vector2(0, -1);
                case Direction.Left:
                    return new Vector2(-1, 0);
                case Direction.Right:
                    return new Vector2(1, 0);
                default:
                    return Vector2.One;
            }
        }
    }
}
