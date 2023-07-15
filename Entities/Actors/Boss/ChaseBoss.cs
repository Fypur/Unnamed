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
    public class ChaseBoss : Actor
    {
        public Vector2[] Positions;

        public ChaseBoss(Vector2[] positions, int id) : base(positions[0] + new Vector2(-4), 32, 32, 0, new Sprite(Color.Red))
        {
            Positions = positions.Addition(new Vector2(-4));

            if (id == 0)
                AddComponent(new Coroutine(Room1()));
            else if(id == 1)
                AddComponent(new Coroutine(Jump(Positions[1], 0.4f, 300),
                    Jump(Positions[2], 0.4f, 100), Coroutine.WaitSeconds(0.3f), Jump(Positions[3], 0.4f, 100), Coroutine.WaitSeconds(0.2f), MachineGun(45, 13, true)
                    ));
            else if(id == 2)
                AddComponent(new Coroutine(
                    Jump(Positions[1], 0.4f, 300),
                    Coroutine.WaitSeconds(0.1f),
                    Jump(Positions[2], 0.4f, 100),
                    Coroutine.WaitSeconds(0.2f),
                    Jump(Positions[3], 0.4f, 100),
                    Coroutine.WaitSeconds(0.2f),
                    Jump(Positions[4], 0.4f, 100),
                    MachineGun(45, 13, false)
                    ));

            GetComponent<Coroutine>().Enumerator.MoveNext();
        }

        private IEnumerator Room1()
        {
            float time = 0;

            Vector2 initPos = Pos + new Vector2(0, -100);

            while (time < 0.7f)
            {
                Vector2 aim = Vector2.Lerp(initPos, Positions[1], Ease.CubicIn(time / 0.7f));

                MoveX(aim.X - Pos.X, OnCollision);
                MoveY(aim.Y - Pos.Y, OnCollision);

                time += Engine.Deltatime;
                yield return 0;
            }

            MoveX(Positions[1].X - Pos.X, OnCollision);
            MoveY(Positions[1].Y - Pos.Y, OnCollision);

            Engine.Cam.LightShake();

            Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, 6, new Rectangle((int)Pos.X, (int)Pos.Y + Height - 3, Width, 3), null, 0, Particles.Dust.Color);
            Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, 6, new Rectangle((int)Pos.X, (int)Pos.Y + Height - 3, Width, 3), null, 180, Particles.Dust.Color);

            yield return new Coroutine.WaitForSeconds(0.7f);

            initPos = Pos;
            time = 0;

            Vector2[] controlPoints = new Vector2[] { initPos, new Vector2((initPos.X + Positions[2].X) / 2, initPos.Y - 300), Positions[2] };
            while (time < 0.6f)
            {
                Vector2 aim = Bezier.Quadratic(controlPoints, time / 0.6f);

                MoveX(aim.X - Pos.X, OnCollision);
                MoveY(aim.Y - Pos.Y, OnCollision);

                time += Engine.Deltatime;
                yield return 0;
            }

            MoveX(Positions[2].X - Pos.X, OnCollision);
            MoveY(Positions[2].Y - Pos.Y, OnCollision);

            Engine.Cam.LightShake();

            Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, 6, new Rectangle((int)Pos.X, (int)Pos.Y + Height - 3, Width, 3), null, 0, Particles.Dust.Color);
            Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, 6, new Rectangle((int)Pos.X, (int)Pos.Y + Height - 3, Width, 3), null, 180, Particles.Dust.Color);

            Sprite.Color = Color.Yellow;

            yield return new Coroutine.WaitForSeconds(0.2f);

            int numBullets = 10;
            float range = 180;

            for (int i = 1; i <= numBullets; i++)
            {
                var m = Engine.CurrentMap.Instantiate(new MachineGunBullet(MiddlePos, i * range / numBullets + 180));
                Engine.CurrentMap.CurrentLevel.DestroyOnUnload(m);
                Engine.Cam.Shake(0.3f, 0.7f);
                yield return new Coroutine.WaitForSeconds(0.1f);
            }

            Sprite.Color = Color.Red;

        }

        private IEnumerator Room2()
        {
            yield return null;
        }

        private IEnumerator Jump(Vector2 to, float jumpTime, float height)
        {
            float time = 0;
            Vector2 initPos = Pos;

            Vector2[] controlPoints = new Vector2[] { initPos, new Vector2((initPos.X + to.X) / 2, initPos.Y - height), to };
            while (time < jumpTime)
            {
                Vector2 aim = Bezier.Quadratic(controlPoints, time / jumpTime);

                MoveX(aim.X - Pos.X, OnCollision);
                MoveY(aim.Y - Pos.Y, OnCollision);

                time += Engine.Deltatime;
                yield return 0;
            }

            MoveX(to.X - Pos.X, OnCollision);
            MoveY(to.Y - Pos.Y, OnCollision);

            Engine.Cam.LightShake();

            Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, 6, new Rectangle((int)Pos.X, (int)Pos.Y + Height - 3, Width, 3), null, 0, Particles.Dust.Color);
            Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, 6, new Rectangle((int)Pos.X, (int)Pos.Y + Height - 3, Width, 3), null, 180, Particles.Dust.Color);
        }

        private IEnumerator MachineGun(float range, int numBullets, bool rightSide)
        {
            Sprite.Color = Color.Yellow;

            yield return new Coroutine.WaitForSeconds(0.2f);

            for (int i = 1; i <= numBullets; i++)
            {
                var m = Engine.CurrentMap.Instantiate(new MachineGunBullet(MiddlePos, rightSide ? -i * range / numBullets : i * range / numBullets + 180));
                Engine.CurrentMap.CurrentLevel.DestroyOnUnload(m);
                Engine.Cam.Shake(0.3f, 0.7f);
                yield return new Coroutine.WaitForSeconds(0.1f);
            }

            Sprite.Color = Color.Red;
        }

            private void OnCollision(Entity entity)
        {
            if (entity is FallingPlatform f)
                f.Fall();
        }
    }
}
