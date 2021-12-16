using Basic_platformer.Components;
using Basic_platformer.Triggers;
using Basic_platformer.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Mapping
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
            Pos = position;
            this.fromLevel = fromLevel;
            this.toLevel = toLevel;
        }

        public override void OnTriggerEnter(Actor actor)
        {
            toLevel.Load();

            switch (direction)
            {
                case Direction.Up:
                    cam.MoveTo(cam.Position - new Vector2(0, Platformer.ScreenSize.Y), transitionTime, Ease.QuintInAndOut);
                    break;
                case Direction.Down:
                    cam.MoveTo(cam.Position + new Vector2(0, Platformer.ScreenSize.Y), transitionTime, Ease.QuintInAndOut);
                    break;
                case Direction.Left:
                    cam.MoveTo(cam.Position - new Vector2(Platformer.ScreenSize.X, 0), transitionTime, Ease.QuintInAndOut);
                    break;
                case Direction.Right:
                    cam.MoveTo(cam.Position + new Vector2(Platformer.ScreenSize.X, 0), transitionTime, Ease.QuintInAndOut);
                    break;
            }
            fromLevel.Unload();
        }
    }
}
