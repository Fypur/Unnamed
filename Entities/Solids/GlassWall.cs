using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fiourp;
using Microsoft.Xna.Framework;

namespace Platformer
{
    public class GlassWall : Solid
    {
        public bool DestroyOnX;
        public Direction? SolidDir;

        private readonly static ParticleType glass = new ParticleType()
        {
            LifeMin = 1,
            LifeMax = 5,
            SpeedMin = 5,
            SpeedMax = 150,
            Color = Color.LightBlue,
            Size = 1,
            SizeRange = 1,
            SizeChange = ParticleType.FadeModes.None,
            FadeMode = ParticleType.FadeModes.EndLinear,
            Acceleration = new Vector2(0, 9.81f * 20)
        };
        public float BreakVelocity;

        public GlassWall(Vector2 position, Direction? fullSolid, int width, int height, float breakVelocity) : base(position, width, height, new Sprite(Color.LightBlue))
        {
            BreakVelocity = breakVelocity;
            DestroyOnX = Width >= Height ? false : true;
            SolidDir = fullSolid;

            if(fullSolid != null)
            {
                Point spritePos;
                Point spriteSize;
                switch (fullSolid)
                {
                    case Direction.Left:
                        spritePos = Point.Zero;
                        spriteSize = new Point(4, height);
                        break;
                    case Direction.Right:
                        spritePos = new Point(width - 4, 0);
                        spriteSize = new Point(4, height);
                        break;
                    case Direction.Up:
                        spritePos = Point.Zero;
                        spriteSize = new Point(width, 4);
                        break;
                    default:
                        spritePos = new Point(0, height - 4);
                        spriteSize = new Point(width, 4);
                        break;
                }

                RemoveComponent(Sprite);
                AddComponent(new Sprite(Color.Gray, new Rectangle(Pos.ToPoint() + spritePos, spriteSize), 0.1f));
                AddComponent(Sprite);
            }
        }

        public bool Break(Player player, Vector2 particleDirection, bool collisionDirectionIsX)
        {
            if (!Conditions(player, collisionDirectionIsX))
                return false;

            Engine.CurrentMap.MiddlegroundSystem.Emit(glass, 200, Bounds, null, particleDirection.ToAngleDegrees(), glass.Color);
            Engine.Cam.Shake(0.2f, 1.7f);
            player.HitStop(0.05f);

            //Audio.PlayEvent("GlassBreak");

            SelfDestroy();
            return true;
        }

        private bool Conditions(Player player, bool collDir)
        {
            if (collDir != DestroyOnX)
                return false;

            if (collDir)
            {
                if (Math.Abs(player.Velocity.X) < BreakVelocity)
                    return false;

                if (SolidDir == Direction.Left)
                    return player.Pos.X > Pos.X;
                else if (SolidDir == Direction.Right)
                    return player.Pos.X < Pos.X;
            }
            else
            {
                if (Math.Abs(player.Velocity.Y) < BreakVelocity)
                    return false;

                if (SolidDir == Direction.Up)
                    return player.Pos.Y > Pos.Y;
                else if (SolidDir == Direction.Down)
                    return player.Pos.Y < Pos.Y;
            }

            return true;
        } 
    }
}
