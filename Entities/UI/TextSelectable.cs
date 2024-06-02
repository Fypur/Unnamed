using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unnamed
{
    public class TextSelectable : Button
    {
        public TextBox Text;
        public TextSelectable(string text, string fontID, Vector2 position, int width, int height, float fontSize, Color color, bool centered = false, TextBox.Alignement alignement = TextBox.Alignement.Center, Action onPressed = null) : base(position, width, height, centered, null, onPressed)
        {
            Selectable = true;
            Text = (TextBox)AddChild(new TextBox(text, fontID, position, width, height, fontSize, color, centered, alignement));
        }

        public override void OnSelected()
        {
            base.OnSelected();

            Text.Color.B = 120;
        }

        public override void OnLeaveSelected()
        {
            base.OnLeaveSelected();

            Text.Color.B = 255;
        }

        public override void OnAddSelectable()
        {
            base.OnAddSelectable();
            Text.Color = Color.White;
        }

        public override void OnRemoveSelectable()
        {
            base.OnRemoveSelectable();
            Text.Color = Color.Gray;
        }
    }
}
