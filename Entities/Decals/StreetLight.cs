using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class StreetLight : Decoration
    {
        private Light light;
        private static ParticleType Spark = new ParticleType()
        {
            LifeMin = 0.4f,
            LifeMax = 10,
            Color = Color.Yellow,
            FadeMode = ParticleType.FadeModes.None,
            Size = 1,
            SizeRange = 0,
            SizeChange = ParticleType.FadeModes.EndLinear,
            Acceleration = Vector2.UnitY * 300,
            SpeedMin = 20,
            SpeedMax = 120,
            DirectionRange = 90,
            Direction = -90,
        };

        public StreetLight(Vector2 position, int width, int height, Rectangle? turnOffRect = null) : base(position, width, height, new Sprite(Color.White))
        {
            Sprite.Add(Sprite.AllAnimData["StreetLight"]);
            Sprite.Play("light");

            light = (Light)AddComponent(
                new Light(Sprite.CurrentAnimation.Slices[0].Rect.Location.ToVector2(), 60, new Color(Color.White, 30), Color.Transparent));
           
            if(turnOffRect is Rectangle r)
            {
                //TriggerComponent trig = (TriggerComponent)AddComponent(new TriggerComponent(r.Location.ToVector2() - Pos, r.Width, r.Height, new List<Type>() { typeof(Player) }));
                Trigger v = (Trigger)AddChild(new Trigger(r, new List<Type>() { typeof(Player) }, null));
                v.OnTriggerEnterAction = (entity) => {
                    Sprite.Play("blink");
                    RemoveChild(v);
                    Sprite.OnChange =
                    () =>
                    {
                        Engine.CurrentMap.MiddlegroundSystem.Emit(Spark, Pos + light.LocalPosition, 20);
                        Sprite.OnChange = null;
                    };
                };
            }
        }

        public override void Update()
        {
            base.Update();

            Debug.LogUpdate(Engine.CurrentMap.MiddlegroundSystem.Particles.Count);

            if(Sprite.CurrentAnimationFrame.Tag is string tag)
            {
                if (tag.StartsWith("on"))
                    light.Visible = true;
                else if (tag.StartsWith("off"))
                    light.Visible = false;
            }

            if(Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
                Engine.CurrentMap.MiddlegroundSystem.Emit(Spark, Pos + light.LocalPosition, 20);

            Spark = new ParticleType()
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
}
