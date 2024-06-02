using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using static Unnamed.MainMenu;

namespace Unnamed
{
    public class ChapterTransition : PlayerTrigger
    {
        public string InitLevel;
        public ChapterTransition(Rectangle bounds, string initLevel) : base(bounds, null)
        {
            InitLevel = initLevel;
        }

        public ChapterTransition(Vector2 position, Vector2 size, string initLevel) : base(position, size, null)
        {
            InitLevel = initLevel;
        }

        public ChapterTransition(Vector2 position, int width, int height, string initLevel) : base(position, width, height, null)
        {
            InitLevel = initLevel;
        }

        public override void OnTriggerEnter(Player player)
        {
            base.OnTriggerEnter(player);

            Engine.CurrentMap.Instantiate(new ScreenWipe(1.5f, Color.Black, () =>
            {
                Platformer.InitLevel = InitLevel;
                Platformer.InitWorld++;

                if(Platformer.WorldsUnlocked < Platformer.InitWorld)
                    Platformer.WorldsUnlocked = Platformer.InitWorld;

                Saving.SaveAndLoad(new()
                {
                    CurrentLevel = Platformer.InitLevel,
                    CurrentWorld = Platformer.InitWorld,
                    WorldUnlocked = Platformer.WorldsUnlocked,
                    CanJetpack = Platformer.World.Iid == LDtkTypes.Worlds.Boss.Iid || Platformer.World.Iid == LDtkTypes.Worlds.SwingJetpack.Iid,
                });

                ScreenWipe s = (ScreenWipe)Engine.CurrentMap.Data.EntitiesByType[typeof(ScreenWipe)][0];

                Platformer.EndGame();

                Engine.CurrentMap.Instantiate(new ChapterTransitionUI());

                Engine.CurrentMap.Data.Entities.Add(s);
                Engine.CurrentMap.Data.UIElements.Add(s);
                if (!Engine.CurrentMap.Data.EntitiesByType.TryGetValue(typeof(ScreenWipe), out var l))
                    Engine.CurrentMap.Data.EntitiesByType[typeof(ScreenWipe)] = new();
                Engine.CurrentMap.Data.EntitiesByType[typeof(ScreenWipe)].Add(s);
            }));
        }

        public class ChapterTransitionUI : UIElement
        {
            public ChapterTransitionUI() : base(Vector2.Zero, 1280, 720, null)
            {
                AddChild(new MainSubMenu());
            }

            public class MainSubMenu : SubMenu
            {
                public MainSubMenu() : base(Vector2.Zero, 1280, 720, true)
                {
                }

                public override List<UIElement> GetElements()
                {
                    List<UIElement> l = new();

                    l.Add(new TextBox("Chapter Finished!", "LexendDeca", new Vector2(640, 100), 1000, 230, 2, Color.White, true));

                    l.Add(new TextSelectable("Next Chapter", "LexendDeca", new Vector2(640, 360), 500, 50, 1, Color.White, true, TextBox.Alignement.Center, 
                        () => 
                        {
                            Engine.CurrentMap.Instantiate(new ScreenWipe(1.5f, Color.White, () =>
                            {
                                ScreenWipe s = (ScreenWipe)Engine.CurrentMap.Data.EntitiesByType[typeof(ScreenWipe)][0];
                                Platformer.StartGame();
                                Engine.CurrentMap.Data.Entities.Add(s);
                                Engine.CurrentMap.Data.UIElements.Add(s);
                                if (!Engine.CurrentMap.Data.EntitiesByType.TryGetValue(typeof(ScreenWipe), out var l))
                                    Engine.CurrentMap.Data.EntitiesByType[typeof(ScreenWipe)] = new();
                                Engine.CurrentMap.Data.EntitiesByType[typeof(ScreenWipe)].Add(s);
                            }));
                        }));

                    return l;
                }

                public override IEnumerator OnOpen()
                {
                    Vector2[] offsets = new Vector2[Children.Count];
                    offsets[0] = -Vector2.UnitY * 200;

                    for (int i = 1; i < offsets.Length; i++)
                    {
                        if (i % 2 == 1)
                            offsets[i] = -Vector2.UnitX * 1200;
                        else
                            offsets[i] = Vector2.UnitX * 1200;
                    }

                    var s = Slide(1, offsets, Children, false);
                    s.MoveNext();
                    return s;
                }
            }
        }
    }
}
