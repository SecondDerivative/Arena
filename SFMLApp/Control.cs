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

        public Control(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            view = new View(Width, Height);
            view.InitEvents(Close, KeyDown, MouseDown, MouseUp, MouseMove);
            state = ControlState.BattleState;
            arena = new Arena();
            arena.NewMap("bag");
            int tagprifio = arena.AddPlayer("prifio");
            //int tagbot = arena.AddPlayer("bot");
            view.AddPlayer(tagprifio);
            arena.MovePlayer("prifio", Utily.MakePair<double>(1, 1));
            view.MovePlayer(tagprifio, Utily.MakePair<double>(1, 1));
            //arena.FirePlayer("prifio", Utily.MakePair<double>(1, 1));
        }
        
        public void UpDate(long time)
        {
            if (state == ControlState.BattleState)
            {
                arena.Update();
                view.UpdateAnimation();
                view.DrawBattle(arena.players, arena.Arrows, arena.Drops, arena.ArenaPlayer, arena.map.players, arena.map.arrows, arena.map.Field, arena.map.drops);
            }
            if (time > 0)
                view.DrawText((1000 / time).ToString(), 5, 5, 10, Fonts.Arial, Color.Black);
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
