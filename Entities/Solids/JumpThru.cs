using Fiourp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class JumpThru : Platform
    {
        private static Dictionary<string, Dictionary<string, Texture2D>> CroppedTextures = new();
        private static Dictionary<string, List<Texture2D>> CroppedMiddleTextures = new();
        public JumpThru(Vector2 position, int width, int height, string textureId) : base(position, width, height, new Sprite())
        {
            Texture2D texture = DataManager.Objects["jumpthrus/" + textureId];
            Vector2 size = new Vector2(8, 8);
            Layer = 2;

            if (!CroppedTextures.ContainsKey(textureId))
            {
                CroppedTextures[textureId] = new Dictionary<string, Texture2D>
                {
                    { "wallLeft", texture.CropTo(Vector2.Zero, size) },
                    { "endLeft", texture.CropTo(Vector2.UnitY * 8, size) },
                    { "wallRight", texture.CropTo(new Vector2(texture.Width - 8, 0), size) },
                    { "endRight", texture.CropTo(new Vector2(texture.Width - 8, 8), size) },
                };
            }

            if (!CroppedMiddleTextures.ContainsKey(textureId))
            {
                CroppedMiddleTextures[textureId] = new List<Texture2D>();
                for (int x = 0; x < texture.Width / 8 - 2; x++)
                    for (int y = 0; y < texture.Height / 8; y++)
                        CroppedMiddleTextures[textureId].Add(texture.CropTo(new Vector2(x + 1, y) * 8, size));
            }

            bool onWallRight = false, onWallLeft = false;
            if(Engine.CurrentMap.Data.EntitiesByType.TryGetValue(typeof(Grid), out List<Entity> grids))
                foreach(Grid grid in grids)
                {
                    if (grid.Collider.Collide(Pos - Vector2.UnitX)) onWallLeft = true;
                    if (grid.Collider.Collide(Pos + new Vector2(Width, 0) + Vector2.UnitX)) onWallRight = true;
                }

            Sprite.NineSliceSettings = new NineSliceRandom((int)(Pos.X + Pos.Y))
            {
                TopLeft = onWallLeft ? new List<Texture2D>() { CroppedTextures[textureId]["wallLeft"] } : new List<Texture2D>() { CroppedTextures[textureId]["endLeft"] },

                Top = CroppedMiddleTextures[textureId],

                TopRight = onWallRight ? new List<Texture2D>() { CroppedTextures[textureId]["wallRight"] } : new List<Texture2D>() { CroppedTextures[textureId]["endRight"] },
                Repeat = true,
            };
        }

        public override bool CollidingConditions(Collider other)
        {
            if (other.ParentEntity is JumpThru && other.AbsoluteTop != Collider.AbsoluteBottom - 1)
                return false;

            /*Debug.PointUpdate(new Vector2(Collider.AbsolutePosition.X, Collider.AbsoluteTop), new Vector2(other.AbsolutePosition.X, other.AbsoluteBottom));

            Debug.LogUpdate("jumpthru: " + new Vector2(Collider.AbsolutePosition.X, Collider.AbsoluteTop), "other: " + new Vector2(other.AbsolutePosition.X, other.AbsoluteBottom));*/

            if (other.ParentEntity is Actor actor)
            {
                if (actor.Velocity.Y < 0)
                    return false;
            }

            if (other.ParentEntity is MovingSolid solid && solid.Velocity.Y < 0)
                return false;

            if (Collider.AbsoluteTop != other.AbsoluteBottom - 1)
                return false;

            

            return true;
        }
    }
}
