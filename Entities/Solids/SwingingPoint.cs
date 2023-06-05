using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platformer
{
    public class SwingingPoint : MovingSolid, ISwinged
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
            : base(position, width, height, new Sprite(DataManager.Objects["swingingPoint"]))
        {
            SwingingPoints.Add(this);

            Collider.Collidable = false;
            MaxSwingDistance = maxSwingDistance;

            AddComponent(new CycleMover(position, width, height, goingForwards, positions, timesBetweenPositions, easeFunction, out Vector2 p));
            ExactPos = p;
        }

        public override void Awake()
        {
            base.Awake();

            AddComponent(new CircleLight(HalfSize, Math.Min(MaxSwingDistance, 100), new Color(Color.LightBlue, 50), new Color(Color.LightBlue, 0)));
            polygon = Polygon.GetCircleVisibilityPolygon(MiddlePos, MaxSwingDistance);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            SwingingPoints.Remove(this); 
        }
        
        void ISwinged.OnSwing(Entity grappledEntity, Func<bool> isAtSwingEnd)
        {
            swinged = true;

            if (Parent is FallingPlatform falling)
                falling.Fall();
        }

        void ISwinged.OnStopSwing(Entity unGrappledEntity)
        {
            swinged = false;
        }

        public override void Update()
        {
            base.Update();

            //if(ExactPos != PreviousExactPos)
            {
                polygon = Polygon.GetCircleVisibilityPolygon(MiddlePos, MaxSwingDistance);

                if (!swinged)
                    return;

                List<Vector2> cornersToCheck = new List<Vector2>(Engine.CurrentMap.CurrentLevel.Corners);

                RemoveGrapplingPoints();

                Player p = (Player)Engine.Player;

                if (p.SwingPositions.Count > 1)
                    cornersToCheck.Remove(p.SwingPositions[1]);

                AddGrapplingPoints(cornersToCheck, p.SwingPositions.Count > 1 ? p.SwingPositions[1] : p.MiddleExactPos);



                void AddGrapplingPoints(List<Vector2> cornersToCheck, Vector2 checkingFrom)
                {
                    float angle = VectorHelper.GetAngle(checkingFrom - PreviousExactPos - HalfSize, checkingFrom - ExactPos - HalfSize);

                    if (angle == 0)
                        return;

                    float distanceFromPoint = Vector2.DistanceSquared(ExactPos + HalfSize, checkingFrom);

                    float closestAngle = angle;
                    Vector2? closestPoint = null;

                    List<Vector2> nextCorners = new List<Vector2>();

                    foreach (Vector2 corner in cornersToCheck)
                    {
                        float cornerDistance = Vector2.DistanceSquared(checkingFrom, corner);
                        if (cornerDistance > distanceFromPoint)
                        {
                            continue;
                        }

                        float pointAngle = VectorHelper.GetAngle(checkingFrom - PreviousExactPos - HalfSize, checkingFrom - corner);

                        if (pointAngle * Math.Sign(angle) >= 0 && pointAngle * Math.Sign(angle) <= angle * Math.Sign(angle))
                        {
                            if (pointAngle * Math.Sign(closestAngle) <= closestAngle * Math.Sign(closestAngle))
                            {
                                closestAngle = pointAngle;
                                closestPoint = corner;
                            }

                            nextCorners.Add(corner);
                        }
                    }

                    if (closestPoint is Vector2 foundCorner)
                    {
                        p.SwingPositions.Insert(1, foundCorner);
                        p.SwingPositionsSign.Insert(1, -Math.Sign(angle));

                        nextCorners.Remove(foundCorner);

                        if (nextCorners.Count > 0)
                            AddGrapplingPoints(nextCorners, foundCorner);
                    }
                }

                void RemoveGrapplingPoints()
                {
                    Player p = (Player)Engine.Player;


                    //Debug.LogUpdate(p.SwingPositions.Count, p.SwingPositionsSign.Count);
                    
                    for (int i = 1; i < p.SwingPositions.Count; i++)
                    {
                        Vector2 prevPos = (i == p.SwingPositions.Count - 1) ? p.MiddleExactPos : p.SwingPositions[i + 1];
                        float grappleAngle = VectorHelper.GetAngle(prevPos - MiddleExactPos, prevPos - p.SwingPositions[i]);

                        //Debug.LogUpdate(p.SwingPositionsSign[i]);
                        if (Math.Sign(grappleAngle) == -p.SwingPositionsSign[i])
                        {
                            cornersToCheck.Remove(p.SwingPositions[i]);
                            p.SwingPositions.RemoveAt(i);
                            p.SwingPositionsSign.RemoveAt(i);
                        }
                        else
                            break;
                    }
                }

            }
        }

        public override void Render()
        {
            base.Render();

            if (Engine.Player != null && !swinged && !new Raycast(Raycast.RayTypes.MapTiles, MiddleExactPos, Engine.Player.Pos + Engine.Player.HalfSize).Hit && Vector2.Distance(MiddleExactPos, Engine.Player.Pos + Engine.Player.HalfSize) < MaxSwingDistance)
                Drawing.DrawDottedLine(MiddlePos, Engine.Player.Pos + Engine.Player.HalfSize, new Color(Color.LightBlue, 120), 1, 4, 4);

            //Polygon.DrawCirclePolygon(polygon, MiddlePos, MaxSwingDistance, new Color(Color.LightBlue, 120));
            Polygon.DrawCirclePolygon(polygon, MiddlePos, MaxSwingDistance, new Color(Color.DeepSkyBlue, 120));

            if (Debug.DebugMode)
                Drawing.DrawCircleEdge(MiddleExactPos, MaxSwingDistance, 0.1f, new Color(Color.LightBlue, 120), 1);


            
            /*Drawing.DrawLine(MiddlePos + Vector2.UnitX * 300, Input.MousePos, Color.Yellow);
            foreach (Vector2 p in Collision.LineCircleIntersection(MiddlePos + Vector2.UnitX * 300, Input.MousePos, MiddlePos, MaxSwingDistance))
                Debug.PointUpdate(p);*/


        }
    }
}
