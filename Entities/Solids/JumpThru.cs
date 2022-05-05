using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class JumpThru : Solid
    {
        public JumpThru(Vector2 position, int width, int height, Sprite sprite) : base(position, width, height, sprite)
        {
        }

        public override bool CollidingConditions(Collider other)
            => !(other.ParentEntity is JumpThru && other.AbsoluteTop != Collider.AbsoluteBottom - 1) && 
            !(Collider.AbsoluteTop != other.AbsoluteBottom - 1) && 
            !(other.ParentEntity is Actor actor && actor.Velocity.Y < 0) &&
            !(other.ParentEntity is MovingSolid solid && solid.Velocity.Y < 0);
    }
}
