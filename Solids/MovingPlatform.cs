using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace Basic_platformer.Solids
{
    public class MovingPlatform : MovingSolid
    {
        Vector2[] positions;
        float[] times;

        public MovingPlatform(Vector2 position, int width, int height, Vector2[] Otherpositions, float[] timesBetweenPositions)
            : base(position, width, height, 0)
        {
            Contract.Requires(timesBetweenPositions.Length - 1 == Otherpositions.Length);
            positions = Otherpositions;
        }
        
        public override void Update()
        {
            base.Update();
        }

        public override void Render()
        {
            base.Render();
        }
    }
}
