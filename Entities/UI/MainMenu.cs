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
            //new MainSubMenu().Instantiate();
            AddChild(new OptionsSubMenu());
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
                h.AddChild(new TextBox("Unnamed.", "Pixel", h.Pos, h.Width, h.Height, 5, Color.Black, true));
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
                foreach (var child in Children) child.Pos = Vector2.Zero;

                return Slide(1, Vector2.UnitX * -200, Children);
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

                var screenSizeSwitcher = new Switcher(Options.DefaultScreenSize / 2 + new Vector2(0, -50), 700, 100, true, "Screen Size", Options.CurrentScreenSizeMultiplier, 3, 8, (size) => Options.SetSize(size));

                var fullscreenSwitcher = new BoolSwitcher(Options.DefaultScreenSize / 2 + new Vector2(0, -200), 700, 100, true, "Full Screen", Engine.Graphics.IsFullScreen,
                    () => { Options.FullScreen(); screenSizeSwitcher.Selectable = false; }, () => { Options.FullScreen(); screenSizeSwitcher.Selectable = true; });
                //screenSizeSwitcher.Selectable = false;

                returned.Add(fullscreenSwitcher);
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
                    SwitchTo(new MainSubMenu()); }));

                MakeList(returned, true);

                return returned;
            }

            public override void OnBack()
            {
                base.OnBack();
                SwitchTo(new MainSubMenu());
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
