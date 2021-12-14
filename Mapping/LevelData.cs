using Basic_platformer.Solids;
using Basic_platformer.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basic_platformer.Mapping
{
    public static class LevelData
    {
        public static List<Entity> GetLevelData(int index)
        {
            switch (index)
            {
                case 1:
                    Player player = new Player(new Vector2(Platformer.ScreenSize.X / 2, Platformer.ScreenSize.Y - 300), 32, 32);
                    GrapplingPoint gP = new GrapplingPoint(new Vector2[] { new Vector2(Platformer.ScreenSize.X / 2, 60), new Vector2(Platformer.ScreenSize.X / 2 + 200, 60), new Vector2(Platformer.ScreenSize.X / 2 + 200, 60 + 200), new Vector2(Platformer.ScreenSize.X / 2, 60 + 200) },
                        new float[] { 1, 0.2f, 3f }, Ease.QuintInAndOut);
                    CyclingPlatform c = new CyclingPlatform(10, 200, Color.Red, new Vector2[] { Vector2.Zero, Vector2.One * 200, Vector2.UnitY * 500 }, new float[] { 0.4f, 1f }, Ease.QuintInAndOut);
                    PulledPlatform pulled = new PulledPlatform(new Vector2(30, Platformer.ScreenSize.Y - 400), 200, 10, new Vector2(30 + 200, Platformer.ScreenSize.Y - 400), 2f, Ease.QuintOut, true);
                    GrapplingTrigger platformTrigger = new GrapplingTrigger(new Vector2(pulled.Pos.X + pulled.Width, pulled.Pos.Y), true, pulled.movingTime, pulled.Pulled);
                    return new List<Entity> { player, gP, c, pulled, platformTrigger };

                default:
                    throw new Exception("Couldn't find Level");
            }
        }
    }
}
