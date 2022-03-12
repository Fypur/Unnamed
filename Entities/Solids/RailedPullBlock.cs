using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic_platformer
{
    public class RailedPullBlock : MovingSolid, ISwinged
    {
        private const float amountMoved = 100f;
        public Vector2[] RailPositions;
        private Entity grappledEntity;

        //Indicates the index of positions[] where the platform is inbetween
        private int currentPosIndex;

        public RailedPullBlock(Vector2[] positions, int initialIndexPosition, int width, int height) : base(positions[initialIndexPosition], width, height, new Sprite(Color.Beige))
        {
            RailPositions = positions;
            currentPosIndex = initialIndexPosition;
            GrapplingPoint.GrapplingSolids.Add(this);

            AddComponent(new LineRenderer(RailPositions.ToList(), 1, Color.BlueViolet, null, (line) => { line.Positions = RailPositions.ToList(); }));
        }

        public RailedPullBlock(Vector2[] positions, Vector2 initPos, int width, int height) : base(DetermineInitPos(initPos, positions, width, height, out int initialIndex), width, height, new Sprite(Color.Beige))
        {
            RailPositions = positions;
            currentPosIndex = initialIndex;
            GrapplingPoint.GrapplingSolids.Add(this);

            AddComponent(new LineRenderer(RailPositions.ToList(), 1, Color.BlueViolet, null, (line) => { line.Positions = RailPositions.ToList(); }));
        }

        public override void Update()
        {
            base.Update();
            if(grappledEntity == null)
                return;

            Velocity = Vector2.Normalize(grappledEntity.MiddleExactPos - MiddleExactPos) * amountMoved * Engine.Deltatime;

            foreach (Vector2 velocity in SplitVelocity(Velocity, MiddleExactPos))
                Move(velocity);
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
