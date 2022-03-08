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
        private const float amountMoved = 4f;
        public Vector2[] RailPositions;
        private Entity grappledEntity;

        //Indicates the index of positions[] where the platform is inbetween
        private int currentPosIndex;

        public RailedPullBlock(Vector2[] positions, int initialIndexPosition, int width, int height) : base(positions[initialIndexPosition], width, height, new Sprite(Color.Beige))
        {
            RailPositions = positions;
            currentPosIndex = initialIndexPosition;
            GrapplingPoint.GrapplingSolids.Add(this);

            AddComponent(new LineRenderer(RailPositions.ToList(), 3, Color.BlueViolet, null, (line) => { line.Positions = RailPositions.ToList(); }));
        }

        public RailedPullBlock(Vector2[] positions, Vector2 initPos, int width, int height) : base(DetermineInitPos(initPos, positions, out int initialIndex), width, height, new Sprite(Color.Beige))
        {
            RailPositions = positions;
            currentPosIndex = initialIndex;
            GrapplingPoint.GrapplingSolids.Add(this);

            AddComponent(new LineRenderer(RailPositions.ToList(), 3, Color.BlueViolet, null, (line) => { line.Positions = RailPositions.ToList(); }));
        }

        public override void Update()
        {
            base.Update();
            if(grappledEntity == null)
                return;

            Velocity = Vector2.Normalize(grappledEntity.Pos - MiddlePos) * amountMoved * Engine.Deltatime;
            foreach (Vector2 velocity in SplitVelocity(Velocity))
                Move(velocity);
        }

        private List<Vector2> SplitVelocity(Vector2 splitedVelocity)
        {
            Vector2 axis = RailPositions[currentPosIndex] - RailPositions[currentPosIndex + 1];
            splitedVelocity = VectorHelper.Projection(splitedVelocity * Engine.Deltatime - ExactPos, axis);

            Vector2 headedToRailPos;
            if (Vector2.Dot(splitedVelocity, axis) > 0)
            {
                //Going to previous railPos
                headedToRailPos = RailPositions[currentPosIndex];
            }
            else
                headedToRailPos = RailPositions[currentPosIndex + 1];


            List<Vector2> list = new List<Vector2> { splitedVelocity };
            if (splitedVelocity.Length() > (headedToRailPos - ExactPos).Length())
                list.AddRange(SplitVelocity(splitedVelocity - (headedToRailPos - ExactPos)));

            return list;
        }

        private static Vector2 DetermineInitPos(Vector2 position, Vector2[] railPosition, out int currentIndex)
        {
            currentIndex = 0;
            return position;
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
