using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiourp;
using Microsoft.Xna.Framework;

namespace Unnamed
{
    public class SwingTriggered : SwingingPoint, ISwinged
    {
        private readonly float maxSpeed = 100;
        private readonly float acceleration = 10;
        private readonly float friction = 0.1f;

        public enum Types { Slow, Normal, Fast, VeryFast }

        public Vector2[] Positions;
        public bool Attached = true;
        
        private int currentPosIndex;
        private bool movingForwards = true;
        private Vector2 normalizedDir;
        private Vector2 nextPos;
        private bool isMoving;
        private bool looped;

        public SwingTriggered(Vector2[] positions, float maxSwingDistance, Vector2 initPos, int width, int height, Types type) : base(DetermineInitPos(initPos, positions, out int initIndex), maxSwingDistance)
        {
            Sprite.Texture = DataManager.Objects["swingTriggered"];

            Positions = positions;
            Collider.Collidable = false;
            Velocity = Vector2.Zero;
            if (Positions[0] == Positions[Positions.Length - 1])
                looped = true;

            currentPosIndex = initIndex;
            SwingingPoint.SwingingPoints.Add(this);
            MaxSwingDistance = maxSwingDistance;

            switch (type)
            {
                case Types.Slow:
                    maxSpeed = 30;
                    acceleration = 5;
                    break;
                case Types.Normal:
                    maxSpeed = 100;
                    acceleration = 20;
                    break;
                case Types.Fast:
                    maxSpeed = 150;
                    acceleration = 20;
                    break;
                case Types.VeryFast:
                    maxSpeed = 200;
                    acceleration = 50;
                    break;
            }
        }

        void ISwinged.OnSwing(Entity grappledEntity, Func<bool> isAtSwingEnd)
        {
            if (grappledEntity is Player player)
            {
                isMoving = true;
                Trigger();
            }
        }

        void ISwinged.OnStopSwing(Entity unGrappledEntity)
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

            if (Attached)
            {
                if (isMoving)
                    Velocity += normalizedDir * acceleration;

                Velocity -= Velocity * friction;
                Velocity = Vector2.Clamp(Velocity, -new Vector2(maxSpeed), new Vector2(maxSpeed));
                if (Engine.Deltatime != 0)
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
            }

            Velocity += gravityVector * GravityScale;

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

        public override void Render()
        {
            base.Render();

            for(int i = 0; i < Positions.Length - 1; i++)
            {
                Drawing.DrawDottedLine(Positions[i] + HalfSize, Positions[i + 1] + HalfSize, new Color(Color.GreenYellow, 100), 1, 4, 4);
            }
        }
    }
}
