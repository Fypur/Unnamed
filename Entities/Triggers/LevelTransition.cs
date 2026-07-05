using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Unnamed
{
    public class LevelTransition : PlayerTrigger
    {
        public Direction Direction;
        private const float transitionTime = 1f;
        public LDtk.LDtkLevel LDtkToLevel;

        private static List<Entity> destroyOnTransition = new();

        public LevelTransition(Vector2 position, Vector2 size, LDtk.LDtkLevel ldtk, Direction dir)
            : base(position, size, null)
        {
            this.LDtkToLevel = ldtk;
            Direction = dir;
        }

        public override void OnTriggerEnter(Player player)
        {
            base.OnTriggerEnter(player);

            List<Entity> toDestroy = new(destroyOnTransition);

            Level oldLevel = LevelManager.CurrentLevel;
            SwingingPoint.SwingingPoints.Clear();

            Platformer.GameCam.RemoveBoundaries();

            for (int i = Light.AllLights.Count - 1; i >= 0; i--)
                Light.AllLights[i].Visible = false;

            Level ToLevel = LevelManager.GetLevel(LDtkToLevel);
            ToLevel.Load();

            player.CanMove = false;

            Vector2 size = ToLevel.Size;
            if (size.Y == 184)
                size.Y = 180;

            Platformer.GameCam.Move(Platformer.GameCam.InBoundsPos(player.Pos, new Rectangle(ToLevel.Pos.ToPoint(), size.ToPoint())) - Platformer.GameCam.Pos, transitionTime, Ease.CubeInAndOut, null);

            switch (Direction)
            {
                case Direction.Up:
                    player.Pos.Y = Pos.Y - player.AABBCollider.Height;
                    break;
                case Direction.Down:
                    player.Pos.Y = Pos.Y + player.AABBCollider.Height;
                    break;
                case Direction.Left:
                    player.Pos.X = Pos.X - player.AABBCollider.Width;
                    break;
                case Direction.Right:
                    player.Pos.X = Pos.X + player.AABBCollider.Width;
                    break;
            }

            //p.UpdateChildrenPos();
            player.CancelJump();

            AddComponent(new Timer(transitionTime - Engine.Deltatime, null, () =>
            {
                player.CanMove = true;
                player.RefillJetpack();
                player.ResetSwing();

                LevelManager.CurrentLevel = ToLevel;

                if (Direction == Direction.Up)
                {
                    player.Velocity.Y = Math.Min(player.Velocity.Y, -200);
                    //p.LimitJetpackY(0.5f, 0.4f, () => p.Velocity.Y >= 0);
                }

                Platformer.GameCam.SetBoundaries(ToLevel.Pos, size);

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
            LevelManager.CurrentLevel.DontDestroyOnUnload(entity);
            destroyOnTransition.Add(entity);
        }

        public static void InstaTransition(Level toLevel)
        {
            if (toLevel == null)
                throw new Exception("Level to Insta Transition to is null");

            List<Entity> toDestroy = new(destroyOnTransition);

            LevelManager.CurrentLevel.Unload();
            SwingingPoint.SwingingPoints.Clear();

            toLevel.Load();

            Platformer.Player.Pos = Engine.CurrentMap.Data.GetEntities<RespawnTrigger>()[0].RespawnPoint;

            Vector2 size = toLevel.Size;
            if (size.Y == 184)
                size.Y = 180;

            //Platformer.Player.UpdateChildrenPos();
            Platformer.Player.CancelJump();

            Platformer.Player.RefillJetpack();
            Platformer.Player.ResetSwing();

            LevelManager.CurrentLevel = toLevel;

            Platformer.GameCam.SetBoundaries(toLevel.Pos, size);

            foreach (Entity e in toDestroy)
            {
                Engine.CurrentMap.Destroy(e);
            }
        }

        public static void InstaTransition(LDtk.LDtkLevel toLevelLDtk)
        {
            List<Entity> toDestroy = new(destroyOnTransition);

            LevelManager.CurrentLevel.Unload();
            SwingingPoint.SwingingPoints.Clear();

            Level toLevel = LevelManager.GetLevel(toLevelLDtk);
            toLevel.Load();

            var respawns = Engine.CurrentMap.Data.GetEntities<RespawnTrigger>();
            if (respawns.Count > 0)
                Platformer.Player.Pos = respawns[respawns.Count - 1].RespawnPoint;


            Vector2 size = toLevel.Size;
            if (size.Y == 184)
                size.Y = 180;

            Player p = Platformer.Player;

            p.RespawnPoint = p.Pos;
            //p.UpdateChildrenPos();
            p.CancelJump();

            p.RefillJetpack();
            p.ResetSwing();

            LevelManager.CurrentLevel = toLevel;

            Platformer.GameCam.SetBoundaries(toLevel.Pos, size);

            foreach (Entity e in toDestroy)
            {
                Engine.CurrentMap.Destroy(e);
            }
        }
    }
}
