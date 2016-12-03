using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;

namespace SFMLApp
{
    public class Server
    {
        public PlayerServer[] Players { get; private set; }
        public int CountClient { get; private set; }

        public Server(int cnt)
        {
            CountClient = cnt;
            Players = new PlayerServer[cnt];
            for (int i = 0; i < cnt; i++)
            {
                Players[i] = new PlayerServer();
            }
        }

        public void SendInfo(string s, int ind)
        {
            //   
        }
        //server code
    }

    public class PlayerServer
    {
        public Tuple<int, int> MousePos { get; private set; }
        public Queue<int> KeyDown { get; private set; }
        public bool IsOnLine { get; private set; }
        public string Names { get; set; }
        public bool IsRemote { get; set; }

        public int Forward { get; private set; }
        public int Left { get; private set; }

        public void AddKey(int key)
        {
            KeyDown.Enqueue(key);
            if (key == (int)Keyboard.Key.W)
                Forward = 1;
            if (key == (int)Keyboard.Key.S)
                Forward = -1;
            if (key == (int)Keyboard.Key.A)
                Left = 1;
            if (key == (int)Keyboard.Key.D)
                Left = -1;
        }

        public void MouseDown(int button)
        {
            KeyDown.Enqueue(-1);
            KeyDown.Enqueue(button);
        }

        public void KeyUp(int key)
        {
            if (key == (int)Keyboard.Key.W || key == (int)Keyboard.Key.S)
                Forward = 0;
            if (key == (int)Keyboard.Key.A || key == (int)Keyboard.Key.D)
                Left = 0;
        }

        public PlayerServer()
        {
            IsOnLine = false;
            IsRemote = true;
            Names = "";
            MousePos = new Tuple<int, int>(0, 0);
            KeyDown = new Queue<int>();
            Forward = Left = 0;
        }
    }
}
