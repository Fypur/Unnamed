using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    internal class BoolSwitcherText : BoolSwitcher
    {
        public BoolSwitcherText(Vector2 position, int width, int height, bool centered, string fieldName, bool startValue, Action onOn, Action onOff) : base(position, width, height, centered, fieldName, "LexendDeca", null, startValue, onOn, onOff)
        {
            FieldTextBox.Color = Color.White;
            ValueTextBox.Color = Color.White;
        }

        public override void OnSelected()
        {
            base.OnSelected();

            FieldTextBox.Color.B = 120;
            ValueTextBox.Color.B = 120;
        }

        public override void OnLeaveSelected()
        {
            base.OnLeaveSelected();

            FieldTextBox.Color.B = 255;
            ValueTextBox.Color.B = 255;
        }
    }
}
