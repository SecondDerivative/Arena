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

		public Font TextFont = Fonts.Arial;
		public int InnerTextSize;
		public string InnerText = "";

		public int PaddingX { get; set; }
		public int PaddingY { get; set; }

		public Button(int positionX, int positionY, int width, int height) {
			PositionX = positionX;
			PositionY = positionY;
			Width = width;
			Height = height;
		}

		public Button(string path) {
			SaveTextFile file = new SaveTextFile();
			file.Load(path);
			PositionX = file.GetPropertyInt("positionX");
			PositionY = file.GetPropertyInt("positionY");
			Width = file.GetPropertyInt("width");
			Height = file.GetPropertyInt("height");
			styleDefault = new Texture(file.GetPropertyString("styleDefault"));
			styleActive = new Texture(file.GetPropertyString("styleActive"));
			styleFocused = new Texture(file.GetPropertyString("styleFocused"));
			TextFont = new Font(file.GetPropertyString("TextFont"));
			InnerTextSize = file.GetPropertyInt("InnerTextSize");
			InnerText = file.GetPropertyString("InnerText");
			PaddingX = file.GetPropertyInt("paddingX");
			PaddingY = file.GetPropertyInt("paddingY");
			styleTextColorDefault = file.GetPropertyColor("styleTextColorDefault");
			styleTextColorActive = file.GetPropertyColor("styleTextColorActive");
			styleTextColorFocused = file.GetPropertyColor("styleTextColorFocused");
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

		public void SetStyles(int paddingX, int paddingY) {
			PaddingX = paddingX;
			PaddingY = paddingY;
		}

		public bool CheckPosition(int mousex, int mousey) {
			return (mousex > PositionX) && (mousey > PositionY) && (mousex < PositionX + Width) && (mousey < PositionY + Height);
		}

		public void CheckFocusing(int mouseX, int mouseY, ButtonStatus ApplyStatus, ButtonStatus ApplyArgument) {
			if (CheckPosition(mouseX, mouseY)) {
				if (status == ApplyArgument) {
					status = ApplyStatus;
				}
			} else {
				status = ButtonStatus.Default;
			}
		}
	}
}
