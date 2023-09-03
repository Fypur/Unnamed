using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class JetpackPickUp : Entity
    {
        private PlayerTriggerComponent trig;
        private bool inside;
        private bool collected;
        private int id;

        private Guid iid;

        ParticleType lightStuff = new()
        {
            LifeMin = 0.5f,
            LifeMax = 4f,
            SpeedMin = 1,

            SpeedMax = 50,
            SizeChange = ParticleType.FadeModes.EndSmooth,
            Color = Color.White,
            DirectionRange = 360,
        };

        public JetpackPickUp(Vector2 position, Guid iid, int id) : base(position, 1, 1, new Sprite(Color.White))
        {
            AddComponent(new CircleLight(Vector2.Zero, 4, new Color(Color.White, 100), new Color(Color.White, 0)));
            trig = (PlayerTriggerComponent)AddComponent(new PlayerTriggerComponent(-Vector2.One * 3, Collider));

            this.iid = iid;
            this.id = id;

            trig.Trigger.Width = 8;
            trig.Trigger.Height = 8;
            trig.Trigger.OnTriggerEnterAction = (e) => inside = true;
            trig.Trigger.OnTriggerExitAction = (e) => inside = false;
        }

        public override void Awake()
        {
            base.Awake();

            if (id == 1)
            {
                Engine.CurrentMap.Data.GetEntity<TextSpawn>().Text = "Press X to pick up object";

                /*Engine.CurrentMap.Data.GetEntity<TextSpawn>().Active = false;
                Engine.CurrentMap.Data.GetEntity<TextSpawn>().Visible = false;*/
            }
        }

        public override void Update()
        {
            base.Update();

            if (inside && !collected && Player.JetpackControls.IsDown())
            {
                collected = true;
                AddComponent(new Coroutine(Collect()));
            }
        }

        public IEnumerator Collect()
        {
            Platformer.player.CanMove = false;
            Platformer.player.CanAnimateSprite = true;

            Platformer.player.Sprite.Play("pickUp");

            yield return new Coroutine.WaitForSeconds(1f);

            Engine.CurrentMap.MiddlegroundSystem.Emit(lightStuff, Pos, 25);
            RemoveComponent(GetComponent<Light>());

            Platformer.player.CanJetpack = true;
            Platformer.player.Sprite.OnLastFrame = null;

            AddComponent(new Timer(0.5f, true, (t) =>
            {
                Platformer.player.Sprite.PixelShader = DataManager.PixelShaders["WhiteBar"];
                DataManager.PixelShaders["WhiteBar"].Parameters["barLocation"].SetValue(Ease.CubeInAndOut(t.AmountCompleted()));
            }, () => Platformer.player.Sprite.PixelShader = null));

            yield return new Coroutine.WaitForSeconds(1f);

            //Show Jetpack Picked Up UI
            TextBox s = (TextBox)AddChild(new TextBox("Jetpack has been acquired", "LexendDeca", Engine.CurrentMap.CurrentLevel.Pos + Engine.Cam.Size / 2 - new Vector2(0, 0), 800, 20, 1, Color.Transparent, true, TextBox.Alignement.Center));
            
            for(float t = 0; t < 0.5f; t += Engine.Deltatime)
            {
                s.Color = new Color(Color.White, t / 0.5f);
                yield return null;
            }

            s.Color = Color.White;

            //yield return new Coroutine.PausedUntil(Player.JumpControls.Is);
            yield return new Coroutine.WaitForSeconds(2.5f);

            Levels.LevelNonRespawn.Add(iid);

            Platformer.player.CanMove = true;
            Platformer.player.CanAnimateSprite = false;
            Platformer.player.Sprite.Play("pickUpReverse");

            for (float t = 0.5f; t > 0; t -= Engine.Deltatime)
            {
                s.Color = new Color(Color.White, t / 0.5f);
                Sprite.Color = new Color(Color.White, t / 0.5f);
                yield return null;
            }

            Sprite.Active = false;
            Sprite.Visible = false;

            if(id == 1)
            {
                Engine.CurrentMap.Data.GetEntity<TextSpawn>().Text = "You Just a got a jetpack!\nHold X or Square and the 4\narrow keys to use it";
                Engine.CurrentMap.Data.GetEntity<TextSpawn>().TextBox.ProgressiveRemove(0.01f);
            }

            s.SelfDestroy();

            yield return new Coroutine.PausedUntil(() => Engine.CurrentMap.Data.GetEntity<TextSpawn>().TextBox.Text == "");

            Engine.CurrentMap.Data.GetEntity<TextSpawn>().TextBox.ProgressiveDraw("You Just a got a jetpack!\nHold X or Square and the 4\narrow keys to use it", 0.01f, true);

            SelfDestroy();
        }
    }
}
