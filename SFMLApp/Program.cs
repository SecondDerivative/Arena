﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

//sf::RenderWindow Window(sf::VideoMode(800, 600, 32), "Something Productive");

namespace SFMLApp
{
    class Program
    {
        public const int FPS = 60; 

        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;//double.Parse(str) use format a.b 
            Control control = new Control(1024, 726);
            long TimeDrawing = 1;
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            control.UpDate(0);
            control.UpDate(1000);
            control.UpDate(40);
			Items.getAllItems ();

            while (control.view.MainForm.IsOpen)
            {
                timer.Start();
                control.view.MainForm.DispatchEvents();
                control.UpDate(TimeDrawing);
                control.view.MainForm.Display();
                TimeDrawing = timer.ElapsedMilliseconds;
                timer.Reset();
                if (TimeDrawing < 1000 / FPS)
                {
                    Thread.Sleep((int)(1000 / FPS - TimeDrawing));
                }
            }

        }
    }
}
