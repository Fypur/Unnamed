using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class CameraOffset : PlayerTrigger
    {
        private const float OffsetTransitionTime = 0.4f;
        public bool OverrideOffset = false;

        public Vector2 Offset;
        public CameraOffset(Vector2 position, Vector2 size, Vector2 offset, bool overrideOffset) : base(position, size, null)
        {
            Offset = offset;
            OverrideOffset = overrideOffset;
            Collider.DebugColor = Color.GreenYellow;
        }

        public override void OnTriggerEnter(Player player)
        {
            /*AddComponent(new Timer(OffsetTransitionTime, true, (timer) =>
            {
                Engine.Cam.Offset = Offset * Ease.QuintInAndOut(Ease.Reverse(timer.Value / timer.MaxValue));
            }, () => Engine.Cam.Offset = Offset));*/

            if(OverrideOffset)
                Engine.Cam.InBoundsOffset = Offset;
            else
                Engine.Cam.InBoundsOffset += Offset;
        }

        public override void OnTriggerExit(Player player)
        {
            /*if (Engine.Cam.Offset == Offset)
            {
                AddComponent(new Timer(OffsetTransitionTime, true, (timer) =>
                {
                    Engine.Cam.Offset = Offset * Ease.QuintInAndOut(timer.Value / timer.MaxValue);
                }, () => Engine.Cam.Offset = Vector2.Zero));
            }*/
            
            Engine.Cam.InBoundsOffset -= Offset;
        }
    }
}
