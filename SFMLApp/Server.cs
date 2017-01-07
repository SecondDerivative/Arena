using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace SFMLApp
{
    public class Server
    {
        public PlayerServer[] Players { get; private set; }

        private Socket Listner;
        
        public int CountClient { get; private set; }

        public Server(int cnt, string IP)
        {
            CountClient = cnt;
            Players = new PlayerServer[cnt];
            for (int i = 0; i < cnt; i++)
            {
                Players[i] = new PlayerServer();
            }
            Listner = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Listner.Bind(new IPEndPoint(IPAddress.Parse(IP), 11000));
            Listner.Listen(20);
        }

        private Task<Socket> Listen()
        {
            TaskCompletionSource<Socket> tcs = new TaskCompletionSource<Socket>();
            Listner.BeginAccept(iar => {Listner.EndAccept(iar);
                tcs.SetResult(Listner.EndAccept(iar));
            }, Listner);
            return tcs.Task;
        } 

        public async Task<int> NextClient()
        {
            var sock = await Listen();
            int i = 0;
            while (i < CountClient && !Players[i].IsOnline)
                ++i;
            if (i == CountClient)
                return -1;
            Players[i] = new PlayerServer();
            Players[i].SetOnlive(sock);
            return i;
        }
    }

    public class PlayerServer
    {
        public Tuple<int, int> MousePos { get; private set; }
        public Queue<int> KeyDown { get; private set; }
        public bool IsOnline { get; private set; }
        public string Names { get; set; }
        public bool IsRemote { get; set; }

        public int Forward { get; private set; }
        public int Left { get; private set; }

        public Socket Socket { get; private set; }
        private Stopwatch ReceiveTimer;

        const int MaxBufferSize = 4096;
        const int MaxWaitTime = 5000;
        public void SetOnlive(Socket sock)
        {
            Socket = sock;
            IsOnline = true;
            ReceiveTimer.Start();
        }
        public void SetNotRemote()
        {
            IsOnline = true;
            IsRemote = false;
        }
        public void CheckOnline()
        {
            if (IsRemote && ReceiveTimer.ElapsedMilliseconds > MaxWaitTime)
                IsOnline = false;
        }
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
        public Task<int> TryReceiveAsync(byte[] buffer, int offset, int size, SocketFlags flags)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            Socket.BeginReceive(buffer, offset, size, flags, iar =>
            {
                tcs.SetResult(Socket.EndReceive(iar));
            }, Socket);
            return tcs.Task;
        }
        public async Task<string> ReceiveAsync()
        {
            int size = 0;
            byte[] data = new byte[MaxBufferSize];
            while (true)
            {
                var received = await TryReceiveAsync(data, size, MaxBufferSize - size, SocketFlags.None);
                size += received;
                ReceiveTimer.Restart();
                if (data[size - 1] == '\n')
                    return Encoding.ASCII.GetString(data, 0, size);
                Console.WriteLine(received);
            }
        }
        public async Task InfReceive()
        {
            while (IsOnline)
            {
                string message = await ReceiveAsync();
                var arr = message.Split('\n');
                for (int i = 0; i < arr.Length; ++i)
                    ApplyString(arr[i]);
            }
        }
        public void ApplyString(string s)
        {
            //decode
        }
        public Task<int> TrySendAsync(byte[] buffer, int offset, int size, SocketFlags flags)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            Socket.BeginSend(buffer, offset, size, flags, iar =>
            {
                tcs.SetResult(Socket.EndSend(iar));
            }, Socket);
            return tcs.Task;
        }
        public async Task SendAsync(string s)
        {
            if (!IsOnline)
                return;
            byte[] data = Encoding.ASCII.GetBytes(s + '\n');
            int itt = 0;
            int left = data.Length;
            while (left > 0)
            {
                var sended = await TrySendAsync(data, itt, left, SocketFlags.None);
                itt += sended;
                left -= sended;
            }
        }
        public PlayerServer()
        {
            IsOnline = false;
            IsRemote = true;
            Names = "";
            MousePos = new Tuple<int, int>(0, 0);
            KeyDown = new Queue<int>();
            Forward = Left = 0;
            ReceiveTimer = new Stopwatch();
        }
    }
}
