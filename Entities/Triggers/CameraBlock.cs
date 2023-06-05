using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class CameraBlock : Entity
    {
        public CameraBlock(Vector2 position, int width, int height) : base(position, width, height, null)
        {
        }

        public CameraBlock(Vector2 position, Vector2 size) : base(position, (int)size.X, (int)size.Y, null)
        {
        }

        public override void Awake()
        {
            base.Awake();

            Engine.Cam.TreatBoundsPos += Restrict;
        }

        private Vector2 Restrict(Vector2 inBoundsPos)
        {
            throw new NotImplementedException();
            /*if (inBoundsPos.X - Engine.Cam.HalfSize.X > Pos.X + Width || inBoundsPos.X + Engine.Cam.HalfSize.X < Pos.X)
                return inBoundsPos;

            
            
            float InBoundsPosX(float x)
            {
                if (x - Engine.Cam.HalfSize.X > Pos.X + Width)
                    return x;
                if (x + Engine.Cam.HalfSize.X < Pos.X))
                    
                else
                {
                    float correctedX = x - Engine.Cam.HalfSize.X;

                    if (correctedX < Bounds.X)
                        correctedX = Bounds.X;
                    else if (correctedX + Size.X > Bounds.X + Bounds.Width)
                        correctedX = Bounds.X + Bounds.Width - Size.X;

                    correctedX += Engine.Cam.HalfSize.X;

                    return correctedX;
                }
            }

            float InBoundsPosY(float y)
            {
                //float halfSizeY = Engine.ScreenSize.Y / 2 / RenderTargetScreenSizeCoef;
                if (y - Engine.Cam.HalfSize.Y > Bounds.Y && y + Engine.Cam.HalfSize.Y < Bounds.Y + Bounds.Height)
                    return y;
                else
                {
                    float correctedY = y - Engine.Cam.HalfSize.Y;

                    if (correctedY < Bounds.Y)
                        correctedY = Bounds.Y;
                    else if (correctedY + Size.Y > Bounds.Y + Bounds.Height)
                        correctedY = Bounds.Y + Bounds.Height - Size.Y;

                    correctedY += Engine.Cam.HalfSize.Y;

                    return correctedY;
                }
            }*/
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            Engine.Cam.TreatBoundsPos -= Restrict;
        }
    }
}
