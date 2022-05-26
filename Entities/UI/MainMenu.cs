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
        public MainMenu() : base(Vector2.Zero, (int)Engine.ScreenSize.X, (int)Engine.ScreenSize.Y, new Sprite(Color.Black), null)
        {
            AddElements(new()
            {
                new UIImage(Engine.ScreenSize / 2 + new Vector2(0, -250), 700, 200, true, new Sprite(Color.White), new()
                {
                    //new TextBox("Abandonned", "LexendDeca", .Pos, img.Width, img.Height, Color.Gray, 3, true)
                }),
                new NSButton(Engine.ScreenSize / 2 + new Vector2(0, -50), 700, 100, true, "Start", () => { Platformer.StartGame(); }),
                new NSButton(Engine.ScreenSize / 2 + new Vector2(0, 100), 700, 100, true, "Options", () => { }),
                new NSButton(Engine.ScreenSize / 2 + new Vector2(0, 250), 700, 100, true, "Quit", () => { Platformer.instance.Exit(); })
            });
        }

        public override void Update()
        {
            base.Update();
            Debug.LogUpdate(Input.MousePosNoRenderTarget);
        }

        private void AddElements(List<UIElement> uiElements)
        {
            foreach(UIElement element in uiElements)
                AddChild(element);
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
