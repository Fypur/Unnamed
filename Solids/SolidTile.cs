using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class SolidTile : Solid
    {
        protected Texture2D texture;

        public SolidTile(Texture2D texture ,Vector2 position, int width, int height) : base(position, width, height)
        {
            this.texture = texture;
        }

        public override void Render()
        {
            Drawing.Draw(texture, new Rectangle((int)Pos.X, (int)Pos.Y, Width, Height), Color.White);

            if(Debug.DebugMode)
                Drawing.DrawEdge(new Rectangle((int)Pos.X, (int)Pos.Y, Width, Height), 1, Color.Red);
        }
    }
}