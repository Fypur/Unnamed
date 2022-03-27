using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiourp;
using Microsoft.Xna.Framework;

namespace Basic_platformer.Entities.Solids
{
    public class SwingTriggeredBlock : MovingSolid, ISwinged
    {
        public Vector2[] Positions;
        
        private int currentPosIndex;
        private bool movingForwards;
        private bool isMoving;

        public SwingTriggeredBlock(Vector2 position, Vector2[] positions, int width, int height, Sprite sprite) : base(DetermineInitPos(position, positions, out int initIndex), width, height, sprite)
        {
            Positions = positions;
            currentPosIndex = initIndex;
            SwingingPoint.SwingingPoints.Add(this);
        }

        void ISwinged.OnGrapple(Entity grappledEntity, Func<bool> isAtSwingEnd)
        {
            if (grappledEntity is Player player)
                Trigger();
        }

        public void Trigger()
        {
            if (isMoving)
                return;

            isMoving = true;

            if(currentPosIndex == Positions.Length - 1)
                movingForwards = false;
            else if(currentPosIndex == 0)
                movingForwards = true;


        }

        private static Vector2 DetermineInitPos(Vector2 position, Vector2[] positions, out int index)
        {
            Vector2 initPos = Vector2.Zero;
            float distance = float.MaxValue;
            index = 0;
            
            for (int i = 0; i < positions.Length; i++)
            {
                float d = Vector2.DistanceSquared(position, positions[i]);
                if (d < distance)
                {
                    initPos = positions[i];
                    distance = d;
                    index = i;
                }
            }

            return initPos;
        }
    }
}
