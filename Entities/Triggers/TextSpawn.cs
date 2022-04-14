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
            TextBox = new TextBox(text, "LexendDeca", Engine.Cam.RenderTargetToScreenPosition(textPos), int.MaxValue, int.MaxValue, 2);
            TextBox.Active = false;
            Debug.Log($"position: {Engine.Cam.RenderTargetToScreenPosition(textPos)}, {text}");
            AddChild(TextBox);
        }

        public override void OnTriggerEnter(Entity entity)
        {
            TextBox.Active = true;
            TextBox.ClearText();
            TextBox.ProgressiveDraw(Text, 0.01f, true);
        }

        public override void OnTriggerExit(Entity entity)
        {
            TextBox.Active = false;
        }
    }
}
