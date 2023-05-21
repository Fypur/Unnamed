using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class HangingWire : Decoration
    {
        public HangingWire(Vector2[] controlPoints) : base(GetStats(controlPoints out int width, out int height), width, height, null)
        {
            AddComponent(new BezierCurve(Color.Gray, 1, controlPoints));
        }

        private static Vector2 GetStats(Vector2[] controlPoints, out int width, out int height)
        {
            Vector2 topLeft = controlPoints[0];
            Vector2 bottomRight = controlPoints[0];
            foreach(Vector2 controlPoint in controlPoints)
            {
                if(controlPoint.X < topLeft.X)
                    topLeft.X = controlPoint.X;
                if (controlPoint.Y < topLeft.Y)
                    topLeft.Y = controlPoint.Y;

                if (controlPoint.X > bottomRight.X)
                    bottomRight.X = controlPoint.X;
                if (controlPoint.Y > bottomRight.Y)
                    bottomRight.Y = controlPoint.Y;
            }

            width = (int)Math.Ceiling(bottomRight.X - topLeft.X);
            height = (int)Math.Ceiling(bottomRight.Y - topLeft.Y);

            return topLeft;
        }
    }
}
