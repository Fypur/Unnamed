using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiourp;
using Microsoft.Xna.Framework;

namespace Basic_platformer
{
    public class TextSpawn : PlayerTrigger
    {
        public string Text;
        public TextBox TextBox;

        public TextSpawn(Vector2 position, Vector2 size, Vector2 textPos, string text) : base(position, size, Sprite.None)
        {
            Text = text;
            Vector2 offset = Engine.Cam.WorldToScreenPosition(textPos) + Engine.Cam.ScreenToWorldPosition(Vector2.Zero);
            TextBox = new TextBox(text, "LexendDeca", offset, int.MaxValue, int.MaxValue, 0.7f);
            TextBox.Active = false;
            AddChild(TextBox);
        }

        public override void OnTriggerEnter(Entity entity)
        {
            //TODO: Progressive Text Removing
            TextBox.Active = true;
            Debug.Log(TextBox);
            TextBox.ClearText();
            TextBox.ProgressiveDraw(Text, 0.01f, true);
        }

        public override void OnTriggerExit(Entity entity)
        {
            TextBox.Active = false;
            TextBox.StopProgressiveDraw();
            TextBox.ClearText();
        }
    }
}
