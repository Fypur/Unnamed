using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiourp;
using Microsoft.Xna.Framework;

namespace Platformer
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
            TextBox = new TextBox("", "LexendDeca", Engine.Cam.RenderTargetToWorldPosition(textPos), int.MaxValue, int.MaxValue, Color.White, 0.7f);
            AddChild(TextBox);
        }

        public override void Update()
        {
            base.Update();
            TextBox.Pos = Engine.Cam.RenderTargetToWorldPosition(initPos);
        }

        public override void OnTriggerEnter(Player player)
        {
            TextBox.StopAllCoroutines();
            TextBox.ProgressiveDraw(Text[TextBox.Text.Length..], 0.01f, true);
        }

        public override void OnTriggerExit(Player player)
        {
            TextBox.StopAllCoroutines();
            TextBox.ProgressiveRemove(0.01f);
        }
    }
}
