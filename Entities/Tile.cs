using Fiourp;
using Microsoft.Xna.Framework;

namespace Unnamed
{
    public class Tile : Entity
    {
        public Sprite Sprite;
        public Tile(Vector2 position, int width, int height, Sprite sprite) : base(position)
        {
            AddComponent(Sprite = sprite);
        }
    }
}
