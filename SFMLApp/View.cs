using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.System;
using SFML.Audio;
using SFML.Graphics;

namespace SFMLApp
{
    public class View
    {
        public RenderWindow MainForm { get; private set; }
        private int Width, Height;
        private Sprite Menu;
        private Button MenuButtonStart;
        private Button MenuButtonExit;

        public void InitEvents(EventHandler Close, EventHandler<KeyEventArgs> KeyDown, EventHandler<MouseButtonEventArgs> MouseDown, EventHandler<MouseButtonEventArgs> MouseUp, EventHandler<MouseMoveEventArgs> MouseMove)
        {
            MainForm.Closed += Close;
            MainForm.KeyPressed += KeyDown;
            MainForm.MouseButtonPressed += MouseDown;
            MainForm.MouseButtonReleased += MouseUp;
            MainForm.MouseMoved += MouseMove;
        }

        public View(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            MainForm = new RenderWindow(new VideoMode((uint)Width, (uint)Height), "SFML.net", Styles.Titlebar | Styles.Close);
            Menu = new Sprite(new Texture("data/Menu.png"));
            Menu.Position = new Vector2f(0, 0);
            #region StartButton params
            MenuButtonStart = new Button(Width / 2 - 150, Height / 2 - 160, 300, 80);
            MenuButtonStart.SetStyles(new Texture("data/Styles/Default.png"), new Texture("data/Styles/Focused.png"), new Texture("data/Styles/Active.png"), Fonts.Arial);
            MenuButtonStart.InnerText = "Start";
            MenuButtonStart.SetStyles(new Color(255, 0, 0), new Color(0, 255, 0), new Color(0, 0, 255), 40);
            #endregion
            #region ExitButton params
            MenuButtonExit = new Button(Width / 2 - 150, Height / 2 - 70, 300, 80);
            MenuButtonExit.SetStyles(new Texture("data/Styles/Default.png"), new Texture("data/Styles/Focused.png"), new Texture("data/Styles/Active.png"), Fonts.Arial);
            MenuButtonExit.InnerText = "Exit";
            MenuButtonExit.SetStyles(new Color(255, 0, 0), new Color(0, 255, 0), new Color(0, 0, 255), 40);
            #endregion
        }

        public void Clear()
        {
            MainForm.Clear(Color.White);
        }

        public void Clear(Color cl)
        {
            MainForm.Clear(cl);
        }

        public void DrawMenu()
        {
            MainForm.Draw(Menu);
            DrawButton(ref MenuButtonStart);
            DrawButton(ref MenuButtonExit);
        }

        private void DrawButton(ref Button button)
        {
            Texture ButtonCurrentStyle = null;
            Color ButtonTextCurrentColor = new Color(0, 0, 0);
            if (button.status == ButtonStatus.Default)
            {
                ButtonCurrentStyle = button.styleDefault;
                ButtonTextCurrentColor = button.styleTextColorDefault;
            }
            if (button.status == ButtonStatus.Focused)
            {
                ButtonCurrentStyle = button.styleFocused;
                ButtonTextCurrentColor = button.styleTextColorFocused;
            }
            if (button.status == ButtonStatus.Active)
            {
                ButtonCurrentStyle = button.styleActive;
                ButtonTextCurrentColor = button.styleTextColorActive;
            }
            Sprite buttonsprite = new Sprite(ButtonCurrentStyle, new IntRect(button.PositionX, button.PositionY, button.Width, button.Height));
            buttonsprite.Position = new Vector2f(button.PositionX, button.PositionY);
            MainForm.Draw(buttonsprite);
            DrawText(button.InnerText, button.PositionX, button.PositionY, button.InnerTextSize, button.TextFont, ButtonTextCurrentColor);
        }

        public void DrawText(string s, int x, int y, int size, Font Font, Color cl)
        {
            Text TextOut = new Text(s, Font);
            TextOut.CharacterSize = (uint)size;
            TextOut.Color = cl;
            TextOut.Position = new Vector2f(x, y);
            MainForm.Draw(TextOut);
        }

        public void DrawBattle(Dictionary<string, Player> ArenaPlayers, Dictionary<string, AArow> ArenaArrows, Dictionary<string, ADrop> ArenaDrops,
            Dictionary<string, MPlayer> MapPlayers, Dictionary<string, MArrow> MapArrows, List<List<Square>> Field, Dictionary<string, MDrop> MapDrops)
        {
            
        }

        public void OnMouseMove(ref MouseMoveEventArgs args)
        {
            MenuButtonStart.CheckFocusing(args.X, args.Y, ButtonStatus.Focused, ButtonStatus.Default);
            MenuButtonExit.CheckFocusing(args.X, args.Y, ButtonStatus.Focused, ButtonStatus.Default);
        }

        public void OnMouseDown(ref MouseButtonEventArgs args)
        {
            MenuButtonStart.CheckFocusing(args.X, args.Y, ButtonStatus.Active, ButtonStatus.Focused);
            MenuButtonExit.CheckFocusing(args.X, args.Y, ButtonStatus.Active, ButtonStatus.Focused);
        }

        public void OnMouseUp(ref MouseButtonEventArgs args)
        {
            MenuButtonStart.CheckFocusing(args.X, args.Y, ButtonStatus.Focused, ButtonStatus.Active);
            MenuButtonExit.CheckFocusing(args.X, args.Y, ButtonStatus.Focused, ButtonStatus.Active);
        }

    }
}
