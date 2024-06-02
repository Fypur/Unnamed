using Fiourp;
using Microsoft.Xna.Framework;
using Unnamed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Engine.Cam.InBoundsOffset = Offset;
            else*/

            Engine.Cam.InBoundsOffset += Offset;
        }

        public override void OnTriggerExit(Player player)
        {
            base.OnTriggerExit(player);
            Engine.Cam.InBoundsOffset -= Offset;
        }
    }
}
