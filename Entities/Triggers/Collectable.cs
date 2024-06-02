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
    public abstract class Collectable : PlayerTrigger
    {
        private Vector2 initPos;
        private bool following;
        public Guid iid;

        public Collectable(Vector2 position, int width, int height, Guid iid, Sprite sprite)
            : base(position, width, height, sprite) { initPos = Pos; this.iid = iid; }

        public override void OnTriggerEnter(Player player)
        {
            base.OnTriggerEnter(player);

            if (!following)
            {
                following = true;
                AddComponent(new Coroutine(WaitCollected(player)));
            }
        }

        public abstract void OnCollected(Player player);

        public virtual void WhileWait(Player player) { }

        private IEnumerator WaitCollected(Player player)
        {
            Engine.CurrentMap.CurrentLevel.DontDestroyOnUnload(this);

            bool breakLoop = false;
            player.OnDeath += () => breakLoop = true;

            while (!CollectingConditions())
            {
                Pos = Vector2.Lerp(Pos, player.Pos - player.Facing * Vector2.UnitX * 9 - Vector2.UnitY * 4, 0.2f);
                WhileWait(player);
                if (breakLoop)
                    break;
                yield return null;
            }

            if (breakLoop)
            {
                Engine.CurrentMap.CurrentLevel.DestroyOnUnload(this);
                Vector2 oldPos = Pos;
                AddComponent(new Timer(0.5f, true, (timer) =>
                {
                    Pos = Vector2.Lerp(oldPos, initPos, Ease.CubeInAndOut(Ease.Reverse(timer.Value / timer.MaxValue)));
                }, () => Pos = initPos));

                RemoveComponent(GetComponent<Coroutine>());
                yield return null;
            }

            Levels.LevelNonRespawn.Add(iid);
            OnCollected(player);
        }

        protected abstract bool CollectingConditions(); 
    }
}
