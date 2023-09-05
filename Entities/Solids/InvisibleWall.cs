using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class InvisibleWall : Solid
    {
        public InvisibleWall(Vector2 position, int width, int height) : base(position, width, height, null)
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
