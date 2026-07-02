using Fiourp;
using Microsoft.Xna.Framework;

namespace Unnamed
{
    public class GameCamera : Entity
    {
        public Rectangle Bounds;
        public Camera Camera;

        public GameCamera(Camera camera) : base(camera.Pos)
        {
            Camera = camera;
        }

        public void LightShake()
            => Shake(0.2f, 1);

        public void Shake(float time, float intensity)
        {
            Shaker shaker = GetComponent<Shaker>();
            if (shaker == null || time > shaker.Time || intensity > shaker.Intensity)
            {
                RemoveComponent(shaker);
                AddComponent(new Shaker(time, intensity, () => CenteredPos));
            }
        }

        public void SetBoundaries(Rectangle bounds)
        {
            this.Bounds = bounds;
            CenteredPos = CenteredPos;
        }

        public void SetBoundaries(Vector2 position, Vector2 size)
        {
            Bounds = new Rectangle(position.ToPoint(), size.ToPoint());
            CenteredPos = CenteredPos;
        }

        public Vector2 InBoundsPos(Vector2 position)
        {
            if ((Bounds.Contains(position - HalfSize) && Bounds.Contains(position + HalfSize)) || Bounds == Rectangle.Empty)
                return position;

            Vector2 inBounds = new Vector2(InBoundsPosX(position.X), InBoundsPosY(position.Y));

            return inBounds;
        }

        public Vector2 RenderTargetToWorldPosition(Vector2 position)
            => position + Engine.Cam.WorldToScreenPosition(position) * (Engine.Cam.ScreenSizeCoef - 1);
    }
}
