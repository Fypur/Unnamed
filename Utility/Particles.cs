using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public static class Particles
    {
        public static readonly ParticleType Dust = new ParticleType()
        {
            Color = Color.White,
            Size = 2,
            SizeRange = 1,
            LifeMin = 0.05f,
            LifeMax = 0.4f,
            SpeedMin = 5,
            SpeedMax = 30,
            Direction = -90,
            DirectionRange = 45,
            FadeMode = ParticleType.FadeModes.EndLinear,
            SizeChange = ParticleType.FadeModes.EndSmooth
        };
    }
}
