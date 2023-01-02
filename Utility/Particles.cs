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

        public static readonly ParticleType WaterFall = new ParticleType()
        {
            Color = new Color(Color.LightSkyBlue, 100),
            Size = 1,
            SizeRange = 0,
            LifeMin = 0.5f,
            LifeMax = 4f,
            SpeedMin = 65,
            SpeedMax = 70,
            Direction = 0,
            DirectionRange = 10,
            Acceleration = Vector2.UnitY * 100,
            Friction = 0.1f,
            FadeMode = ParticleType.FadeModes.None,
            SizeChange = ParticleType.FadeModes.EndSmooth,
        };

        public static readonly ParticleType Debug = new ParticleType()
        {
            Color = Color.Red,
            Size = 5,
            SizeRange = 0,
            LifeMin = 0.5f,
            LifeMax = 4f,
            SpeedMin = 65,
            SpeedMax = 70,
            Direction = 0,
            DirectionRange = 360,
            Acceleration = Vector2.Zero,
            Friction = 0.1f,
            FadeMode = ParticleType.FadeModes.EndLinear,
            SizeChange = ParticleType.FadeModes.EndSmooth,
        };

        public static readonly ParticleType Jetpack = new ParticleType()
        {
            LifeMin = 0.2f,
            LifeMax = 0.4f,
            Color = Color.Orange,
            Color2 = Color.Yellow,
            Size = 4,
            SizeRange = 3,
            SizeChange = ParticleType.FadeModes.Linear,
            Direction = 180,
            SpeedMin = 2,
            SpeedMax = 5,
            CustomRender = (p) => Drawing.DrawCircle(p.Pos, p.Size.X / p.StartSize * p.StartSize * 4, 1, new Color(p.Color, 20), Color.Transparent),
        };

        public static readonly ParticleType Explosion = new ParticleType()
        {
            LifeMin = 0.2f,
            LifeMax = 1,
            Color = Color.White,
            Color2 = Color.Orange,
            Size = 3,
            SizeRange = 2,
            SizeChange = ParticleType.FadeModes.Linear,
            Direction = 0,
            DirectionRange = 360,
            SpeedMin = 4,
            SpeedMax = 100,
            Acceleration = Vector2.UnitY * 10,
            CustomRender = (p) => Drawing.DrawCircle(p.Pos, p.Size.X / p.StartSize * p.StartSize * 10, 1, new Color(p.Color, 13), Color.Transparent),
        };

        public static ParticleType Spark = new ParticleType()
        {
            LifeMin = 0.4f,
            LifeMax = 0.7f,
            Color = Color.Yellow,
            FadeMode = ParticleType.FadeModes.Linear,
            Size = 1,
            SizeRange = 0,
            //SizeChange = ParticleType.FadeModes.EndLinear,
            Acceleration = Vector2.UnitY * 300,
            SpeedMin = 20,
            SpeedMax = 80,
            DirectionRange = 90,
            Direction = -90,
        };
    }
}
