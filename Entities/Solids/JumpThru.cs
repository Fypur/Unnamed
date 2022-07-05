using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class JumpThru : Platform
    {
        public JumpThru(Vector2 position, int width, int height, Sprite sprite) : base(position, width, height, sprite)
        { }

        public override bool CollidingConditions(Collider other)
        {
            if (other.ParentEntity is JumpThru && other.AbsoluteTop != Collider.AbsoluteBottom - 1)
                return false;

            /*Debug.PointUpdate(new Vector2(Collider.AbsolutePosition.X, Collider.AbsoluteTop), new Vector2(other.AbsolutePosition.X, other.AbsoluteBottom));

            Debug.LogUpdate("jumpthru: " + new Vector2(Collider.AbsolutePosition.X, Collider.AbsoluteTop), "other: " + new Vector2(other.AbsolutePosition.X, other.AbsoluteBottom));*/

            if (Collider.AbsoluteTop != other.AbsoluteBottom - 1)
                return false;

            if(other.ParentEntity is Actor actor && actor.Velocity.Y < 0)
                return false;
            
            if (other.ParentEntity is MovingSolid solid && solid.Velocity.Y < 0)
                return false;

            return true;
        }
    }
}
