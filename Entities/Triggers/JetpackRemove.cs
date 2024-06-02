using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unnamed
{
    public class JetpackRemove : PlayerTrigger
    {
        private Boss3 boss;
        private bool activated;
        private Vector2[] positions;

        public JetpackRemove(Vector2 position, Vector2 size, Vector2[] positions) : base(position, size, null)
        {

        }

        public override void OnTriggerEnter(Player entity)
        {
            if (activated)
                return;

            activated = true;   

            base.OnTriggerEnter(entity);

            AddComponent(new Coroutine(Cinematic()));
        }

        private IEnumerator Cinematic()
        {
            //Platformer.player.CanMove = false;

            boss = new Boss3(new Vector2(6415, -2300));
            boss.Active = false;
            boss.Sprite.SpriteEffect = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;

            AddComponent(new Coroutine(Cannon()));

            ///Coroutine jump = boss.BezierJump()

            yield return null;

            boss.Pos = new Vector2(6377, -2170);
        }

        private IEnumerator Cannon()
        {
            while (true)
            {
                boss.Collider.Update();
                boss.SetCannonPos(0.2f);
                boss.Pos = Input.MousePos;
                yield return null;
            }
        }

        public override void Render()
        {
            base.Render();

            boss?.Render();
        }
    }
}
