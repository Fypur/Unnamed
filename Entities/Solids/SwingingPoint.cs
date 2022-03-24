using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class SwingingPoint : CyclingSolid, ISwinged
    {
        public static List<Solid> SwingingPoints = new List<Solid>();
        private const int width = 7;
        private const int height = 7;

        public SwingingPoint(Vector2 position) : base(position, width, height, new Sprite(DataManager.Textures["swingingPoint"].CropTo(new Vector2(0, 16), new Vector2(8, 8)), new Rectangle(position.ToPoint(), new Point(width, height)))) 
        {
            SwingingPoints.Add(this);
            Collider.Collidable = false;
        }

        public SwingingPoint(Vector2[] positions, float[] timesBetweenPositions, Func<float, float> easeFunction = null)
            : base(width, height, new Sprite(Color.Blue), positions, timesBetweenPositions, easeFunction) { SwingingPoints.Add(this); }

        public override void OnDestroy()
            => SwingingPoints.Remove(this);
    }
}
