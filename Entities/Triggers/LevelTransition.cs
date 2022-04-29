using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platformer
{
    public class LevelTransition : Trigger
    {
        private const float transitionTime = 1.5f;

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
            {
                toLevel = new Level(Levels.GetLevelData(ldtk));
                toLevel.LoadNoAutoTile();
            }
            else
                toLevel.Load();

            Player p = (Player)entity;
            p.canMove = false;

            cam.Move(cam.InBoundsPos(p.Pos, new Rectangle(toLevel.Pos.ToPoint(), toLevel.Size.ToPoint())) - cam.Pos, transitionTime, Ease.QuintInAndOut);

            switch (direction)
            {
                case Direction.Up:
                    p.Pos.Y = Pos.Y - p.Height;
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

            AddComponent(new Timer(transitionTime - Engine.Deltatime, true, null, () => {
                p.canMove = true;
                p.CancelJump();
                p.ResetJetpack();

                if(direction == Direction.Up)
                {
                    p.Velocity.Y = Math.Min(p.Velocity.Y, -200);
                    p.LimitJetpackY(0.5f, 0.4f, () => p.Velocity.Y >= 0);
                }

                Engine.Cam.SetBoundaries(toLevel.Pos, toLevel.Size);
                oldLevel.Unload();
            }));
        }
    }
}
