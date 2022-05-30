using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class MainMenu : UIElement
    {
        public List<UIElement> Main;
        public List<UIElement> OptionsMenu;
        public List<UIElement> ControlsMenu;
        public enum ScreenSizes { x3, x4, x5, x6, x7 }
        public MainMenu() : base(Vector2.Zero, (int)Engine.ScreenSize.X, (int)Engine.ScreenSize.Y, new Sprite(Color.Black))
        {
            CreateSubMenus();
            AddElements(Main);
        }

        private void CreateSubMenus()
        {
            Main = new();
            var h = new UIImage(Engine.ScreenSize / 2 + new Vector2(0, -250), 700, 200, true, new Sprite(Color.White));
            h.AddChild(new TextBox("Abandonned", "Pixel", h.Pos, h.Width, h.Height, Color.Black, 12, true));

            Main.Add(h);
            List<UIElement> buttons = new();
            var start = new NSButton(Engine.ScreenSize / 2 + new Vector2(0, -50), 700, 100, true, "Start", () => { Platformer.StartGame(); });
            start.OnSelected();
            Main.Add(start);
            Main.Add(new NSButton(Engine.ScreenSize / 2 + new Vector2(0, 100), 700, 100, true, "Options", () => { SwitchToSubMenu(OptionsMenu); }));
            Main.Add(new NSButton(Engine.ScreenSize / 2 + new Vector2(0, 250), 700, 100, true, "Quit", () => { Platformer.instance.Exit(); }));
            //Main.Add(new CheckBox(Vector2.Zero, 100, 100, true, true));
            MakeList(Main, true);

            OptionsMenu = new();

            var screenSizeSwitcher = new EnumSwitcher<ScreenSizes>(Engine.ScreenSize / 2 + new Vector2(0, -50), 700, 100, true, "Screen Size", ScreenSizes.x4, new()
            {
                { ScreenSizes.x3, () => Options.SetSize(3) },
                { ScreenSizes.x4, () => Options.SetSize(4) },
                { ScreenSizes.x5, () => Options.SetSize(5) },
                { ScreenSizes.x6, () => Options.SetSize(6) },
                { ScreenSizes.x7, () => Options.SetSize(7) },
            });

            OptionsMenu.Add(new BoolSwitcher(Engine.ScreenSize / 2 + new Vector2(0, -200), 700, 100, true, "Full Screen", false, 
                () => { Options.FullScreen(); screenSizeSwitcher.Selectable = false; }, () => { Options.FullScreen(); screenSizeSwitcher.Selectable = true; }));

            OptionsMenu.Add(screenSizeSwitcher);

            OptionsMenu.Add(new NSButton(Engine.ScreenSize / 2 + new Vector2(0, 100), 700, 100, true, "Change Controls", () => SwitchToSubMenu(ControlsMenu)));

            OptionsMenu.Add(new NSButton(Engine.ScreenSize / 2 + new Vector2(0, 250), 700, 100, true, "Back", () => { SwitchToSubMenu(Main); }));

            MakeList(OptionsMenu, true);

            ControlsMenu = new();
            ControlsMenu.Add(new TextBox($"{Input.UIAction1.GetAllControlNames(" or ")} : Change controls \n{Input.UIAction1.GetAllControlNames(" or ")} : Clear", "LexendDeca", Engine.ScreenSize / 2 + new Vector2(-500, -300), 1000, 100, Color.White, 1, true));

            ControlsMenu.Add(new ControlTaker(Engine.ScreenSize / 2 + new Vector2(0, -150), 700, 100, true, "Jump", Player.JumpControls, null));

            ControlsMenu.Add(new ControlTaker(Engine.ScreenSize / 2 + new Vector2(0, 0), 700, 100, true, "Jetpack", Player.JetpackControls, null));
            ControlsMenu.Add(new ControlTaker(Engine.ScreenSize / 2 + new Vector2(0, 150), 700, 100, true, "Rope Swing", Player.SwingControls, null));
            ControlsMenu.Add(new NSButton(Engine.ScreenSize / 2 + new Vector2(0, 300), 700, 100, true, "Back", () => { SwitchToSubMenu(OptionsMenu); }));

            MakeList(ControlsMenu, true);
            AddElements(ControlsMenu);
            foreach(UIElement ui in ControlsMenu)
            {
                ui.Active = false;
                ui.Visible = false;
            }
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

        /*public class MainSubMenu : SubMenu
        {
            public MainSubMenu() : base(null, true)
            { }


        }

        public class OptionsSubMenu : SubMenu
        {
            public OptionsSubMenu() : base()
        }*/
    }

    /*public class MainElements : Menu
    {
        public override List<Entity> GetElements()
        {
            var img = new UIImage(Engine.ScreenSize / 2 + new Vector2(0, -250), 700, 200, true, new Sprite(Color.White));
            return new()
            {
                img,
                new TextBox("Abandonned", "LexendDeca", img.Pos, img.Width, img.Height, Color.Gray, 3, true),
                new NSButton(Engine.ScreenSize / 2 + new Vector2(0, -50), 700, 100, true, "Start", () => { Platformer.StartGame(); }),
                new NSButton(Engine.ScreenSize / 2 + new Vector2(0, 100), 700, 100, true, "Options", () => { }),
                new NSButton(Engine.ScreenSize / 2 + new Vector2(0, 250), 700, 100, true, "Quit", () => { Platformer.instance.Exit(); })
            };
        }
    }

    public class OptionsMenu : Menu
    {
        public override List<Entity> GetElements()
        {
            return base.GetElements();
        }
    }*/
}
