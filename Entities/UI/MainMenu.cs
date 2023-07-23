using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Platformer.MainMenu;

namespace Platformer
{
    public class MainMenu : UIElement
    {
        public enum ScreenSizes { x1, x2, x3, x4, x5, x6, x7 }
        public MainMenu() : base(Vector2.Zero, (int)Engine.ScreenSize.X, (int)Engine.ScreenSize.Y, new Sprite(Color.Black))
        {
            Platformer.PauseMenu = null;
            AddChild(new MainSubMenu());
        }

        public void SwitchToSubMenu(List<UIElement> subMenu)
        {
            foreach(UIElement element in Children)
            {
                if (element.Selected)
                    element.OnLeaveSelected();
                element.Active = false;
                element.Visible = false;
            }

            //TODO: Maybe instead of using active and visible re Instantiate the objects on menu to save on RAM
            bool doOnce = true;
            foreach(UIElement element in subMenu)
            {
                if (Children.Contains(element))
                {
                    element.Active = true;
                    element.Visible = true;
                }
                else
                    AddChild(element);

                if (element.Selectable && doOnce)
                {
                    element.OnSelected();
                    doOnce = false;
                }
            }
        }

        public class MainSubMenu : SubMenu
        {
            public MainSubMenu()
                : base(Vector2.Zero, 1280, 720, true)
            { }

            public override List<UIElement> GetElements()
            {
                List<UIElement> returned = new List<UIElement>();

                var h = new UIImage(new Vector2(208, 32), 400, 96, false, new Sprite(Color.White));
                h.AddChild(new TextBox("Unnamed.", "Pixel", h.Pos + h.HalfSize, h.Width, h.Height, 5, Color.Black, true));
                returned.Add(h);

                returned.Add(new TextSelectable("Play", "LexendDeca", new Vector2(48, 272), 384, 64, 1, Color.White, false, TextBox.Alignement.Left, () =>
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



                returned.Add(new TextSelectable("Options", "LexendDeca", new Vector2(48, 352), 384, 64, 1, Color.White, false, TextBox.Alignement.Left, () => { SwitchTo(new OptionsSubMenu()); }));

                returned.Add(new TextSelectable("Chapter Select", "LexendDeca", new Vector2(48, 432), 384, 64, 1, Color.White, false, TextBox.Alignement.Left, () => { SwitchTo(new WorldsSubMenu()); }));

                returned.Add(new TextSelectable("Quit", "LexendDeca", new Vector2(48, 512), 384, 64, 1, Color.White, false, TextBox.Alignement.Left, () => { Platformer.instance.Exit(); }));

                MakeList(returned, true);

                return returned;
            }

            public override IEnumerator OnOpen()
            {
                Vector2[] offsets = new Vector2[Children.Count];
                Array.Fill(offsets, Vector2.UnitX * -200);

                offsets[0] -= Vector2.UnitX * 500;
                offsets[3] -= Vector2.UnitX * 100;

                var s = Slide(1, offsets, Children);
                s.MoveNext();
                return s;
            }

            public override IEnumerator OnClose()
            {
                Vector2[] offsets = new Vector2[Children.Count];
                Array.Fill(offsets, Vector2.UnitX * -200);

                offsets[0] -= Vector2.UnitX * 500;
                offsets[3] -= Vector2.UnitX * 100;

                var s = SlideTo(0.5f, offsets, Children);
                s.MoveNext();
                return s;
            }
        }

        public class OptionsSubMenu : SubMenu
        {
            public OptionsSubMenu()
                : base(Vector2.Zero, 1280, 720, true)
            { }

            public override List<UIElement> GetElements()
            {
                List<UIElement> returned = new();

                returned.Add(new TextBox("Options", "LexendDeca", new Vector2(640, 100), 300, 200, 2, Color.White, true));

                /*returned.Add(new TextBox("Switcher n1", "LexendDeca", new Vector2(640, 300), 300, 200, 1, Color.White, true));
                returned.Add(new TextBox("Switcher n2", "LexendDeca", new Vector2(640, 400), 300, 200, 1, Color.White, true));*/

                var screenSizeSwitcher = new SwitcherText(new Vector2(640, 250), 750, 50, true, "Screen Size", Options.CurrentScreenSizeMultiplier, 3, 7, (size) => Options.SetSize(size));

                if (Engine.Graphics.IsFullScreen)
                    screenSizeSwitcher.Selectable = false;


                returned.Add(screenSizeSwitcher);


                var fullscreen = new BoolSwitcherText(new Vector2(640, 300), 750, 50, true, "FullScreen", Engine.Graphics.IsFullScreen, () => { Options.FullScreen(); screenSizeSwitcher.Selectable = false; }, () => { Options.FullScreen(); screenSizeSwitcher.Selectable = true; });

                returned.Add(fullscreen);

                returned.Add(new TextSelectable("Controls", "LexendDeca", new Vector2(640, 350), 500, 50, 1, Color.White, true, TextBox.Alignement.Center, () => SwitchTo(new ControlsSubMenu())));


                returned.Add(new SwitcherText(new Vector2(640, 400), 750, 50, true, "Music volume", (int)(Audio.GetGroupVolume("Musics") * 10), 0, 10, (volume) => Audio.SetGroupVolume("Musics", volume / (float)10)));

                returned.Add(new SwitcherText(new Vector2(640, 450), 750, 50, true, "Sound Effects volume", (int)(Audio.GetGroupVolume("Sound Effects") * 10), 0, 10, (volume) => Audio.SetGroupVolume("Sound Effects", volume / (float)10)));

                returned.Add(new SwitcherText(new Vector2(640, 500), 750, 50, true, "Master Volume", (int)(Audio.GetMasterVolume() * 10), 0, 10, (volume) => Audio.SetMasterVolume(volume / (float)10)));

                returned.Add(new TextSelectable("Back", "LexendDeca", new Vector2(640, 550), 500, 50, 1, Color.White, true, TextBox.Alignement.Center, () => SwitchTo(new MainSubMenu())));

                MakeList(returned, true);

                return returned;
            }

            public override void OnBack()
            {
                base.OnBack();
                SwitchTo(new MainSubMenu());
            }

            public override IEnumerator OnOpen()
            {
                Vector2[] offsets = new Vector2[Children.Count];
                offsets[0] = -Vector2.UnitY * 200;

                for(int i = 1; i < offsets.Length; i++)
                {
                    if(i % 2 == 1)
                        offsets[i] = -Vector2.UnitX * 1200;
                    else
                        offsets[i] = Vector2.UnitX * 1200;
                }

                var s = Slide(1, offsets, Children, false);
                s.MoveNext();
                return s;
            }
        }

        public class ControlsSubMenu : SubMenu
        {
            public ControlsSubMenu() : base(Vector2.Zero, 1280, 720, true)
            { }

            public override List<UIElement> GetElements()
            {
                List<UIElement> ControlsMenu = new();
                Vector2 screenSize = Options.DefaultScreenSize;

                ControlsMenu.Add(new TextBox($"Change Controls: {Input.UIAction1.GetAllControlNames(" or ")} \nClear Controls: {Input.UIActionBack.GetAllControlNames(" or ")}", "Recursive", screenSize.OnlyX() / 2, 1000, 200, 0.3f, Color.White, true, TextBox.Alignement.Center));

                ControlsMenu.Add(new ControlTaker(screenSize / 2 + new Vector2(0, -200), 700, 100, true, null, Color.White, "LexendDeca", 1, "Jump", Player.JumpControls, null));

                ControlsMenu.Add(new ControlTaker(screenSize / 2 + new Vector2(0, -50), 700, 100, true, null, Color.White, "LexendDeca", 1, "Jetpack", Player.JetpackControls, null));
                ControlsMenu.Add(new ControlTaker(screenSize / 2 + new Vector2(0, 100), 700, 100, true, null, Color.White, "LexendDeca", 1, "Rope Swing", Player.SwingControls, null));

                ControlsMenu.Add(new TextSelectable("Back", "LexendDeca", screenSize / 2 + new Vector2(0, 250), 700, 50, 1, Color.White, true, TextBox.Alignement.Center, () => SwitchTo(new OptionsSubMenu())));

                MakeList(ControlsMenu, true);

                return ControlsMenu;
            }

            public override IEnumerator OnOpen()
            {
                Vector2[] offsets = new Vector2[Children.Count];
                offsets[0] = -Vector2.UnitY * 200;

                for(int i = 1; i < offsets.Length; i++)
                {
                    if(i % 2 == 1)
                        offsets[i] = -Vector2.UnitX * 1200;
                    else
                        offsets[i] = Vector2.UnitX * 1200;
                }

                var s = Slide(1, offsets, Children, false);
                s.MoveNext();
                return s;
            }

            public override void OnBack()
            {
                base.OnBack();
                SwitchTo(new OptionsSubMenu());
            }
        }

        public class WorldsSubMenu : SubMenu
        {
            public WorldsSubMenu()
                : base(Vector2.Zero, 1280, 720, true)
            { }

            public override List<UIElement> GetElements()
            {
                List<UIElement> returned = new List<UIElement>();

                var h = new UIImage(new Vector2(208, 32), 400, 96, false, new Sprite(Color.White));
                h.AddChild(new TextBox("Unnamed.", "Pixel", h.Pos + h.HalfSize, h.Width, h.Height, 5, Color.Black, true));
                returned.Add(h);

                int i = 0;
                for(i = 0; i < Platformer.MaxWorlds; i++)
                {
                    string initLevel = i switch
                    {
                        0 => "0",
                        1 => "20",
                        2 => "Boss3",
                        _ => throw new Exception("init level not defined for chapter")
                    };

                    string name = i switch
                    {
                        0 => "Jetpack",
                        1 => "Swing",
                        2 => "Boss",
                        _ => throw new Exception("World Name not defined")
                    };

                    int wrld = i;

                    TextSelectable t = new TextSelectable(name, "LexendDeca", new Vector2(48, 272 + i * 80), 384, 64, 1, Color.White, false, TextBox.Alignement.Left, () =>
                    {
                        Engine.CurrentMap.Instantiate(new ScreenWipe(1.5f, Color.White, () =>
                        {
                            Platformer.InitLevel = initLevel;
                            Platformer.InitWorld = wrld;
                            Platformer.RefreshWorld();

                            ScreenWipe s = (ScreenWipe)Engine.CurrentMap.Data.EntitiesByType[typeof(ScreenWipe)][0];
                            Platformer.StartGame();
                            Engine.CurrentMap.Data.Entities.Add(s);
                            Engine.CurrentMap.Data.UIElements.Add(s);
                            if (!Engine.CurrentMap.Data.EntitiesByType.TryGetValue(typeof(ScreenWipe), out var l))
                                Engine.CurrentMap.Data.EntitiesByType[typeof(ScreenWipe)] = new();
                            Engine.CurrentMap.Data.EntitiesByType[typeof(ScreenWipe)].Add(s);
                        }));
                    });

                    if (i > Platformer.WorldsUnlocked)
                        t.Selectable = false;

                    returned.Add(t);
                }

                returned.Add(new TextSelectable("Back", "LexendDeca", new Vector2(48, 272 + i * 80), 384, 64, 1, Color.White, false, TextBox.Alignement.Left, () => SwitchTo(new MainSubMenu())));

                MakeList(returned, true);

                return returned;
            }

            public override IEnumerator OnOpen()
            {
                Vector2[] offsets = new Vector2[Children.Count];
                Array.Fill(offsets, Vector2.UnitX * -200);

                offsets[0] -= Vector2.UnitX * 500;

                var s = Slide(1, offsets, Children);
                s.MoveNext();
                return s;
            }

            public override IEnumerator OnClose()
            {
                Vector2[] offsets = new Vector2[Children.Count];
                Array.Fill(offsets, Vector2.UnitX * -200);

                offsets[0] -= Vector2.UnitX * 500;

                var s = SlideTo(0.5f, offsets, Children);
                s.MoveNext();
                return s;
            }

            public override void OnBack()
            {
                base.OnBack();
                SwitchTo(new MainSubMenu());
            }
        }
    }
}
