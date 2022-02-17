using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic_platformer.Solids
{
    public class RailedPullBlock : MovingSolid, ISwinged
    {
        private const float amountMoved = 4f;
        public Vector2[] RailPositions;
        private Entity grappledEntity;

        public RailedPullBlock(Vector2[] positions, int initialIndexPosition, int width, int height) : base(positions[initialIndexPosition], width, height, new Sprite(Color.Beige))
        {
            RailPositions = positions;
            GrapplingPoint.GrapplingSolids.Add(this);
        }

        public override void Update()
        {
            base.Update();
            if(grappledEntity == null)
                return;

            Vector2 dir = Vector2.Normalize(grappledEntity.Pos - MiddlePos) * amountMoved;
            
        }

        void ISwinged.OnGrapple(Entity grappledEntity)
        {
            this.grappledEntity = grappledEntity;
        }

        void ISwinged.OnStopGrapple(Entity unGrappledEntity)
        {
            if(grappledEntity == unGrappledEntity)
                grappledEntity = null;
        }
    }
}
