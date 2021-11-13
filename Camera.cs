using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class Camera
    {
        private bool hasChanged;

        private Vector2 pos;
        private Vector2 origin;
        public Vector2 Position
        {
            get => pos;

            set {
                hasChanged = true;
                origin = value;
                pos = value - Platformer.ScreenSize / 2;
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
                    return view = Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
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

        public Vector2 WorldToScreenPosition(Vector2 position)
            => Vector2.Transform(position, ViewMatrix);

        public Vector2 ScreenToWorldPosition(Vector2 position)
            => Vector2.Transform(position, InverseViewMatrix);
    }
}