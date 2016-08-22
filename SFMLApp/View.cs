using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.System;
using SFML.Audio;
using SFML.Graphics;

namespace SFMLApp {

    public class View {

        public RenderWindow MainForm { get; private set; }

		private int Width;
		private int Height;

		private Sprite Menu;
        private Button MenuButtonStart;
        private Button MenuButtonExit;

        public void InitEvents(EventHandler Close, EventHandler<KeyEventArgs> KeyDown, EventHandler<MouseButtonEventArgs> MouseDown, EventHandler<MouseButtonEventArgs> MouseUp, EventHandler<MouseMoveEventArgs> MouseMove) {
            MainForm.Closed += Close;
            MainForm.KeyPressed += KeyDown;
            MainForm.MouseButtonPressed += MouseDown;
            MainForm.MouseButtonReleased += MouseUp;
            MainForm.MouseMoved += MouseMove;
        }

        public View(int Width, int Height) {
            this.Width = Width;
            this.Height = Height;
            MainForm = new RenderWindow(new VideoMode((uint)Width, (uint)Height), "SFML.net", Styles.Titlebar | Styles.Close);
            Menu = new Sprite(new Texture("data/Menu.png"));
            Menu.Position = new Vector2f(0, 0);
			MenuButtonStart = new Button("data/Buttons/MainMenuStartButton.txt");
			MenuButtonExit = new Button("data/Buttons/MainMenuExitButton.txt");
		}

		public void Clear() {
            MainForm.Clear(Color.White);
        }

        public void Clear(Color cl) {
            MainForm.Clear(cl);
        }

        public void DrawMenu() {
            MainForm.Draw(Menu);
            DrawButton(ref MenuButtonStart);
            DrawButton(ref MenuButtonExit);
        }

		private void DrawButton(ref Button button) {
			Texture ButtonCurrentStyle = null;
			Color ButtonTextCurrentColor = new Color(0, 0, 0);
			if (button.status == ButtonStatus.Default) {
				ButtonCurrentStyle = button.styleDefault;
				ButtonTextCurrentColor = button.styleTextColorDefault;
			}
			if (button.status == ButtonStatus.Focused) {
				ButtonCurrentStyle = button.styleFocused;
				ButtonTextCurrentColor = button.styleTextColorFocused;
			}
			if (button.status == ButtonStatus.Active) {
				ButtonCurrentStyle = button.styleActive;
				ButtonTextCurrentColor = button.styleTextColorActive;
			}
			Sprite buttonsprite = new Sprite(ButtonCurrentStyle, new IntRect(button.PositionX, button.PositionY, button.Width, button.Height));
			buttonsprite.Position = new Vector2f(button.PositionX, button.PositionY);
			MainForm.Draw(buttonsprite);
			DrawText(button.InnerText, button.PositionX + button.PaddingX, button.PositionY + button.PaddingY, button.InnerTextSize, button.TextFont, ButtonTextCurrentColor);
		}

        public void DrawText(string s, int x, int y, int size, Font Font, Color cl) {
            Text TextOut = new Text(s, Font);
            TextOut.CharacterSize = (uint)size;
            TextOut.Color = cl;
            TextOut.Position = new Vector2f(x, y);
            MainForm.Draw(TextOut);
        }

        public void DrawBattle(Dictionary<string, Player> ArenaPlayers,  // словарь игроков (инфа)
								Dictionary<string, AArow> ArenaArrows,   // словарь стрел (инфа)
								Dictionary<string, ADrop> ArenaDrops,    // словарь дроп (инфа)
								Dictionary<string, MPlayer> MapPlayers,  // словарь игроков (координаты)
								Dictionary<string, MArrow> MapArrows,    // словарь стрел (координаты)
								Dictionary<string, MDrop> MapDrops,		 // словарь дроп (координаты)
								List<List<Square>> Field,				 // 2-мерный массив
								string PlayerTag = "JaleChaki") {
			Clear(new Color(0, 0, 0, 255));		 
            foreach (var p in MapPlayers) {
				if (p.Value.Tag == PlayerTag) {
					Sprite healthbar = new Sprite(new Texture("data/HealthBar.png"));
					healthbar.Position = new Vector2f(Width - 350, 0);
					healthbar.Scale = new Vector2f(1, 1);
					MainForm.Draw(healthbar);
					//Sprite healthState = new Sprite(new Texture("data/HealthState.png"));
					//healthState.Position
					break;
				}
			}
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

		public void OnMouseUp(ref MouseButtonEventArgs args) {
			MenuButtonStart.CheckFocusing(args.X, args.Y, ButtonStatus.Focused, ButtonStatus.Active);
			MenuButtonExit.CheckFocusing(args.X, args.Y, ButtonStatus.Focused, ButtonStatus.Active);
		}
	}
}
