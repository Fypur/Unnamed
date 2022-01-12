using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class Spike : DeathTrigger
    {
        public Spike(Vector2 position, int width, int height)
            : base(position, width, height)
        { }

        public override void Render()
        {
            Drawing.Draw(new Rectangle(Pos.ToPoint(), Size.ToPoint()), Color.IndianRed);
        }
    }
}
