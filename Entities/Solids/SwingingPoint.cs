using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platformer
{
    public class SwingingPoint : CyclingSolid, ISwinged
    {
        public static List<Solid> SwingingPoints = new List<Solid>();
        private const int width = 8;
        private const int height = 8;

        public SwingingPoint(Vector2 position) : base(position, width, height, new Sprite(DataManager.Objects["swingingPoint"])) 
        {
            SwingingPoints.Add(this);
            Collider.Collidable = false;
        }

        public SwingingPoint(Vector2 position, Vector2[] positions, float[] timesBetweenPositions, bool goingForwards, Func<float, float> easeFunction = null)
            : base(position, width, height, new Sprite(DataManager.Objects["swingingPoint"]), goingForwards, positions, timesBetweenPositions, easeFunction) { SwingingPoints.Add(this); }

        public override void OnDestroy()
        {
            base.OnDestroy();
            SwingingPoints.Remove(this); 
        }
    }
}
