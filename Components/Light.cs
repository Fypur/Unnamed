using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class Light : Renderer
    {
        public Vector2 LocalPosition;
        public float Radius;
        public Color InsideColor;
        public Color OutsideColor;

        public static readonly Color TransparentWhite = new Color(Color.White, 0);

        public Light(Vector2 localPosition, float radius, Color insideColor, Color outsideColor)
        {
            LocalPosition = localPosition;
            Radius = radius;
            InsideColor = insideColor;
            OutsideColor = outsideColor;
        }

        public override void Render()
        {
            Drawing.DrawCircle(ParentEntity.Pos + LocalPosition, Radius, 0.1f, InsideColor, OutsideColor);
        }
    }
}
