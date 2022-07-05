using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class PauseMenu : UIElement
    {
        public PauseMenu()
            : base (Vector2.Zero, 1280, 720, Sprite.None)
        { }

        public void Show()
        {
            var m = new MainSubMenu();
            m.RefreshElements();
            AddChild(m);
        }

        private class MainSubMenu : SubMenu
        {
            public MainSubMenu() : base(Vector2.Zero, 1280, 720, true)
            { }

            public override List<UIElement> GetElements()
            {
                List<UIElement> elements = new List<UIElement>();

                elements.Add(new NSButton(Options.DefaultScreenSize / 2, 700, 100, true, "Resume", () =>
                {
                    Platformer.PauseMenu.RemoveChild(this);
                    Platformer.Unpause();
                }));

                elements[0].OnSelected();

                elements.Add(new NSButton(Options.DefaultScreenSize / 2 + new Vector2(0, 150), 700, 100, true, "Return to Main Menu", () => {
                    Platformer.Unpause();
                    Platformer.PauseMenu.RemoveChild(this);
                    Platformer.EndGame();
                    Engine.CurrentMap.Instantiate(new MainMenu());
                }));

                MakeList(elements, true);

                return elements;
            }
        }
    }
}
