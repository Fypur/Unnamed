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
        public UIButton ResumeButton;
        public UIButton QuitButton;

        public PauseMenu()
            : base (Vector2.Zero, (int)Engine.ScreenSize.X, (int)Engine.ScreenSize.Y, Sprite.None)
        {
            ResumeButton = (UIButton)AddChild(new UIButton(new Vector2(200, 200), 400, 50, () => Platformer.Unpause()));
            QuitButton = (UIButton)AddChild(new UIButton(new Vector2(200, 300), 400, 50, () => Platformer.instance.Exit()));
        }
    }
}
