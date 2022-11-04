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

            GetSwingPolygon();
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

        private void GetSwingPolygon()
        {
            //Obtenir tous les corners à l'interieur du cercle
            //Cast sur les corners pour éliminer
            Vector2 middle = MiddlePos;
            float sqrdMaxedDist = MaxSwingDistance * MaxSwingDistance;
            List<Vector2> allCorners = new List<Vector2>(Engine.CurrentMap.CurrentLevel.Corners);
            allCorners.AddRange(Engine.CurrentMap.CurrentLevel.InsideCorners);
            List<Vector2> corners = new();
            Dictionary<Vector2, float> distancesSquared = new();


            foreach (Vector2 corner in allCorners)
            {
                float d = Vector2.DistanceSquared(middle, corner);

                //Les 5 raycast comme ça c'est juste pcq des fois c'est un peu funky et un seul raycast trouve que y a collision
                //au pixel près. Donc j'en fait 5 pcq un raycast sur maptiles c'est pas expensive
                Raycast bestRay = Raycast.FiveRays(middle, corner, false, true, 1);
                /*var r = new Raycast(Raycast.RayTypes.MapTiles, middle, corner);
                var r2 = new Raycast(Raycast.RayTypes.MapTiles, middle, corner - Vector2.UnitX);
                var r3 = new Raycast(Raycast.RayTypes.MapTiles, middle, corner - Vector2.UnitY);
                var r4 = new Raycast(Raycast.RayTypes.MapTiles, middle, corner + Vector2.UnitX);
                var r5 = new Raycast(Raycast.RayTypes.MapTiles, middle, corner + Vector2.UnitY);
                //Debug.PointUpdate(Color.DarkGreen, r5.EndPoint);

                Raycast bestRay = r;
                if (bestRay.Hit)
                    bestRay = r2;
                if (bestRay.Hit)
                    bestRay = r3;
                if (bestRay.Hit)
                    bestRay = r4;
                if (bestRay.Hit)
                    bestRay = r5;*/

                if (d < sqrdMaxedDist && !bestRay.Hit)
                {
                    corners.Add(corner);
                    distancesSquared[corner] = d;
                }
            }

            //Cast derrière les corners pour voir les points aux edges du cercle
            List<Vector2> points = new();
            foreach (Vector2 corner in corners)
            {
                points.Add(corner);
                var r = new Raycast(Raycast.RayTypes.MapTiles, middle, corner - middle, MaxSwingDistance);
                var r2 = new Raycast(Raycast.RayTypes.MapTiles, middle, corner - middle - Vector2.UnitX * 0.01f, MaxSwingDistance);
                var r3 = new Raycast(Raycast.RayTypes.MapTiles, middle, corner - middle - Vector2.UnitY * 0.01f, MaxSwingDistance);
                var r4 = new Raycast(Raycast.RayTypes.MapTiles, middle, corner - middle + Vector2.UnitX * 0.01f, MaxSwingDistance);
                var r5 = new Raycast(Raycast.RayTypes.MapTiles, middle, corner - middle + Vector2.UnitY * 0.01f, MaxSwingDistance);
                //Debug.PointUpdate(Color.DarkGreen, r5.EndPoint);

                Raycast bestRay = r;
                if(r2.DistanceSquared >= bestRay.DistanceSquared)
                    bestRay = r2;
                if (r3.DistanceSquared >= bestRay.DistanceSquared)
                    bestRay = r3;
                if (r4.DistanceSquared >= bestRay.DistanceSquared)
                    bestRay= r4;
                if (r5.DistanceSquared >= bestRay.DistanceSquared)
                    bestRay = r5;

                if (bestRay.DistanceSquared > distancesSquared[corner] && Vector2.DistanceSquared(bestRay.EndPoint, corner) > 1f)
                {
                    distancesSquared[bestRay.EndPoint] = bestRay.DistanceSquared;
                    points.Add(bestRay.EndPoint);
                }
            }

            //On determine toutes les edges (non opti là ça fait par tile)
            List<int[]> edges = new();
            for (int y = 0; y < Engine.CurrentMap.CurrentLevel.Organisation.GetLength(0); y++)
                for (int x = 0; x < Engine.CurrentMap.CurrentLevel.Organisation.GetLength(1); x++)
                {
                    if (Engine.CurrentMap.CurrentLevel.GetOrganisation(x, y) != 0)
                    {
                        if (Engine.CurrentMap.CurrentLevel.GetOrganisation(x - 1, y) == 0)
                            edges.Add(new int[4] { x, y, x, y + 1 } );
                        if (Engine.CurrentMap.CurrentLevel.GetOrganisation(x + 1, y) == 0)
                        {
                            edges.Add(new int[4] { x + 1, y, x + 1, y + 1 });
                            //edges.Add(new int[4] { x + 1, y, x + 1, y + 1 });
                        }
                        if (Engine.CurrentMap.CurrentLevel.GetOrganisation(x, y - 1) == 0)
                            edges.Add(new int[4] { x, y, x + 1, y });
                        if (Engine.CurrentMap.CurrentLevel.GetOrganisation(x, y + 1) == 0)
                            edges.Add(new int[4] { x, y + 1, x + 1, y + 1 });
                    }
                }

            //On cherche les points d'intersection sur les edges, puis on vérifie par raycast
            foreach (int[] edge in edges)
            {
                Vector2 coord1 = new Vector2(edge[0], edge[1]) * 8 + Engine.CurrentMap.CurrentLevel.Pos;
                Vector2 coord2 = new Vector2(edge[2], edge[3]) * 8 + Engine.CurrentMap.CurrentLevel.Pos;

                if (Vector2.DistanceSquared(middle, coord1) > MaxSwingDistance * MaxSwingDistance && Vector2.DistanceSquared(middle, coord2) > MaxSwingDistance * MaxSwingDistance)
                    continue;

                //Debug.PointUpdate(Color.Blue, coord1, coord2);
                Vector2[] collPoints = Collision.LineCircleIntersection(coord1, coord2, middle, MaxSwingDistance);
                foreach(Vector2 p in collPoints)
                {
                    if (points.Contains(p))
                        continue;

                    Raycast bestRay = Raycast.FiveRays(middle, p, false, true, 1);

                    //Debug.PointUpdate(Color.Green, collPoints);
                    /*var r = new Raycast(Raycast.RayTypes.MapTiles, middle, p);
                    var r2 = new Raycast(Raycast.RayTypes.MapTiles, middle, p - Vector2.UnitX);
                    var r3 = new Raycast(Raycast.RayTypes.MapTiles, middle, p - Vector2.UnitY);
                    var r4 = new Raycast(Raycast.RayTypes.MapTiles, middle, p + Vector2.UnitX);
                    var r5 = new Raycast(Raycast.RayTypes.MapTiles, middle, p + Vector2.UnitY);
                    Raycast bestRay = r;
                    if ((bestRay.Hit || bestRay.DistanceSquared < r2.DistanceSquared) && !r2.Hit)
                        bestRay = r2;
                    if ((bestRay.Hit || bestRay.DistanceSquared < r3.DistanceSquared) && !r3.Hit)
                        bestRay = r3;
                    if ((bestRay.Hit || bestRay.DistanceSquared < r4.DistanceSquared) && !r4.Hit)
                        bestRay = r4;
                    if ((bestRay.Hit || bestRay.DistanceSquared < r4.DistanceSquared) && !r5.Hit)
                        bestRay = r5;*/

                    if (!bestRay.Hit)
                    {
                        distancesSquared[p] = Vector2.DistanceSquared(middle, p);
                        points.Add(p);
                    }
                }
            }

            //On sort les points par angle
            Dictionary<Vector2, float> angles = new();
            foreach(Vector2 point in points)
                angles[point] = (point - middle).ToAngle();

            points.Sort((vec1, vec2) =>
                {
                    return Math.Sign(angles[vec1] - angles[vec2]);
                });

            for (int i = 0; i < points.Count - 2; i++)
                if(Vector2.Floor(points[i]) == Vector2.Floor(points[i + 1])) points.RemoveAt(i + 1);
            if (points.Count > 1)
                if (Vector2.Floor(points[points.Count - 1]) == Vector2.Floor(points[0])) points.RemoveAt(points.Count - 1);

            void CheckAngleAndChange(int index0, int index1, int index2)
            {
                Vector2 p0 = points[index0];
                Vector2 p1 = points[index1];
                Vector2 p2 = points[index2];

                var r0 = new Raycast(Raycast.RayTypes.MapTiles, p0, p1);
                var r1 = new Raycast(Raycast.RayTypes.MapTiles, p1, p2);

                bool p0p1 = r0.Hit || ((p0 - p1).X == 0 || (p0 - p1).Y == 0);
                bool p1p2 = r1.Hit || ((p1 - p2).X == 0 || (p1 - p2).Y == 0);

                if (p0p1 && p1p2)
                    return;

                var r3 = new Raycast(Raycast.RayTypes.MapTiles, p0, p2);
                /*if (!r3.Hit && !p0p1 && p1p2)
                {
                    Vector2 a = points[index1];
                    points[index1] = points[index2];
                    points[index2] = a;
                }*/


                Debug.PointUpdate(p0);

                /*Vector2 sub = points[index0] - points[index1];
                Vector2 sub2 = points[index0] - points[index2];

                if (Math.Round(distancesSquared[points[index1]], 2) + 1 > MaxSwingDistance * MaxSwingDistance && ((sub2.X == 0 && sub.X != 0) || (sub2.Y == 0 && sub.Y != 0)))
                {
                    Vector2 a = points[index1];
                    points[index1] = points[index2];
                    points[index2] = a;
                }*/

                /*if (Vector2.DistanceSquared(points[index0], points[index1]) < 1)
                {
                    points.RemoveAt(index1);
                    if (Vector2.DistanceSquared(points[index0], points[index1 < points.Count ? index1 : 0]) < 1)
                        points.RemoveAt(index1);
                    return;
                }
                if (Vector2.DistanceSquared(points[index0], points[index2]) < 1)
                    points.RemoveAt(index2);*/
            }

            //On essaie de résoudre le problème de 2 points ayant le même angle en utilisant le fait que 2 points soit alignés
            for(int i = 0; i < points.Count - 2; i++)
                CheckAngleAndChange(i, i + 1, i + 2);
            if (points.Count > 2)
                CheckAngleAndChange(points.Count - 2, points.Count - 1, 0);
            if (points.Count > 1)
                CheckAngleAndChange(points.Count - 1, 0, 1);

            //On vérifie si 2 points sont curved en vérifiant si 2 points sont connectés et sont à la limite du cercle
            polygon = new PolygonPoint[points.Count];
            for(int i = 0; i < polygon.Length; i++)
            {
                //distancesSquared[points[i]] = Vector2.DistanceSquared(middle, points[i]);
                PolygonPoint point = new PolygonPoint(points[i]);
                if(Math.Round(distancesSquared[points[i]], 2) + 1 >= MaxSwingDistance * MaxSwingDistance)
                {
                    if (Math.Round(distancesSquared[points[i + 1 < points.Count ? i + 1 : 0]], 5) + 1 >= MaxSwingDistance * MaxSwingDistance)
                        point.ArchedRight = true;
                    if (Math.Round(distancesSquared[points[i - 1 > 0 ? i - 1 : points.Count - 1]], 5) + 1 >= MaxSwingDistance * MaxSwingDistance)
                        point.ArchedLeft = true;
                }
                polygon[i] = point;
            }

            //Debug
            for(int i = 0; i < polygon.Length; i++)
            {
               // Debug.PointUpdate(polygon[i].Position);
                if (Vector2.DistanceSquared(Input.MousePos, polygon[i].Position) < 1f)
                {
                    Debug.LogUpdate(i, polygon[i].ArchedLeft, polygon[i].ArchedRight, polygon[i].Position);
                    Debug.LogUpdate((polygon[i].Position - middle).ToAngle());
                }
            }
        }

        public override void Update()
        {
            base.Update();

            //Debug.PointUpdate(new Raycast(Raycast.RayTypes.MapTiles, MiddlePos, Input.MousePos).EndPoint);
            /*Vector2[] coll = Collision.LineCircleIntersection(MiddlePos, Input.MousePos, MiddlePos, MaxSwingDistance);
            foreach (Vector2 p in coll)
                Debug.PointUpdate(Color.GreenYellow, p);*/

            GetSwingPolygon();
        }

        public override void Render()
        {
            base.Render();

            if (Engine.Player != null && !swinged && !new Raycast(Raycast.RayTypes.MapTiles, MiddleExactPos, Engine.Player.Pos + Engine.Player.HalfSize).Hit && Vector2.Distance(MiddleExactPos, Engine.Player.Pos + Engine.Player.HalfSize) < MaxSwingDistance)
                Drawing.DrawDottedLine(MiddlePos, Engine.Player.Pos + Engine.Player.HalfSize, new Color(Color.Gray, 50), 1, 4, 4);

            for (int i = 0; i < polygon.Length; i++)
            {
                if (polygon[i].ArchedRight && polygon[i + 1 < polygon.Length ? i + 1 : 0].ArchedLeft)
                {
                    if (!Debug.DebugMode)
                        Drawing.DrawArc(MiddleExactPos, MaxSwingDistance, polygon[i].Position, polygon[i + 1 < polygon.Length ? i + 1 : 0].Position, 0.01f, new Color(Color.Gray, 50), 1);
                    else
                        Drawing.DrawLine(polygon[i].Position, polygon[i + 1 < polygon.Length ? i + 1 : 0].Position, new Color(Color.Yellow, 130));
                }
                else
                {
                    if(!Debug.DebugMode)
                        Drawing.DrawLine(polygon[i].Position, polygon[i + 1 < polygon.Length ? i + 1 : 0].Position, new Color(Color.Gray, 50));
                    else
                        Drawing.DrawLine(polygon[i].Position, polygon[i + 1 < polygon.Length ? i + 1 : 0].Position, new Color(Color.Green, 130));
                }
            }


            /*Drawing.DrawLine(MiddlePos + Vector2.UnitX * 300, Input.MousePos, Color.Yellow);
            foreach (Vector2 p in Collision.LineCircleIntersection(MiddlePos + Vector2.UnitX * 300, Input.MousePos, MiddlePos, MaxSwingDistance))
                Debug.PointUpdate(p);*/


            if (Debug.DebugMode)
                Drawing.DrawCircleEdge(MiddleExactPos, MaxSwingDistance, 0.1f, new Color(Color.Gray, 50), 1);
        }

        private class PolygonPoint
        {
            public Vector2 Position;
            public bool ArchedRight;
            public bool ArchedLeft;

            public PolygonPoint(Vector2 position, bool archedRight, bool archedLeft)
            {
                Position = position;
                ArchedRight = archedRight;
                ArchedLeft = archedLeft;
            }

            public PolygonPoint(Vector2 position)
            {
                Position = position;
            }
        }
    }
}
