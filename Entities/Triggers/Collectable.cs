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
    public abstract class Collectable : PlayerTrigger
    {
        private Vector2 initPos;
        private bool following;
        public Guid iid;
        private Guid levelIid;

        public Collectable(Vector2 position, int width, int height, Guid iid, Guid levelIid, Sprite sprite)
            : base(position, width, height, sprite) { initPos = Pos; this.iid = iid; this.levelIid = levelIid; }

        public override void OnTriggerEnter(Player player)
        {
            if (!following)
            {
                following = true;
                AddComponent(new Coroutine(WaitCollected(player), OnCollected(player)));
            }
        }

        public abstract IEnumerator OnCollected(Player player);

        public virtual void WhileWait(Player player) { }

        private IEnumerator WaitCollected(Player player)
        {
            Engine.CurrentMap.CurrentLevel.DontDestroyOnUnload(this);

            bool breakLoop = false;
            player.OnDeath += () => breakLoop = true;

            while (!player.Safe)
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
                    Pos = Vector2.Lerp(oldPos, initPos, Ease.QuintInAndOut(Ease.Reverse(timer.Value / timer.MaxValue)));
                }, () => Pos = initPos));

                RemoveComponent(GetComponent<Coroutine>());
                yield return null;
            }

            if (!Levels.LevelNonRespawn.ContainsKey(levelIid))
                Levels.LevelNonRespawn[levelIid] = new();
            Levels.LevelNonRespawn[levelIid].Add(iid);
        }
    }
}
