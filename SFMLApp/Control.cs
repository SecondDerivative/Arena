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

        private int Forward = 0, Left = 0;//what button is use on WASD
        private int MainPlayer;

        public Control(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            view = new View(Width, Height);
            view.InitEvents(Close, KeyDown, KeyUp, MouseDown, MouseUp, MouseMove);
            state = ControlState.BattleState;
            arena = new Arena();
            arena.NewMap("bag");
            MainPlayer = arena.AddPlayer("prifio");
            int tagbot = arena.AddPlayer("bot");
            view.AddPlayer(MainPlayer);
            view.AddPlayer(tagbot);
        }
        
        public void UpDate(long time)
        {
            if (state == ControlState.BattleState)
            {
                arena.Update();
                MovePlayer();
                view.UpdateAnimation();
                view.DrawBattle(arena.players, arena.Arrows, arena.Drops, arena.ArenaPlayer, arena.map.players, arena.map.arrows, arena.map.Field, arena.map.drops);
            }
            if (time > 0)
                view.DrawText((1000 / time).ToString(), 5, 5, 10, Fonts.Arial, Color.Black);
        }

        public void MovePlayer()
        {
            var vect = view.AngleByMousePos();
            if (Utily.Hypot2(vect.Item1, vect.Item2) < 150)
            {
                arena.MovePlayer(MainPlayer, Utily.MakePair<double>(0, 0));
                view.MovePlayer(MainPlayer, Utily.MakePair<double>(0, 0));
            }
            var newvect = Utily.MakePair<double>(vect.Item1 * Forward + vect.Item2 * Left, vect.Item2 * Forward - vect.Item1 * Left);
            arena.MovePlayer(MainPlayer, newvect);
            view.MovePlayer(MainPlayer, newvect);
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            if (state == ControlState.BattleState)
            {
                if (e.Code == Keyboard.Key.W)
                    Forward = 1;
                if (e.Code == Keyboard.Key.S)
                    Forward = -1;
                if (e.Code == Keyboard.Key.A)
                    Left = 1;
                if (e.Code == Keyboard.Key.D)
                    Left = -1;
                if (e.Code == Keyboard.Key.Q)
                    arena.players[MainPlayer].NextItem();
                if (e.Code == Keyboard.Key.E)
                    arena.ChangeArrow(MainPlayer, 1);
            }
        }

        public void KeyUp(object sender, KeyEventArgs e)
        {
            if (state == ControlState.BattleState)
            {
                if (e.Code == Keyboard.Key.W || e.Code == Keyboard.Key.S)
                    Forward = 0;
                if (e.Code == Keyboard.Key.A || e.Code == Keyboard.Key.D)
                    Left = 0;
            }
        }

        public void MouseDown(object sender, MouseButtonEventArgs e)
        {
			view.OnMouseDown(ref e);
            if (state == ControlState.BattleState)
            {
                if (e.Button == Mouse.Button.Left)
                {
                    var vect = view.AngleByMousePos();
                    if (Utily.Hypot2(vect.Item1, vect.Item2) == 0)
                        return;
                    int tag = arena.FirePlayer(MainPlayer, vect);
                    if (tag != -1)
                        view.AddArrow(tag);
                }
            }
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
