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
        PlayerTriggerComponent trig;
        bool inside;
        bool collected;

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

        public JetpackPickUp(Vector2 position) : base(position, 1, 1, new Sprite(Color.White))
        {
            AddComponent(new CircleLight(Vector2.Zero, 4, new Color(Color.White, 100), new Color(Color.White, 0)));
            trig = (PlayerTriggerComponent)AddComponent(new PlayerTriggerComponent(-Vector2.One * 3, Collider));

            trig.Trigger.Width = 8;
            trig.Trigger.Height = 8;
            trig.Trigger.OnTriggerEnterAction = (e) => inside = true;
            trig.Trigger.OnTriggerExitAction = (e) => inside = false;
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
            Platformer.player.CanJetpack = true;
            Platformer.player.Sprite.OnLastFrame = null;

            AddComponent(new Timer(0.5f, true, (t) =>
            {
                Platformer.player.Sprite.PixelShader = DataManager.PixelShaders["WhiteBar"];
                DataManager.PixelShaders["WhiteBar"].Parameters["barLocation"].SetValue(Ease.CubeInAndOut(t.AmountCompleted()));
            }, () => Platformer.player.Sprite.PixelShader = null));

            yield return new Coroutine.WaitForSeconds(1f);

            //Show Jetpack Picked Up UI
            //var s = Engine.CurrentMap.Instantiate(new )


            yield return new Coroutine.PausedUntil(Player.JumpControls.Is);

            Platformer.player.CanMove = true;
            Platformer.player.CanAnimateSprite = false;
            Platformer.player.Sprite.Play("pickUpReverse");
            SelfDestroy();
        }
    }
}
