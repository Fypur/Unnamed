using Fiourp;
using Microsoft.Xna.Framework;

namespace Unnamed
{
    public class CameraLock : PlayerTrigger
    {
        public CameraLock(Rectangle triggerRect) : base(triggerRect, Sprite.None)
        {
            Collider.DebugColor = Color.RosyBrown;
        }

        public CameraLock(Vector2 position, Vector2 size) : base(position, size, Sprite.None)
        {
            Collider.DebugColor = Color.RosyBrown;
        }

        public CameraLock(Vector2 position, int width, int height) : base(position, width, height, Sprite.None)
        {
            Collider.DebugColor = Color.RosyBrown;
        }

        public override void OnTriggerStay(Player player)
        {
            Platformer.GameCam.Locked = true;
            Platformer.GameCam.Pos = Platformer.GameCam.FollowedPos(4.5f, 4.5f, new Rectangle(new Vector2(-Engine.ScreenSize.X / 6, -Engine.ScreenSize.Y / 12).ToPoint(), new Vector2(Engine.ScreenSize.X / 3, Engine.ScreenSize.Y / 6).ToPoint()), Collider.Bounds);
        }

        public override void OnTriggerExit(Player player)
        {
            base.OnTriggerExit(player);

            Platformer.GameCam.Locked = false;
        }
    }
}
