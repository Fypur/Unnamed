using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Platformer
{
    public class ChaseBoss : Actor
    {
        private int id;
        public Vector2[] Positions;
        private List<float> circleLengths = new();

        Tuple<Sprite, Sprite, Sprite>[] cannons;
        Vector2 cannonPos;

        private Guid iid;

        public ChaseBoss(Vector2[] positions, int id, Guid iid) : base(positions[0] + new Vector2(-4), 24, 16, 0, new Sprite(Color.White))
        {
            Positions = positions.Addition(new Vector2(-4));

            AddComponent(new HurtBox(Vector2.Zero, Width, Height));
            this.id = id;

            cannons = new Tuple<Sprite, Sprite, Sprite>[2];
            for (int i = 0; i < 2; i++)
            {
                Sprite cannonPart1 = new Sprite(DataManager.Textures["Boss/arm"]);
                Sprite cannon = new Sprite(DataManager.Textures["Boss/gunArm"]);

                cannonPart1.Origin = Vector2.UnitY;
                Sprite cannonPart2 = cannonPart1.Copy();

                AddComponent(cannonPart1);
                AddComponent(cannonPart2);
                AddComponent(cannon);

                cannons[i] = new(cannonPart1, cannonPart2, cannon);
            }

            Sprite.Add(Sprite.AllAnimData["Boss"]);
            Sprite.Origin = HalfSize;
            Sprite.Offset = HalfSize;
            this.iid = iid;

            /*RemoveComponent(Collider);
            Collider = new BoxColliderRotated(Vector2.Zero, Width, Height, 0, HalfSize);
            AddComponent(Collider);*/ //This seems to break the boss's collision detection somehow
        }

        public override void Awake()
        {
            Coroutine c = new(Jump(Vector2.Zero, 0, 0));

            if (id == 0)
                c = (Coroutine)AddComponent(new Coroutine(Room1()));
            else if (id == 1)
                c = (Coroutine)AddComponent(new Coroutine(Jump(Positions[1], 0.4f, 300),
                    Jump(Positions[2], 0.4f, 100),
                    Coroutine.WaitSeconds(0.3f),
                    Jump(Positions[3], 0.4f, 100),
                    Coroutine.WaitSeconds(0.2f),
                    MachineGun(45, 13, true)
                    ));
            else if (id == 2)
                c = (Coroutine)AddComponent(new Coroutine(
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
                c = (Coroutine)AddComponent(new Coroutine(
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

                c = (Coroutine)AddComponent(new Coroutine(
                    Coroutine.Do(() => FallClosestFallingPlatform()),
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

                AddComponent(new Coroutine(FreezeWithInput(7f, () => 
                Engine.Player.Pos.Y < -1305)));

                float t = 0.3f;
                c = (Coroutine)AddComponent(new Coroutine(
                    Coroutine.Do(() => {
                        GetComponent<HurtBox>().Active = false;
                    }),
                    Jump(Positions[1], t, 200),
                    Coroutine.WaitSeconds(0.5f),
                    Jump(Positions[2], t, 100),
                    Coroutine.WaitSeconds(0.2f),
                    Jump(Positions[3], t, 100),
                    Coroutine.WaitSeconds(0.4f),
                    Jump(Positions[4], t, 100),
                    Coroutine.WaitSeconds(0.4f),
                    Jump(Positions[5], t, 100),
                    Coroutine.WaitSeconds(1),
                    Jump(Positions[6], 1.5f, 310),

                    Coroutine.Do(() => {
                        Engine.Cam.Shake(0.3f, 2);
                        Sprite.Visible = false;
                        foreach(var b in cannons)
                        {
                            b.Item1.Visible = false;
                            b.Item2.Visible = false;
                            b.Item3.Visible = false;
                        }
                        ParticleType explosion = new();
                        explosion.CopyFrom(Particles.Explosion);
                        explosion.Direction = -90;
                        explosion.DirectionRange = 180;
                        explosion.SpeedMin = 10f;
                        explosion.SpeedMax = 200f;
                        Engine.CurrentMap.MiddlegroundSystem.Emit(explosion, Bounds, 100); })
                    ));
            }
            else if (id == 6)
            {
                if (Levels.LevelNonRespawn.Contains(iid))
                {
                    SelfDestroy();
                    return;
                }

                Pos = Positions[0] + new Vector2(50, -50);
                Platformer.Music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

                AddComponent(new Coroutine(FreezeInput(4f)));
                GetComponent<HurtBox>().Active = false;

                c = (Coroutine)AddComponent(new Coroutine(
                    Jump(Positions[1], 0.5f, 100),
                    Coroutine.Do(() => AddComponent(new Coroutine(ProjectPlayer()))),
                    Coroutine.WaitSeconds(1.5f),
                    Jump(Positions[2], 0.2f, 40),
                    Coroutine.WaitSeconds(1.5f),
                    Jump(Positions[3] + new Vector2(-100, -200), 0.5f, 150)
                    ));
            }

            c.Enumerator.MoveNext();

            base.Awake();
        }

        IEnumerator FreezeInput(float t)
        {
            Input.State s = Input.CurrentState;
            //Input.State oldS = Input.OldState;

            float time = 0;
            while (time < t)
            {
                Input.CurrentState = new Input.State();
                //Input.OldState = new Input.State();
                time += Engine.Deltatime;
                yield return null;
            }

            Input.CurrentState = s;
            //Input.OldState = oldS;
        }

        IEnumerator FreezeWithInput(float t, Func<bool> early = null)
        {
            Input.State s = Input.CurrentState;
            Input.State oldS = Input.OldState;

            float time = 0;

            Func<bool> a = () => early == null ? false : early();
            while (time < t && !a())
            {
                Input.CurrentState = s;
                Input.OldState = oldS;
                time += Engine.Deltatime;
                Debug.LogUpdate(Engine.Player.Pos);
                yield return null;
            }

            Input.CurrentState = s;
            Input.OldState = oldS;
        }

        private IEnumerator Room1()
        {
            float time = 0;

            Vector2 initPos = Pos + new Vector2(0, -100);


            bool once = true;

            while (time < 0.7f)
            {
                Vector2 aim = Vector2.Lerp(initPos, Positions[1], Ease.CubicIn(time / 0.7f));

                MoveX(aim.X - Pos.X, OnCollision);
                MoveY(aim.Y - Pos.Y, OnCollision);

                if(time > 0.7f - 0.3f && once)
                {
                    once = false;
                    AddComponent(new Sound3D("SFX/Boss/JumpLandSlam", autoRemove: true));
                }

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
            once = true;

            Vector2[] controlPoints = new Vector2[] { initPos, new Vector2((initPos.X + Positions[2].X) / 2, initPos.Y - 300), Positions[2] };
            while (time < 0.6f)
            {
                Vector2 aim = Bezier.Quadratic(controlPoints, time / 0.6f);

                MoveX(aim.X - Pos.X, OnCollision);
                MoveY(aim.Y - Pos.Y, OnCollision);

                if (time < 0.6f - 0.3f && once)
                {
                    once = false;
                    AddComponent(new Sound3D("SFX/Boss/JumpLandSlam", autoRemove: true));
                }

                time += Engine.Deltatime;
                yield return 0;
            }

            MoveX(Positions[2].X - Pos.X, OnCollision);
            MoveY(Positions[2].Y - Pos.Y, OnCollision);

            Engine.Cam.LightShake();

            Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, 6, new Rectangle((int)Pos.X, (int)Pos.Y + Height - 3, Width, 3), null, 0, Particles.Dust.Color);
            Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, 6, new Rectangle((int)Pos.X, (int)Pos.Y + Height - 3, Width, 3), null, 180, Particles.Dust.Color);

            cannons[1].Item1.Color = Color.Yellow;
            cannons[1].Item2.Color = Color.Yellow;
            cannons[1].Item3.Color = Color.Yellow;

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

            cannons[1].Item1.Color = Color.White;
            cannons[1].Item2.Color = Color.White;
            cannons[1].Item3.Color = Color.White;
        }

        private IEnumerator Jump(Vector2 to, float jumpTime, float height)
        {
            float time = 0;
            Vector2 initPos = Pos;
            Sprite.Rotation = 0;
            bool forward = Rand.NextDouble() < 0.5f;

            AddComponent(new Timer(jumpTime - 0.3f, true, null, () =>
            {
                AddComponent(new Sound3D("SFX/Boss/JumpLandSlam", autoRemove: true));
            }));

            Vector2[] controlPoints = new Vector2[] { initPos, new Vector2((initPos.X + to.X) / 2, initPos.Y - height), to };
            while (time < jumpTime)
            {
                Vector2 aim = Bezier.Quadratic(controlPoints, time / jumpTime);

                MoveX(aim.X - Pos.X, OnCollision);
                MoveY(aim.Y - Pos.Y, OnCollision);

                if(forward)
                    Sprite.Rotation = MathHelper.Lerp(0, (float)Math.PI * 2, time / jumpTime);
                else
                    Sprite.Rotation = MathHelper.Lerp(0, (float)-Math.PI * 2, time / jumpTime);

                time += Engine.Deltatime;
                yield return 0;
            }

            MoveX(to.X - Pos.X, OnCollision);
            MoveY(to.Y - Pos.Y, OnCollision);
            Sprite.Rotation = 0;

            Engine.Cam.LightShake();

            Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, 6, new Rectangle((int)Pos.X, (int)Pos.Y + Height - 3, Width, 3), null, 0, Particles.Dust.Color);
            Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, 6, new Rectangle((int)Pos.X, (int)Pos.Y + Height - 3, Width, 3), null, 180, Particles.Dust.Color);
        }

        private IEnumerator MachineGun(float range, int numBullets, bool rightSide)
        {
            cannons[1].Item1.Color = Color.Yellow;
            cannons[1].Item2.Color = Color.Yellow;
            cannons[1].Item3.Color = Color.Yellow;

            yield return new Coroutine.WaitForSeconds(0.2f);

            for (int i = 1; i <= numBullets; i++)
            {
                var m = Engine.CurrentMap.Instantiate(new MachineGunBullet(cannonPos, rightSide ? -i * range / numBullets : i * range / numBullets + 180));
                Engine.CurrentMap.CurrentLevel.DestroyOnUnload(m);
                Engine.Cam.Shake(0.3f, 0.7f);
                yield return new Coroutine.WaitForSeconds(0.1f);
            }

            cannons[1].Item1.Color = Color.White;
            cannons[1].Item2.Color = Color.White;
            cannons[1].Item3.Color = Color.White;
        }

        public IEnumerator MachineGun2(float middleAngle, float range, int numBullets, bool clockWise)
        {
            cannons[1].Item1.Color = Color.Yellow;
            cannons[1].Item2.Color = Color.Yellow;
            cannons[1].Item3.Color = Color.Yellow;

            middleAngle -= range / 2 * (clockWise ? 1 : -1);

            float increment = range / numBullets * (clockWise ? 1 : -1);
            for (int i = 0; i < numBullets; i++)
            {
                var m = Engine.CurrentMap.Instantiate(new MachineGunBullet(cannonPos, middleAngle + i * increment));
                Engine.CurrentMap.CurrentLevel.EntityData.Add(m);

                Engine.Cam.Shake(0.3f, 0.7f);
                yield return new Coroutine.WaitForSeconds(0.1f);
            }

            cannons[1].Item1.Color = Color.White;
            cannons[1].Item2.Color = Color.White;
            cannons[1].Item3.Color = Color.White;
        }

        public IEnumerator Missile()
        {
            yield return new Coroutine.WaitForSeconds(0.4f);
            Engine.CurrentMap.Instantiate(new HomingMissile(cannonPos, -45));
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

        public override void Update()
        {
            base.Update();

            SetCannonPos();
        }

        public override void Render()
        {
            if (Engine.Player.MiddlePos.X < MiddlePos.X)
                Sprite.SpriteEffect = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
            else
                Sprite.SpriteEffect = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;

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

        private void SetCannonPos()
        {
            float finalRot = (Engine.Player.MiddlePos - MiddlePos).ToAngleRad();

            float[] rot1 = new float[] {
                5 * (float)Math.PI / 4 + Rand.NextFloat(-1, 1) * (float)Math.PI / 4,
                7 * (float)Math.PI / 4 + Rand.NextFloat(-1, 1) * (float)Math.PI / 4,
                //rotation + 4 * (float)Math.PI / 4 + Rand.NextFloat(-1, 1) * (float)Math.PI / 4,
            };

            for(int i = 0; i < 2; i++)
            {
                if (rot1[i] < 0)
                    rot1[i] += (float)Math.PI * 2;
                rot1[i] %= (float)Math.PI * 2;
            }

            float[] rot2 = new float[3];
            for (int i = 0; i < 2; i++)
                rot2[i] = (float)(Rand.NextDouble() * Math.PI * 2);

            for(int i = 0; i < 2; i++)
            {
                cannons[i].Item2.Rotation += 0.01f;
                cannons[i].Item1.Rotation = Rot(cannons[i].Item1.Rotation, rot1[i], 0.05f);
                cannons[i].Item2.Rotation = Rot(cannons[i].Item2.Rotation, rot2[i], 0.02f);
                cannons[i].Item3.Rotation = finalRot;


                //cannons[i].Item1.Offset = rotColl.Rect[0] - Pos + VectorHelper.Rotate(new Vector2(6, 4), rotation);
                cannons[i].Item1.Offset = VectorHelper.Rotate(new Vector2(6, 4), 0);
                cannons[i].Item2.Offset = cannons[i].Item1.Offset + VectorHelper.Rotate(new Vector2(Boss3.CannonLength, 0), cannons[i].Item1.Rotation);
                cannons[i].Item3.Offset = cannons[i].Item2.Offset + VectorHelper.Rotate(new Vector2(Boss3.CannonLength, 0), cannons[i].Item2.Rotation);

            }

            cannonPos = Pos + cannons[1].Item3.Offset + VectorHelper.Rotate(new Vector2(21, 3), cannons[1].Item3.Rotation);

            float Rot(float from, float to, float lerp)
            {
                if (Math.Abs(to - from) < Math.Abs(to - (float)Math.PI * 2 - from))
                    return MathHelper.Lerp(from, to, lerp);
                else
                    return MathHelper.Lerp(from, to - (float)Math.PI * 2, lerp);
            }
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

            Action fireSpeed = () => p.ChangeSpeed(origSpeed);
            (Engine.Player as Player).OnDeath += fireSpeed;

            cannons[1].Item1.Color = Color.Yellow;
            cannons[1].Item2.Color = Color.Yellow;
            cannons[1].Item3.Color = Color.Yellow;
            float timer = 0;
            while (timer < 1)
                timer += Engine.Deltatime;

            Coroutine coroutine = new Coroutine(MachineGun2(180, 80, 10, true));

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

            (Engine.Player as Player).OnDeath -= fireSpeed;
        }

        private IEnumerator ProjectPlayer()
        {
            Player player = Platformer.player;

            float t = 0;
            float mT = 0.4f;

            player.Sprite.Play("projected");
            player.CanMove = false;
            while(t < mT)
            {
                player.Pos = Bezier.Quadratic(new Vector2[] { player.Pos, (player.Pos + new Vector2(6330, -2172)) / 2 + new Vector2(0, -20), new Vector2(6330, -2168) }, t / mT);
                player.Sprite.Rotation = MathHelper.Lerp(0, -(float)Math.PI * 2 - (float)Math.PI / 4, t / mT);
                t += Engine.Deltatime;
                yield return null;
            }

            Entity light = Engine.CurrentMap.Instantiate(new Entity(player.MiddlePos));
            light.AddComponent(new CircleLight(Vector2.Zero, 4, new Color(Color.White, 200), new Color(Color.White, 0)));

            t = 0;
            mT = 3f;
            float mT2 = 0.2f;
            while (t < mT)
            {
                /*if (t <= mT2)
                {
                    player.Pos = Bezier.Quadratic(new Vector2[] { player.Pos, (player.Pos + new Vector2(6323, -2170)) / 2 + new Vector2(0, -20), new Vector2(6323, -2170) }, t / mT2);
                    player.Sprite.Rotation = MathHelper.Lerp(-0.490887642f, -0.490887642f + (float)Math.PI * 2, t / mT2);
                }
                else
                {
                    player.Sprite.Rotation = -0.490887642f;
                    player.Pos = new Vector2(6323, -2170);
                }*/

                light.Pos = Bezier.Quadratic(new Vector2[] { light.Pos, (light.Pos + new Vector2(6306, -2086)) / 2 + new Vector2(0, -60), new Vector2(6306, -2086) }, t / mT); 

                t += Engine.Deltatime;
                yield return null;
            }

            yield return new Coroutine.WaitForSeconds(2f);

            t = 0;
            mT = 0.1f;

            float initRot = player.Sprite.Rotation;
            Vector2 initPos = Vector2.Round(player.Pos);
            while(t < mT)
            {
                player.Pos = initPos + new Vector2(0, -3) * t / mT;
                player.Sprite.Rotation = MathHelper.Lerp(initRot, 0, t / mT);
                t += Engine.Deltatime;
            }

            player.Pos = initPos + new Vector2(0, -3);
            player.Sprite.Rotation = 0;
            player.Sprite.Play("pickUpReverse");

            player.CanAnimateSprite = true;
            player.CanJetpack = false;

            light.SelfDestroy();

            yield return new Coroutine.WaitForSeconds(1.5f);

            TextBox s = (TextBox)AddChild(new TextBox("Jetpack has been lost", "LexendDeca", Engine.Cam.Pos + Engine.Cam.Size / 2, 800, 20, 1, Color.Transparent, true, TextBox.Alignement.Center));

            Levels.LevelNonRespawn.Add(iid);

            for (t = 0; t < 0.5f; t += Engine.Deltatime)
            {
                s.Color = new Color(Color.White, t / 0.5f);
                yield return null;
            }

            s.Color = Color.White;

            //yield return new Coroutine.PausedUntil(Player.JumpControls.Is);
            yield return new Coroutine.WaitForSeconds(2.5f);

            //Levels.LevelNonRespawn.Add(iid);

            for (t = 0.5f; t > 0; t -= Engine.Deltatime)
            {
                s.Color = new Color(Color.White, t / 0.5f);
                yield return null;
            }

            s.SelfDestroy();

            player.CanMove = true;
        }
    }
}
