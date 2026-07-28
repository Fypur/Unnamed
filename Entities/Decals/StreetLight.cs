using Fiourp;
using Microsoft.Xna.Framework;

namespace Unnamed
{
    public class StreetLight : Entity
    {
        private Sprite Sprite;
        private CircleLight light;
        private Sound3D sound;

        private Rectangle? turnOffRect;

        public StreetLight(Vector2 position, Rectangle? turnOffRect = null) : base(position)
        {
            Sprite = new Sprite(Color.White);
            AddComponent(Sprite);

            Sprite.Add(Sprite.AllAnimData["StreetLight"]);
            Sprite.Play("light");

            light = (CircleLight)AddComponent(new CircleLight(Sprite.CurrentAnimation.Slices[0].Rect.Location.ToVector2(), 60, new Color(Color.White, 50), new Color(Color.White, 0)));

            this.turnOffRect = turnOffRect;
        }

        public override void Awake()
        {
            base.Awake();

            if (turnOffRect is Rectangle r)
            {
                SpecialTrigger v = new SpecialTrigger(r.Location.ToVector2(), r.Size.ToVector2(), null);
                v.OnTriggerEnterAction = (entity) =>
                {
                    Sprite.Play("blink");
                    v.SelfDestroy();

                    Sprite.OnChange =
                    () =>
                    {
                        Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Spark, Pos + light.LocalPosition, 20);
                        Sprite.OnChange = null;
                    };
                };

                Engine.CurrentMap.Instantiate(v);
                LevelManager.CurrentLevel.DestroyOnUnload(v);
            }

            sound = (Sound3D)AddComponent(new Sound3D("Ambience/StreetLight"));
        }

        public override void Update()
        {
            base.Update();

            if (Sprite.CurrentAnimationFrame.Tag is string tag)
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
