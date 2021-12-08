using Basic_platformer.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Solids
{
    public class PulledPlatform : MovingSolid
    {
        /// <summary>
        /// if this is true it means it can only move in a predefined line
        /// </summary>
        public bool Railed;

        public Vector2 originalPos;
        public Vector2 PulledPos;
        public float PulledOutTime;
        public float speed = 200f;
        public Func<float, float> EasingFunction;

        public float movingTime { get => 2 + PulledOutTime; }

        private float distanceBetweenPos;

        public PulledPlatform(Vector2 position, int width, int height, Vector2 pulledPos, float pulledOutTime, Func<float, float> easingFunction = null, bool railed = false) : base(position, width, height, 0)
        {
            Pos = position;
            Width = width;
            Height = height;
            originalPos = position;
            PulledPos = pulledPos;
            distanceBetweenPos = Vector2.Distance(originalPos, PulledPos);
            PulledOutTime = pulledOutTime;
            Railed = railed;
            EasingFunction = easingFunction;
        }

        public void Pulled()
        {
            Vector2 initPos = Pos;
            Vector2 newPos = PulledPos;
            AddComponent(new Timer(1f, true, (timer) =>
            {
                MoveTo(Vector2.Lerp(initPos, newPos,
                     (EasingFunction ?? DefaultEasing).Invoke(Ease.Reverse(timer.Value / timer.MaxValue))));
                Debug.LogUpdate(timer.Value);
            },
            () =>
            {
                Pos = newPos;
                AddComponent(new Timer(PulledOutTime, false, null, () => Unpulled(Ease.QuintInAndOut)));
            }));
        }

        public void Unpulled(Func<float, float> easingFunction = null)
        {
            Vector2 initPos = Pos;
            Vector2 newPos = originalPos;
            AddComponent(new Timer(1f, true, (timer) =>
            {
                MoveTo(Vector2.Lerp(initPos, newPos,
                         (easingFunction ?? DefaultEasing).Invoke(Ease.Reverse(timer.Value / timer.MaxValue))));
            }, () => { Pos = originalPos; }));
        }

        public override void Update()
        {
            base.Update();

            MoveX(Velocity.X * Platformer.Deltatime);
            MoveY(Velocity.Y * Platformer.Deltatime);
        }

        public override void Render()
        {
            Drawing.Draw(new Rectangle((int)Pos.X, (int)Pos.Y, Width, Height), Color.Yellow);
        }

        private float DefaultEasing(float x)
            => x;
    }
}