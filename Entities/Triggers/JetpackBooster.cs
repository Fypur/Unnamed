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
        private Vector2 boostingDir;

        #region NineSlice Textures

        private static readonly NineSliceSettings defaultNineSlice = 
            new NineSliceSettings(DataManager.Objects["JetpackBooster"].CropTo(Vector2.Zero, new Vector2(8)),
                DataManager.Objects["JetpackBooster"].CropTo(new Vector2(8, 0), new Vector2(8)),
                null, true);

        private static Texture2D fillUp = DataManager.Objects["JetpackBooster"].CropTo(new Vector2(8), new Vector2(16));
        private static Texture2D fillDown = fillUp.FlipY();
        private static Texture2D fillRight = fillUp.Rotate90();
        private static Texture2D fillLeft = fillRight.FlipX();
        private static Dictionary<Direction, Texture2D> Fill = new() { { Direction.Up, fillUp }, { Direction.Down, fillDown }, { Direction.Left, fillLeft }, { Direction.Right, fillRight } };

        #endregion

        public JetpackBooster(Rectangle bounds, Direction direction) : base(bounds, new Sprite())
        {
            Sprite.NineSliceSettings = defaultNineSlice;
            Sprite.NineSliceSettings.Fill = Fill[direction];
            boostingDir = DirectionToVector2(direction);
        }

        public JetpackBooster(Vector2 position, Vector2 size, Direction direction) : base(position, size, new Sprite())
        {
            Sprite.NineSliceSettings = defaultNineSlice;
            Sprite.NineSliceSettings.Fill = Fill[direction];
            boostingDir = DirectionToVector2(direction);
        }

        public JetpackBooster(Vector2 position, int width, int height, Direction direction) : base(position, width, height, new Sprite())
        {
            Sprite.NineSliceSettings = defaultNineSlice;
            Sprite.NineSliceSettings.Fill = Fill[direction];
            boostingDir = DirectionToVector2(direction);
        }

        public override void OnTriggerEnter(Player player)
        {
            player.jetpackDirectionalPowerCoef += boostingDir * new Vector2(10, 1.3f);
        }

        public override void OnTriggerExit(Player player)
        {
            player.jetpackDirectionalPowerCoef -= boostingDir * new Vector2(10, 1.3f);
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
