using Basic_platformer.Utility;
using Microsoft.Xna.Framework;
using System;
using Basic_platformer.Components;
using Basic_platformer.Entities;

namespace Basic_platformer
{
    public class Camera
    {
        private Rectangle bounds = Rectangle.Empty;
        private bool hasChanged;
        public bool FollowsPlayer;
        private Timer timer;

        private Vector2 pos;
        public Vector2 Pos
        {
            get => pos;

            set
            {
                pos = InBoundsPos(value, out bool changed);
                if(changed) hasChanged = true;
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

        public Camera(Vector2 position, float rotation, float zoomLevel, Rectangle? bounds = null)
        {
            Pos = position;
            Rotation = rotation;
            ZoomLevel = zoomLevel;

            if (bounds != null)
                SetBoundaries((Rectangle)bounds);
        }
        
        public void Update()
        {
            if(timer != null)
                timer.Update();

            if (FollowsPlayer)
                Follow(Platformer.player, 5, 5, new Rectangle(new Vector2(-Platformer.ScreenSize.X / 6, -Platformer.ScreenSize.Y / 12).ToPoint(),
                    new Vector2(Platformer.ScreenSize.X / 3, Platformer.ScreenSize.Y / 6).ToPoint()));
        }

        public void Follow(Actor actor, float xSmooth, float ySmooth, Rectangle strictFollowBounds)
        {
            strictFollowBounds.Location += Pos.ToPoint();

            if (strictFollowBounds.Contains(actor.Pos))
            {
                Pos = new Vector2(MathHelper.Lerp(Pos.X, InBoundsPosX(actor.Pos.X), Platformer.Deltatime * xSmooth),
                    MathHelper.Lerp(Pos.Y, InBoundsPosY(actor.Pos.Y), Platformer.Deltatime * ySmooth));
            }
            else
            {
                Pos = new Vector2(MathHelper.Lerp(Pos.X, InBoundsPosX(actor.Pos.X), Platformer.Deltatime * xSmooth),
                    MathHelper.Lerp(Pos.Y, InBoundsPosY(actor.Pos.Y), Platformer.Deltatime * ySmooth * 2.5f));
            }
        }

        public void Move(Vector2 offset, float time, Func<float, float> easingFunction = null)
        {
            Vector2 initPos = Pos;
            Vector2 newPos = Pos + offset;
            timer = new Timer(time, false, (t) =>

                Pos = Vector2.Lerp(initPos, newPos,
                     (easingFunction ?? Ease.Default).Invoke(Ease.Reverse(t.Value / t.MaxValue))),

                () => Pos = newPos);
        }

        public Vector2 InBoundsPos(Vector2 position, out bool changed)
        {
            changed = false;

            if (bounds == null)
                return position;

            if ((bounds.Contains(position - Platformer.ScreenSize / 2 * ZoomLevel) && bounds.Contains(position + Platformer.ScreenSize / 2 * ZoomLevel)) || bounds == Rectangle.Empty)
            {
                if(position != Pos)
                    changed = true;

                return position;
            }
            else
            {
                Vector2 correctedPos = position - Platformer.ScreenSize / 2 * ZoomLevel;

                if (correctedPos.X < bounds.X)
                    correctedPos.X = bounds.X;
                else if (correctedPos.X + Platformer.ScreenSize.X * ZoomLevel > bounds.X + bounds.Width)
                    correctedPos.X = bounds.X + bounds.Width - Platformer.ScreenSize.X;

                if (correctedPos.Y < bounds.Y)
                    correctedPos.Y = bounds.Y;
                else if (correctedPos.Y + Platformer.ScreenSize.Y * ZoomLevel > bounds.Y + bounds.Height)
                    correctedPos.Y = bounds.Y + bounds.Height - Platformer.ScreenSize.Y;

                correctedPos += Platformer.ScreenSize / 2 * ZoomLevel;

                if (Pos != correctedPos)
                    changed = true;

                return correctedPos;
            }
        }

        public Vector2 InBoundsPos(Vector2 position)
        {
            if ((bounds.Contains(position - Platformer.ScreenSize / 2 * ZoomLevel) && bounds.Contains(position + Platformer.ScreenSize / 2 * ZoomLevel)) || bounds == Rectangle.Empty)
                return position;
            else
            {
                Vector2 correctedPos = position - Platformer.ScreenSize / 2 * ZoomLevel;

                if (correctedPos.X < bounds.X)
                    correctedPos.X = bounds.X;
                else if (correctedPos.X + Platformer.ScreenSize.X * ZoomLevel > bounds.X + bounds.Width)
                    correctedPos.X = bounds.X + bounds.Width - Platformer.ScreenSize.X;

                if (correctedPos.Y < bounds.Y)
                    correctedPos.Y = bounds.Y;
                else if (correctedPos.Y + Platformer.ScreenSize.Y * ZoomLevel > bounds.Y + bounds.Height)
                    correctedPos.Y = bounds.Y + bounds.Height - Platformer.ScreenSize.Y;

                correctedPos += Platformer.ScreenSize / 2 * ZoomLevel;

                return correctedPos;
            }
        }

        public float InBoundsPosX(float x)
        {
            if(x - Platformer.ScreenSize.X / 2 * ZoomLevel > bounds.X && x - Platformer.ScreenSize.X / 2 * ZoomLevel < bounds.X + bounds.Width &&
                x + Platformer.ScreenSize.X / 2 * ZoomLevel > bounds.X && x + Platformer.ScreenSize.X / 2 * ZoomLevel < bounds.X + bounds.Width)
                return x;
            else
            {
                float correctedX = x - Platformer.ScreenSize.X / 2 * ZoomLevel;

                if (correctedX < bounds.X)
                    correctedX = bounds.X;
                else if (correctedX + Platformer.ScreenSize.X * ZoomLevel > bounds.X + bounds.Width)
                    correctedX = bounds.X + bounds.Width - Platformer.ScreenSize.X;

                correctedX += Platformer.ScreenSize.X / 2 * ZoomLevel;

                return correctedX;
            }
        }

        public float InBoundsPosY(float y)
        {
            if (y - Platformer.ScreenSize.Y / 2 > bounds.Y && y - Platformer.ScreenSize.Y / 2 < bounds.Y + bounds.Height &&
                y + Platformer.ScreenSize.Y / 2 > bounds.Y && y + Platformer.ScreenSize.Y / 2 < bounds.Y + bounds.Height)
                return y;
            else
            {
                float correctedY = y - Platformer.ScreenSize.Y / 2;

                if (correctedY < bounds.Y)
                    correctedY = bounds.Y;
                else if (correctedY + Platformer.ScreenSize.Y > bounds.Y + bounds.Height)
                    correctedY = bounds.Y + bounds.Height - Platformer.ScreenSize.Y;

                correctedY += Platformer.ScreenSize.Y / 2;

                return correctedY;
            }
        }

        public void MoveTo(Vector2 position, float time, Func<float, float> easingFunction = null)
            => Move(position - Pos, time, easingFunction);

        public void SetBoundaries(Rectangle bounds)
            => this.bounds = bounds;

        public void SetBoundaries(Vector2 position, Vector2 size)
            => bounds = new Rectangle(position.ToPoint(), size.ToPoint());

        public Vector2 WorldToScreenPosition(Vector2 position)
            => Vector2.Transform(position, ViewMatrix);

        public Vector2 ScreenToWorldPosition(Vector2 position)
            => Vector2.Transform(position, InverseViewMatrix);
    }
}