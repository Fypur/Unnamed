using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class ParticleSoundEmitter : Entity
    {
        public Guid Iid;
        public ParticleSoundEmitter(Vector2 position, ParticleType particleType, int amount, float? direction, Color color, Guid iid) : base(position)
        {
            AddComponent(new ParticleEmitter(Engine.CurrentMap.BackgroundSystem, particleType, Vector2.Zero, amount, direction, color));
            AddComponent(new Sound3D("Ambience/WaterLeak"));

            Iid = iid;
            Levels.LevelNonRespawn.Add(Iid);
        }

        public override void Awake()
        {
            base.Awake();

            LevelTransition.DontDestroyOnDeath(this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Levels.LevelNonRespawn.Remove(Iid);
        }
    }
}
