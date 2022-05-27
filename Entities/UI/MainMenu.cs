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
            h.AddChild(new TextBox("Abandonned", "LexendDeca", h.Pos, h.Width, h.Height, Color.Black, 3, true));

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

            OptionsMenu.Add(new BoolSwitcher(Engine.ScreenSize / 2 + new Vector2(0, -200), 700, 100, true, "Full Screen", false, () => Options.FullScreen(), () => Options.FullScreen()));

            OptionsMenu.Add(new EnumSwitcher<ScreenSizes>(Engine.ScreenSize / 2 + new Vector2(0, -50), 700, 100, true, "Screen Size", ScreenSizes.x4, new()
            {
                { ScreenSizes.x3, () => Options.SetSize(3) },
                { ScreenSizes.x4, () => Options.SetSize(4) },
                { ScreenSizes.x5, () => Options.SetSize(5) },
                { ScreenSizes.x6, () => Options.SetSize(6) },
                { ScreenSizes.x7, () => Options.SetSize(7) },
            }));

            OptionsMenu.Add(new NSButton(Engine.ScreenSize / 2 + new Vector2(0, 250), 700, 100, true, "Back", () => { SwitchToSubMenu(Main); }));
            //TODO: AddControls
            MakeList(OptionsMenu, true);
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
