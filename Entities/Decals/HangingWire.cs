using Fiourp;
using Microsoft.Xna.Framework;

namespace Unnamed
{
    public class HangingWire : Entity
    {
        private float moveAmount = 3;
        private float cycleTime = 5;

        public Vector2[] ControlPoints;


        private BezierCurveRenderer bezier;
        private Timer timer;

        public HangingWire(Vector2[] controlPoints) : base(controlPoints[0])
        {
            ControlPoints = controlPoints;
            bezier = (BezierCurveRenderer)AddComponent(new BezierCurveRenderer(new Color(27, 31, 28), 1, controlPoints));

            RestartTimer(true);
        }

        private void RestartTimer(bool going)
        {
            timer = (Timer)AddComponent(new Timer(cycleTime / 2, (timer) =>
            {
                for (int i = 1; i < bezier.ControlPoints.Count - 1; i++)
                {
                    if (going)
                        bezier.ControlPoints[i] = ControlPoints[i] + Vector2.UnitY * moveAmount * Ease.Reverse(timer.Value / timer.MaxValue);
                    else
                        bezier.ControlPoints[i] = ControlPoints[i] + Vector2.UnitY * moveAmount * timer.Value / timer.MaxValue;
                }
            }, () => RestartTimer(!going)));
        }

        /*public override void Update()
        {
            base.Update();

            //bezier.ControlPoints = new List<Vector2>(ControlPoints);
            for(int i = 1; i < bezier.ControlPoints.Count - 1; i++)
            {
                //bezier.ControlPoints[i] += Vector2.One * moveMultiplier * Rand.NextFloat(0, 0);
                bezier.ControlPoints[i] = Input.MousePos;
            }
        }*/
    }
}
