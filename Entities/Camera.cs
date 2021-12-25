using Basic_platformer.Utility;
using Microsoft.Xna.Framework;
using System;
using Basic_platformer.Components;
using Basic_platformer.Entities;

namespace Basic_platformer
{
    public class Camera : Entity
    {
        private Rectangle bounds = Rectangle.Empty;
        private bool hasChanged;
        public bool FollowsPlayer;

        private Vector2 pos;
        public Vector2 Pos
        {
            get => pos;

            set
            {
                hasChanged = true;
                
                if((bounds.Contains(value - Platformer.ScreenSize / 2 * ZoomLevel) && bounds.Contains(value + Platformer.ScreenSize / 2 * ZoomLevel)) || bounds == Rectangle.Empty)
                {
                    hasChanged = true;
                    pos = value;
                } 
                else
                {
                    Vector2 correctedPos = value - Platformer.ScreenSize / 2 * ZoomLevel;
                    
                    if(correctedPos.X < bounds.X)
                        correctedPos.X = bounds.X;
                    else if(correctedPos.X + Platformer.ScreenSize.X * ZoomLevel > bounds.X + bounds.Width)
                        correctedPos.X = bounds.X + bounds.Width - Platformer.ScreenSize.X;

                    if (correctedPos.Y < bounds.Y)
                        correctedPos.Y = bounds.Y;
                    else if (correctedPos.Y + Platformer.ScreenSize.Y * ZoomLevel > bounds.Y + bounds.Height)
                        correctedPos.Y = bounds.Y + bounds.Height - Platformer.ScreenSize.Y;
                    
                    correctedPos += Platformer.ScreenSize / 2 * ZoomLevel;

                    if (pos != correctedPos)
                    {
                        hasChanged = true;
                        pos = correctedPos;
                    }
                }
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

        public override void Update()
        {
            base.Update();

            if (FollowsPlayer)
                Follow(Platformer.player, 3, 3, new Rectangle(new Vector2(-Platformer.ScreenSize.X / 6, -Platformer.ScreenSize.Y / 12).ToPoint(),
                    new Vector2(Platformer.ScreenSize.X / 3, Platformer.ScreenSize.Y / 6).ToPoint()));
        }

        public void Follow(Actor actor, float xSmooth, float ySmooth, Rectangle strictFollowBounds)
        {
            strictFollowBounds.Location += Pos.ToPoint();

            if (strictFollowBounds.Contains(actor.Pos))
            {
                Pos = new Vector2(MathHelper.Lerp(Pos.X, actor.Pos.X, Platformer.Deltatime * xSmooth),
                    MathHelper.Lerp(Pos.Y, actor.Pos.Y, Platformer.Deltatime * ySmooth));
            }
            else
            {
                Pos = new Vector2(MathHelper.Lerp(Pos.X, actor.Pos.X, Platformer.Deltatime * xSmooth),
                    MathHelper.Lerp(Pos.Y, actor.Pos.Y, Platformer.Deltatime * ySmooth * 2.5f));
            }
        }

        public void Move(Vector2 offset, float time, Func<float, float> easingFunction = null)
        {
            Vector2 initPos = Pos;
            Vector2 newPos = Pos + offset;
            AddComponent(new Timer(time, true, (timer) =>

            Pos = Vector2.Lerp(initPos, newPos,
                     (easingFunction ?? Ease.Default).Invoke(Ease.Reverse(timer.Value / timer.MaxValue))),

             () => Pos = newPos));
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