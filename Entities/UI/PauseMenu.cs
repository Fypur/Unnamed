using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class PauseMenu : UIElement
    {
        private static bool showing;

        public PauseMenu()
            : base (Vector2.Zero, 1280, 720, Sprite.None)
        { }

        public override void Render()
        {
            if(Active)
                Drawing.Draw(Drawing.PointTexture, Vector2.Zero, new Color(Color.Black, 20));
            base.Render();
        }

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

                elements.Add(new TextSelectable("Resume", "LexendDeca", HalfSize - new Vector2(0, 50), 500, 100, 1, Color.White, true, TextBox.Alignement.Center, () =>
                {
                    Platformer.PauseMenu.RemoveChild(this);
                    Platformer.Unpause();
                }));

                elements.Add(new TextSelectable("Options", "LexendDeca", HalfSize + new Vector2(0, 50), 500, 100, 1, Color.White, true, TextBox.Alignement.Center, () =>
                {
                    SwitchTo(new MainMenu.OptionsSubMenu<MainSubMenu>());
                }));

                elements.Add(new TextSelectable("Save & Exit to Menu", "LexendDeca", HalfSize + new Vector2(0, 150), 500, 100, 1, Color.White, true, TextBox.Alignement.Center, () => {
                    Saving.Save(new()
                    {
                        CurrentLevel = Levels.LastLDtkLevel.Identifier,
                        CurrentWorld = Platformer.InitWorld,
                        WorldUnlocked = Platformer.WorldsUnlocked,
                        CanJetpack = Platformer.player.CanJetpack
                    });

                    Audio.GetBus("Sound effects").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);

                    Platformer.LoadWorldSave(Saving.Load());

                    Platformer.Unpause();
                    Platformer.PauseMenu.RemoveChild(this);
                    Platformer.EndGame();
                    Engine.CurrentMap.Instantiate(new MainMenu());
                }));

                /*elements.Add(new NSButton(Options.DefaultScreenSize / 2, 700, 100, true, "Resume", () =>
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
                }));*/

                MakeList(elements, true);

                return elements;
            }

            public override void OnBack()
            {
                base.OnBack();

                Platformer.PauseMenu.RemoveChild(this);
                Platformer.Unpause();
            }
        }
    }
}
