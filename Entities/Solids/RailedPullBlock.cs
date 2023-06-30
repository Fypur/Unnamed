using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class RailedPullBlock : MovingSolid, ISwinged
    {
        private const float amountMoved = 300;
        public Vector2[] RailPositions;
        private Entity grappledEntity;
        private Func<bool> isAtSwingEnd;

        //Indicates the index of positions[] where the platform is inbetween
        private int currentPosIndex;

        public float MaxSwingDistance { get; set; }

        public RailedPullBlock(Vector2[] positions, float maxSwingDistance, int initialIndexPosition, int width, int height) : base(positions[initialIndexPosition], width, height, new Sprite(Color.Beige))
        {
            RailPositions = positions;
            currentPosIndex = initialIndexPosition;
            SwingingPoint.SwingingPoints.Add(this);
            MaxSwingDistance = maxSwingDistance;
            GravityScale = 1;

            AddComponent(new LineRenderer(RailPositions.ToList(), Drawing.PointTexture, 1, Color.BlueViolet, null, (line) => { line.Positions = RailPositions.ToList(); }));
        }

        public RailedPullBlock(Vector2[] positions, float maxSwingDistance, Vector2 initPos, int width, int height) : base(DetermineInitPos(initPos, positions, width, height, out int initialIndex), width, height, new Sprite(Color.Beige))
        {
            RailPositions = positions;
            currentPosIndex = initialIndex;
            SwingingPoint.SwingingPoints.Add(this);
            MaxSwingDistance = maxSwingDistance;
            GravityScale = 1;

            AddComponent(new LineRenderer(RailPositions.ToList(), Drawing.PointTexture, 1, Color.BlueViolet, null, (line) => { line.Positions = RailPositions.ToList(); }));
        }

        public override void Update()
        {
            base.Update();

            /*if (PreviousPos == ExactPos)
                Velocity = Vector2.Zero;*/

            if (MiddleExactPos == RailPositions[0] || MiddleExactPos == RailPositions[RailPositions.Length - 1])
                Velocity = Vector2.Zero;

            Gravity();

            if (grappledEntity == null || grappledEntity.Collider.CollideAt(this, grappledEntity.ExactPos + new Vector2(0, 1)))
            { }
            else
            {
                if (isAtSwingEnd())
                {
                    {
                        //Vector2 v = Velocity * Approach(Velocity.Length(), amountMoved, (grappledEntity.MiddleExactPos - MiddleExactPos).Length()) * Engine.Deltatime;
                        //Vector2 v = (grappledEntity.MiddleExactPos - MiddleExactPos + Velocity) * Engine.Deltatime;

                        /*float VelocityApproach(float acceleration, float friction)
                        {
                            if (xMoving != 0)
                            {
                                return Approach(Velocity.X, (xMoving > 0.3f ? 1 : xMoving < -0.3f ? -1 : xMoving) * maxSpeed, acceleration * Engine.Deltatime);
                            }
                            return Approach(Velocity.X, 0, friction * Engine.Deltatime);
                        }*/

                        /*float Approach(float value, float approached, float move)
                        {
                            if (value < approached)
                                return Math.Min(value + move, approached);
                            return Math.Max(value - move, approached);
                        }


                        foreach (Vector2 velocity in SplitVelocity(v, MiddleExactPos))
                        {
                            Velocity += velocity;
                            Move(velocity);
                        }*/
                    }

                    Vector2 testPos = grappledEntity.MiddleExactPos;
                    if (grappledEntity is Actor a)
                        testPos += a.Velocity * Engine.Deltatime;
                    //Velocity = VectorHelper.ClosestOnSegment(testPos, RailPositions[currentPosIndex], RailPositions[currentPosIndex + 1]) - MiddleExactPos;

                    Velocity += grappledEntity.MiddleExactPos - MiddleExactPos;

                    

                    //TODO: Rework this to make it more smooth
                }
            }


            foreach (Vector2 velocity in SplitVelocity(Velocity * Engine.Deltatime, MiddleExactPos))
            {
                Move(velocity);
            }

            Debug.LogUpdate(Velocity);

            /*Velocity -= Velocity * 0.1f;

            Move(Velocity * Engine.Deltatime);

            Debug.LogUpdate(ExactPos);*/
        }

        private List<Vector2> SplitVelocity(Vector2 splittedVelocity, Vector2 from)
        {
            Vector2 old = splittedVelocity;
            splittedVelocity = VectorHelper.ClosestOnSegment(from + splittedVelocity, RailPositions[currentPosIndex], RailPositions[currentPosIndex + 1]) - from;

            List<Vector2> list = new();

            if (currentPosIndex != 0 && from == RailPositions[currentPosIndex])
            {
                Vector2 other = VectorHelper.ClosestOnSegment(from + old, RailPositions[currentPosIndex - 1], RailPositions[currentPosIndex]) - from;

                if ((splittedVelocity == Vector2.Zero || other.LengthSquared() > splittedVelocity.LengthSquared()) && other != Vector2.Zero)
                {
                    splittedVelocity = other;
                    currentPosIndex--;
                } 
            }
            else if(currentPosIndex != RailPositions.Length - 2 && from == RailPositions[currentPosIndex + 1])
            {
                Vector2 other = VectorHelper.ClosestOnSegment(from + old, RailPositions[currentPosIndex + 1], RailPositions[currentPosIndex + 2]) - from;

                if ((splittedVelocity == Vector2.Zero || other.LengthSquared() > splittedVelocity.LengthSquared()) && other != Vector2.Zero)
                {
                    splittedVelocity = other;
                    currentPosIndex++;
                }
            }

            if (splittedVelocity != Vector2.Zero)
            {
                list.Add(splittedVelocity);
                if (from + splittedVelocity == RailPositions[currentPosIndex])
                    list.AddRange(SplitVelocity(splittedVelocity, RailPositions[currentPosIndex]));
                else if (from + splittedVelocity == RailPositions[currentPosIndex + 1])
                    list.AddRange(SplitVelocity(splittedVelocity, RailPositions[currentPosIndex + 1]));
            }

            return list;
        }

        private static Vector2 DetermineInitPos(Vector2 position, Vector2[] railPositions, int width, int height, out int currentIndex)
        {
            position += new Vector2(width / 2, height / 2);
            currentIndex = -1;
            float minDistance = float.PositiveInfinity;
            Vector2 EndPos = position;
            for(int i = 0; i < railPositions.Length - 1; i++)
            {
                Vector2 possiblePos = VectorHelper.ClosestOnSegment(position, railPositions[i], railPositions[i + 1]);
                float distance = Vector2.Distance(possiblePos, position);
                if (distance < minDistance)
                {
                    EndPos = possiblePos;
                    currentIndex = i;
                    minDistance = distance;
                }
            }

            return EndPos - new Vector2(width / 2, height / 2);
        }

        /*private static float Length(this Vector2[] v)
        {
            float length = 0;
            foreach (Vector2 v1 in v)
                length += v1.Length();
            return length;
        }*/

        public override void OnDestroy()
        {
            base.OnDestroy();
            SwingingPoint.SwingingPoints.Remove(this);
        }

        void ISwinged.OnSwing(Entity grappledEntity, Func<bool> atSwingEnd)
        {
            this.grappledEntity = grappledEntity;
            isAtSwingEnd = atSwingEnd;
        }

        void ISwinged.OnStopSwing(Entity unGrappledEntity)
        {
            if(grappledEntity == unGrappledEntity)
                grappledEntity = null;
        }
    }
}
