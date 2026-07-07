using Fiourp;
using Microsoft.Xna.Framework;
using System;

namespace Unnamed
{
    public class Grid : Kinematic
    {
        public GridCollider GridCollider;

        public Sprite[,] Tiles;

        private new GridCollider Collider => base.Collider as GridCollider;

        public Grid(Vector2 position, GridCollider gridCollider, Sprite[,] tileSprites) : base(position, gridCollider, null)
        {
            GridCollider = gridCollider;
            Tiles = tileSprites;
            AddComponent(GridCollider);
        }
        public override void Render()
        {
            base.Render();

            if (Drawing.GetCurrentPixelShader() != null)
                Drawing.SwitchPixelShader(null);

            Vector2 startPos = (Platformer.GameCam.Pos - Platformer.GameCam.HalfSize - Pos) / GridCollider.GridSize;

            if (startPos.X > this.GridCollider.Width || startPos.Y > this.GridCollider.Height || startPos.X + Platformer.GameCam.Width < 0 || startPos.Y + Platformer.GameCam.Height < 0)
                return;



            Vector2 size = new Vector2((float)Platformer.GameCam.Width / GridCollider.TileWidth, (float)Platformer.GameCam.Height / GridCollider.TileHeight);

            for (int x = Math.Max((int)startPos.X, 0); x < Math.Min(startPos.X + size.X, Tiles.GetLength(1)); x++)
            {
                for (int y = Math.Max((int)startPos.Y, 0); y < Math.Min(startPos.Y + size.Y, Tiles.GetLength(0)); y++)
                {
                    Vector2 pos = new Vector2(x * GridCollider.TileWidth, y * GridCollider.TileHeight) + Collider.WorldPos;
                    if (Tiles[y, x] != Sprite.None && Tiles[y, x] != null)
                        Tiles[y, x].Draw(pos);
                    if (Debug.DebugMode && GridCollider.GridLayout[y, x])
                        Drawing.DrawEdge(new Rectangle(pos.ToPoint(), new Point(GridCollider.TileWidth, GridCollider.TileHeight)), 1, Color.Blue);
                    /*if (Organisation[y, x] != 0 && (Tiles[y, x] == null || Tiles[y, x] == Sprite.None))
                        Debug.LogUpdate(Organisation[y, x], Tiles[y, x], pos);
                    */
                }
            }
        }
    }
}