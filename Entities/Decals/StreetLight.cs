using Fiourp;
using FMOD.Studio;
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
        private static ParticleType Spark = new ParticleType()
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

        private Light light;
        private EventInstance lightSound;

        public StreetLight(Vector2 position, int width, int height, Rectangle? turnOffRect = null) : base(position, width, height, new Sprite(Color.White))
        {
            Sprite.Add(Sprite.AllAnimData["StreetLight"]);
            Sprite.Play("light");

            light = (Light)AddComponent(
                new Light(Sprite.CurrentAnimation.Slices[0].Rect.Location.ToVector2(), 60, new Color(Color.White, 50), new Color(Color.White, 0)));
           
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

            lightSound = Audio.PlayEvent("StreetLight");
        }

        public override void Update()
        {
            base.Update();

            if(Engine.Player != null)
            {
                Debug.LogUpdate(Vector2.Distance(Engine.Player.Pos, Pos + light.LocalPosition) * Math.Sign(Engine.Player.Pos.X - Pos.X - light.LocalPosition.X) / 250);
                lightSound.setParameterByName("Distance", Vector2.Distance(Engine.Player.Pos, Pos + light.LocalPosition) * Math.Sign(Engine.Player.Pos.X - Pos.X - light.LocalPosition.X) / 120);
                //lightSound.setParameterByName("DistanceX", (Engine.Player.Pos.X - Pos.X - light.LocalPosition.X) / 120);
            }

            if(Sprite.CurrentAnimationFrame.Tag is string tag)
            {
                if (tag.StartsWith("on"))
                {
                    light.Visible = true;
                    lightSound.setVolume(1);
                }
                else if (tag.StartsWith("off"))
                {
                    light.Visible = false;
                    lightSound.setVolume(0);
                }
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            Audio.StopEvent(lightSound);
        }
    }
}
