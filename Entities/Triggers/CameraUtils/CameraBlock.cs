using Fiourp;
using Microsoft.Xna.Framework;

namespace Unnamed
{
    public class CameraBlock : Solid
    {
        public CameraBlock(Vector2 position, int width, int height) : base(position, new AABBCollider(Vector2.Zero, width, height), null)
        {
        }

        public CameraBlock(Vector2 position, Vector2 size) : base(position, new AABBCollider(Vector2.Zero, (int)size.X, (int)size.Y), null)
        {
        }

        public override bool CollidingConditions(Collider other)
        {
            if (other.ParentEntity != Platformer.GameCam)
                return false;

            return base.CollidingConditions(other);
        }

        public override void Awake()
        {
            base.Awake();

            Platformer.GameCam.CameraSolids.Add(this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            Platformer.GameCam.CameraSolids.Remove(this);
        }
    }
}
