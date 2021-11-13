using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class Camera
    {
        private Vector2 pos;
        public Vector2 Position
        {
            get => pos;

            set {
                hasChanged = true;
                pos = value - new Vector2(Platformer.graphics.PreferredBackBufferWidth / 2, Platformer.graphics.PreferredBackBufferHeight / 2);
            }
        }


        private float rot;
        public float Rotation
        {
            get => rot;
            set
            {
                hasChanged = true;
                rot = value % 360 + value < 0 ? 360 : 0;
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
                    return Matrix.CreateTranslation(new Vector3(Position.X, Position.Y, 0)) *
                    Matrix.CreateScale(ZoomLevel) *
                    Matrix.CreateRotationZ(MathHelper.ToRadians(Rotation));
                }
                else
                    return view;
            }

            set => view = value;
        }

        private bool hasChanged;

        public Camera(Vector2 position, float rotation, float zoomLevel)
        {
            Position = position;
            Rotation = rotation;
            ZoomLevel = zoomLevel;
        }
    }
}