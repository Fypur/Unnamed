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
        private int id;
        public Vector2[] Positions;
        private List<float> circleLengths = new();

        public ChaseBoss(Vector2[] positions, int id) : base(positions[0] + new Vector2(-4), 32, 32, 0, new Sprite(Color.Red))
        {
            Positions = positions.Addition(new Vector2(-4));

            AddComponent(new HurtBox(Vector2.Zero, Width, Height));
            this.id = id;
        }

        public override void Awake()
        {
            if (id == 0)
                AddComponent(new Coroutine(Room1()));
            else if (id == 1)
                AddComponent(new Coroutine(Jump(Positions[1], 0.4f, 300),
                    Jump(Positions[2], 0.4f, 100), Coroutine.WaitSeconds(0.3f), Jump(Positions[3], 0.4f, 100), Coroutine.WaitSeconds(0.2f), MachineGun(45, 13, true)
                    ));
            else if (id == 2)
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
            else if (id == 3)
                AddComponent(new Coroutine(
                    Jump(Positions[1], 0.4f, 0),
                    Coroutine.WaitSeconds(1f),
                    Jump(Positions[2], 0.4f, 200),
                    Scream(1, 0.2f),
                    MachineGun2(170, 90, 12, true)
                    ));
            else if (id == 4)
            {
                var b = Engine.CurrentMap.Data.GetEntity<ChaseBoss>();
                if (b != null && b!= this)
                    b.SelfDestroy();

                AddComponent(new Coroutine(
                    Jump(Positions[1], 0.4f, 300),
                    Room4(),
                    Scream(3, 0.5f)
                    ));
            }
            else if (id == 5)
            {
                var b = Engine.CurrentMap.Data.GetEntity<ChaseBoss>();
                if (b != null && b != this)
                    b.SelfDestroy();

                AddComponent(new Coroutine(
                    Jump(Positions[1], 0.2f, 200),
                    Coroutine.WaitSeconds(0.5f),
                    Jump(Positions[2], 0.2f, 100),
                    Coroutine.WaitSeconds(0.2f),
                    Jump(Positions[3], 0.2f, 100),
                    Coroutine.WaitSeconds(0.4f),
                    Jump(Positions[4], 0.2f, 100),
                    Coroutine.WaitSeconds(0.4f),
                    Jump(Positions[5], 0.2f, 100),
                    Coroutine.WaitSeconds(1),
                    Jump(Positions[6], 1.5f, 320),
                    Coroutine.Do(() => Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Explosion, Bounds, 100))
                    //Scream
                    ));
            }

            GetComponent<Coroutine>().Enumerator.MoveNext();

            base.Awake();
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

        public IEnumerator MachineGun2(float middleAngle, float range, int numBullets, bool clockWise)
        {
            Sprite.Color = Color.Yellow;

            middleAngle -= range / 2 * (clockWise ? 1 : -1);

            float increment = range / numBullets * (clockWise ? 1 : -1);
            for (int i = 0; i < numBullets; i++)
            {
                var m = Engine.CurrentMap.Instantiate(new MachineGunBullet(MiddlePos, middleAngle + i * increment));
                Engine.CurrentMap.CurrentLevel.EntityData.Add(m);

                Engine.Cam.Shake(0.3f, 0.7f);
                yield return new Coroutine.WaitForSeconds(0.1f);
            }

            Sprite.Color = Color.Red;
        }

        public IEnumerator Missile()
        {
            yield return new Coroutine.WaitForSeconds(0.4f);
            Engine.CurrentMap.Instantiate(new HomingMissile(MiddlePos, -45));
        }

        public IEnumerator Scream(int numScreams, float screamTime)
        {
            Engine.Cam.Shake(screamTime * numScreams, 1);

            for (int i = 0; i < numScreams; i++)
            {
                AddComponent(new Coroutine(AddCircle()));
                yield return new Coroutine.WaitForSeconds(screamTime);
            }

            //Scream sfx

            IEnumerator AddCircle()
            {
                int index = circleLengths.Count;
                circleLengths.Add(1);
                for (int i = 1; i < 500; i++)
                {
                    if (index >= circleLengths.Count)
                        break;

                    circleLengths[index] = i * 3;
                    yield return null;
                }

                if (index < circleLengths.Count)
                    circleLengths[index] = 0;
            }
        }

        public override void Render()
        {
            base.Render();

            Drawing.EndPrimitives();
            Drawing.BeginPrimitives(Engine.PrimitivesRenderTarget, null, Microsoft.Xna.Framework.Graphics.BlendState.Opaque);

            if (circleLengths.Count >= 1 && circleLengths[0] == 0)
                circleLengths.RemoveAt(0);

            foreach (float c in circleLengths)
            {
                if (c > 0)
                    Drawing.DrawCircle(MiddlePos, c, 0.1f, Color.White);

                float t = 1.02f * c - 12;
                if (t > 0)
                    Drawing.DrawCircle(MiddlePos, t, 0.1f, Color.Transparent);
            }

            Drawing.EndPrimitives();
            Drawing.BeginPrimitives(Engine.PrimitivesRenderTarget);
        }

        private void OnCollision(Entity entity)
        {
            if (entity is FallingPlatform f)
                f.Fall();
        }

        private void FallClosestFallingPlatform()
        {
            var platforms = Engine.CurrentMap.Data.GetEntities<FallingPlatform>();
            float distance = float.MaxValue;
            FallingPlatform falling = null;
            foreach(FallingPlatform p in platforms)
                if(!p.HasFallen && Vector2.DistanceSquared(MiddlePos, p.MiddlePos) < distance)
                {
                    falling = p;
                    distance = Vector2.DistanceSquared(MiddlePos, falling.MiddlePos);
                }

            falling.Fall();
        }

        private IEnumerator MoveCameraBlock(float time, Vector2 amount)
        {
            CameraBlock c = Engine.CurrentMap.Data.GetEntity<CameraBlock>();

            Vector2 initPos = c.Pos;
            float timer = 0;
            while(timer < time)
            {
                c.Pos = Vector2.Lerp(initPos, initPos + amount, Ease.CubeInAndOut(timer / time));
                timer += Engine.Deltatime;
                yield return null;
            }

            c.SelfDestroy();
        }

        private IEnumerator Room4()
        {
            PushingFire p = Engine.CurrentMap.Data.GetEntity<PushingFire>();
            float origSpeed = 0;
            if(p != null)
            {
                origSpeed = p.Speed;
                p.ChangeSpeed(p.Speed / 2);
            }
            

            Sprite.Color = Color.Yellow;
            float timer = 0;
            while (timer < 1)
                timer += Engine.Deltatime;

            Coroutine coroutine = new Coroutine(MachineGun2(180, 90, 10, true));

            while (timer < 2)
            {
                coroutine.Update();
                timer += Engine.Deltatime;
                yield return null;
            }

            FallClosestFallingPlatform();
            FallClosestFallingPlatform();

            while (coroutine.Enumerator != null) { coroutine.Update(); yield return null; }

            coroutine = new Coroutine(MoveCameraBlock(2, new Vector2(0, -184)));
            while (coroutine.Enumerator != null)
            {
                coroutine.Update();
                yield return null;
            }

            if (p != null)
                p.ChangeSpeed(origSpeed);
        }
    }
}
