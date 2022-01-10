using Basic_platformer.Components;
using Basic_platformer.Entities;
using Basic_platformer.Mapping;
using Basic_platformer.Triggers;
using Basic_platformer.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Triggers
{
    public class LevelTransition : Trigger
    {
        private const float transitionTime = 2f;
        public enum Direction { Up, Down, Left, Right }

        private Camera cam = Platformer.Cam;
        private Level fromLevel;
        private Level toLevel;
        private Direction direction;

        public LevelTransition(Vector2 position, Vector2 size, Level fromLevel, Level toLevel, Direction dir)
            : base(position, size, new List<Type>() { typeof(Player) })
        {
            this.fromLevel = fromLevel;
            this.toLevel = toLevel;
            direction = dir;
        }

        public LevelTransition(Rectangle triggerRect, Level fromLevel, Level toLevel, Direction dir)
            : base(triggerRect, new List<Type>() { typeof(Player) })
        {
            this.fromLevel = fromLevel;
            this.toLevel = toLevel;
            direction = dir;
        }

        public override void OnTriggerEnter(Entity entity)
        {
            toLevel.Load();

            Player p = Platformer.player;
            p.canMove = false;

            switch (direction)
            {
                case Direction.Up:
                    cam.MoveTo(cam.Pos - new Vector2(0, Platformer.ScreenSize.Y), transitionTime, Ease.QuintInAndOut);
                    p.Pos.Y = Pos.Y - p.Height;
                    p.Velocity.Y = Math.Min(p.Velocity.Y, -300);
                    break;
                case Direction.Down:
                    cam.MoveTo(cam.Pos + new Vector2(0, Platformer.ScreenSize.Y), transitionTime, Ease.QuintInAndOut);
                    p.Pos.Y = Pos.Y;
                    break;
                case Direction.Left:
                    cam.MoveTo(cam.Pos - new Vector2(Platformer.ScreenSize.X, 0), transitionTime, Ease.QuintInAndOut);
                    p.Pos.X = Pos.X - p.Width;
                    break;
                case Direction.Right:
                    cam.MoveTo(cam.Pos + new Vector2(Platformer.ScreenSize.X, 0), transitionTime, Ease.QuintInAndOut);
                    p.Pos.X = Pos.X;
                    break;
            }

            AddComponent(new Timer(transitionTime, true, null, () => {
                p.canMove = true;
                Platformer.CurrentMap.CurrentLevel = toLevel;
                fromLevel.Unload();
            }));
        }
    }
}
