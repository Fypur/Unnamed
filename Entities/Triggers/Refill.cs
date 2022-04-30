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
        private const float size = 10;
        public float RespawnTime = 4f;

        private bool canActivate = true;

        public Refill(Vector2 position, float respawnTime) : base(position, new Vector2(size), new Sprite(Color.OrangeRed)) 
        {
            RespawnTime = respawnTime;
        }

        public override void OnTriggerEnter(Player player)
        {
            if (!canActivate)
                return;

            player.ResetJetpack();
            canActivate = false;

            //Remove this once you add animations
            Visible = false;

            //Again remove Visible = true
            AddComponent(new Timer(RespawnTime, true, null, () => { canActivate = true; Active = true; Visible = true; }));
        }
    }
}
