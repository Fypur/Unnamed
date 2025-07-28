using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unnamed
{
    public class ParallaxBackground : Entity
    {
        public Tile[] Tiles;
        public float[] XMoveMultipliers;

        private Vector2 OldCamSize;

        public ParallaxBackground(Sprite[] sprites, float[] parallaxMoveXMultiplier) : base(Vector2.Zero, sprites.MaxBy((s) => s.Width).Width, sprites.MaxBy((s) => s.Height).Height, null)
        {
            Tiles = new Tile[sprites.Length];
            for (int i = 0; i < Tiles.Length; i++)
                Tiles[i] = new Tile(Vector2.Zero, sprites[i].Width, sprites[i].Height, sprites[i]);

            XMoveMultipliers = parallaxMoveXMultiplier;

            Visible = false; //Render Called at a different time
        }

        public override void LateUpdate() //Late Update because we need the camera to have moved
        {
            base.LateUpdate();

            Vector2 move = Engine.Cam.PreviousPos - Engine.Cam.WholePos;// - Vector2.UnitX * (Engine.Cam.Size - OldCamSize);

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
                //Tiles[i].Sprite.Scale = new Vector2((float)Engine.Cam.Width / Tiles[i].Width); //->Engine.Camera.Width
                Tiles[i].Render();
                Tiles[i].Pos.X -= Width;
                Tiles[i].Render();
                Tiles[i].Pos.X += Width; //Render twice for loop
            }
        }
    }
}
