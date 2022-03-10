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
            /*Velocity = VectorHelper.ClosestOnSegement(MiddleExactPos + Velocity, RailPositions[currentPosIndex], RailPositions[currentPosIndex + 1]) - MiddleExactPos;*/
            foreach (Vector2 velocity in SplitVelocity(Velocity, MiddleExactPos))
                Move(velocity);
        }

        private List<Vector2> SplitVelocity(Vector2 splittedVelocity, Vector2 from)
        {
            Vector2 axis = RailPositions[currentPosIndex] - RailPositions[currentPosIndex + 1];
            splittedVelocity = VectorHelper.ClosestOnSegement(from + Velocity, RailPositions[currentPosIndex], RailPositions[currentPosIndex + 1]) - from;

            Vector2 headedToRailPos;
            int indexAdd = 1;            

            if (Vector2.Dot(splittedVelocity, axis) > 0)
            {
                //Going to previous railPos
                headedToRailPos = RailPositions[currentPosIndex];
                indexAdd = -1;
            }
            else
            {
                headedToRailPos = RailPositions[currentPosIndex + 1];
                indexAdd = 1;
            }

            List<Vector2> list = new List<Vector2> { splittedVelocity };
            Debug.LogUpdate(currentPosIndex);
            if (from + splittedVelocity == headedToRailPos && splittedVelocity != Vector2.Zero && currentPosIndex + indexAdd >= 0 && currentPosIndex + indexAdd < RailPositions.Length - 1)
            {
                currentPosIndex += indexAdd;
                list.AddRange(SplitVelocity(splittedVelocity, headedToRailPos));
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
                Vector2 possiblePos = VectorHelper.ClosestOnSegement(position, railPositions[i], railPositions[i + 1]);
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
