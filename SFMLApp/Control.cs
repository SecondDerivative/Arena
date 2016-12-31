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

        const int CountPlayer = 2;
        private int[] TagByNum;

        public Control(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            view = new View(Width, Height);
            view.InitEvents(Close, KeyDown, KeyUp, MouseDown, MouseUp, MouseMove);
            state = ControlState.BattleState;
            arena = new Arena();
            server = new Server(CountPlayer, "192.168.1.40");
            TagByNum = new int[CountPlayer];
            server.Players[0].IsRemote = false;
            arena.NewMap("bag");
            TagByNum[0] = arena.AddPlayer("prifio");
            int tagbot = arena.AddPlayer("bot");
            view.AddPlayer(TagByNum[0]);
            view.AddPlayer(tagbot);
        }

        public void UpDate(long time)
        {
            if (state == ControlState.BattleState)
            {
                arena.Update();
                view.UpdateAnimation();
                view.DrawBattle(arena.players, arena.Arrows, arena.Drops, arena.ArenaPlayer, arena.map.players, arena.map.arrows, arena.map.Field, arena.map.drops);
                for (int i = 0; i < CountPlayer; i++)
                    if (server.Players[i].IsOnline || !server.Players[i].IsRemote)
                    {
                        while (server.Players[i].KeyDown.Count > 0)
                        {
                            int key = server.Players[i].KeyDown.Dequeue();
                            if (key != -1)//-1 = mouse Code
                                ReleaseKeyDown(TagByNum[i], key);
                            else
                            {
                                int button = server.Players[i].KeyDown.Dequeue();
                                ReleaseMouseDown(TagByNum[i], button);
                            }
                        }
                        MovePlayer(TagByNum[i], server.Players[i].Forward, server.Players[i].Left);
                    }
                var info = arena.GetAllInfo();
                for (int i = 0; i < server.CountClient; i++)
                    if (server.Players[i].IsOnline)
                        server.Players[i].SendAsync(info[TagByNum[i]]);
            }
            if (time > 0)
                view.DrawText((1000 / time).ToString(), 5, 5, 10, Fonts.Arial, Color.Black);
        }

        public void MovePlayer(int tag, int Forw, int Left)
        {
            var vect = view.AngleByMousePos(); //need change
            if (Utily.Hypot2(vect.Item1, vect.Item2) < 150)
            {
                arena.MovePlayer(tag, Utily.MakePair<double>(0, 0));
                view.MovePlayer(tag, Utily.MakePair<double>(0, 0));
            }
            var newvect = Utily.MakePair<double>(vect.Item1 * Forw + vect.Item2 * Left, vect.Item2 * Forw - vect.Item1 * Left);
            arena.MovePlayer(tag, newvect);
            view.MovePlayer(tag, newvect);
            //need create Class for UserKeyBord State. change forward etc
        }

        public void ReleaseKeyDown(int tag, int key)
        {
            if (state == ControlState.BattleState)
            {
                if (key == (int)Keyboard.Key.Q)
                    arena.ChangeItem(tag, 1);
                if (key == (int)Keyboard.Key.E)
                    arena.ChangeArrow(tag, 1);
            }
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            server.Players[0].AddKey((int)e.Code);
            //ReleaseKeyDown(MainPlayer, (int)e.Code);
        }

        public void ReleaseKeyUp(int tag, int key)
        {

        }

        public void KeyUp(object sender, KeyEventArgs e)
        {
            //ReleaseKeyUp(MainPlayer, (int)e.Code);
            server.Players[0].KeyUp((int)e.Code);
        }

        public void ReleaseMouseDown(int tag, int button)
        {
            if (state == ControlState.BattleState)
            {
                if (button == (int)Mouse.Button.Left)
                {
                    var vect = view.AngleByMousePos();//need change
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
            view.OnMouseDown(ref e);
            server.Players[0].MouseDown((int)e.Button);
        }

        public void MouseUp(object sender, MouseButtonEventArgs e)
        {
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
