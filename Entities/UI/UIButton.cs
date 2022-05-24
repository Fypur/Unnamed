using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class UIButton : Button
    {
        public static NineSliceSettings nineSliceSettings = new NineSliceSettings(DataManager.CropTo(DataManager.Objects["Button"], Vector2.Zero, Vector2.One * 8), DataManager.CropTo(DataManager.Objects["Button"], Vector2.UnitX * 8, Vector2.One * 8), DataManager.CropTo(DataManager.Objects["Button"], Vector2.One * 8, Vector2.One * 8), false);
        public UIButton(Vector2 position, int width, int height, Action onPressed) : base(position, width, height, new Sprite(nineSliceSettings), onPressed)
        {
        }
    }
}
