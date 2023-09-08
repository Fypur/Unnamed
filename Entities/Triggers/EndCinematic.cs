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
    public class EndCinematic : PlayerTrigger
    {
        private Player player;
        private Vector2[] positions;

        public EndCinematic(Rectangle bounds, Vector2[] positions) : base(bounds, null)
        {
            this.positions = positions;
        }

        public EndCinematic(Vector2 position, Vector2 size, Vector2[] positions) : base(position, size, null)
        {
            this.positions = positions;
        }

        public EndCinematic(Vector2 position, int width, int height, Vector2[] positions) : base(position, width, height, null)
        {
            this.positions = positions;
        }

        public override void OnTriggerEnter(Player player)
        {
            base.OnTriggerEnter(player);

            this.player = player;
            //Jump
            //Fade out -> go to new scene where player is falling
            //Credits (a game by Fypur || with the help of catapillie || special thanks to Celeste / Hollow Knight / Ori for inspiration) roll while player is falling (in empty corridor ?)
            //Fade out to black
            //Put Image and Game Logo
            //Text: The End
            

            AddComponent(new Coroutine(
                FreezeInput(0.5f),
                Coroutine.Do(() =>
                {
                    player.CanMove = false;
                    player.CanAnimateSprite = true;
                }),
                MovePlayer(positions[0], 0.7f, "run", null, null),
                Coroutine.Do(() => AddComponent(new Coroutine(MovePlayer(positions[1], 0.6f, "jump", null, true)))),
                MovePlayer(positions[1], 0.6f, "jump", Ease.QuintOut, false)
                ));
        }

        private IEnumerator MovePlayer(Vector2 to, float time, string anim, Func<float, float> ease, bool? XYOrBoth)
        {
            float t = 0;

            Vector2 init = player.Pos;
            player.Sprite.Play(anim);

            while(t < time)
            {
                if (XYOrBoth == null)
                    player.Pos = Vector2.Lerp(init, to, ease == null ? t / time : ease(t / time));
                else if (XYOrBoth == true)
                    player.Pos.X = MathHelper.Lerp(init.X, to.X, ease == null ? t / time : ease(t / time));
                else if (XYOrBoth == false)
                    player.Pos.Y = MathHelper.Lerp(init.Y, to.Y, ease == null ? t / time : ease(t / time));
                t += Engine.Deltatime;
                yield return null;
            }

            player.Pos = to;
        }

        private IEnumerator FreezeInput(float t)
        {
            Input.State s = Input.CurrentState;
            Input.State oldS = Input.OldState;

            float time = 0;
            while (time < t)
            {
                Input.CurrentState = new Input.State();
                Input.OldState = new Input.State();
                time += Engine.Deltatime;
                yield return null;
            }

            Input.CurrentState = s;
            Input.OldState = oldS;
        }

        /*private IEnumerator MovePlayerUntil(Vector2 moved, float to, bool aimX, bool higher, float maxTime, string anim)
        {
            float t = 0;

            Vector2 init = player.Pos;
            player.Sprite.Play(anim);

            while (t < maxTime)
            {
                player.Pos += moved * Engine.Deltatime;

                if (aimX && player.MiddlePos.X > to)
                    break;
                else if (!aimX && player.MiddlePos.Y > to)
                    break;

                t += Engine.Deltatime;
                yield return null;
            }

            player.Pos = to;
        }*/

    }
}
