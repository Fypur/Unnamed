using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic_platformer
{
    public class PauseMenu : UIElement
    {
        public Button ResumeButton;
        public Button QuitButton;

        public PauseMenu()
            : base (Vector2.Zero, (int)Engine.ScreenSize.X, (int)Engine.ScreenSize.Y, Sprite.None)
        {
            ResumeButton = (Button)AddChild(new Button(new Vector2(200, 200), 400, 50, new Sprite(Color.White), () => Platformer.Unpause()));
            QuitButton = (Button)AddChild(new Button(new Vector2(200, 300), 400, 50, new Sprite(Color.White), () => Platformer.instance.Exit()));
        }
    }
}
