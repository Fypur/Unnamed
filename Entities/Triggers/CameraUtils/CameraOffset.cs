using Fiourp;
using Microsoft.Xna.Framework;

namespace Unnamed
{
    public class CameraOffset : PlayerTrigger
    {
        public bool OverrideOffset = false;

        public Vector2 Offset;
        public CameraOffset(Vector2 position, Vector2 size, Vector2 offset, bool overrideOffset) : base(position, size, null)
        {
            Offset = offset;
            //OverrideOffset = overrideOffset;
            Collider.DebugColor = Color.GreenYellow;
        }

        public override void OnTriggerEnter(Player player)
        {
            base.OnTriggerEnter(player);

            /*if(OverrideOffset)
                Platformer.GameCam.InBoundsOffset = Offset;
            else*/

            Platformer.GameCam.InBoundsOffset += Offset;
        }

        public override void OnTriggerExit(Player player)
        {
            base.OnTriggerExit(player);
            Platformer.GameCam.InBoundsOffset -= Offset;
        }
    }
}
