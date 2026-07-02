using Fiourp;
using Microsoft.Xna.Framework;

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

            float initWidth = Platformer.GameCam.Camera.Width;

            AddComponent(new Timer(ZoomTime,
                (timer) => Zoom((int)MathHelper.Lerp(initWidth, TargetCamWidth, Ease.CubeInAndOut(Ease.Reverse(timer.Value / timer.MaxValue)))),
                () => Zoom(TargetCamWidth)));
        }

        public override void OnTriggerExit(Player player)
        {
            base.OnTriggerExit(player);

            KillAllCameraZoomTimers();

            int initWidth = Platformer.GameCam.Camera.Width;
            int targWidth = 480;

            AddComponent(new Timer(ZoomTime,
                (timer) => Zoom((int)MathHelper.Lerp(initWidth, targWidth, Ease.CubeInAndOut(Ease.Reverse(timer.Value / timer.MaxValue)))),
                () => Zoom(targWidth)));
        }

        private void Zoom(int camWidth)
        {
            int camHeight = (int)(9 * (float)camWidth / 16);
            Engine.Cam.Pos += new Vector2(Platformer.GameCam.Camera.Width - camWidth, Platformer.GameCam.Camera.Height - camHeight) / 2;
            Engine.Cam.Size = new Vector2(camWidth, camHeight);
        }

        private void KillAllCameraZoomTimers()
        {
            foreach (CameraZoom camZoom in Engine.CurrentMap.Data.EntitiesByType[typeof(CameraZoom)])
                camZoom.RemoveAllComponents<Timer>();
        }
    }
}
