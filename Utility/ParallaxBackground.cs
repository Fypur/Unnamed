using Fiourp;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Unnamed
{
    public class ParallaxBackground : Entity
    {
        public int Width;
        public int Height;
        public Tile[] Tiles;
        public float[] XMoveMultipliers;

        private Vector2 OldCamSize;


        public ParallaxBackground(Sprite[] sprites, float[] parallaxMoveXMultiplier) : base(Vector2.Zero)
        {
            Width = sprites.MaxBy((s) => s.Width).Width;
            Height = sprites.MaxBy((s) => s.Height).Height;

            Tiles = new Tile[sprites.Length];
            for (int i = 0; i < Tiles.Length; i++)
                Tiles[i] = new Tile(Vector2.Zero, sprites[i].Width, sprites[i].Height, sprites[i]);

            XMoveMultipliers = parallaxMoveXMultiplier;

            Visible = false; //Render Called at a different time
        }

        public override void LateUpdate() //Late Update because we need the camera to have moved
        {
            base.LateUpdate();

            Vector2 move = Platformer.GameCam.PreviousPos - Platformer.GameCam.Pos;// - Vector2.UnitX * (Platformer.GameCam.Size - OldCamSize);

            for (int i = 0; i < Tiles.Length; i++)
            {
                Tiles[i].Pos.X += move.X * XMoveMultipliers[i];

                while (Tiles[i].Pos.X < 0)
                    Tiles[i].Pos.X += Width;

                Tiles[i].Pos.X %= Width;
            }
        }

        public override void Render()
        {
            base.Render();

            for (int i = 0; i < Tiles.Length; i++)
            {
                //Tiles[i].Sprite.Scale = new Vector2((float)Platformer.GameCam.Width / Tiles[i].Width); //->Platformer.GameCamera.Width
                Tiles[i].Render();
                Tiles[i].Pos.X -= Width;
                Tiles[i].Render();
                Tiles[i].Pos.X += Width; //Render twice for loop
            }
        }
    }
}
