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

            float initWidth = Engine.Cam.Width;

            AddComponent(new Timer(ZoomTime, true,
                (timer) => Zoom((int)MathHelper.Lerp(initWidth, TargetCamWidth, Ease.CubeInAndOut(Ease.Reverse(timer.Value / timer.MaxValue)))),
                () => Zoom(TargetCamWidth)));
        }

        public override void OnTriggerExit(Player player)
        {
            base.OnTriggerExit(player);

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
            //Engine.Cam.Follow(Engine.Player, 3, 3, Rectangle.Empty);
            Engine.Cam.Size = new Vector2(camWidth, camHeight);

            Engine.RenderTarget = new RenderTarget2D(Platformer.GraphicsManager.GraphicsDevice, camWidth, camHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            Engine.PrimitivesRenderTarget = new RenderTarget2D(Platformer.GraphicsManager.GraphicsDevice, camWidth, camHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            Platformer.SecondRenderTarget = new RenderTarget2D(Platformer.GraphicsManager.GraphicsDevice, camWidth, camHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        }
    }
}
