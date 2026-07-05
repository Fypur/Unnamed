using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Unnamed
{
    public class GameCamera : Actor
    {
        private Camera camera;

        public int ViewportWidth;
        public int ViewportHeight;

        public int Width;
        public int Height;
        public Vector2 UnboundedOffset;
        public Vector2 BoundedOffset;
        public bool Locked;

        public Vector2 PreviousPos;

        public Rectangle Bounds;
        public Rectangle StrictFollowBounds => new Rectangle(new Point(-Width / 2, -Height / 4), new Point(Width, Height / 2));

        public List<Solid> CameraSolids = new List<Solid>();

        private bool moving;

        public Matrix ViewMatrix => camera.ViewMatrix;
        public Matrix InverseViewMatrix => camera.InverseViewMatrix;

        private AABBCollider aabbCollider => (AABBCollider)this.Collider;

        public Vector2 Size
        {
            get => new Vector2(Width, Height);
            set { Width = (int)value.X; Height = (int)value.Y; }
        }
        public Vector2 HalfSize => new Vector2(Width / 2, Height / 2);


        public GameCamera(Vector2 pos, int viewportWidth, int viewportHeight) : base(pos, new AABBCollider(Vector2.Zero, viewportWidth, viewportHeight), null)
        {
            ViewportWidth = viewportWidth;
            ViewportHeight = viewportHeight;
            Width = viewportWidth;
            Height = viewportHeight;

            camera = new Camera(pos, 0f, Vector2.One, Width, Height);
            Engine.Cam = camera;
        }

        public override void LateUpdate()
        {
            base.LateUpdate();

            PreviousPos = Pos;

            aabbCollider.Width = Width;
            aabbCollider.Height = Height;
            aabbCollider.LocalPos = -Size / 2;

            camera.ViewportWidth = Width;
            camera.ViewportHeight = Height;

            if (Platformer.Player != null && !Locked && !moving)
                Follow(4, 4, StrictFollowBounds);
            else
            {
                Vector2 aim = InBoundsPos(Pos + BoundedOffset, Bounds) + UnboundedOffset;
                MoveX(aim.X - Pos.X);
                MoveY(aim.Y - Pos.Y);
            }
        }

        public void LightShake()
            => Shake(0.2f, 1);

        public void Shake(float time, float intensity)
        {
            Shaker shaker = GetComponent<Shaker>();
            if (shaker == null || time > shaker.Time || intensity > shaker.Intensity)
            {
                RemoveComponent(shaker);
                AddComponent(new Shaker(time, intensity, () => Pos));
            }
        }

        public void Follow(float xSmooth, float ySmooth, Rectangle strictFollowBounds)
        {
            Vector2 amount = FollowedPos(xSmooth, ySmooth, strictFollowBounds, Bounds) - Pos;
            Vector2 previous = ExactPos;

            //Debug.LogUpdate(FollowedPos(actor, xSmooth, ySmooth, strictFollowBounds, Bounds));
            //if(Math.Abs(amount.X) >= 0.1f)
            MoveX(amount.X, new List<Kinematic>(CameraSolids), null);
            //if (Math.Abs(amount.Y) >= 0.1f)
            MoveY(amount.Y, new List<Kinematic>(CameraSolids), null);

            /*if (HasComponent<Shaker>())
            {
                //ExactPos = previous;
                shakerInitPos += ExactPos - previous;
            }*/

            if (!Bounds.Contains(Pos + Vector2.One) || !Bounds.Contains(Pos + Size - Vector2.One))
                Pos = FollowedPos(xSmooth, ySmooth, strictFollowBounds, Bounds);
        }


        public Vector2 FollowedPos(float xSmooth, float ySmooth, Rectangle strictFollowBounds, Rectangle bounds)
        {
            Player player = Platformer.Player;

            strictFollowBounds.Location += Pos.ToPoint();
            Vector2 inBoundsActorPos = InBoundsPos(InBoundsPos(player.MiddlePos, bounds) + BoundedOffset, bounds);

            return new Vector2(
                MathHelper.Lerp(Pos.X, inBoundsActorPos.X, Engine.Deltatime * xSmooth),
                MathHelper.Lerp(Pos.Y, inBoundsActorPos.Y,
                    Engine.Deltatime * ySmooth * (strictFollowBounds.Contains(player.MiddlePos) ? 1 : 2.5f)));
        }

        public void RemoveBoundaries()
            => Bounds = Rectangle.Empty;

        public void SetBoundaries(Vector2 position, Vector2 size)
            => Bounds = new Rectangle(position.ToPoint(), size.ToPoint());

        public Vector2 InBoundsPos(Vector2 position)
            => InBoundsPos(position, Bounds);

        public Vector2 InBoundsPos(Vector2 position, Rectangle bounds)
        {
            if (Bounds == Rectangle.Empty || BoundsContainsWholeCameraAtPosition(position))
                return position;

            float InBoundsPosAlongAxis(float x, float width, float boundsX, float boundsWidth)
            {
                if (x - width / 2 > boundsX && x + width / 2 < boundsX + boundsWidth)
                    return x;

                float correctedX = x - width / 2;

                if (correctedX < boundsX)
                    correctedX = boundsX;
                else if (correctedX + width > boundsX + boundsWidth)
                    correctedX = boundsX + boundsWidth - width;

                correctedX += width / 2;

                return correctedX;
            }

            return new Vector2(InBoundsPosAlongAxis(position.X, Width, Bounds.X, Bounds.Width), InBoundsPosAlongAxis(position.Y, Height, Bounds.Y, Bounds.Height)); ;
        }

        private bool BoundsContainsWholeCameraAtPosition(Vector2 position)
            => Bounds.Contains(position - HalfSize) && Bounds.Contains(position + HalfSize);

        public void Move(Vector2 offset, float time, Func<float, float> easingFunction = null, Func<bool> stop = null)
        {
            Vector2 initPos = Pos;
            Vector2 newPos = Pos + offset;

            moving = true;
            AddComponent(new Timer(time, (t) =>
            {
                if (stop != null && stop.Invoke())
                {
                    RemoveComponent(t);
                    return;
                }

                Pos = Vector2.Lerp(initPos, newPos, (easingFunction ?? Ease.None).Invoke(Ease.Reverse(t.Value / t.MaxValue)));

            },

                () =>
                {
                    Pos = newPos;
                    moving = false;
                }));
        }
    }
}
