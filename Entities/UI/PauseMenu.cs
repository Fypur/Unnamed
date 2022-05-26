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
        public NSButton ResumeButton;
        public NSButton QuitButton;

        public PauseMenu()
            : base (Vector2.Zero, (int)Engine.ScreenSize.X, (int)Engine.ScreenSize.Y, Sprite.None, null)
        {
            ResumeButton = (NSButton)AddChild(new NSButton(Engine.ScreenSize / 2, 400, 50, true, "Resume", () => Platformer.Unpause()));
            QuitButton = (NSButton)AddChild(new NSButton(Engine.ScreenSize / 2 + new Vector2(0, 150), 400, 50, true, "Quit", () => Platformer.instance.Exit()));
        }
    }
}
