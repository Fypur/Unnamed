using Fiourp;
using Microsoft.Xna.Framework;

namespace Unnamed
{
    public class ProjectorLight : Tile
    {
        public enum ProjectorType { Ground, Corner, Ceiling };

        public Vector2 Direction;
        private Rectangle lightRect;
        public QuadPointLight QuadLight;

        public ProjectorLight(Vector2 position, Vector2 directionPoint, float range, Color color, ProjectorType type) : base(position, 10, 10, new Sprite(Color.White))
        {
            Sprite.Add(Sprite.AllAnimData["ProjectorLight"]);
            Sprite.Play(type.ToString());
            lightRect = Sprite.CurrentAnimation.Slices[0].Rect;
            //Sprite.CurrentAnimation.Slices[0].

            //QuadLight = (QuadPointLight)AddComponent(new QuadPointLight(lightRect.Location.ToVector2(), (lightRect.Location + lightRect.Size).ToVector2() - Vector2.UnitY, directionPoint, 170, new Color(Color.LightYellow, 90), new Color(color, 0), range));
        }

        public override void Awake()
        {
            base.Awake();

            QuadLight.Direction -= Pos;
            QuadLight.Direction -= new Vector2(LevelManager.CurrentGrid.GridCollider.TileWidth, LevelManager.CurrentGrid.GridCollider.TileHeight) / 2;
            //Direction -= Sprite.CurrentAnimation.Slices[0].Rect.Center.ToVector2();
            QuadLight.Direction.Normalize();
        }
    }
}
