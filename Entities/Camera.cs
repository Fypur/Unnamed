using Basic_platformer.Utility;
using Microsoft.Xna.Framework;
using System;
using Basic_platformer.Components;

namespace Basic_platformer
{
    public class Camera : Entity
    {
        private bool hasChanged;

        private Vector2 pos;
        public Vector2 Position
        {
            get => pos;

            set
            {
                hasChanged = true;
                pos = value;
            }
        }


        private float rot;
        public float Rotation
        {
            get => rot;
            set
            {
                hasChanged = true;
                rot = value % 360 + (value < 0 ? 360 : 0);
            }
        }

        private float zoom;
        public float ZoomLevel
        {
            get => zoom;
            set
            {
                hasChanged = true;
                zoom = value;
            }
        }

        private Matrix view;
        public Matrix ViewMatrix
        {
            get
            {
                if (hasChanged)
                {
                    hasChanged = false;
                    return view = Matrix.CreateTranslation(new Vector3(-pos, 0.0f)) *
                           Matrix.CreateScale(ZoomLevel) *
                           Matrix.CreateRotationZ(MathHelper.ToRadians(Rotation)) *
                           Matrix.CreateTranslation(new Vector3(Platformer.ScreenSize / 2, 0.0f));
                }
                else
                    return view;
            }
        }

        public Matrix InverseViewMatrix
        {
            get => Matrix.Invert(ViewMatrix);
        }

        public Camera(Vector2 position, float rotation, float zoomLevel)
        {
            Position = position;
            Rotation = rotation;
            ZoomLevel = zoomLevel;
        }
        
        public void Move(Vector2 newPosition, float time, Func<float, float> easingFunction = null)
        {
            Vector2 initPos = Position;
            Vector2 newPos = Position + newPosition;
            AddComponent(new Timer(time, true, (timer) =>

            Position = Vector2.Lerp(initPos, newPos,
                     (easingFunction ?? DefaultEasing).Invoke(Ease.Reverse(timer.Value / timer.MaxValue))),

             () => Position = newPos));
        }

        public void MoveTo(Vector2 newPosition, float time, Func<float, float> easingFunction = null)
            => Move(newPosition - Position, time, easingFunction);

        private float DefaultEasing(float x)
            => x;
        public Vector2 WorldToScreenPosition(Vector2 position)
            => Vector2.Transform(position, ViewMatrix);

        public Vector2 ScreenToWorldPosition(Vector2 position)
            => Vector2.Transform(position, InverseViewMatrix);
    }
}