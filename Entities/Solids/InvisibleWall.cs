using Fiourp;
using Microsoft.Xna.Framework;

namespace Unnamed
{
    public class InvisibleWall : Solid
    {
        public InvisibleWall(Vector2 position, int width, int height) : base(position, new AABBCollider(Vector2.Zero, width, height), null)
        {
        }

        public override bool CollidingConditions(Collider other)
        {
            if (other.ParentEntity is not Player)
                return false;

            return base.CollidingConditions(other);
        }
    }
}
