using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unnamed
{
    public class Boss2 : Actor, ISwinged
    {
        public float MaxSwingDistance { get => 110; set { } }

        private bool swinged;

        public Boss2(Vector2 position) : base(position, 32, 16, 0, new Sprite(Color.Red))
        {
            SwingingPoint.SwingingPoints.Add(this);
        }

        public override void Awake()
        {
            SolidPlatform p = new SolidPlatform(Pos - Vector2.One * 5, Width + 10, 5, new Sprite(Color.White));
            AddChild(p);
            Engine.CurrentMap.Data.Platforms.Add(p);
            Engine.CurrentMap.Data.Solids.Add(p);

            base.Awake();
        }

        void ISwinged.OnSwing(Entity grappledEntity, Func<bool> isAtSwingEnd)
             => swinged = true;
        void ISwinged.OnStopSwing(Entity unGrappledEntity)
            => swinged = false;

        public override void Update()
        {
            base.Update();

            Velocity.Y = 0.95f * Velocity.Y;

            if (swinged)
                gravityScale = 0.05f;
            else
                gravityScale = 0f;

            Gravity();


            Move(Velocity * Engine.Deltatime, null, null);
        }

        public override void Render()
        {
            base.Render();

             Polygon.DrawCirclePolygon(Polygon.GetCircleVisibilityPolygon(MiddlePos, MaxSwingDistance), MiddlePos, MaxSwingDistance, new Color(Color.DeepSkyBlue, 120));
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            SwingingPoint.SwingingPoints.Remove(this);
            Engine.CurrentMap.Data.Platforms.Remove((SolidPlatform)Children[0]);
            Engine.CurrentMap.Data.Solids.Remove((SolidPlatform)Children[0]);
        }
    }
}
