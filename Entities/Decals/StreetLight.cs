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
        private Light light;
        private Sound3D sound;

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
                        Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Spark, Pos + light.LocalPosition, 20);
                        Sprite.OnChange = null;
                    };
                };
            }

            sound = (Sound3D)AddComponent(new Sound3D("StreetLight"));
        }

        public override void Update()
        {
            //lightSound = Audio.PlayEvent("StreetLight2");
            base.Update();

            if(Sprite.CurrentAnimationFrame.Tag is string tag)
            {
                if (tag.StartsWith("on"))
                {
                    light.Visible = true;
                    sound.Sound.setVolume(1);
                }
                else if (tag.StartsWith("off"))
                {
                    light.Visible = false;
                    sound.Sound.setVolume(0);
                }
            }
        }
    }
}
