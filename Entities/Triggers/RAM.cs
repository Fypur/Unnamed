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
    public class RAM : Collectable
    {
        private static ParticleType WaitingParticle = new ParticleType()
        {
            LifeMin = 0.4f,
            LifeMax = 1.2f,
            SpeedMin = 4,
            SpeedMax = 70,
            Size = 2,
            SizeRange = 1f,
            FadeMode = ParticleType.FadeModes.Linear,
            SizeChange = ParticleType.FadeModes.Linear,
            Color = Color.White,
            Color2 = new Color(17, 52, 21),
            Direction = 0,
            DirectionRange = 360,
        };

        private static ParticleType CollectedParticle = new ParticleType()
        {
            LifeMin = 0.4f,
            LifeMax = 6f,
            SpeedMin = 4,
            SpeedMax = 70,
            Size = 3,
            SizeRange = 2,
            FadeMode = ParticleType.FadeModes.Linear,
            SizeChange = ParticleType.FadeModes.Linear,
            Color = Color.White,
            Color2 = new Color(17, 52, 21),
            Direction = 0,
            DirectionRange = 360,
        };

        private float emittedAmount;

        public RAM(Vector2 position, Guid iid) : base(position, 10, 15, iid, new Sprite())
        {
            Sprite.Add(Sprite.AllAnimData["RAM"]);
            Sprite.Offset.Y += 1;
            Sprite.Play("rotate");

            AddComponent(new CircleLight(HalfSize, 30, new Color(Color.Orange, 50), new Color(Color.White, 0)));
        }

        public override void WhileWait(Player player)
        {
            Sprite.Color = Color.Lerp(Color.White, Color.Black, player.SafePercentage);
            if(player.SafePercentage > 0.2f)
                emittedAmount += player.SafePercentage;
            Engine.CurrentMap.MiddlegroundSystem.Emit(WaitingParticle, MiddlePos,  (int)emittedAmount);
            emittedAmount -= (int)emittedAmount;
        }

        public override void OnCollected(Player player)
        {
            AddComponent(new Timer(0.2f, true, (timer) =>
            {
                Sprite.Scale.Y = timer.Value / timer.MaxValue;
            }, () => {

                Engine.Cam.LightShake();
                Engine.CurrentMap.MiddlegroundSystem.Emit(CollectedParticle, Pos + HalfSize.OnlyX(), 30);
                SelfDestroy();

            }));
        }

        protected override bool CollectingConditions() => ((Player)Engine.Player).Safe;
    }
}
