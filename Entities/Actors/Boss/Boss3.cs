using Fiourp;
using LDtkTypes;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Platformer
{
    public class Boss3 : Actor
    {
        public List<Vector2> EscapePoints = new();

        public int Health = 5;

        private readonly States[][] attackSequences = new States[][]
        {
            new States[] { States.MachineGun, States.MachineGun, States.EnergyBeam, States.Missile },
            new States[] { States.Missile, States.EnergyBeam, States.MachineGun, States.MachineGun },
            new States[] { States.EnergyBeam, States.Missile, States.Missile, States.MachineGun },
        };

        public StateMachine<States> stateMachine;
        public enum States { Waiting, Jumping, MachineGun, EnergyBeam, Missile }
        private Player player;
        private bool clockWise;

        private Vector2 jumpPos1;
        private Vector2 jumpPos2;
        private int stateIndex;
        private int chosenSequence;

        private bool invulnerable;

        public Boss3(Vector2 position) : base(position, 16, 16, 0, new Sprite(Color.Red))
        {
            player = Engine.Player as Player;
        }

        public override void Awake()
        {
            base.Awake();

            foreach (IdentifierTrigger s in Engine.CurrentMap.Data.GetEntities<IdentifierTrigger>())
            {
                EscapePoints.Add(s.Pos);
                if (s.Id == 1)
                    jumpPos1 = s.Pos;
                else if (s.Id == 2)
                    jumpPos2 = s.Pos;
            }

            stateMachine = new StateMachine<States>(States.Waiting);

            stateMachine.SetStateFunctions(States.Waiting, () => AddComponent(new Coroutine(Waiting())), null, null);
            stateMachine.SetStateFunctions(States.MachineGun, () => AddComponent(new Coroutine(MachineGun())), null, null);
            stateMachine.SetStateFunctions(States.EnergyBeam, () => AddComponent(new Coroutine(EnergyBeam())), null, null);
            stateMachine.SetStateFunctions(States.Missile, () => AddComponent(new Coroutine(Missile())), null, null);
            stateMachine.SetStateFunctions(States.Jumping, () => AddComponent(new Coroutine(Jump())), null, null);

            AddComponent(stateMachine);

            stateMachine.Switch(States.Jumping);

            //TODO: Implement missile states
            //TODO: Implement jumping
        }

        public override void Update()
        {
            base.Update();

            Debug.LogUpdate(stateMachine.CurrentState);
            Debug.LogUpdate(stateIndex);
            Debug.LogUpdate(chosenSequence);

            if (Collider.Collide(Engine.Player))
                Hit();
        }

        public IEnumerator Jump()
        {
            //Teleport();
            Vector2 init = Pos;
            Vector2 jumpPos = MiddlePos.X - Engine.CurrentMap.CurrentLevel.Pos.X < Engine.CurrentMap.CurrentLevel.Size.X ? jumpPos1 : jumpPos2;
            Vector2 target = EscapePoints[Rand.NextInt(0, EscapePoints.Count)];
            while (target == Pos || target == jumpPos)
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
            { yield return null; }

            IEnumerator BezierJump(Vector2 init, Vector2 to, float jumpTime, float height, bool? cubicIn)
            {
                float time = 0;

                Vector2[] controlPoints = new Vector2[] { init, new Vector2((init.X + to.X) / 2, init.Y - height), to };
                while (time < jumpTime)
                {
                    if(cubicIn == true)
                        Pos.Y = Bezier.Quadratic(controlPoints, Ease.CubicIn(time / jumpTime)).Y;
                    else if(cubicIn == false)
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

            yield return new Coroutine.WaitForSeconds(1);

            int numBullets = 3;
            float range = 10;

            float initAngle = (player.MiddlePos - MiddlePos).ToAngleDegrees();

            initAngle += range / 2 * (clockWise ? 1 : -1);

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

            yield return new Coroutine.WaitForSeconds(0.9f);
            Vector2 targDir = 1000 * (player.MiddlePos - MiddlePos).Normalized();
            yield return new Coroutine.WaitForSeconds(0.1f);

            LineRenderer l = (LineRenderer)AddComponent(new LineRenderer(MiddlePos, MiddlePos + targDir, null, 5, Color.LightCoral, null, null));

            float timer = 1;
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
            yield return new Coroutine.WaitForSeconds(0.7f);

            Engine.CurrentMap.Instantiate(new HomingMissile(MiddlePos, -45));

            Sprite.Color = Color.Red;
            yield return new Coroutine.WaitForSeconds(1);

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
            AddComponent(new Timer(1, true, null, () => invulnerable = false));

            Health--;
            if (Health <= 0)
            {
                SelfDestroy();
                return;
            }

            if(!stateMachine.Is(States.Jumping))
                RemoveComponent<Coroutine>();
            stateMachine.Switch(States.Jumping);
            stateIndex = -1;
            chosenSequence = Rand.NextInt(0, attackSequences.Length);

            Engine.Cam.LightShake();

            //Accelerate attack speed on hit, slow down initial attack speeds
        }
    }
}
