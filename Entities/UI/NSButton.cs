using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    /// <summary>
    /// Button using the specified NineSliceSettings for its Sprite
    /// </summary>
    public class NSButton : Button
    {
        public TextBox Text;

        public static NineSlice nineSliceSettings = new NineSliceSimple(DataManager.CropTo(DataManager.Objects["Button"], Vector2.Zero, Vector2.One * 8), DataManager.CropTo(DataManager.Objects["Button"], Vector2.UnitX * 8, Vector2.One * 8), DataManager.CropTo(DataManager.Objects["Button"], Vector2.One * 8, Vector2.One * 8), false);

        public NSButton(Vector2 position, int width, int height, bool centered, string text, Action onPressed) : base(position, width, height, centered, new Sprite(nineSliceSettings), onPressed)
        {
            Text = new TextBox(text, "Recursive", position, width, height, 0.5f, Color.Black, true);
            AddChild(Text);
        }
    }
}
