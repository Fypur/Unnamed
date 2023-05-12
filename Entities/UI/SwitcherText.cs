using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    internal class SwitcherText : Switcher
    {
        public SwitcherText(Vector2 position, int width, int height, bool centered, string fieldName, int startValue, int numValues, Dictionary<int, Action> actions) : base(position, width, height, centered, fieldName, "LexendDeca", null, startValue, numValues, actions)
        {
            FieldTextBox.Color = Color.White;
            ValueTextBox.Color = Color.White;
            ValueTextBox.SetText("< " + ValueTextBox.Text + " >");
        }

        public SwitcherText(Vector2 position, int width, int height, bool centered, string fieldName, int startValue, int minValue, int maxValue, Action<int> action) : base(position, width, height, centered, fieldName, "LexendDeca", null, startValue, minValue, maxValue, action)
        {
            FieldTextBox.Color = Color.White;
            ValueTextBox.Color = Color.White;
            ValueTextBox.SetText("< " + ValueTextBox.Text + " >");
        }

        public override void OnMove()
        {
            base.OnMove();

            ValueTextBox.SetText("< " + ValueTextBox.Text + " >");
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

        public override void NotPossible()
        {
            
        }
    }
}
