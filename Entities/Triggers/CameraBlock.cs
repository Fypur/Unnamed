using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class CameraBlock : MovingSolid
    {
        public CameraBlock(Vector2 position, int width, int height) : base(position, width, height, null)
        {
        }

        public CameraBlock(Vector2 position, Vector2 size) : base(position, (int)size.X, (int)size.Y, null)
        {
        }

        public override bool CollidingConditions(Collider other)
        {
            if(other.ParentEntity != Engine.Cam)
                return false;

            return base.CollidingConditions(other);
        }

        public override void Awake()
        {
            base.Awake();

            Engine.CurrentMap.Data.CameraSolids.Add(this);
            Move(-Vector2.UnitY);
            Move(Vector2.UnitY);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            Engine.CurrentMap.Data.CameraSolids.Remove(this);
        }
    }
}
