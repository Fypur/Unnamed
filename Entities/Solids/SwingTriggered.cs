using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiourp;
using Microsoft.Xna.Framework;

namespace Platformer
{
    public class SwingTriggered : MovingSolid, ISwinged
    {
        public  float MaxSpeed = 10000;
        public  float Acceleration = 100;
        public  float friction = 0.1f;

        public Vector2[] Positions;
        
        private int currentPosIndex;
        private bool movingForwards = true;
        private Vector2 normalizedDir;
        private Vector2 nextPos;
        private bool isMoving;
        private bool looped;

        public SwingTriggered(Vector2 position, Vector2[] positions, int width, int height) : base(DetermineInitPos(position, positions, out int initIndex), width, height, new Sprite(Color.LightBlue))
        {
            Positions = positions;
            Collider.Collidable = false;
            Velocity = Vector2.Zero;
            if (Positions[0] == Positions[Positions.Length - 1])
                looped = true;

            currentPosIndex = initIndex;
            SwingingPoint.SwingingPoints.Add(this);
        }

        void ISwinged.OnGrapple(Entity grappledEntity, Func<bool> isAtSwingEnd)
        {
            if (grappledEntity is Player player)
            {
                isMoving = true;
                Trigger();
            }
        }

        void ISwinged.OnStopGrapple(Entity unGrappledEntity)
            => isMoving = false;

        public void Trigger()
        {
            if(looped && currentPosIndex == Positions.Length - 1)
                currentPosIndex = 0;

            if(currentPosIndex == Positions.Length - 1)
                movingForwards = false;
            else if(currentPosIndex == 0)
                movingForwards = true;

            if(movingForwards)
                nextPos = Positions[currentPosIndex + 1];
            else
                nextPos = Positions[currentPosIndex - 1];

            normalizedDir = Vector2.Normalize(nextPos - ExactPos);
        }

        public override void Update()
        {
            base.Update();
            Debug.LogUpdate(Velocity);
            foreach (Solid s in SwingingPoint.SwingingPoints) Debug.PointUpdate(s.Pos);

            if (isMoving)
                Velocity += normalizedDir * Acceleration;

            Velocity -= Velocity * friction;
            Velocity = Vector2.Clamp(Velocity, -new Vector2(MaxSpeed), new Vector2(MaxSpeed));
            if(Engine.Deltatime != 0)
                Velocity = (VectorHelper.ClosestOnSegment(ExactPos + Velocity * Engine.Deltatime, Positions[currentPosIndex], nextPos) - ExactPos) / Engine.Deltatime;

            if (Vector2.DistanceSquared(ExactPos + Velocity * Engine.Deltatime, nextPos) < 2)
            {
                Velocity = Vector2.Zero;

                if (movingForwards)
                    currentPosIndex++;
                else
                    currentPosIndex--;

                Trigger();
            }

            Move(Velocity * Engine.Deltatime);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            SwingingPoint.SwingingPoints.Remove(this);
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
