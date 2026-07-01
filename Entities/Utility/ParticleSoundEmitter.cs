using Fiourp;
using Microsoft.Xna.Framework;
using System;

namespace Unnamed
{
    public class ParticleSoundEmitter : Entity
    {
        public Guid Iid;
        public ParticleSoundEmitter(Vector2 position, ParticleType particleType, int amount, float? direction, Color color, Guid iid) : base(position)
        {
            AddComponent(new ParticleEmitter(Engine.CurrentMap.BackgroundSystem, particleType, Vector2.Zero, amount, direction, color));
            AddComponent(new Sound3D("Ambience/WaterLeak"));

            Iid = iid;
            LevelManager.NonRespawnEntityIIds.Add(Iid);
        }

        public override void Awake()
        {
            base.Awake();

            LevelTransition.DontDestroyOnDeath(this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            LevelManager.NonRespawnEntityIIds.Remove(Iid);
        }
    }
}
