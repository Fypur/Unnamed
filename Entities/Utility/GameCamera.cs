using Fiourp;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Unnamed
{
    public class GameCamera : Actor
    {
        public int ViewportWidth;
        public int ViewportHeight;

        public int Width;
        public int Height;
        public Vector2 UnboundedOffset;
        public Vector2 BoundedOffset;
        public bool Locked;

        public Rectangle Bounds;
        private Camera camera;

        private bool moving;

        public List<Solid> CameraSolids = new List<Solid>();

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
            camera = new Camera(pos, 0f, Vector2.One);
        }

        public override void LateUpdate()
        {
            base.LateUpdate();

            aabbCollider.Width = Width;
            aabbCollider.Height = Height;
            aabbCollider.LocalPos = -Size / 2;

            camera.Pos = InBoundsPos(Pos + BoundedOffset, Bounds) + UnboundedOffset;
            if (Engine.Player != null && FollowsPlayer && !Locked && (moveTimer == null || moveTimer.Value <= 0))
            {
                Follow(Engine.Player, 4, 4, StrictFollowBounds);
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


        private Vector2 FollowedPos(float xSmooth, float ySmooth, Rectangle strictFollowBounds, Rectangle bounds)
        {
            Player player = Platformer.Player;

            strictFollowBounds.Location += Pos.ToPoint();
            Vector2 inBoundsActorPos = InBoundsPos(InBoundsPos(player.MiddlePos, bounds) + BoundedOffset, bounds);

            return new Vector2(
                MathHelper.Lerp(Pos.X, inBoundsActorPos.X, Engine.Deltatime * xSmooth),
                MathHelper.Lerp(Pos.Y, inBoundsActorPos.Y,
                    Engine.Deltatime * ySmooth * (strictFollowBounds.Contains(player.MiddlePos) ? 1 : 2.5f)));
        }

        public void SetBoundaries(Vector2 position, Vector2 size)
            => Bounds = new Rectangle(position.ToPoint(), size.ToPoint());

        public Vector2 InBoundsPos(Vector2 position)
            => InBoundsPos(position, Bounds);

        private Vector2 InBoundsPos(Vector2 position, Rectangle bounds)
        {
            if (BoundsContainsWholeCameraAtPosition(position) || Bounds == Rectangle.Empty)
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
    }
}
