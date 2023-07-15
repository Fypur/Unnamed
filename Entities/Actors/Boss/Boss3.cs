using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Platformer
{
    public class Boss3 : Actor
    {
        public List<Vector2> EscapePoints = new();

        public int Health = 5;
        private static bool dead;

        private readonly States[][] attackSequences = new States[][]
        {
            new States[] { States.MachineGun, States.MachineGun, States.EnergyBeam, States.Missile },
            new States[] { States.Missile, States.EnergyBeam, States.MachineGun, States.MachineGun },
            new States[] { States.EnergyBeam, States.Missile, States.Missile, States.MachineGun },
        };

        public StateMachine<States> stateMachine;
        public enum States { Waiting, Jumping, MachineGun, EnergyBeam, Missile, Dead }
        private Player player;
        private bool clockWise;

        private Vector2 jumpPos1;
        private Vector2 jumpPos2;
        private Vector2 jumpPos3;
        private int stateIndex;
        private int chosenSequence;

        private bool invulnerable;
        private float speedMult;

        public Boss3(Vector2 position) : base(position, 16, 16, 0, new Sprite(Color.Red))
        {
            player = Engine.Player as Player;
        }

        public override void Awake()
        { 
            base.Awake();

            if (dead)
            {
                Visible = false;

                AddComponent(new Coroutine(Wait()));
                IEnumerator Wait()
                {
                    yield return null;
                    Entity p = Engine.CurrentMap.Data.GetEntity<SolidPlatform>();
                    if (p != null)
                        p.SelfDestroy();
                    SelfDestroy();
                }
                return;
            }

            speedMult = GetSpeed();

            foreach (IdentifierTrigger s in Engine.CurrentMap.Data.GetEntities<IdentifierTrigger>())
            {
                if(s.Id != 3)
                    EscapePoints.Add(s.Pos);

                if (s.Id == 1)
                    jumpPos1 = s.Pos;
                else if (s.Id == 2)
                    jumpPos2 = s.Pos;
                else if (s.Id == 3)
                    jumpPos3 = s.Pos;
            }

            stateMachine = new StateMachine<States>(States.Waiting);

            stateMachine.SetStateFunctions(States.Waiting, () => AddComponent(new Coroutine(Waiting())), null, null);
            stateMachine.SetStateFunctions(States.MachineGun, () => AddComponent(new Coroutine(MachineGun())), null, null);
            stateMachine.SetStateFunctions(States.EnergyBeam, () => AddComponent(new Coroutine(EnergyBeam())), null, null);
            stateMachine.SetStateFunctions(States.Missile, () => AddComponent(new Coroutine(Missile())), null, null);
            stateMachine.SetStateFunctions(States.Jumping, () => AddComponent(new Coroutine(Jump())), null, null);
            stateMachine.SetStateFunctions(States.Dead, () => AddComponent(new Coroutine(DestroyWall())), null, null);

            AddComponent(stateMachine);

            stateMachine.Switch(States.Jumping);
        }

        public override void Update()
        {
            base.Update();

            Debug.LogUpdate("Health : " +  player.Health);
            Debug.LogUpdate("Boss Health : " +  Health);

            if (Collider.Collide(Engine.Player))
                Hit();
        }

        public IEnumerator Jump()
        {
            Sprite.Color = Color.Red;

            Vector2 init = Pos;
            Vector2 jumpPos = MiddlePos.X - Engine.CurrentMap.CurrentLevel.Pos.X < Engine.CurrentMap.CurrentLevel.Size.X / 2 ? jumpPos1 : jumpPos2;
            if (player.Collider.Collide(jumpPos))
                jumpPos = jumpPos3;

            Vector2 target = EscapePoints[Rand.NextInt(0, EscapePoints.Count)];
            while (target == Pos || target == jumpPos || player.Collider.Collide(target))
                target = EscapePoints[Rand.NextInt(0, EscapePoints.Count)];

            float t = 0.4f;
            IEnumerator enumerator;
            if ((jumpPos == jumpPos1 && Pos == jumpPos2) || (jumpPos == jumpPos2 && Pos == jumpPos1))
                enumerator = BezierJump(init, jumpPos, t, 200, null);
            else
                enumerator = BezierJump(init, jumpPos, t, 0, true);

            while(enumerator.MoveNext())
            { yield return null; }

            if ((target == jumpPos1 && Pos == jumpPos2) || (target == jumpPos2 && Pos == jumpPos1))
                enumerator = BezierJump(jumpPos, target, t + 0.1f, 200, null);
            else
                enumerator = BezierJump(jumpPos, target, t + 0.1f, 0, false);

            while (enumerator.MoveNext())
                yield return null;

            NextState();
        }

        public IEnumerator Waiting()
        {
            yield return 0;
            stateMachine.Switch(attackSequences[chosenSequence][stateIndex]);
        }

        public IEnumerator MachineGun()
        {
            Sprite.Color = Color.Yellow;
            clockWise = !clockWise;

            yield return new Coroutine.WaitForSeconds(1 * speedMult);
            yield return new Coroutine.PausedUntil(() => !Close());

            int numBullets = 3;
            float range = 10;

            float initAngle = (player.MiddlePos - MiddlePos).ToAngleDegrees();

            initAngle -= range / 2 * (clockWise ? 1 : -1);

            float increment = range / numBullets * (clockWise ? 1 : -1);
            for (int i = 0; i < numBullets; i++)
            {

                var m = Engine.CurrentMap.Instantiate(new MachineGunBullet(MiddlePos, initAngle + i * increment));
                Engine.CurrentMap.CurrentLevel.EntityData.Add(m);

                Engine.Cam.Shake(0.3f, 0.7f);
                yield return new Coroutine.WaitForSeconds(0.1f);
            }

            Sprite.Color = Color.Red;

            NextState();
        }

        public IEnumerator EnergyBeam()
        {
            Sprite.Color = Color.Green;

            Vector2 targDir = MiddlePos + (player.MiddlePos - MiddlePos).Normalized() * 1000;
            Raycast r = new Raycast(Raycast.RayTypes.MapTiles, MiddlePos, targDir, true);

            if (r.Hit)
                targDir = r.EndPoint;

            LineRenderer l = (LineRenderer)AddComponent(new LineRenderer(MiddlePos, targDir, null, 1, Color.LightCoral, null, null));

            float timer = 0.7f * speedMult;
            for (int i = 0; timer >= 0; i++)
            {
                targDir = MiddlePos + (player.MiddlePos - MiddlePos).Normalized() * 1000;
                r = new Raycast(Raycast.RayTypes.MapTiles, MiddlePos, targDir, true);
                if (r.Hit)
                    targDir = r.EndPoint;

                l.Positions[1] = targDir;

                timer -= Engine.Deltatime;
                yield return 0;
            }

            yield return new Coroutine.WaitForSeconds(0.3f * speedMult);
            yield return new Coroutine.PausedUntil(() => !Close());

            l.Thickness = 5;

            timer = 0.5f;
            for(int i = 0; timer >= 0; i++)
            {
                if (i % 2 == 0)
                    l.Thickness += 1;
                else
                    l.Thickness -= 1;

                timer -= Engine.Deltatime;

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

            Sprite.Color = Color.Red;

            RemoveComponent(l);

            NextState();
        }

        public IEnumerator Missile()
        {
            Sprite.Color = Color.Orange;
            yield return new Coroutine.WaitForSeconds(0.7f * speedMult);
            yield return new Coroutine.PausedUntil(() => !Close());

            Engine.CurrentMap.Instantiate(new HomingMissile(MiddlePos, -45));

            Sprite.Color = Color.Red;
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

        private void Hit()
        {
            if (invulnerable)
                return;

            invulnerable = true;
            AddComponent(new Timer(2, true, null, () => invulnerable = false));

            if (!stateMachine.Is(States.Jumping))
            {
                if (stateMachine.Is(States.EnergyBeam) && HasComponent<LineRenderer>(out LineRenderer l))
                {
                    int orig = l.Thickness;
                    AddComponent(new Timer(0.5f, true, (timer) => l.Thickness = (int)(orig * timer.Value / timer.MaxValue), () => RemoveComponent(l)));
                }

                RemoveComponent<Coroutine>();
            }

            Health--;

            //TODO: Hit Animation
            //Small float away from player then jump

            speedMult = GetSpeed();

            stateMachine.Switch(States.Jumping);
            stateIndex = -1;
            chosenSequence = Rand.NextInt(0, attackSequences.Length);

            Engine.Cam.LightShake();

            Vector2 dir = (MiddlePos - player.MiddlePos).Normalized();
            Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Explosion, Bounds, 20);

            AddComponent(new Timer(1, true, (t) =>
            {
                Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Fire, Bounds, 1);
                Engine.CurrentMap.MiddlegroundSystem.Emit(Particles.Spark, Bounds, 3);
            }, null));

            if (Health <= 0)
                stateMachine.Switch(States.Dead);
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

        private IEnumerator BezierJump(Vector2 init, Vector2 to, float jumpTime, float height, bool? cubicIn)
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

        private IEnumerator DestroyWall()
        {
            dead = true;

            IEnumerator e;
            if (Pos != jumpPos1)
            {
                e = BezierJump(Pos, jumpPos1, 0.5f, 50, null);
                while (e.MoveNext())
                    yield return null;
            }

            yield return new Coroutine.WaitForSeconds(1.5f);

            Sprite.Color = Color.Green;

            Vector2 aimed = Engine.CurrentMap.CurrentLevel.Pos + new Vector2(464, 112);
            Vector2 targDir = MiddlePos + (aimed - MiddlePos).Normalized() * 1000;

            LineRenderer l = (LineRenderer)AddComponent(new LineRenderer(MiddlePos, targDir, null, 1, Color.LightCoral, null, null));

            yield return new Coroutine.WaitForSeconds(0.7f);

            //Destroy wall
            //Instantiate Camera Fire
            Engine.CurrentMap.Data.GetEntity<SolidPlatform>().SelfDestroy();
            PushingFire p = (PushingFire)Engine.CurrentMap.Instantiate(new PushingFire(Engine.CurrentMap.CurrentLevel.Pos, 64));
            p.Height = Engine.CurrentMap.CurrentLevel.Height;
            p.GetComponent<HurtBox>().Trigger.Height = p.Height;

            player.Health = 1;

            l.Thickness = 5;

            float timer = 0.5f;
            for (int i = 0; timer >= 0; i++)
            {
                if (i % 2 == 0)
                    l.Thickness += 1;
                else
                    l.Thickness -= 1;

                timer -= Engine.Deltatime;

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

            Sprite.Color = Color.Red;

            RemoveComponent(l);

            yield return new Coroutine.WaitForSeconds(1);

            e = BezierJump(Pos, Engine.CurrentMap.CurrentLevel.Pos + new Vector2(Engine.CurrentMap.CurrentLevel.Size.X, 0), 1.5f, 300, false);

            while (e.MoveNext())
                yield return null;
        }
    }
}
