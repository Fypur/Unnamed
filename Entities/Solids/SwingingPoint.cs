using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Unnamed
{
    public class SwingingPoint : Solid, ISwinged
    {
        public static List<ISwinged> SwingingPoints = new List<ISwinged>();

        private const int width = 8;
        private const int height = 8;

        private bool swinged;
        private PolygonPoint[] polygon;
        private Vector2 previousExactPos;

        public float MaxSwingDistance { get; set; }
        public SwingingPoint(Vector2 position, float maxSwingDistance) : base(position, new AABBCollider(Vector2.Zero, width, height), new Sprite(DataManager.Objects["swingingPoint"]))
        {
            SwingingPoints.Add(this);
            Collider.Collidable = false;
            MaxSwingDistance = maxSwingDistance;
            previousExactPos = ExactPos;
        }

        public SwingingPoint(Vector2 position, float maxSwingDistance, Vector2[] positions, float[] timesBetweenPositions, bool goingForwards, Func<float, float> easeFunction = null)
            : base(position, new AABBCollider(Vector2.Zero, width, height), new Sprite(DataManager.Objects["swingingPoint"]))
        {
            SwingingPoints.Add(this);

            Collider.Collidable = false;
            MaxSwingDistance = maxSwingDistance;

            AddComponent(new CycleMover(position, width, height, goingForwards, positions, timesBetweenPositions, easeFunction, out Vector2 p));
            ExactPos = p;
            previousExactPos = ExactPos;
        }

        public override void Awake()
        {
            base.Awake();

            AddComponent(new CircleLight(AABBCollider.HalfSize, Math.Min(MaxSwingDistance, 100), new Color(Color.LightBlue, 50), new Color(Color.LightBlue, 0)));
            polygon = Polygon.GetCircleVisibilityPolygon(MiddlePos, MaxSwingDistance, LevelManager.CurrentGrid.GridCollider);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            SwingingPoints.Remove(this);
        }

        void ISwinged.OnSwing(Kinematic grappledEntity, Func<bool> isAtSwingEnd)
        {
            swinged = true;

            if (Parent is FallingPlatform falling)
                falling.Fall();
        }

        void ISwinged.OnStopSwing(Kinematic unGrappledEntity)
        {
            swinged = false;
        }

        public override void Update()
        {
            base.Update();

            //if(ExactPos != PreviousExactPos)
            {
                polygon = Polygon.GetCircleVisibilityPolygon(MiddlePos, MaxSwingDistance, LevelManager.CurrentGrid.GridCollider);

                if (!swinged)
                    return;

                List<Vector2> cornersToCheck = new List<Vector2>(LevelManager.CurrentGrid.GridCollider.Corners);

                RemoveGrapplingPoints();

                Player player = Platformer.Player;

                if (player.SwingPositions.Count > 1)
                    cornersToCheck.Remove(player.SwingPositions[1]);

                AddGrapplingPoints(cornersToCheck, player.SwingPositions.Count > 1 ? player.SwingPositions[1] : player.MiddleExactPos);



                void AddGrapplingPoints(List<Vector2> cornersToCheck, Vector2 checkingFrom)
                {
                    float angle = VectorHelper.GetAngle(checkingFrom - previousExactPos - AABBCollider.HalfSize, checkingFrom - ExactPos - AABBCollider.HalfSize);

                    if (angle == 0)
                        return;

                    float distanceFromPoint = Vector2.DistanceSquared(ExactPos + AABBCollider.HalfSize, checkingFrom);

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

                        float pointAngle = VectorHelper.GetAngle(checkingFrom - previousExactPos - AABBCollider.HalfSize, checkingFrom - corner);

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
                        player.SwingPositions.Insert(1, foundCorner);
                        player.SwingPositionsSign.Insert(1, -Math.Sign(angle));

                        nextCorners.Remove(foundCorner);

                        if (nextCorners.Count > 0)
                            AddGrapplingPoints(nextCorners, foundCorner);
                    }
                }

                void RemoveGrapplingPoints()
                {
                    Player player = Platformer.Player;


                    //Debug.LogUpdate(p.SwingPositions.Count, p.SwingPositionsSign.Count);

                    for (int i = 1; i < player.SwingPositions.Count; i++)
                    {
                        Vector2 prevPos = (i == player.SwingPositions.Count - 1) ? player.MiddleExactPos : player.SwingPositions[i + 1];
                        float grappleAngle = VectorHelper.GetAngle(prevPos - MiddleExactPos, prevPos - player.SwingPositions[i]);

                        //Debug.LogUpdate(p.SwingPositionsSign[i]);
                        if (Math.Sign(grappleAngle) == -player.SwingPositionsSign[i])
                        {
                            cornersToCheck.Remove(player.SwingPositions[i]);
                            player.SwingPositions.RemoveAt(i);
                            player.SwingPositionsSign.RemoveAt(i);
                        }
                        else
                            break;
                    }
                }

            }

            previousExactPos = ExactPos;
        }

        public override void Render()
        {
            GetComponent<CircleLight>().Visible = true;
            base.Render();

            if (Platformer.Player != null && !swinged && !Raycast.FastRay(MiddleExactPos, Platformer.Player.Pos + Platformer.Player.AABBCollider.HalfSize, LevelManager.CurrentGrid.GridCollider).Hit && Vector2.Distance(MiddleExactPos, Platformer.Player.Pos + Platformer.Player.AABBCollider.HalfSize) < MaxSwingDistance)
                Drawing.DrawDottedLine(MiddlePos, Platformer.Player.Pos + Platformer.Player.AABBCollider.HalfSize, new Color(Color.DeepSkyBlue * (40f / 255), 255), 1, 4, 4);

            Polygon.DrawCirclePolygon(polygon, MiddlePos, MaxSwingDistance, new Color(Color.DeepSkyBlue * (40f / 255), 255));

            if (Debug.DebugMode)
                Drawing.DrawCircleEdge(MiddleExactPos, MaxSwingDistance, 0.1f, new Color(Color.LightBlue, 120), 1);

            /*Drawing.DrawLine(MiddlePos + Vector2.UnitX * 300, Input.MousePos, Color.Yellow);
            foreach (Vector2 p in Collision.LineCircleIntersection(MiddlePos + Vector2.UnitX * 300, Input.MousePos, MiddlePos, MaxSwingDistance))
                Debug.PointUpdate(p);*/


        }
    }
}
