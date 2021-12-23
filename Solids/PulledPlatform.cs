using Basic_platformer.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using Basic_platformer.Components;

namespace Basic_platformer.Solids
{
    public class PulledPlatform : MovingSolid
    {
        public Vector2 originalPos;
        public Vector2 PulledPos;
        public float PulledOutTime;
        public float speed = 200f;
        public Func<float, float> EasingFunction;

        public float movingTime { get => 2 + PulledOutTime; }

        private float distanceBetweenPos;

        public PulledPlatform(Vector2 position, int width, int height, Vector2 pulledPos, float pulledOutTime, 
            Func<float, float> easingFunction = null) : base(position, width, height)
        {
            Pos = position;
            Width = width;
            Height = height;
            originalPos = position;
            PulledPos = pulledPos;
            distanceBetweenPos = Vector2.Distance(originalPos, PulledPos);
            PulledOutTime = pulledOutTime;
            EasingFunction = easingFunction;
        }

        public void Pull()
        {
            Vector2 initPos = Pos;
            Vector2 newPos = PulledPos;
            AddComponent(new Timer(1f, true, (timer) =>
            {
                MoveTo(Vector2.Lerp(initPos, newPos,
                     (EasingFunction ?? Ease.QuintInAndOut).Invoke(Ease.Reverse(timer.Value / timer.MaxValue))));
                Debug.LogUpdate(timer.Value);
            },
            () =>
            {
                MoveTo(newPos);
                AddComponent(new Timer(PulledOutTime, false, null, () => Unpull(Ease.QuintInAndOut)));
            }));
        }

        private void Unpull(Func<float, float> easingFunction = null)
        {
            Vector2 initPos = Pos;
            Vector2 newPos = originalPos;
            AddComponent(new Timer(1f, true, (timer) =>
            {
                MoveTo(Vector2.Lerp(initPos, newPos,
                         (easingFunction ?? DefaultEasing).Invoke(Ease.Reverse(timer.Value / timer.MaxValue))));
            }, () => { MoveTo(originalPos); }));
        }

        public override void Render()
        {
            Drawing.Draw(new Rectangle((int)Pos.X, (int)Pos.Y, Width, Height), Color.Yellow);
        }

        private float DefaultEasing(float x)
            => x;
    }
}