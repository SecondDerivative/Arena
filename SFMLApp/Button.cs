using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.System;
using SFML.Audio;
using SFML.Graphics;

public enum ButtonStatus {
	Default,
	Focused,
	Active
}

namespace SFMLApp {
	class Button {
		public int Width;
		public int Height;
		public int PositionX;
		public int PositionY;

		public ButtonStatus status { get; private set; }

		public Texture styleDefault;
		public Texture styleActive;
		public Texture styleFocused;

		public Color styleTextColorDefault;
		public Color styleTextColorActive;
		public Color styleTextColorFocused;

		public int InnerTextSize;
		public Font TextFont = Fonts.Arial;
		public string InnerText = "";

		public Button(int positionX, int positionY, int width, int height) {
			PositionX = positionX;
			PositionY = positionY;
			Width = width;
			Height = height;
		}

		public void SetStyles(Texture ButtonStyleDefault, Texture ButtonStyleFocused, Texture ButtonStyleActive, Font ButtonStyleFont) {
			styleDefault = ButtonStyleDefault;
			styleFocused = ButtonStyleFocused;
			styleActive = ButtonStyleActive;
			TextFont = ButtonStyleFont;
		}

		public void SetStyles(Color TextStyleDefault, Color TextStyleFocused, Color TextStyleActive, int TextSize) {
			styleTextColorDefault = TextStyleDefault;
			styleTextColorFocused = TextStyleFocused;
			styleTextColorActive = TextStyleActive;
			InnerTextSize = TextSize;
		}

		public bool CheckPosition(int mousex, int mousey) {
			return (mousex > PositionX) && (mousey > PositionY) && (mousex < PositionX + Width) && (mousey < PositionY + Height);
		}

		public void CheckFocusing(int mousex, int mousey, ButtonStatus applystatus) {
			if (CheckPosition(mousex, mousey)) {
				status = applystatus;
			} else {
				status = ButtonStatus.Default;
			}
		}
	}
}
