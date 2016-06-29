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
    public class Control
    {
        public View view { get; private set; }
        private int Width, Height;

        public Control(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            view = new View(Width, Height);
            view.InitEvents(Close, KeyDown, MouseDown, MouseUp, MouseMove);
        }
        
        public void UpDate(long time)
        {
            view.Clear(Color.Black);
            view.DrawMenu();
            if (time > 0)
                view.DrawText((1000 / time).ToString(), 5, 5, 10, Fonts.Arial, Color.White);
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
        }

        public void MouseDown(object sender, MouseButtonEventArgs e)
        {
			view.OnMouseDown(ref e);
        }

        public void MouseUp(object sender, MouseButtonEventArgs e)
        {
			//view.MainForm.Size = new Vector2u(512, 372);
			view.OnMouseUp(ref e);
        }

        public void MouseMove(object sender, MouseMoveEventArgs e)
        {
			view.OnMouseMove(ref e);
        }

        public void Close(object send, EventArgs e)
        {
            ((RenderWindow)send).Close();
        }
    }
}
