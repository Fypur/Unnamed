using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fiourp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer
{
    public class GlassWall : Solid
    {
        public float BreakVelocity;
        public bool DestroyOnX;
        public Direction? SolidDir;

        private TextBox text;
        private static Color Color = Color.LightBlue;
        private static Color DestroyableColor = new Color(68, 112, 148);

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
                AddComponent(new Sprite(Color.Gray, new Rectangle(Pos.ToPoint() + spritePos, spriteSize), 0.5f));
                AddComponent(Sprite);
            }

            text = (TextBox)AddChild(new TextBox(BreakVelocity.ToString(), "Pixel", Pos + HalfSize, width, height, 1, DestroyableColor, true, TextBox.Alignement.Center));
        }

        public bool Break(Player player, Vector2 particleDirection, bool collisionDirectionIsX)
        {
            if (!Conditions(player, collisionDirectionIsX))
                return false;

            Engine.CurrentMap.MiddlegroundSystem.Emit(glass, 200, Bounds, null, particleDirection.ToAngleDegrees(), glass.Color);
            Engine.Cam.Shake(0.2f, 1.7f);
            player.HitStop(0.05f);

            FMOD.Studio.EventInstance g = Audio.PlayEvent("GlassBreak");
            g.setVolume((float)32 / (DestroyOnX ? Height : Width));

            SelfDestroy();
            return true;
        }

        public override void Update()
        {
            base.Update();

            float percentage = Math.Clamp(((Player)Engine.Player).Velocity.Length() / BreakVelocity, 0, 1);
            text.SetText((int)(percentage * 100) + "%");
            text.TextScale = 3;

            if (percentage == 1)
            {
                text.Color = new Color(176, 74, 56);
                text.SetText("BREAK");
                text.TextScale = 2;
            }
            else
                text.Color = DestroyableColor;
        }

        public override void Render()
        {
            base.Render();

            Player p = (Player)Engine.Player;
            Debug.LogUpdate(text.Pos);

            int length; //Draw part Square
            if (DestroyOnX)
            {
                length = (int)(Height * (float)Math.Clamp(p.Velocity.Length() / BreakVelocity, 0, 1));
                Debug.LogUpdate(length);
                Debug.LogUpdate(p.Velocity.Length() / BreakVelocity);
                Drawing.Draw(Drawing.PointTexture, new Vector2(Pos.X, Pos.Y + Height - length), new Vector2(Width, length), new Color(DestroyableColor, 10), 0, Vector2.Zero, SpriteEffects.None, 0.1f);
            }
            else
            {
                length = Width * (int)Math.Clamp(p.Velocity.Length() / BreakVelocity, 0, 1);
                Drawing.Draw(Drawing.PointTexture, new Vector2(Pos.X + Width - length, Pos.Y), new Vector2(length, Height), DestroyableColor, 0, Vector2.Zero, SpriteEffects.None, 0.1f);
            }
        }

        private bool Conditions(Player player, bool horizontal)
        {
            /*if (horizontal != DestroyOnX)
                return false;*/

            if (player.Velocity.LengthSquared() < BreakVelocity * BreakVelocity)
                return false;

            if (horizontal)
            {
                /*if (Math.Abs(player.Velocity.X) < BreakVelocity)
                    return false;*/
                if (SolidDir == Direction.Left)
                    return player.Pos.X > Pos.X;
                else if (SolidDir == Direction.Right)
                    return player.Pos.X < Pos.X;
            }
            else
            {
                /*if (Math.Abs(player.Velocity.Y) < BreakVelocity)
                    return false;*/

                if (SolidDir == Direction.Up)
                    return player.Pos.Y > Pos.Y;
                else if (SolidDir == Direction.Down)
                    return player.Pos.Y < Pos.Y;
            }

            return true;
        } 
    }
}
