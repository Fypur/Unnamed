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
        
        private static readonly NineSliceSimple nineSliceUp =
            new NineSliceSimple(DataManager.Objects["JetpackBooster"].CropTo(Vector2.Zero, new Vector2(8)), DataManager.Objects["JetpackBooster"].CropTo(new Vector2(8, 0), new Vector2(8)), DataManager.Objects["JetpackBooster"].CropTo(new Vector2(0, 8), new Vector2(16)), true);

        private static readonly NineSliceSimple nineSliceDown = new NineSliceSimple(nineSliceUp.TopLeft, nineSliceUp.Top, nineSliceUp.Fill.FlipY(), true);
        private static readonly NineSliceSimple nineSliceRight = new NineSliceSimple(nineSliceUp.TopLeft, nineSliceUp.Top, nineSliceUp.Fill.Rotate90(), true);
        private static readonly NineSliceSimple nineSliceLeft = new NineSliceSimple(nineSliceUp.TopLeft, nineSliceUp.Top, nineSliceRight.Fill.FlipX(), true);

        public JetpackBooster(Rectangle bounds, Direction direction) : this(bounds.Location.ToVector2(), bounds.Width, bounds.Height, direction)
        { }

        public JetpackBooster(Vector2 position, Vector2 size, Direction direction) : this(position, (int)size.X, (int)size.Y, direction)
        { }

        public JetpackBooster(Vector2 position, int width, int height, Direction direction) : base(position, width, height, new Sprite())
        {
            switch (direction)
            {
                case Direction.Left: Sprite.NineSliceSettings = nineSliceLeft; break;
                case Direction.Right: Sprite.NineSliceSettings = nineSliceRight; break;
                case Direction.Up: Sprite.NineSliceSettings = nineSliceUp; break;
                case Direction.Down: Sprite.NineSliceSettings = nineSliceDown; break;
            }

            boostingDir = DirectionToVector2(direction);
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
