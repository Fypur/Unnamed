using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer
{
    public class LevelTransition : Trigger
    {
        private const float transitionTime = 1.5f;
        public enum Direction { Up, Down, Left, Right }

        private Camera cam = Platformer.Cam;
        private Level toLevel;
        private LDtk.LDtkLevel ldtk;
        private Direction direction;

        public LevelTransition(Vector2 position, Vector2 size, Level toLevel, Direction dir)
            : base(position, size, new List<Type>() { typeof(Player) }, null)
        {
            this.toLevel = toLevel;
            direction = dir;
        }

        public LevelTransition(Vector2 position, Vector2 size, LDtk.LDtkLevel ldtk, Direction dir)
            : base(position, size, new List<Type>() { typeof(Player) }, null)
        {
            this.ldtk = ldtk;
            direction = dir;
        }

        public LevelTransition(Rectangle triggerRect, Level toLevel, Direction dir)
            : base(triggerRect, new List<Type>() { typeof(Player) }, null)
        {
            this.toLevel = toLevel;
            direction = dir;
        }

        public override void OnTriggerEnter(Entity entity)
        {
            Level oldLevel = Engine.CurrentMap.CurrentLevel;
            SwingingPoint.SwingingPoints.Clear();

            cam.SetBoundaries(Rectangle.Empty);

            if (toLevel == null)
                toLevel = new Level(Levels.GetLevelData(ldtk));
            toLevel.Load();

            Player p = (Player)entity;
            p.canMove = false;

            cam.Move(toLevel.Pos - oldLevel.Pos, transitionTime, Ease.QuintInAndOut);

            switch (direction)
            {
                case Direction.Up:
                    p.Pos.Y = Pos.Y - p.Height;
                    p.Velocity.Y = Math.Min(p.Velocity.Y, -300);
                    break;
                case Direction.Down:
                    p.Pos.Y = Pos.Y + Height;
                    break;
                case Direction.Left:
                    p.Pos.X = Pos.X - p.Width;
                    break;
                case Direction.Right:
                    p.Pos.X = Pos.X + Width;
                    break;
            }

            AddComponent(new Timer(transitionTime, true, null, () => {
                p.canMove = true;
                Engine.CurrentMap.CurrentLevel = toLevel;
                Engine.Cam.SetBoundaries(toLevel.Pos, toLevel.Size);
                oldLevel.Unload();
            }));
        }
    }
}
