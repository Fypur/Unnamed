using Fiourp;
using LDtkTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.ConstrainedExecution;

namespace Platformer
{
    public class Boss3 : Actor
    {
        public List<Vector2> EscapePoints = new();

        public int Health = 4;
        public static bool Dead = false;

        private readonly States[][] attackSequences = new States[][]
        {
            new States[] { States.MachineGun, States.MachineGun, States.EnergyBeam, States.Missile },
            new States[] { States.Missile, States.EnergyBeam, States.MachineGun, States.MachineGun },
            new States[] { States.EnergyBeam, States.Missile, States.Missile, States.MachineGun },
        };

        public StateMachine<States> stateMachine;
        public enum States { Waiting, Jumping, MachineGun, EnergyBeam, Missile, Dead, Hit, Cinematic }
        private Player player;
        private bool clockWise;

        private BoxColliderRotated rotColl;
        private Vector2 jumpPos0;
        private Vector2 jumpPos1;
        private Vector2 jumpPos2;
        private int stateIndex;
        private int chosenSequence;

        private bool invulnerable;
        private float speedMult;

        private Sprite cannonPart1;
        private Sprite cannonPart2;
        private Sprite cannon;
        private Vector2 cannonPos;
        public const float CannonLength = 12;

        private ParticleType dust;
        private bool invicibleTimer;
        private List<float> circleLengths = new();

        private static Tile healthTile;
        public static Tile PlayerHealthTile;
        private int healthTileIndex = 1;

        private float rotation
        {
            get => rotColl.Rotation;
            set
            {
                rotColl.Rotation = value;
                Sprite.Rotation = value;
            }
        }

        public Boss3(Vector2 position) : base(position, 24, 16, 0, new Sprite(Color.White))
        {
            player = Engine.Player as Player;

            RemoveComponent(Collider);
            rotColl = new BoxColliderRotated(Vector2.Zero, Width, Height, 0, HalfSize);
            Collider = rotColl;
            AddComponent(Collider);


            cannonPart1 = new Sprite(DataManager.Textures["Boss/arm"]);
            cannon = new Sprite(DataManager.Textures["Boss/gunArm"]);

            cannonPart1.Origin = Vector2.UnitY;
            cannonPart2 = cannonPart1.Copy();

            AddComponent(cannonPart1);
            AddComponent(cannonPart2);
            AddComponent(cannon);

            Sprite.Add(Sprite.AllAnimData["Boss"]);
            Sprite.Origin = HalfSize;
            Sprite.Offset = HalfSize;

            dust = Particles.Dust.Copy();
        }

        public override void Awake()
        {
            base.Awake();

            player.Health = 3;

            healthTile = (Tile)Engine.CurrentMap.Instantiate(new Tile(new Vector2(211, 120), 34, 19, new Sprite(DataManager.Objects["scenery/bossScreen1"])));
            PlayerHealthTile = (Tile)Engine.CurrentMap.Instantiate(new Tile(new Vector2(218, 162), 22, 14, new Sprite(DataManager.Objects["scenery/playerHealth4"])));
            PlayerHealthTile.Layer = -1;
            healthTile.Layer = -1;
            player.OnDamage += (health) => PlayerHealthTile.Sprite.Texture = DataManager.Objects["scenery/playerHealth" + (health + 1).ToString()];

            speedMult = GetSpeed();

            List<IdentifierTrigger> idTrigs = Engine.CurrentMap.Data.GetEntities<IdentifierTrigger>();

            idTrigs.RemoveAll((trig) => trig.Id >= 100);
            idTrigs.Sort((t1, t2) => t1.Id.CompareTo(t2.Id));

            jumpPos0 = idTrigs[0].Pos;
            jumpPos1 = idTrigs[1].Pos;
            jumpPos2 = idTrigs[idTrigs.Count - 1].Pos;
            idTrigs.RemoveAt(idTrigs.Count - 1);

            foreach (IdentifierTrigger s in idTrigs)
                EscapePoints.Add(s.Pos);

            stateMachine = new StateMachine<States>(States.Waiting);

            stateMachine.SetStateFunctions(States.Waiting, () => AddComponent(new Coroutine(Waiting())), null, null);
            stateMachine.SetStateFunctions(States.MachineGun, () => AddComponent(new Coroutine(MachineGun())), null, null);
            stateMachine.SetStateFunctions(States.EnergyBeam, () => AddComponent(new Coroutine(EnergyBeam())), null, null);
            stateMachine.SetStateFunctions(States.Missile, () => AddComponent(new Coroutine(Missile())), null, null);
            stateMachine.SetStateFunctions(States.Jumping, () => AddComponent(new Coroutine(Jump())), null, null);
            stateMachine.SetStateFunctions(States.Dead, () => AddComponent(new Coroutine(DestroyWall())), null, null);

            AddComponent(stateMachine);

            if (Dead)
            {
                Visible = false;

                AddComponent(new Coroutine(Wait()));
                IEnumerator Wait()
                {
                    yield return null;
                    Entity pl = Engine.CurrentMap.Data.GetEntity<SolidPlatform>();
                    if (pl != null)
                        pl.SelfDestroy();
                    SelfDestroy();

                    player.Health = 1;
                    healthTile.Sprite.Texture = DataManager.Objects["scenery/bossScreen5"];

                    if (Engine.CurrentMap.Data.GetEntity<PushingFire>() == null)
                    {
                        PushingFire p = (PushingFire)Engine.CurrentMap.Instantiate(new PushingFire(Engine.CurrentMap.CurrentLevel.Pos, 64));
                        p.Height = Engine.CurrentMap.CurrentLevel.Height;
                        p.GetComponent<HurtBox>().Trigger.Height = p.Height;
                        p.ChangeSpeed(64);
                    }

                }
                return;
            }

            stateMachine.Switch(States.Cinematic);
            Visible = false;
        }

        public void Start(bool fast)
        {
            //throw new NotImplementedException();
            Visible = true;

            if (!fast)
            {
                Pos = Pos + new Vector2(120, -208);
                invulnerable = true;
                AddComponent(new Coroutine(BezierJump(Pos, Pos - new Vector2(120, -208), 0.4f, 0, true), Coroutine.WaitSeconds(1), Scream()));
            }
            else
                AddComponent(new Coroutine(Scream()));
        }

        public override void Update()
        {
            base.Update();

            if(!stateMachine.Is(States.EnergyBeam) && !stateMachine.Is(States.Dead))
                SetCannonPos();

            if (!stateMachine.Is(States.Dead) && Collider.Collide(Engine.Player))
                Hit();
        }

        public IEnumerator Jump()
        {
            invulnerable = true;
            Vector2 init = Pos;
            Vector2 jumpPos = MiddlePos.X - Engine.CurrentMap.CurrentLevel.Pos.X < Engine.CurrentMap.CurrentLevel.Size.X / 2 ? jumpPos0 : jumpPos1;

            if (Pos == jumpPos0)
                jumpPos = jumpPos1;
            else if (Pos == jumpPos1)
                jumpPos = jumpPos0;

            if (Vector2.DistanceSquared(player.Pos, jumpPos) <= 40 * 40)
                jumpPos = jumpPos2;

            int id = Rand.NextInt(0, EscapePoints.Count);
            Vector2 target = EscapePoints[id];
            while (target == Pos || target == jumpPos || Vector2.DistanceSquared(player.Pos, target) <= 40 * 40)
            {
                id = Rand.NextInt(0, EscapePoints.Count);
                target = EscapePoints[id];
            }

            AddComponent(new Sound3D("SFX/Boss/Jump", autoRemove: true));

            IEnumerator enumerator;
            float t = 0.4f;
            SwitchRot(0, t);
            AddComponent(new Timer(t - 0.3f, true, null, () =>
            {
                AddComponent(new Sound3D("SFX/Boss/JumpLandSlam", autoRemove: true));
            }));
            if ((jumpPos == jumpPos0 && Pos == jumpPos1) || (jumpPos == jumpPos1 && Pos == jumpPos0))
                enumerator = BezierJump(init, jumpPos, t, 200, null);
            else
                enumerator = BezierJump(init, jumpPos, t, 0, true);

            while (enumerator.MoveNext())
            { yield return null; }
            EmitLandingParticules();

            t += 0.1f;
            SwitchRot(id, t);
            AddComponent(new Timer(t - 0.3f, true, null, () =>
            {
                AddComponent(new Sound3D("SFX/Boss/JumpLandSlam", autoRemove: true));
            }));
            if ((target == jumpPos0 && Pos == jumpPos1) || (target == jumpPos1 && Pos == jumpPos0))
                enumerator = BezierJump(jumpPos, target, t, 200, null);
            else
                enumerator = BezierJump(jumpPos, target, t, 0, false);

            while (enumerator.MoveNext())
                yield return null;
            EmitLandingParticules();

            if(!invicibleTimer)
                invulnerable = false;
            NextState();
        }

        private void EmitLandingParticules()
        {
            Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, rotColl.Rect[2], rotColl.Rect[3], 30, rotation);
            Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, rotColl.Rect[2], rotColl.Rect[3], 30, rotation + 180);
        }

        private void SwitchRot(int rotId, float time)
        {
            float initRot = rotation;
            float toRot;

            switch (rotId)
            {
                case 0: case 1:
                    toRot = 0;
                    break;
                case 2:
                    toRot = 90;
                    break;
                case 3:
                    toRot = 270;
                    break;
                case 4:
                    toRot = 180;
                    break;
                default:
                    toRot = 0;
                    break;
            }

            if (initRot == toRot)
                return;

            toRot = MathHelper.ToRadians(toRot);

            AddComponent(new Timer(time, true, (timer) =>
            {
                if (initRot > (float)Math.PI / 2 && initRot < (float)Math.PI)
                    rotation = MathHelper.Lerp(initRot, toRot + (float)Math.PI * 2, timer.AmountCompleted());
                else
                    rotation = MathHelper.Lerp(initRot, toRot, timer.AmountCompleted());

            }, () =>
            {

                rotation = toRot;
            }
            ));
        }

        public IEnumerator Waiting()
        {
            cannonPart1.Color = Color.White;
            cannonPart2.Color = Color.White;
            cannon.Color = Color.White;

            float approxRot = (float)Math.Round(MathHelper.ToDegrees(rotation));

            if (approxRot == 90)
                Sprite.Play("wallRight");
            else if(approxRot == 270)
                Sprite.Play("wallRight");
            else
                Sprite.Play("idle");
            yield return 0;
            stateMachine.Switch(attackSequences[chosenSequence][stateIndex]);
        }

        public IEnumerator MachineGun()
        {
            cannonPart1.Color = Color.Yellow;
            cannonPart2.Color = Color.Yellow;
            cannon.Color = Color.Yellow;
            clockWise = !clockWise;

            yield return new Coroutine.WaitForSeconds(1 * speedMult);
            yield return new Coroutine.PausedUntil(() => !Close());

            int numBullets = 3;
            float range = 10;

            float initAngle = (player.MiddlePos - cannonPos).ToAngleDegrees();

            initAngle -= range / 2 * (clockWise ? 1 : -1);

            float increment = range / numBullets * (clockWise ? 1 : -1);
            for (int i = 0; i < numBullets; i++)
            {

                var m = Engine.CurrentMap.Instantiate(new MachineGunBullet(cannonPos, initAngle + i * increment));
                Engine.CurrentMap.CurrentLevel.EntityData.Add(m);

                Engine.Cam.Shake(0.3f, 0.7f);
                yield return new Coroutine.WaitForSeconds(0.1f);
            }

            cannonPart1.Color = Color.White;
            cannonPart2.Color = Color.White;
            cannon.Color = Color.White;

            NextState();
        }

        public IEnumerator EnergyBeam()
        {
            cannonPart1.Color = Color.Green;
            cannonPart2.Color = Color.Green;
            cannon.Color = Color.Green;

            Vector2 targDir = cannonPos + (player.MiddlePos - cannonPos).Normalized() * 1000;
            Raycast r = new Raycast(Raycast.RayTypes.MapTiles, cannonPos, targDir, true);

            if (r.Hit)
                targDir = r.EndPoint;

            LineRenderer l = (LineRenderer)AddComponent(new LineRenderer(cannonPos, targDir, null, 1, Color.LightCoral, null, null));

            Sound3D sfx = new("SFX/Boss/ChargeBeam", autoRemove: true);
            AddComponent(sfx);

            float timer = 0.7f * speedMult;
            for (int i = 0; timer >= 0; i++)
            {
                targDir = cannonPos + (player.MiddlePos - cannonPos).Normalized() * 1000;
                r = new Raycast(Raycast.RayTypes.MapTiles, cannonPos, targDir, true);
                if (r.Hit)
                    targDir = r.EndPoint;

                SetCannonPos();
                l.Positions[0] = cannonPos; 
                l.Positions[1] = targDir;

                timer -= Engine.Deltatime;
                yield return 0;
            }

            yield return new Coroutine.WaitForSeconds(0.3f * speedMult);
            yield return new Coroutine.PausedUntil(() => !Close());

            sfx.Stop();
            AddComponent(new Sound3D("SFX/Boss/BeamExplode", autoRemove: true));

            l.Thickness = 5;
            ParticleType fastFire = Particles.Fire.Copy();
            fastFire.SpeedMin = 10;
            fastFire.SpeedMax = 20;
            fastFire.Acceleration = Vector2.Zero;

            timer = 0.5f;
            for(int i = 0; timer >= 0; i++)
            {
                if (i % 2 == 0)
                    l.Thickness += 1;
                else
                    l.Thickness -= 1;


                timer -= Engine.Deltatime;
                l.Positions[0] = cannonPos;

                Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, l.Positions[0], 4);

                float n1 = VectorHelper.Normal(l.Positions[0] - l.Positions[1]).ToAngleDegrees();
                float n2 = VectorHelper.Normal2(l.Positions[0] - l.Positions[1]).ToAngleDegrees();
                Engine.CurrentMap.MiddlegroundSystem.Emit(fastFire, l.Positions[0], l.Positions[1], 1, n1);
                Engine.CurrentMap.MiddlegroundSystem.Emit(fastFire, l.Positions[0], l.Positions[1], 1, n2);


                if (Collision.LineBoxCollision((BoxCollider)player.Collider, l.Positions[0], l.Positions[1]))
                    player.Damage();

                Engine.Cam.LightShake();
                yield return 0;
            }

            timer = 0.3f;
            for (int i = 0; timer >= 0; i++)
            {
                l.Thickness = (int)(5 * timer / 0.3f);
                timer -= Engine.Deltatime;

                yield return 0;
            }

            cannonPart1.Color = Color.White;
            cannonPart2.Color = Color.White;
            cannon.Color = Color.White;

            RemoveComponent(l);

            NextState();
        }

        public IEnumerator Missile()
        {
            cannonPart1.Color = Color.Orange;
            cannonPart2.Color = Color.Orange;
            cannon.Color = Color.Orange;
            yield return new Coroutine.WaitForSeconds(0.7f * speedMult);
            yield return new Coroutine.PausedUntil(() => !Close());

            Engine.CurrentMap.Instantiate(new HomingMissile(cannonPos, -45));

            cannonPart1.Color = Color.White;
            cannonPart2.Color = Color.White;
            cannon.Color = Color.White;
            yield return new Coroutine.WaitForSeconds(1 * speedMult);

            NextState();
        }

        public void Teleport()
        {
            Vector2 farthest = Vector2.Zero;
            float d = 0;
            float d2;
            foreach(Vector2 v in EscapePoints)
            {
                d2 = Vector2.DistanceSquared(v, player.MiddlePos);
                if (d2 >= d)
                {
                    farthest = v;
                    d = d2;
                }
            }

            Pos = farthest;
            //Pos = EscapePoints[Rand.NextInt(0, EscapePoints.Count)];
        }

        private void NextState()
        {
            stateIndex++;

            if(stateIndex >= attackSequences[chosenSequence].Length)
            {
                stateMachine.Switch(States.Jumping);
                stateIndex = -1;
                chosenSequence = Rand.NextInt(0, attackSequences.Length);
            }
            else
                stateMachine.Switch(States.Waiting);
        }

        public void Hit()
        {
            if (invulnerable)
                return;

            invulnerable = true;
            invicibleTimer = true;
            AddComponent(new Timer(2, true, null, () => { invulnerable = false; invicibleTimer = false;  }));

            if (!stateMachine.Is(States.Jumping))
            {
                if (stateMachine.Is(States.EnergyBeam) && HasComponent<LineRenderer>(out LineRenderer l))
                {
                    int orig = l.Thickness;
                    AddComponent(new Timer(0.5f, true, (timer) => l.Thickness = (int)(orig * timer.Value / timer.MaxValue), () => RemoveComponent(l)));
                }

                RemoveComponent<Coroutine>();
                AddComponent(new Coroutine(Hit2()));
            }

            IEnumerator Hit2()
            {
                Health--;
                healthTileIndex++;
                healthTile.Sprite.Texture = DataManager.Objects["scenery/bossScreen" + healthTileIndex.ToString()];

                string path = Health is 0
                    ? "SFX/Boss/HitFinal"
                    : "SFX/Boss/Hit";
                AddComponent(new Sound3D(path, true));

                stateMachine.Switch(States.Hit);

                Sprite.Play("hit");
                Sprite.Origin = new Vector2(24);
                Sprite.OnChange = () =>
                {
                    Sprite.Origin = HalfSize;
                    Sprite.OnChange = null;
                };

                Platformer.Freeze(0.2f);
                Engine.Cam.Shake(0.4f, 2);

                yield return new Coroutine.WaitForSeconds(0.4f);

                AddComponent(new Timer(0.3f, true, (t) =>
                {
                    Platformer.TimeScale = 0.2f + t.AmountCompleted() * 0.7f;
                }, null));

                AddComponent(new Timer(0.2f, true, (t) =>
                {
                    Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Spark, Bounds, 3);
                }, null));

                //yield return new Coroutine.WaitForSeconds(0.7f);

                AddComponent(new Timer(1, true, (t) =>
                {
                    Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Fire, Bounds, 1);
                }, null));

                Engine.Cam.LightShake();


                speedMult = GetSpeed();

                stateMachine.Switch(States.Jumping);
                stateIndex = -1;
                chosenSequence = Rand.NextInt(0, attackSequences.Length);

                Engine.Cam.LightShake();

                Vector2 dir = (MiddlePos - player.MiddlePos).Normalized();
                Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Explosion, Bounds, 20);

                if (Health <= 0)
                    stateMachine.Switch(States.Dead);
            }
        }

        private bool Close()
            => Vector2.DistanceSquared(MiddlePos, player.MiddlePos) < 30 * 30;

        private float GetSpeed()
        {
            switch (Health)
            {
                case 1: return 0.5f;
                case 2: return 0.7f;
                case 3: return 1;
                case 4: return 1;
                case 5: return 1;
            }

            return 1;
        }

        public IEnumerator BezierJump(Vector2 init, Vector2 to, float jumpTime, float height, bool? cubicIn)
        {
            float time = 0;

            Vector2[] controlPoints = new Vector2[] { init, new Vector2((init.X + to.X) / 2, init.Y - height), to };
            while (time < jumpTime)
            {
                if (cubicIn == true)
                    Pos.Y = Bezier.Quadratic(controlPoints, Ease.CubicIn(time / jumpTime)).Y;
                else if (cubicIn == false)
                    Pos.Y = Bezier.Quadratic(controlPoints, Ease.CubicOut(time / jumpTime)).Y;
                else
                    Pos.Y = Bezier.Quadratic(controlPoints, time / jumpTime).Y;

                Pos.X = Vector2.Lerp(init, to, time / jumpTime).X;

                time += Engine.Deltatime;
                yield return 0;
            }

            Pos = to;

            Engine.Cam.LightShake();

            Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, 6, new Rectangle((int)Pos.X, (int)Pos.Y + Height - 3, Width, 3), null, 0, Particles.Dust.Color);
            Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, 6, new Rectangle((int)Pos.X, (int)Pos.Y + Height - 3, Width, 3), null, 180, Particles.Dust.Color);
        }

        public override void Render()
        {
            base.Render();

            //build a boss health bar and player health bar

            Drawing.EndPrimitives();
            Drawing.BeginPrimitives(Engine.PrimitivesRenderTarget, null, BlendState.Opaque);

            foreach(float c in circleLengths)
            {
                if (c > 0)
                    Drawing.DrawCircle(MiddlePos, c, 0.1f, Color.White);

                float t = 1.02f * c - 12;
                if(t > 0)
                    Drawing.DrawCircle(MiddlePos, t, 0.1f, Color.Transparent);
            }

            Drawing.EndPrimitives();
            Drawing.BeginPrimitives(Engine.PrimitivesRenderTarget);
        }

        public void SetCannonPos(float jitter = 1)
        {
            float finalRot = (player.MiddlePos - MiddlePos).ToAngleRad();

            float rot1 = rotation + 3 * (float)Math.PI / 2 + Rand.NextFloat(-1, 1) * (float)Math.PI / 4;
            if (rot1 < 0)
                rot1 += (float)Math.PI * 2;
            rot1 %= (float)Math.PI * 2;

            float rot2 = (float)(Rand.NextDouble() * Math.PI * 2);

            cannonPart2.Rotation += 0.01f * jitter;
            cannonPart1.Rotation = Rot(cannonPart1.Rotation, rot1, 0.05f * jitter);
            cannonPart2.Rotation = Rot(cannonPart2.Rotation, rot2, 0.02f * jitter);
            cannon.Rotation = finalRot;


            cannonPart1.Offset = rotColl.Rect[0] - Pos + VectorHelper.Rotate(new Vector2(6, 4), rotation);
            cannonPart2.Offset = cannonPart1.Offset + VectorHelper.Rotate(new Vector2(CannonLength, 0), cannonPart1.Rotation);
            cannon.Offset = cannonPart2.Offset + VectorHelper.Rotate(new Vector2(CannonLength, 0), cannonPart2.Rotation);

            cannonPos = Pos + cannon.Offset + VectorHelper.Rotate(new Vector2(21, 3), cannon.Rotation);
        }

        private float Rot(float from, float to, float lerp)
        {
            if (Math.Abs(to - from) < Math.Abs(to - (float)Math.PI * 2 - from))
                return MathHelper.Lerp(from, to, lerp);
            else
                return MathHelper.Lerp(from, to - (float)Math.PI * 2, lerp);
        }

        private IEnumerator Scream()
        {
            invulnerable = true;
            player.Velocity += (player.MiddlePos - MiddlePos).Normalized() * 300 + new Vector2(0, -200);

            Platformer.Music.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);

            if(state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
                Platformer.Music = Audio.PlayEvent("Soundtrack/Chase");
            else
            {
                Platformer.Music.getDescription(out var description);
                description.getPath(out var path);
                if (!path.Contains("Chase"))
                {
                    Audio.StopEvent(Platformer.Music, true);
                    Platformer.Music = Audio.PlayEvent("Soundtrack/Chase");
                }
            }

            

            Engine.Cam.Shake(0.5f * 5, 1);
            AddComponent(new Coroutine(FreezeInput(0.5f * 5 + 2)));

            for (int i = 0; i < 5; i++)
            {
                AddComponent(new Coroutine(AddCircle()));
                yield return new Coroutine.WaitForSeconds(0.5f);
            }

            //Scream sfx

            yield return new Coroutine.WaitForSeconds(2);

            circleLengths.Clear();
            stateMachine.Switch(States.Jumping);
            invulnerable = false;


            IEnumerator AddCircle()
            {
                int index = circleLengths.Count;
                circleLengths.Add(1);
                for(int i = 1; i < 500; i++)
                {
                    if (index >= circleLengths.Count)
                        break;

                    circleLengths[index] = i * 3;
                    yield return null;
                }

                if (index < circleLengths.Count)
                    circleLengths[index] = 0;
            }

            IEnumerator FreezeInput(float t)
            {
                Input.State s = Input.CurrentState;
                //Input.State oldS = Input.OldState;

                float time = 0;
                while(time < t)
                {
                    Input.CurrentState = new Input.State();
                    //Input.OldState = new Input.State();
                    time += Engine.Deltatime;
                    yield return null;
                }

                Input.CurrentState = s;
                //Input.OldState = oldS;
            }
        }

        private IEnumerator DestroyWall()
        {
            Dead = true;

            AddComponent(new Sound3D("SFX/Boss/Jump", autoRemove: true));
            AddComponent(new Timer(0.2f, true, null, () =>
            {
                AddComponent(new Sound3D("SFX/Boss/JumpLandSlam", autoRemove: true));
            }));

            IEnumerator e;
            if (Pos != jumpPos0)
            {
                SwitchRot(0, 0.5f);
                e = BezierJump(Pos, jumpPos0, 0.5f, 50, null);
                while (e.MoveNext())
                    yield return null;
            }

            Vector2 aimed = Engine.CurrentMap.CurrentLevel.Pos + new Vector2(464, 112);
            Vector2 targDir = MiddlePos + (aimed - MiddlePos).Normalized() * 1000;

            float timer = 1.5f;
            while (timer >= 0)
            {
                SetCannonPos();
                cannon.Rotation = (targDir - MiddlePos).ToAngleRad();
                cannonPos = Pos + cannon.Offset + VectorHelper.Rotate(new Vector2(21, 3), cannon.Rotation);

                timer -= Engine.Deltatime;
                yield return 0;
            }

            cannonPart1.Color = Color.Green;
            cannonPart2.Color = Color.Green;
            cannon.Color = Color.Green;
            
            LineRenderer l = (LineRenderer)AddComponent(new LineRenderer(cannonPos, targDir, null, 1, Color.LightCoral, null, null));

            Sound3D sfx = new("SFX/Boss/ChargeBeam", autoRemove: true);
            AddComponent(sfx);

            timer = 0.7f * speedMult;
            while(timer >= 0)
            {   
                SetCannonPos();
                cannon.Rotation = (targDir - MiddlePos).ToAngleRad();
                cannonPos = Pos + cannon.Offset + VectorHelper.Rotate(new Vector2(21, 3), cannon.Rotation);

                l.Positions[0] = cannonPos;

                timer -= Engine.Deltatime;
                yield return 0;
            }

            yield return new Coroutine.WaitForSeconds(0.7f);
            sfx.Stop();

            AddComponent(new Sound3D("SFX/Boss/BeamExplode", autoRemove: true));

            Engine.CurrentMap.Data.GetEntity<SolidPlatform>().SelfDestroy();
            PushingFire p = (PushingFire)Engine.CurrentMap.Instantiate(new PushingFire(Engine.CurrentMap.CurrentLevel.Pos, 64));
            p.Height = Engine.CurrentMap.CurrentLevel.Height;
            p.GetComponent<HurtBox>().Trigger.Height = p.Height;

            player.Health = 1;

            cannonPart1.Color = Color.Green;
            cannonPart2.Color = Color.Green;
            cannon.Color = Color.Green;

            l.Thickness = 5;
            ParticleType fastFire = Particles.Fire.Copy();
            fastFire.SpeedMin = 10;
            fastFire.SpeedMax = 20;
            fastFire.Acceleration = Vector2.Zero;

            SetCannonPos();
            cannon.Rotation = (targDir - cannon.Offset - MiddlePos).ToAngleRad();
            cannonPos = Pos + cannon.Offset + VectorHelper.Rotate(new Vector2(21, 3), cannon.Rotation);

            timer = 0.5f;
            for (int i = 0; timer >= 0; i++)
            {
                if (i % 2 == 0)
                    l.Thickness += 1;
                else
                    l.Thickness -= 1;


                timer -= Engine.Deltatime;
                l.Positions[0] = cannonPos;

                Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Dust, l.Positions[0], 4);

                float n1 = VectorHelper.Normal(l.Positions[0] - l.Positions[1]).ToAngleDegrees();
                float n2 = VectorHelper.Normal2(l.Positions[0] - l.Positions[1]).ToAngleDegrees();
                Engine.CurrentMap.MiddlegroundSystem.Emit(fastFire, l.Positions[0], l.Positions[1], 1, n1);
                Engine.CurrentMap.MiddlegroundSystem.Emit(fastFire, l.Positions[0], l.Positions[1], 1, n2);


                if (Collision.LineBoxCollision((BoxCollider)player.Collider, l.Positions[0], l.Positions[1]))
                    player.Damage();

                Engine.Cam.LightShake();
                yield return 0;
            }

            timer = 0.3f;
            for (int i = 0; timer >= 0; i++)
            {
                l.Thickness = (int)(5 * timer / 0.3f);
                timer -= Engine.Deltatime;

                yield return 0;
            }

            cannonPart1.Color = Color.White;
            cannonPart2.Color = Color.White;
            cannon.Color = Color.White;

            RemoveComponent(l);

            yield return new Coroutine.WaitForSeconds(1);

            AddComponent(new Sound3D("SFX/Boss/Jump", autoRemove: true));
            e = BezierJump(Pos, Engine.CurrentMap.CurrentLevel.Pos + new Vector2(Engine.CurrentMap.CurrentLevel.Size.X, 0), 1.5f, 300, false);

            while (e.MoveNext())
                yield return null;
        }

        public override void OnDestroy()
        {
            var l = Engine.CurrentMap.Data.GetEntities<HomingMissile>();
            foreach (var e in l)
                e.SelfDestroy();

            healthTile.SelfDestroy();
            PlayerHealthTile.SelfDestroy();

            base.OnDestroy();
        }
    }
}
