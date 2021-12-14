using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using Basic_platformer.Utility;
using Basic_platformer.Components;

namespace Basic_platformer.Solids
{
    public class CyclingSolid : MovingSolid
    {
        public bool moving;

        public Vector2[] Positions;
        public float[] Times;
        public Func<float, float> EasingFunction;

        private int nextIndex = 1;
        private bool increment = true;
        private Timer movingTimer;

        public CyclingSolid(Vector2 position, int width, int height) : base(position, width, height) { }

        public CyclingSolid(int width, int height, Vector2[] positions, float[] timesBetweenPositions, Func<float, float> easingfunction)
            : base(positions[0], width, height)
        {
            if (timesBetweenPositions.Length != positions.Length - 1) 
                throw new Exception("Times between positions and positions amounts are not synced");
            Contract.EndContractBlock();

            moving = true;

            Pos = positions[0];
            Positions = positions;
            Times = timesBetweenPositions;
            EasingFunction = easingfunction;

            StartTimer();

            //AddComponent(movingTimer);
        }

        private void StartTimer()
        {
            movingTimer = new Timer(Times[nextIndex + (increment ? -1 : 0)], true, (timer) =>
            {
                if (!moving)
                    timer.PauseUntil(() => moving);

                Pos = Vector2.Lerp(Positions[nextIndex + (increment ? -1 : 1)], Positions[nextIndex], EasingFunction.Invoke(Ease.Reverse(timer.Value / timer.MaxValue)));

            }, () =>
            {
                Pos = Positions[nextIndex];

                if (nextIndex == 0 || nextIndex == Positions.Length - 1)
                    increment = !increment;

                if(increment)
                    nextIndex++;
                else
                    nextIndex--;

                StartTimer();
            });

            AddComponent(movingTimer);
        }
    }
}
