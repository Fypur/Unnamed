using Fiourp;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Platformer.MainMenu;

namespace Platformer
{
    public class MainMenu : UIElement
    {
        ParticleType BigFire;
        ParticleType Wind;
        public enum ScreenSizes { x1, x2, x3, x4, x5, x6, x7 }
        public MainMenu() : base(Vector2.Zero, 1280, 720, new Sprite(DataManager.Textures["bg/mainMenu"]))
        {
            Platformer.PauseMenu = null;
            AddChild(new MainSubMenu());

            BigFire = Particles.Fire.Copy();
            BigFire.Acceleration = -Vector2.UnitY * 100;
            BigFire.SpeedMin = 5;
            BigFire.SpeedMax = 200;
            BigFire.Size = 10;
            BigFire.SizeRange = 15;
            BigFire.Direction = -90;
            BigFire.DirectionRange = 90;
            BigFire.Color = new Color(BigFire.Color, 150);
            BigFire.Color2 = new Color(BigFire.Color2.Value, 150);

            Wind = new ParticleType()
            {
                LifeMin = 1,
                LifeMax = 4,
                SpeedMin = 200,
                SpeedMax = 500,
                Size = 4,
                SizeRange = 3,
                SizeChange = ParticleType.FadeModes.EndSmooth,
                FadeMode = ParticleType.FadeModes.Linear,
                Acceleration = Vector2.UnitX * 200,
                Direction = 0,
                DirectionRange = 40,
                Color = new(Color.Gray, 100),
            };
        }

        public override void Update()
        {
            base.Update();

            Sprite.Scale = Size / new Vector2(1280, 720);

            Engine.CurrentMap.ForegroundSystem.Emit(BigFire, new Rectangle((Pos + new Vector2(0, Size.Y)).ToPoint(), (Pos + Size).ToPoint()), 2); 
            Engine.CurrentMap.ForegroundSystem.Emit(Wind, Bounds, 2); 
            //Engine.CurrentMap.ForegroundSystem.Emit(Wind, new Rectangle((Pos - new Vector2(400, 0)).ToPoint(), (Pos + Size - new Vector2(400, 0)).ToPoint()), 2);
        }

        public override void Render()
        {
            base.Render();

            Engine.CurrentMap.ForegroundSystem.Render();
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

                returned.Add(new TextSelectable("Play", "LexendDeca", new Vector2(48, 272), 384, 64, 1, Color.White, false, TextBox.Alignement.Left, () =>
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



                returned.Add(new TextSelectable("Options", "LexendDeca", new Vector2(48, 352), 384, 64, 1, Color.White, false, TextBox.Alignement.Left, () => { SwitchTo(new OptionsSubMenu<MainSubMenu>()); }));

                returned.Add(new TextSelectable("Chapter Select", "LexendDeca", new Vector2(48, 432), 384, 64, 1, Color.White, false, TextBox.Alignement.Left, () => { SwitchTo(new WorldsSubMenu()); }));

                returned.Add(new TextSelectable("Quit", "LexendDeca", new Vector2(48, 512), 384, 64, 1, Color.White, false, TextBox.Alignement.Left, () => { Platformer.instance.Exit(); }));

                MakeList(returned, true);

                return returned;
            }

            public override IEnumerator OnOpen()
            {
                Vector2[] offsets = new Vector2[Children.Count];
                Array.Fill(offsets, Vector2.UnitX * -200);

                offsets[0] -= Vector2.UnitX * 500;
                offsets[3] -= Vector2.UnitX * 100;

                var s = Slide(1, offsets, Children);
                s.MoveNext();
                return s;
            }

            public override IEnumerator OnClose()
            {
                Vector2[] offsets = new Vector2[Children.Count];
                Array.Fill(offsets, Vector2.UnitX * -200);

                offsets[0] -= Vector2.UnitX * 500;
                offsets[3] -= Vector2.UnitX * 100;

                var s = SlideTo(0.5f, offsets, Children);
                s.MoveNext();
                return s;
            }
        }

        public class OptionsSubMenu<T> : SubMenu where T : SubMenu, new()
        {
            public OptionsSubMenu() : base(Vector2.Zero, 1280, 720, true)
            { }

            public override List<UIElement> GetElements()
            {
                List<UIElement> returned = new();

                returned.Add(new TextBox("Options", "LexendDeca", new Vector2(640, 100), 300, 200, 2, Color.White, true));

                /*returned.Add(new TextBox("Switcher n1", "LexendDeca", new Vector2(640, 300), 300, 200, 1, Color.White, true));
                returned.Add(new TextBox("Switcher n2", "LexendDeca", new Vector2(640, 400), 300, 200, 1, Color.White, true));*/

                var screenSizeSwitcher = new SwitcherText(new Vector2(640, 250), 750, 50, true, "Screen Size", Options.CurrentScreenSizeMultiplier, 3, 7, (size) => Options.SetSize(size));

                if (Engine.Graphics.IsFullScreen)
                    screenSizeSwitcher.Selectable = false;


                returned.Add(screenSizeSwitcher);


                var fullscreen = new BoolSwitcherText(new Vector2(640, 300), 750, 50, true, "FullScreen", Engine.Graphics.IsFullScreen, () => { Options.FullScreen(); screenSizeSwitcher.Selectable = false; }, () => { Options.FullScreen(); screenSizeSwitcher.Selectable = true; });

                returned.Add(fullscreen);

                returned.Add(new TextSelectable("Controls", "LexendDeca", new Vector2(640, 350), 500, 50, 1, Color.White, true, TextBox.Alignement.Center, () => SwitchTo(new ControlsSubMenu<OptionsSubMenu<T>>())));

                returned.Add(new SwitcherText(new Vector2(640, 400), 750, 50, true, "Master Volume", (int)(Audio.GetMasterVolume() * 10), 0, 10, (volume) => Audio.SetMasterVolume(volume / (float)10)));

                returned.Add(new SwitcherText(new Vector2(640, 450), 750, 50, true, "Music volume", (int)(Audio.GetGroupVolume("Musics") * 10), 0, 10, (volume) => Audio.SetGroupVolume("Musics", volume / (float)10)));

                returned.Add(new SwitcherText(new Vector2(640, 500), 750, 50, true, "Sound Effects volume", (int)(Audio.GetGroupVolume("Sound Effects") * 10), 0, 10, (volume) => Audio.SetGroupVolume("Sound Effects", volume / (float)10)));

                returned.Add(new TextSelectable("Back", "LexendDeca", new Vector2(640, 550), 500, 50, 1, Color.White, true, TextBox.Alignement.Center, OnBack));

                MakeList(returned, true);

                return returned;
            }

            public override void OnBack()
            {
                base.OnBack();
                SwitchTo(new T());

                Saving.Save(new SaveData()
                {
                    ScreenSize = Options.CurrentScreenSizeMultiplier,
                    MasterVolume = (int)(Audio.GetMasterVolume() * 10),
                    MusicVolume = (int)(Audio.GetGroupVolume("Musics") * 10),
                    SFXVolume = (int)(Audio.GetGroupVolume("Sound Effects") * 10),
                    jumpControls = Player.JumpControls.Controls.ToArray(),
                    jetpackControls = Player.JetpackControls.ToArray(),
                    swingControls = Player.SwingControls.ToArray()
                });
            }

            public override IEnumerator OnOpen()
            {
                Vector2[] offsets = new Vector2[Children.Count];
                offsets[0] = -Vector2.UnitY * 200;

                for(int i = 1; i < offsets.Length; i++)
                {
                    if(i % 2 == 1)
                        offsets[i] = -Vector2.UnitX * 1200;
                    else
                        offsets[i] = Vector2.UnitX * 1200;
                }

                var s = Slide(1, offsets, Children, false);
                s.MoveNext();
                return s;
            }
        }

        public class ControlsSubMenu<T> : SubMenu where T : SubMenu, new()
        {
            public ControlsSubMenu() : base(Vector2.Zero, 1280, 720, true)
            { }

            public override List<UIElement> GetElements()
            {
                List<UIElement> ControlsMenu = new();
                Vector2 screenSize = Options.DefaultScreenSize;

                ControlsMenu.Add(new TextBox($"Change Controls: {Input.UIAction1.GetAllControlNames(" or ")} \n                     Clear Controls: {Input.ButtonClear.GetAllControlNames(" or ")}", "Recursive", new Vector2(1280 / 2, 100), 1000, 200, 0.3f, Color.White, true, TextBox.Alignement.TopCenter));

                ControlsMenu.Add(new ControlTaker(screenSize / 2 + new Vector2(0, -200), 700, 100, true, null, Color.White, "LexendDeca", 1, "Jump", Player.JumpControls, null));

                ControlsMenu.Add(new ControlTaker(screenSize / 2 + new Vector2(0, -50), 700, 100, true, null, Color.White, "LexendDeca", 1, "Jetpack", Player.JetpackControls, null));
                ControlsMenu.Add(new ControlTaker(screenSize / 2 + new Vector2(0, 100), 700, 100, true, null, Color.White, "LexendDeca", 1, "Rope Swing", Player.SwingControls, null));

                ControlsMenu.Add(new TextSelectable("Back", "LexendDeca", screenSize / 2 + new Vector2(0, 250), 700, 50, 1, Color.White, true, TextBox.Alignement.Center, () => SwitchTo(new T())));

                MakeList(ControlsMenu, true);

                return ControlsMenu;
            }

            public override IEnumerator OnOpen()
            {
                Vector2[] offsets = new Vector2[Children.Count];
                offsets[0] = -Vector2.UnitY * 200;

                for(int i = 1; i < offsets.Length; i++)
                {
                    if(i % 2 == 1)
                        offsets[i] = -Vector2.UnitX * 1200;
                    else
                        offsets[i] = Vector2.UnitX * 1200;
                }

                var s = Slide(1, offsets, Children, false);
                s.MoveNext();
                return s;
            }

            public override void OnBack()
            {
                base.OnBack();
                SwitchTo(new T());
            }
        }

        public class WorldsSubMenu : SubMenu
        {
            public WorldsSubMenu()
                : base(Vector2.Zero, 1280, 720, true)
            { }

            public override List<UIElement> GetElements()
            {
                List<UIElement> returned = new List<UIElement>();

                var h = new UIImage(new Vector2(208, 32), 400, 96, false, new Sprite(Color.White));
                h.AddChild(new TextBox("Unnamed.", "Pixel", h.Pos + h.HalfSize, h.Width, h.Height, 5, Color.Black, true));
                returned.Add(h);

                int i = 0;
                for(i = 0; i < Platformer.MaxWorlds; i++)
                {
                    string initLevel = i switch
                    {
                        0 => "0",
                        1 => "20",
                        2 => "69",
                        _ => throw new Exception("init level not defined for chapter")
                    };

                    string name = i switch
                    {
                        0 => "Jetpack",
                        1 => Platformer.WorldsUnlocked >= 1  ? "Swing" : "___",
                        2 => Platformer.WorldsUnlocked >= 2 ? "Boss" : "___",
                        _ => throw new Exception("World Name not defined")
                    };

                    int wrld = i;

                    TextSelectable t = new TextSelectable(name, "LexendDeca", new Vector2(48, 272 + i * 80), 384, 64, 1, Color.White, false, TextBox.Alignement.Left, () =>
                    {
                        Engine.CurrentMap.Instantiate(new ScreenWipe(1.5f, Color.White, () =>
                        {
                            Platformer.InitLevel = initLevel;
                            Platformer.InitWorld = wrld;
                            Platformer.RefreshWorld();

                            ScreenWipe s = (ScreenWipe)Engine.CurrentMap.Data.EntitiesByType[typeof(ScreenWipe)][0];
                            Platformer.StartGame();
                            Engine.CurrentMap.Data.Entities.Add(s);
                            Engine.CurrentMap.Data.UIElements.Add(s);
                            if (!Engine.CurrentMap.Data.EntitiesByType.TryGetValue(typeof(ScreenWipe), out var l))
                                Engine.CurrentMap.Data.EntitiesByType[typeof(ScreenWipe)] = new();
                            Engine.CurrentMap.Data.EntitiesByType[typeof(ScreenWipe)].Add(s);
                        }));
                    });

                    if (i > Platformer.WorldsUnlocked)
                        t.Selectable = false;

                    returned.Add(t);
                }

                returned.Add(new TextSelectable("Back", "LexendDeca", new Vector2(48, 272 + i * 80), 384, 64, 1, Color.White, false, TextBox.Alignement.Left, () => SwitchTo(new MainSubMenu())));

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

                var s = SlideTo(0.5f, offsets, Children);
                s.MoveNext();
                return s;
            }

            public override void OnBack()
            {
                base.OnBack();
                SwitchTo(new MainSubMenu());
            }
        }
    }
}
