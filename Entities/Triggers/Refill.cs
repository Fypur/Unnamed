using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiourp;

namespace Platformer
{
    public class Refill : PlayerTrigger
    {
        private static readonly ParticleType explosion = new ParticleType()
        {
            LifeMin = 0.2f,
            LifeMax = 0.6f,
            SpeedMin = 10f,
            SpeedMax = 40f,
            Direction = 0,
            DirectionRange = 360,
            Size = 2,
            SizeRange = 2,
            SizeChange = ParticleType.FadeModes.Linear,
            Color = Color.Red,
            Color2 = Color.Orange,
        };
        private const float size = 10;
        public float RespawnTime = 4f;

        private bool canActivate = true;

        public Refill(Vector2 position, float respawnTime) : base(position, new Vector2(size), new Sprite()) 
        {
            Sprite.Add(Sprite.AllAnimData["Refill"]);
            Sprite.Play("rotate");
            //Sprite.Scale = Vector2.One * 10;
            RespawnTime = respawnTime;
        }

        public override void OnTriggerEnter(Player player)
        {
            if (!canActivate)
                return;

            player.RefillJetpack();
            canActivate = false;

            Engine.CurrentMap.MiddlegroundSystem.Emit(explosion, MiddlePos, 90);
            Engine.Cam.LightShake();
            //Remove this once you add animations
            Visible = false;

            //Again remove Visible = true
            AddComponent(new Timer(RespawnTime, true, null, () => { canActivate = true; Active = true; Visible = true; }));
        }
    }
}
