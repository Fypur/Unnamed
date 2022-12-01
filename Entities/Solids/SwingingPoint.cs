using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platformer
{
    public class SwingingPoint : CyclingSolid, ISwinged
    {
        public static List<ISwinged> SwingingPoints = new List<ISwinged>();

        private const int width = 8;
        private const int height = 8;

        private bool swinged;
        private PolygonPoint[] polygon;

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

        public override void Awake()
        {
            base.Awake();

            polygon = Polygon.GetCircleVisibilityPolygon(MiddlePos, MaxSwingDistance);
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

        public override void Update()
        {
            base.Update();

            //Debug.PointUpdate(new Raycast(Raycast.RayTypes.MapTiles, MiddlePos, Input.MousePos).EndPoint);
            /*Vector2[] coll = Collision.LineCircleIntersection(MiddlePos, Input.MousePos, MiddlePos, MaxSwingDistance);
            foreach (Vector2 p in coll)
                Debug.PointUpdate(Color.GreenYellow, p);*/

#if DEBUG
            /*if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.Space))
                 MovingTimer.Paused = !MovingTimer.Paused;

             if (Input.GetKey(Microsoft.Xna.Framework.Input.Keys.N))
                 Pos = Input.MousePos;*/
#endif


            polygon = Polygon.GetCircleVisibilityPolygon(MiddlePos, MaxSwingDistance);
        }

        public override void Render()
        {
            base.Render();

            if (Engine.Player != null && !swinged && !new Raycast(Raycast.RayTypes.MapTiles, MiddleExactPos, Engine.Player.Pos + Engine.Player.HalfSize).Hit && Vector2.Distance(MiddleExactPos, Engine.Player.Pos + Engine.Player.HalfSize) < MaxSwingDistance)
                Drawing.DrawDottedLine(MiddlePos, Engine.Player.Pos + Engine.Player.HalfSize, new Color(Color.Gray, 50), 1, 4, 4);

            Polygon.DrawCirclePolygon(polygon, MiddlePos, MaxSwingDistance, new Color(Color.Gray, 50));

            
            /*Drawing.DrawLine(MiddlePos + Vector2.UnitX * 300, Input.MousePos, Color.Yellow);
            foreach (Vector2 p in Collision.LineCircleIntersection(MiddlePos + Vector2.UnitX * 300, Input.MousePos, MiddlePos, MaxSwingDistance))
                Debug.PointUpdate(p);*/


            if (Debug.DebugMode)
                Drawing.DrawCircleEdge(MiddleExactPos, MaxSwingDistance, 0.1f, new Color(Color.Gray, 50), 1);
        }
    }
}
