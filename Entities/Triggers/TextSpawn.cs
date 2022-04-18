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
        Vector2 initPos;
        public string Text;
        public TextBox TextBox;

        public TextSpawn(Vector2 position, Vector2 size, Vector2 textPos, string text) : base(position, size, Sprite.None)
        {
            Text = text;
            initPos = textPos;
            TextBox = new TextBox(text, "LexendDeca", Engine.Cam.RenderTargetToWorldPosition(textPos), int.MaxValue, int.MaxValue, 0.7f);
            TextBox.Active = false;
            AddChild(TextBox);
        }

        public override void Update()
        {
            base.Update();
            TextBox.Pos = Engine.Cam.RenderTargetToWorldPosition(initPos);
        }

        public override void OnTriggerEnter(Entity entity)
        {
            //TODO: Progressive Text Removing
            TextBox.Active = true;
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
