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

        private bool swinged;

        public float MaxSwingDistance { get; set; }
        public SwingingPoint(Vector2 position, float maxSwingDistance) : base(position, width, height, new Sprite(DataManager.Objects["swingingPoint"])) 
        {
            SwingingPoints.Add(this);
            Collider.Collidable = false;
            MaxSwingDistance = maxSwingDistance;
        }

        public SwingingPoint(Vector2 position, float maxSwingDistance, Vector2[] positions, float[] timesBetweenPositions, bool goingForwards, Func<float, float> easeFunction = null)
            : base(position, width, height, new Sprite(DataManager.Objects["swingingPoint"]), goingForwards, positions, timesBetweenPositions, easeFunction)
        {
            SwingingPoints.Add(this);

            Collider.Collidable = false;
            MaxSwingDistance = maxSwingDistance;
        }

        public override void Render()
        {
            base.Render();
            
            Drawing.DrawCircleEdge(Pos + HalfSize, MaxSwingDistance, 0.1f, new Color(Color.Gray, 50), 1);

            if (!swinged && !new Raycast(Raycast.RayTypes.MapTiles, Pos + HalfSize, Engine.Player.Pos + Engine.Player.HalfSize).Hit && Vector2.Distance(Pos + HalfSize, Engine.Player.Pos + Engine.Player.HalfSize) < MaxSwingDistance)
                Drawing.DrawLine(Pos + HalfSize, Engine.Player.Pos + Engine.Player.HalfSize, new Color(Color.Gray, 50), 1);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            SwingingPoints.Remove(this); 
        }
        
        void ISwinged.OnGrapple(Entity grappledEntity, Func<bool> isAtSwingEnd)
        {
            swinged = true;
        }

        void ISwinged.OnStopGrapple(Entity unGrappledEntity)
        {
            swinged = false;
        }

        //TODO: DRAW DOTTED LINE WHEN PLAYER IS NEAR TO SWINGING POINT
    }
}
