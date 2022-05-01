using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiourp;
using Microsoft.Xna.Framework;

namespace Platformer
{
    public class GlassWall : Solid
    {
        public bool DestroyOnX;

        private readonly static ParticleType glass = new ParticleType()
        {
            LifeMin = 1,
            LifeMax = 5,
            SpeedMin = 5,
            SpeedMax = 150,
            Color = Color.LightBlue,
            Size = 1,
            SizeRange = 1,
            SizeChange = ParticleType.FadeModes.None,
            FadeMode = ParticleType.FadeModes.EndLinear,
            Acceleration = new Vector2(0, 9.81f * 20)
        };
        public float BreakVelocity;

        public GlassWall(Vector2 position, int width, int height, float breakVelocity) : base(position, width, height, new Sprite(Color.LightBlue))
        {
            BreakVelocity = breakVelocity;
            DestroyOnX = Width >= Height ? false : true;
        }

        public void Break(Vector2 particleDirection)
        {
            Engine.CurrentMap.MiddlegroundSystem.Emit(glass, 200, Bounds, null, particleDirection.ToAngleDegrees(), glass.Color);
            Engine.Cam.Shake(0.2f, 1.7f);
            Destroy();
        }
    }
}
