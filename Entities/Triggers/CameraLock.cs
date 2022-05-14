using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiourp;
using Microsoft.Xna.Framework;

namespace Platformer
{
    public class CameraLock : PlayerTrigger
    {
        public CameraLock(Rectangle triggerRect) : base(triggerRect, Sprite.None)
        {
        }

        public CameraLock(Vector2 position, Vector2 size) : base(position, size, Sprite.None)
        {
        }

        public CameraLock(Vector2 position, int width, int height) : base(position, width, height, Sprite.None)
        {
        }

        public override void OnTriggerStay(Player player)
        {
            Engine.Cam.Locked = true;
            Engine.Cam.Pos = Engine.Cam.FollowedPos(Engine.Player, 4.5f, 4.5f, new Rectangle(new Vector2(-Engine.ScreenSize.X / 6, -Engine.ScreenSize.Y / 12).ToPoint(), new Vector2(Engine.ScreenSize.X / 3, Engine.ScreenSize.Y / 6).ToPoint()), Bounds);
        }

        public override void OnTriggerExit(Player player)
        {
            Engine.Cam.Locked = false;
        }

        public Vector2 LockedPos()
            => Engine.Cam.FollowedPos(Engine.Player, 4.5f, 4.5f, new Rectangle(new Vector2(-Engine.ScreenSize.X / 6, -Engine.ScreenSize.Y / 12).ToPoint(), new Vector2(Engine.ScreenSize.X / 3, Engine.ScreenSize.Y / 6).ToPoint()), Bounds);
    }
}
