using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                /*var start = new NSButton(Options.DefaultScreenSize / 2 + new Vector2(0, -50), 700, 100, true, "START", () =>
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
                });

                returned.Add(start);
                returned.Add(new NSButton(Options.DefaultScreenSize / 2 + new Vector2(0, 100), 700, 100, true, "OPTIONS", () => { SwitchTo(new OptionsSubMenu()); }));

                returned.Add(new NSButton(Options.DefaultScreenSize / 2 + new Vector2(0, 250), 700, 100, true, "QUIT", () => { Platformer.instance.Exit(); }));*/

                returned.Add(new TextSelectable("Play", "LexendDeca", new Vector2(48, 272), 384, 64, 1, Color.White, false, () =>
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



                returned.Add(new TextSelectable("Options", "LexendDeca", new Vector2(48, 352), 384, 64, 1, Color.White, false, () => { SwitchTo(new OptionsSubMenu()); }));

                returned.Add(new TextSelectable("Quit", "LexendDeca", new Vector2(48, 432), 384, 64, 1, Color.White, false, () => { Platformer.instance.Exit(); }));

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

                var s = SlideTo(1, offsets, Children);
                s.MoveNext();
                return s;

                Fix the pause menu and you also gotta do the player cliiping up stuff going thru a wall, maybe even jumpthru help. More graphics and music to do and finish this damn menu boy
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

                /*var screenSizeSwitcher = new EnumSwitcher<ScreenSizes>(Options.DefaultScreenSize / 2 + new Vector2(0, -50), 700, 100, true, "Screen Size", Options.CurrentScreenSizeMultiplier, new()
                {
                    { ScreenSizes.x1, () => Options.SetSize(1) },
                    { ScreenSizes.x2, () => Options.SetSize(2) },
                    { ScreenSizes.x3, () => Options.SetSize(3) },
                    { ScreenSizes.x4, () => Options.SetSize(4) },
                    { ScreenSizes.x5, () => Options.SetSize(5) },
                    { ScreenSizes.x6, () => Options.SetSize(6) },
                    { ScreenSizes.x7, () => Options.SetSize(7) },
                });*/

                /*var screenSizeSwitcher = new Switcher(Options.DefaultScreenSize / 2 + new Vector2(0, -50), 700, 100, true, "Screen Size", Options.CurrentScreenSizeMultiplier, 3, 8, );

                var fullscreenSwitcher = new BoolSwitcher(Options.DefaultScreenSize / 2 + new Vector2(0, -200), 700, 100, true, "Full Screen", Engine.Graphics.IsFullScreen,
                    () => { Options.FullScreen(); screenSizeSwitcher.Selectable = false; }, () => { Options.FullScreen(); screenSizeSwitcher.Selectable = true; });*/
                //screenSizeSwitcher.Selectable = false;

                /*returned.Add(fullscreenSwitcher);
                if (Engine.Graphics.IsFullScreen)
                    screenSizeSwitcher.Selectable = false;

                returned.Add(screenSizeSwitcher);

                returned.Add(new NSButton(Options.DefaultScreenSize / 2 + new Vector2(0, 100), 700, 100, true, "Change Controls", 
                    () 
                    =>
                    { 
                        SwitchTo(new ControlsSubMenu()); }));

                returned.Add(new NSButton(Options.DefaultScreenSize / 2 + new Vector2(0, 250), 700, 100, true, "Back", () => 
                { 
                    SwitchTo(new MainSubMenu()); }));*/


                returned.Add(new TextBox("Options", "LexendDeca", new Vector2(640, 100), 300, 200, 2, Color.White, true));

                /*returned.Add(new TextBox("Switcher n1", "LexendDeca", new Vector2(640, 300), 300, 200, 1, Color.White, true));
                returned.Add(new TextBox("Switcher n2", "LexendDeca", new Vector2(640, 400), 300, 200, 1, Color.White, true));*/

                var screenSizeSwitcher = new SwitcherText(new Vector2(640, 300), 500, 200, true, "Screen Size", Options.CurrentScreenSizeMultiplier, 3, 8, (size) => Options.SetSize(size));
                

                returned.Add(screenSizeSwitcher);


                var fullscreen = new BoolSwitcherText(new Vector2(640, 400), 500, 200, true, "FullScreen", false, () => { Options.FullScreen(); screenSizeSwitcher.Selectable = false; }, () => { Options.FullScreen(); screenSizeSwitcher.Selectable = true; });
                

                returned.Add(fullscreen);

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
                        offsets[i] = -Vector2.UnitX * 1000;
                    else
                        offsets[i] = Vector2.UnitX * 1000;
                }

                var s = Slide(1, offsets, Children, true);
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

                ControlsMenu.Add(new TextBox($"Change Controls: {Input.UIAction1.GetAllControlNames(" or ")} \nClear Controls: {Input.UIActionBack.GetAllControlNames(" or ")}", "Recursive", screenSize / 2 + new Vector2(-500, -550), 1000, 200, 0.6f, Color.Black, true));

                ControlsMenu.Add(new ControlTaker(screenSize / 2 + new Vector2(0, -200), 700, 100, true, "Jump", Player.JumpControls, null));

                ControlsMenu.Add(new ControlTaker(screenSize / 2 + new Vector2(0, -50), 700, 100, true, "Jetpack", Player.JetpackControls, null));
                ControlsMenu.Add(new ControlTaker(screenSize / 2 + new Vector2(0, 100), 700, 100, true, "Rope Swing", Player.SwingControls, null));
                ControlsMenu.Add(new NSButton(screenSize / 2 + new Vector2(0, 250), 700, 50, true, "Back", () => {
                    SwitchTo(new OptionsSubMenu());
                }));

                MakeList(ControlsMenu, true);

                return ControlsMenu;
            }
        }
    }
}
