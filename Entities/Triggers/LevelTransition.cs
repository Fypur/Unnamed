using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Platformer
{
    public class LevelTransition : Trigger
    {
        public Direction Direction;
        private const float transitionTime = 1f;
        public Level ToLevel;
        public LDtk.LDtkLevel LDtkToLevel;

        private Camera cam = Platformer.Cam;

        private static List<Entity> destroyOnTransition = new();

        public LevelTransition(Vector2 position, Vector2 size, Level toLevel, Direction dir)
            : base(position, size, new List<Type>() { typeof(Player) }, null)
        {
            this.ToLevel = toLevel;
            Direction = dir;
        }

        public LevelTransition(Vector2 position, Vector2 size, LDtk.LDtkLevel ldtk, Direction dir)
            : base(position, size, new List<Type>() { typeof(Player) }, null)
        {
            this.LDtkToLevel = ldtk;
            Direction = dir;
        }

        public LevelTransition(Rectangle triggerRect, Level toLevel, Direction dir)
            : base(triggerRect, new List<Type>() { typeof(Player) }, null)
        {
            this.ToLevel = toLevel;
            Direction = dir;
        }

        public override void OnTriggerEnter(Entity entity)
        {
            base.OnTriggerEnter(entity);

            List<Entity> toDestroy = new(destroyOnTransition);

            Level oldLevel = Engine.CurrentMap.CurrentLevel;
            SwingingPoint.SwingingPoints.Clear();

            cam.SetBoundaries(Rectangle.Empty);

            for (int i = Light.AllLights.Count - 1; i >= 0; i--)
                Light.AllLights[i].Visible = false;

            if (ToLevel == null)
            {
                ToLevel = new Level(Levels.GetLevelData(LDtkToLevel));
                ToLevel.LoadNoAutoTile();
            }
            else
                ToLevel.LoadAutoTile();

            Engine.CurrentMap.CurrentLevel = new MergedLevel(oldLevel, ToLevel);

            Player p = (Player)entity;
            p.CanMove = false;

            Vector2 size = ToLevel.Size;
            if (size.Y == 184)
                size.Y = 180;

            cam.Move(cam.InBoundsPos(p.Pos, new Rectangle(ToLevel.Pos.ToPoint(), size.ToPoint())) - cam.CenteredPos, transitionTime, Ease.CubeInAndOut, () => Engine.CurrentMap.Data.GetEntity<MainMenu>() != null);

            switch (Direction)
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

            //p.UpdateChildrenPos();
            p.CancelJump();

            AddComponent(new Timer(transitionTime - Engine.Deltatime, true, null, () => {
                p.CanMove = true;
                p.RefillJetpack();
                p.ResetSwing();

                Engine.CurrentMap.CurrentLevel = ToLevel;

                if (Direction == Direction.Up)
                {
                    p.Velocity.Y = Math.Min(p.Velocity.Y, -200);
                    //p.LimitJetpackY(0.5f, 0.4f, () => p.Velocity.Y >= 0);
                }

                Engine.Cam.SetBoundaries(ToLevel.Pos, size);

                foreach (Entity e in toDestroy)
                {
                    Engine.CurrentMap.Destroy(e);
                    destroyOnTransition.Remove(e);
                }

                oldLevel.Unload();

            }));
        }

        public static void DontDestroyOnDeath(Entity entity)
        {
            Engine.CurrentMap.CurrentLevel.DontDestroyOnUnload(entity);
            destroyOnTransition.Add(entity);
        }

        public static void InstaTransition(Level toLevel)
        {
            if (toLevel == null)
                throw new Exception("Level to Insta Transition to is null");

            List<Entity> toDestroy = new(destroyOnTransition);

            Engine.CurrentMap.CurrentLevel.Unload();
            SwingingPoint.SwingingPoints.Clear();

            toLevel.LoadNoAutoTile();

            Engine.Player.Pos = Engine.CurrentMap.Data.GetEntities<RespawnTrigger>()[0].RespawnPoint;

            Vector2 size = toLevel.Size;
            if (size.Y == 184)
                size.Y = 180;

            Player p = (Player)Engine.Player;

            //p.UpdateChildrenPos();
            p.CancelJump();

            p.RefillJetpack();
            p.ResetSwing();

            Engine.CurrentMap.CurrentLevel = toLevel;

            Engine.Cam.SetBoundaries(toLevel.Pos, size);

            foreach (Entity e in toDestroy)
            {
                Engine.CurrentMap.Destroy(e);
            }
        }

        public static void InstaTransition(LDtk.LDtkLevel toLevelLDtk)
        {
            List<Entity> toDestroy = new(destroyOnTransition);

            Engine.CurrentMap.CurrentLevel.Unload();
            SwingingPoint.SwingingPoints.Clear();

            Level toLevel = new Level(Levels.GetLevelData(toLevelLDtk));
            toLevel.LoadNoAutoTile();

            var respawns = Engine.CurrentMap.Data.GetEntities<RespawnTrigger>();
            if(respawns.Count > 0)
                Engine.Player.Pos = respawns[respawns.Count - 1].RespawnPoint;
            

            Vector2 size = toLevel.Size;
            if (size.Y == 184)
                size.Y = 180;

            Player p = (Player)Engine.Player;

            p.RespawnPoint = Engine.Player.Pos;
            //p.UpdateChildrenPos();
            p.CancelJump();

            p.RefillJetpack();
            p.ResetSwing();

            Engine.CurrentMap.CurrentLevel = toLevel;

            Engine.Cam.SetBoundaries(toLevel.Pos, size);

            foreach (Entity e in toDestroy)
            {
                Engine.CurrentMap.Destroy(e);
            }
        }
    }
}
