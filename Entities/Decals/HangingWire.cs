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
        private float moveAmount = 3;
        private float cycleTime = 5;

        public Vector2[] ControlPoints;


        private BezierCurveRenderer bezier;
        private Timer timer;

        public HangingWire(Vector2[] controlPoints) : base(GetStats(controlPoints, out int width, out int height), width, height, null)
        {
            ControlPoints = controlPoints;
            bezier = (BezierCurveRenderer)AddComponent(new BezierCurveRenderer(new Color(27, 31, 28), 1, controlPoints));

            RestartTimer(true);
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

        private void RestartTimer(bool going)
        {
            timer = (Timer)AddComponent(new Timer(cycleTime / 2, true, (timer) =>
            {
                for (int i = 1; i < bezier.ControlPoints.Count - 1; i++)
                {
                    if(going)
                        bezier.ControlPoints[i] = ControlPoints[i] + Vector2.UnitY * moveAmount * Ease.Reverse(timer.Value / timer.MaxValue);
                    else
                        bezier.ControlPoints[i] = ControlPoints[i] + Vector2.UnitY * moveAmount * timer.Value / timer.MaxValue;
                }
            }, () => RestartTimer(!going)));
        }

        public override void Update()
        {
            base.Update();

            //bezier.ControlPoints = new List<Vector2>(ControlPoints);
            /*for(int i = 1; i < bezier.ControlPoints.Count - 1; i++)
            {
                //bezier.ControlPoints[i] += Vector2.One * moveMultiplier * Rand.NextFloat(0, 0);
                bezier.ControlPoints[i] = Input.MousePos;
            }*/
        }
    }
}
