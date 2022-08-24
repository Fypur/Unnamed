using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class Light : Entity
    {
        public int Radius;

        public Light(Vector2 position, int radius) : base(position, radius * 2, radius * 2, null)
        {
            Radius = radius;
        }

        public override void Render()
        {
            base.Render();

            Drawing.DrawCircle(Pos, Radius, 0.3f, new Color(Color.White, 120), Color.Transparent);
        }
    }
}
