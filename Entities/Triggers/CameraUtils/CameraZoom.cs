using Fiourp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Unnamed;

namespace Unnamed
{
    public class CameraZoom : PlayerTrigger
    {
        public int TargetCamWidth;
        public float ZoomTime;

        public CameraZoom(Vector2 position, Vector2 size, int targetCamWidth, float zoomTime) : base(position, size, null)
        {
            TargetCamWidth = targetCamWidth;
            ZoomTime = zoomTime;
        }

        public CameraZoom(Vector2 position, int width, int height, int targetCamWidth, float zoomTime) : base(position, width, height, null)
        {
            TargetCamWidth = targetCamWidth;
            ZoomTime = zoomTime;
        }

        public override void OnTriggerEnter(Player player)
        {
            base.OnTriggerEnter(player);

            KillAllCameraZoomTimers();

            float initWidth = Engine.Cam.Width;

            AddComponent(new Timer(ZoomTime, true,
                (timer) => Zoom((int)MathHelper.Lerp(initWidth, TargetCamWidth, Ease.CubeInAndOut(Ease.Reverse(timer.Value / timer.MaxValue)))),
                () => Zoom(TargetCamWidth)));
        }

        public override void OnTriggerExit(Player player)
        {
            base.OnTriggerExit(player);

            KillAllCameraZoomTimers();

            int initWidth = Engine.Cam.Width;
            int targWidth = 480;

            AddComponent(new Timer(ZoomTime, true,
                (timer) => Zoom((int)MathHelper.Lerp(initWidth, targWidth, Ease.CubeInAndOut(Ease.Reverse(timer.Value / timer.MaxValue)))),
                () => Zoom(targWidth)));
        }

        private void Zoom(int camWidth)
        {
            int camHeight = (int)(9 * (float)camWidth / 16);
            Engine.Cam.Pos += new Vector2(Engine.Cam.Width - camWidth, Engine.Cam.Height - camHeight) / 2;
            Engine.Cam.Size = new Vector2(camWidth, camHeight);
        }

        private void KillAllCameraZoomTimers()
        {
            foreach(CameraZoom camZoom in Engine.CurrentMap.Data.EntitiesByType[typeof(CameraZoom)])
                camZoom.RemoveComponents<Timer>();
        }
    }
}
