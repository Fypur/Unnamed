using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiourp;
using Microsoft.Xna.Framework;

namespace Platformer
{
    public class JetpackBooster : PlayerTrigger
    {
        private Vector2 boostingDir;

        public JetpackBooster(Rectangle bounds, Direction direction) : base(bounds, new Sprite(Color.Orange))
        {
            Sprite.Color.A = 100;
            boostingDir = DirectionToVector2(direction);
        }

        public JetpackBooster(Vector2 position, Vector2 size, Direction direction) : base(position, size, new Sprite(Color.Orange))
        {
            Sprite.Color.A = 100;
            boostingDir = DirectionToVector2(direction);
        }

        public JetpackBooster(Vector2 position, int width, int height, Direction direction) : base(position, width, height, new Sprite(Color.Orange))
        {
            Sprite.Color.A = 100;
            boostingDir = DirectionToVector2(direction);
        }

        public override void OnTriggerEnter(Entity entity)
        {
            Player p = entity as Player;
            p.jetpackPowerCoef += boostingDir * new Vector2(0.5f, 0.3f);
        }

        public override void OnTriggerExit(Entity entity)
        {
            Player p = entity as Player;
            p.jetpackPowerCoef -= boostingDir * new Vector2(0.5f, 0.3f);
        }

        public Vector2 DirectionToVector2(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return new Vector2(0, 1);
                case Direction.Down:
                    return new Vector2(0, -1);
                case Direction.Left:
                    return new Vector2(-1, 0);
                case Direction.Right:
                    return new Vector2(1, 0);
                default:
                    return Vector2.One;
            }
        }
    }
}
