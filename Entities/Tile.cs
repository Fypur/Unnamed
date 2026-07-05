using Fiourp;
using Microsoft.Xna.Framework;

namespace Unnamed
{
    public class Tile : Entity
    {
        public Sprite Sprite;
        public int Width;
        public int Height;
        public Tile(Vector2 position, int width, int height, Sprite sprite) : base(position)
        {
            Width = width;
            Height = height;
            AddComponent(Sprite = sprite);
        }

        public override void Update()
        {
            base.Update();

            if (Sprite.Texture == Drawing.PointTexture)
                Sprite.DesinationRectangle = new Rectangle(0, 0, Width, Height);
        }
    }
}
