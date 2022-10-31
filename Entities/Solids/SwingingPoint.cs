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
        private Vector2[] polygon;

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
            /*
            Obtenir tous les corners à l'interieur du cercle
            Cast sur les corners pour éliminer
            Cast derrière les corners pour voir les points aux edges du cercle
            Obtenir toutes les edges à l'interieur du cercle
            Résoudre équation edge cercle et obtenir les points
            Sort les points par angle
            Pour 2 points sur les edges du cercle vérifier si ils font partie de la même edge : si oui straight line, sinon dessiner un arc
            Tracer polygone et arc
            */

            //Obtenir tous les corners à l'interieur du cercle
            //Cast sur les corners pour éliminer
            Vector2 middle = Pos + HalfSize;
            float sqrdMaxedDist = MaxSwingDistance * MaxSwingDistance;
            List<Vector2> corners = new();
            foreach (Vector2 corner in Engine.CurrentMap.CurrentLevel.Corners)
            {
                if (Vector2.DistanceSquared(middle, corner) < sqrdMaxedDist && !new Raycast(Raycast.RayTypes.MapTiles, Pos + HalfSize, corner).Hit)
                    corners.Add(corner);
            }

            //Cast derrière les corners pour voir les points aux edges du cercle
            List<Vector2> points = new();
            foreach (Vector2 corner in corners)
            {
                points.Add(corner);
                var r = new Raycast(Raycast.RayTypes.MapTiles, middle, corner - middle, MaxSwingDistance);
                if (r.Distance * r.Distance > Vector2.DistanceSquared(middle, corner))
                    points.Add(r.EndPoint);
            }

            /*Vector2 GetEdgeEndRight(int orgPointX, int orgPointY)
            {
                if (Engine.CurrentMap.CurrentLevel.GetOrganisation(orgPointX, orgPointY, 0) == 1)
                    return GetEdgeEndRight(orgPointX + 1, orgPointY);
                return new Vector2(orgPointX * Engine.CurrentMap.CurrentLevel.TileWidth, orgPointY * Engine.CurrentMap.CurrentLevel.TileHeight) + Engine.CurrentMap.CurrentLevel.Pos;
            }*/

            List<int[]> edges = new();

            for (int y = 0; y < Engine.CurrentMap.CurrentLevel.Organisation.GetLength(0); y++)
                for (int x = 0; x < Engine.CurrentMap.CurrentLevel.Organisation.GetLength(1); x++)
                {
                    if (Engine.CurrentMap.CurrentLevel.GetOrganisation(x, y) != 0)
                    {
                        if (Engine.CurrentMap.CurrentLevel.GetOrganisation(x - 1, y) == 0)
                            edges.Add(new int[4] { x, y, x, y + 1 } );
                        if (Engine.CurrentMap.CurrentLevel.GetOrganisation(x + 1, y) == 0)
                            edges.Add(new int[4] { x + 1, y, x + 1, y + 1 });
                        if (Engine.CurrentMap.CurrentLevel.GetOrganisation(x, y - 1) == 0)
                            edges.Add(new int[4] { x, y, x + 1, y });
                        if (Engine.CurrentMap.CurrentLevel.GetOrganisation(x, y + 1) == 0)
                            edges.Add(new int[4] { x, y + 1, x + 1, y + 1 });
                    }
                }

            /*foreach (int[] edge in edges)
            {
                Vector2 coord1 = new Vector2(edge[0], edge[1]) * 8 + Engine.CurrentMap.CurrentLevel.Pos;
                Vector2 coord2 = new Vector2(edge[2], edge[3]) * 8 + Engine.CurrentMap.CurrentLevel.Pos;

                Vector2[] collPoints = Collision.LineCircleIntersection(coord1, coord2, middle, MaxSwingDistance);
                foreach(Vector2 p in collPoints)
                {
                    if (points.Contains(p))
                        continue;

                    var r = new Raycast(Raycast.RayTypes.MapTiles, middle, p);
                    if (!r.Hit)
                        points.Add(p);
                }
            }*/

            Dictionary<Vector2, float> angles = new();
            foreach(Vector2 point in points)
                angles[point] = (middle - point).ToAngle();

            points.Sort((vec1, vec2) => Math.Sign(angles[vec1] - angles[vec2]));

            polygon = points.ToArray();
            Debug.Point(polygon);
        }

        public override void Update()
        {
            base.Update();


            Debug.PointUpdate(new Raycast(Raycast.RayTypes.MapTiles, MiddlePos, Input.MousePos).EndPoint);
            //Debug.LogUpdate(VectorHelper.GetAngle(Engine.CurrentMap.CurrentLevel.Pos, Input.MousePos));
            //Debug.LogUpdate((Engine.CurrentMap.CurrentLevel.Pos - Input.MousePos).ToAngle());
        }

        public override void Render()
        {
            base.Render();

            //Drawing.DrawCircleEdge(Pos + HalfSize, MaxSwingDistance, 0.1f, new Color(Color.Gray, 50), 1);

            if (Engine.Player != null && !swinged && !new Raycast(Raycast.RayTypes.MapTiles, Pos + HalfSize, Engine.Player.Pos + Engine.Player.HalfSize).Hit && Vector2.Distance(Pos + HalfSize, Engine.Player.Pos + Engine.Player.HalfSize) < MaxSwingDistance)
                Drawing.DrawDottedLine(Pos + HalfSize, Engine.Player.Pos + Engine.Player.HalfSize, new Color(Color.Gray, 50), 1, 4, 4);

            for (int i = 0; i < polygon.Length - 1; i++)
                Drawing.DrawLine(polygon[i], polygon[i + 1], new Color(Color.Gray, 50), 1);
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
        }
        //TODO: DRAW DOTTED LINE WHEN PLAYER IS NEAR TO SWINGING POINT
    }
}
