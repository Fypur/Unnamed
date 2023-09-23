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

            Audio.StopEvent(Platformer.Music, true);
            player.Sprite.OnFrameChange = null;

            AddComponent(new Coroutine(
                FreezeInput(1.5f),
                Coroutine.Do(() =>
                {
                    player.CanMove = false;
                    player.CanAnimateSprite = true;
                }),
                MovePlayer(positions[0], 0.7f, "run", null, null),
                Coroutine.Do(() => AddComponent(new Coroutine(MovePlayer(positions[2], 1.2f, "jump", null, true)))),
                MovePlayer(positions[1], 0.4f, "jump", Ease.CubicOut, false),
                MovePlayer(positions[2], 0.7f, "jump", Ease.CubicIn, false),
                Credits()
                ));
        }

        private IEnumerator MovePlayer(Vector2 to, float time, string anim, Func<float, float> ease, bool? XYOrBoth)
        {
            float t = 0;

            Vector2 init = player.Pos;
            player.Sprite.Play(anim);
            player.Sprite.OnFrameChange = null;

            while (t < time)
            {
                if (XYOrBoth == null)
                    player.Pos = Vector2.Lerp(init, to, ease == null ? t / time : ease(t / time));
                else if (XYOrBoth == true)
                    player.Pos.X = MathHelper.Lerp(init.X, to.X, ease == null ? t / time : ease(t / time));
                else if (XYOrBoth == false)
                    player.Pos.Y = MathHelper.Lerp(init.Y, to.Y, ease == null ? t / time : ease(t / time));

                player.Sprite.OnFrameChange = null;
                t += Engine.Deltatime;
                yield return null;
            }

            if (XYOrBoth == null)
                player.Pos = to;
            else if (XYOrBoth == true)
                player.Pos.X = to.X;
            else if (XYOrBoth == false)
                player.Pos.Y = to.Y;
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

        private IEnumerator Credits()
        {
            Platformer.CanPause = false;

            UIImage img = (UIImage)Engine.CurrentMap.Instantiate(new UIImage(Vector2.Zero, 1280, 720, false, new Sprite(new Color(Color.Black, 0))));
            img.Overlay = true;
            IEnumerator enumerator;


            enumerator = DoInTime(1f, (t, mT) => img.Sprite.Color = new Color(Color.Black, t / mT));
            while (enumerator.MoveNext()) { yield return null; }

            yield return new Coroutine.WaitForSeconds(2f);

            TextBox text = (TextBox)Engine.CurrentMap.Instantiate(new TextBox("A game by Fypur", "LexendDeca", Vector2.Zero, 1280, 720, 2, Color.Transparent, false, TextBox.Alignement.Center));
            text.Overlay = true;

            enumerator = DoInTime(1f, (t, mT) => text.Color = new Color(Color.White, t / mT));
            while (enumerator.MoveNext()) { yield return null; }

            yield return new Coroutine.WaitForSeconds(3f);

            enumerator = DoInTime(1f, (t, mT) => text.Color = new Color(Color.White, Ease.Reverse(t / mT)));
            while (enumerator.MoveNext()) { yield return null; }

            text.TextScale = 1f;
            text.SetText("with help from catapillie");

            enumerator = DoInTime(1f, (t, mT) => text.Color = new Color(Color.White, t / mT));
            while (enumerator.MoveNext()) { yield return null; }

            yield return new Coroutine.WaitForSeconds(2f);

            enumerator = DoInTime(1f, (t, mT) => text.Color = new Color(Color.White, Ease.Reverse(t / mT)));
            while (enumerator.MoveNext()) { yield return null; }

            text.SetText("With Inspiration from:\n            Celeste\n       Hollow Knight\n               Ori");

            enumerator = DoInTime(1f, (t, mT) => text.Color = new Color(Color.White, t / mT));
            while (enumerator.MoveNext()) { yield return null; }

            yield return new Coroutine.WaitForSeconds(1.5f);

            enumerator = DoInTime(1f, (t, mT) => text.Color = new Color(Color.White, Ease.Reverse(t / mT)));
            while (enumerator.MoveNext()) { yield return null; }

            text.TextScale = 2f;
            text.SetText("Thanks for playing!");

            enumerator = DoInTime(1f, (t, mT) => text.Color = new Color(Color.White, t / mT));
            while (enumerator.MoveNext()) { yield return null; }

            yield return new Coroutine.WaitForSeconds(3f);

            enumerator = DoInTime(1f, (t, mT) => text.Color = new Color(Color.White, Ease.Reverse(t / mT)));
            while (enumerator.MoveNext()) { yield return null; }

            yield return null;
            Saving.Save(new()
            {
                CurrentLevel = Levels.LastLDtkLevel.Identifier,
                CurrentWorld = Platformer.InitWorld,
                WorldUnlocked = Platformer.WorldsUnlocked,
                CanJetpack = Platformer.player.CanJetpack
            });

            Platformer.LoadWorldSave(Saving.Load());

            Platformer.CanPause = true;
            Platformer.EndGame();

            Engine.CurrentMap.Instantiate(new MainMenu());


            IEnumerator DoInTime(float time, Action<float, float> SetColor)
            {
                float t = 0;
                while (t < time)
                {
                    SetColor(t, time);
                    t += Engine.Deltatime;
                    yield return null;
                }

                SetColor(time, time);
            }
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
