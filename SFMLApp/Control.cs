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
    public enum ControlState
    {
        BattleState
    }

    public class Control
    {
        public View view { get; private set; }
        private int Width, Height;
        private ControlState state;
        private Arena arena;
        private Server server;

        const int CountPlayer = 10;
        private int[] TagByNum;

        public Control(int Width, int Height)
        {
            Build(Width, Height, "25.66.15.21");
        }

        public Control(int Width, int Height, string IP)
        {
            Build(Width, Height, IP);
        }

        private void Build(int Width, int Height, string IP)
        {
            this.Width = Width;
            this.Height = Height;
            view = new View(Width, Height);
            view.InitEvents(Close, KeyDown, KeyUp, MouseDown, MouseUp, MouseMove);
            state = ControlState.BattleState;
            arena = new Arena();
            server = new Server(CountPlayer, IP);
            InfListen();
            TagByNum = new int[CountPlayer];
            for (int i = 0; i < CountPlayer; i++)
                TagByNum[i] = -1;
            arena.NewMap("mapa");
        }

        public async Task InfListen()
        {
            int ch1 = 10;
            while (true)
            {
                int x = await server.NextClient();
                if (x != -1)
                {
                    server.Players[x].InfReceive();
                    TagByNum[x] = arena.AddPlayer(ch1 + " player");
                    view.AddPlayer(TagByNum[x]);
                }
            }
        }

        public void UpDate(long time)
        {
            if (state == ControlState.BattleState)
            {
                arena.Update();
                view.UpdateAnimation();
                view.DrawBattle(arena.players, arena.Arrows, arena.Drops, arena.ArenaPlayer, arena.map.players, arena.map.arrows, arena.map.Field, arena.map.drops);
                for (int i = 0; i < CountPlayer; i++)
                {
                    if (server.Players[i].IsOnline && TagByNum[i] != -1)
                    {
                        while (server.Players[i].KeyDown.Count > 0)
                        {
                            var NewUserEvent = server.Players[i].KeyDown.Dequeue();
                            int key = NewUserEvent.Item1;
                            var typeEvent = NewUserEvent.Item2;
                            if (typeEvent != TypeKeyDown.MouseDown)
                                ReleaseKeyDown(TagByNum[i], key);
                            else
                            {
                                ReleaseMouseDown(TagByNum[i], i, key);
                            }
                        }
                        MovePlayer(TagByNum[i], i);
                        arena.ArenaPlayer[TagByNum[i]].RealName = server.Players[i].NickName;
                    }
                    if (!server.Players[i].IsOnline && TagByNum[i] != -1)
                    {
                        int tag = TagByNum[i];
                        TagByNum[i] = -1;
                        view.RemovePlayer(tag);
                        arena.RemovePlayer(tag);
                    }
                }
                var info = arena.GetAllInfo();
                StringBuilder sb = new StringBuilder();
                sb.Append('#');
                bool wasWrite = false;
                for (int i = 0; i < server.CountClient; i++)
                    if (server.Players[i].IsOnline && TagByNum[i] != -1)
                    {
                        if (wasWrite)
                            sb.Append(';');
                        wasWrite = true;
                        sb.AppendFormat("{0},{1},{2}", TagByNum[i], server.Players[i].MousePos.Item1 - arena.map.players[TagByNum[i]].x,
                            server.Players[i].MousePos.Item2 - arena.map.players[TagByNum[i]].y);
                    }
                string direct = sb.ToString();
                for (int i = 0; i < server.CountClient; i++)
                {
                    if (server.Players[i].IsOnline)
                        server.Players[i].CheckOnline();
                    if (server.Players[i].IsOnline && TagByNum[i] != -1)
                        server.Players[i].SendAsync(info[TagByNum[i]] + direct);
                }
            }
            if (time > 0)
                view.DrawText((1000 / time).ToString(), 5, 5, 10, Fonts.Arial, Color.Black);
        }

        public void MovePlayer(int tag, int num)
        {
            Tuple<double, double> vect;
            if (!server.Players[num].IsRemote)
                vect = view.AngleByMousePos();
            else
                vect = Utily.MakePair<double>(server.Players[num].MousePos.Item1 - arena.map.players[tag].x, server.Players[num].MousePos.Item2 - arena.map.players[tag].y);
            if (Utily.Hypot2(vect.Item1, vect.Item2) < Map.RPlayer * Map.RPlayer * 2)
            {
                arena.MovePlayer(tag, Utily.MakePair<double>(0, 0));
                view.MovePlayer(tag, Utily.MakePair<double>(0, 0));
            }
            else
            {
                int Forw = server.Players[num].Forward, Left = server.Players[num].Left;
                var newvect = Utily.MakePair<double>(vect.Item1 * Forw + vect.Item2 * Left, vect.Item2 * Forw - vect.Item1 * Left);
                arena.MovePlayer(tag, newvect);
                view.MovePlayer(tag, newvect);
            }
        }

        public void ReleaseKeyDown(int tag, int key)
        {
            if (state == ControlState.BattleState)
            {
                if (key == (int)Keyboard.Key.Q)
                    arena.ChangeItem(tag, 1);
                if (key == (int)Keyboard.Key.E)
                    arena.ChangeItem(tag, -1);
            }
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            //server.Players[0].AddKey((int)e.Code);
        }

        public void ReleaseKeyUp(int tag, int key)
        {

        }

        public void KeyUp(object sender, KeyEventArgs e)
        {
            //server.Players[0].KeyUp((int)e.Code);
        }

        public void ReleaseMouseDown(int tag, int num, int button)
        {
            if (state == ControlState.BattleState)
            {
                if (button == (int)Mouse.Button.Left)
                {
                    Tuple<double, double> vect;
                    if (!server.Players[num].IsRemote)
                        vect = view.AngleByMousePos();
                    else
                        vect = vect = Utily.MakePair<double>(server.Players[num].MousePos.Item1 - arena.map.players[tag].x, server.Players[num].MousePos.Item2 - arena.map.players[tag].y);
                    if (Utily.Hypot2(vect.Item1, vect.Item2) == 0)
                        return;
                    int tagArr = arena.FirePlayer(tag, vect);
                    if (tagArr != -1)
                        view.AddArrow(tagArr);
                }
            }
        }

        public void MouseDown(object sender, MouseButtonEventArgs e)
        {
            //view.OnMouseDown(ref e);
            //server.Players[0].MouseDown((int)e.Button);
        }

        public void MouseUp(object sender, MouseButtonEventArgs e)
        {
            //view.OnMouseUp(ref e);
        }

        public void MouseMove(object sender, MouseMoveEventArgs e)
        {
            //view.OnMouseMove(ref e);
        }

        public void Close(object send, EventArgs e)
        {
            ((RenderWindow)send).Close();
        }
    }
}
