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
            return Task<Socket>.Factory.FromAsync(
                Listner.BeginAccept, Listner.EndAccept, null);
        }

        public async Task<int> NextClient()
        {
            var sock = await Listen();
            int i = 0;
            while (i < CountClient && (Players[i].IsOnline || !Players[i].IsRemote))
                ++i;
            if (i == CountClient)
                return -1;
            Players[i] = new PlayerServer();
            Players[i].SetOnline(sock);
            return i;
        }
    }

    public class PlayerServer
    {
        public Tuple<int, int> MousePos { get; private set; }
        public Queue<Tuple<int, TypeKeyDown> > KeyDown { get; private set; }
        public bool IsOnline { get; private set; }
        public string NickName { get; set; }
        public bool IsRemote { get; set; }

        public int Forward { get; private set; }
        public int Left { get; private set; }
        private bool isW, isS, isA, isD;

        public Socket Socket { get; private set; }
        private Stopwatch ReceiveTimer;

        const int MaxBufferSize = (1 << 20);
        const int MaxWaitTime = 5000;
        public void SetOnline(Socket sock)
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
            IsOnline = IsOnline && (!IsRemote || ReceiveTimer.ElapsedMilliseconds < MaxWaitTime);
        }
        private void UpdateDir()
        {
            if (isW)
                Forward = 1;
            else if (isS)
                Forward = -1;
            else
                Forward = 0;
            if (isA)
                Left = 1;
            else if (isD)
                Left = -1;
            else
                Left = 0;
        }
        public void AddKey(int key)
        {
            KeyDown.Enqueue(Utily.MakePair<int, TypeKeyDown>(key, TypeKeyDown.KeyDown));
            if (key == (int)Keyboard.Key.W)
                isW = true;
            if (key == (int)Keyboard.Key.S)
                isS = true;
            if (key == (int)Keyboard.Key.A)
                isA = true;
            if (key == (int)Keyboard.Key.D)
                isD = true;
            UpdateDir();
        }
        public void MouseDown(int button)
        {
            KeyDown.Enqueue(Utily.MakePair<int, TypeKeyDown>(button, TypeKeyDown.MouseDown));
        }
        public void KeyUp(int key)
        {
            if (key == (int)Keyboard.Key.W)
                isW = false;
            if (key == (int)Keyboard.Key.S)
                isS = false;
            if (key == (int)Keyboard.Key.A)
                isA = false;
            if (key == (int)Keyboard.Key.D)
                isD = false;
            UpdateDir();
        }
        public Task<int> TryReceiveAsync(byte[] buffer, int offset, int size, SocketFlags flags)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            Socket.BeginReceive(buffer, offset, size, flags, iar =>
            {
                try
                {
                    var r = Socket.EndReceive(iar);
                    tcs.SetResult(r);
                }
                catch
                {
                    tcs.SetResult(-1);
                }
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
                if (received == -1)
                    return "";
                size += received;
                if (data[size - 1] == '\n')
                    return Encoding.ASCII.GetString(data, 0, size);
            }
        }
        public async Task InfReceive()
        {
            while (IsOnline)
            {
                string message = await ReceiveAsync();
                ReceiveTimer.Restart();
                var arr = message.Split('\n');
                for (int i = 0; i < arr.Length; ++i)
                    ApplyString(arr[i]);
            }
        }
        public void ApplyString(string s)
        {
            if (s == "")
                return;
            int ind = s.IndexOf('#');
            int[] data = s.Substring(0, ind).Split().Select(int.Parse).ToArray();
            for (int i = 0; i < data.Length - 2; i += 2)
            {
                int code = data[i];
                TypeKeyDown tkd = (TypeKeyDown)data[i + 1];
                if (tkd == TypeKeyDown.KeyDown)
                    AddKey(code);
                if (tkd == TypeKeyDown.KeyUp)
                    KeyUp(code);
                if (tkd == TypeKeyDown.MouseDown)
                    MouseDown(code);
            }
            MousePos = Utily.MakePair<int>(data[data.Length - 2], data.Last());
            NickName = s.Substring(ind + 1);
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
            isW = isS = isA = isD = false;
            NickName = "";
            MousePos = new Tuple<int, int>(0, 0);
            KeyDown = new Queue<Tuple<int, TypeKeyDown> >();
            Forward = Left = 0;
            ReceiveTimer = new Stopwatch();
        }
    }

    public enum TypeKeyDown
    {
        KeyDown,
        KeyUp,
        MouseDown,
        MouseUp
    }
}
