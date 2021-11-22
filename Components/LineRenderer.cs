using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Components
{
    public class LineRenderer : Renderer
    {
        public Vector2 StartPos;
        public Vector2 EndPos;
        public int Width;
        public Color Color;

        private Action<LineRenderer> updateAction;
        private Action<LineRenderer> renderAction;

        public LineRenderer(Vector2 start, Vector2 end, int width, Color lineColor,
            Action<LineRenderer> UpdateAction = null, Action<LineRenderer> RenderAction = null)
        {
            StartPos = start;
            EndPos = end;
            Width = width;
            Color = lineColor;
            updateAction = UpdateAction;
            renderAction = RenderAction;
        }

        public override void Update()
        {
            updateAction?.Invoke(this);
        }

        public override void Render()
        {
            renderAction?.Invoke(this);
            Drawing.DrawLine(StartPos, EndPos, Color, Width);
        }
    }
}
