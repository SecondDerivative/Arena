using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using SFML.Window;
using SFML.System;
using SFML.Audio;
using SFML.Graphics;

namespace SFMLApp
{
    public class View
    {
        public RenderWindow MainForm { get; private set; }
        private int Width, Height;
        private Sprite Menu;
        private Button MenuButtonStart;
        private Button MenuButtonExit;

        public void InitEvents(EventHandler Close, EventHandler<KeyEventArgs> KeyDown, EventHandler<KeyEventArgs> KeyUp, EventHandler<MouseButtonEventArgs> MouseDown, EventHandler<MouseButtonEventArgs> MouseUp, EventHandler<MouseMoveEventArgs> MouseMove)
        {
            MainForm.Closed += Close;
            MainForm.KeyPressed += KeyDown;
            MainForm.MouseButtonPressed += MouseDown;
            MainForm.MouseButtonReleased += MouseUp;
            MainForm.MouseMoved += MouseMove;
            MainForm.KeyReleased += KeyUp;
        }
        public View(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            MainForm = new RenderWindow(new VideoMode((uint)Width, (uint)Height), "SFML.net", Styles.Titlebar | Styles.Close);
            Menu = new Sprite(new Texture("data/Menu.png"));
            Menu.Position = new Vector2f(0, 0);
            Timer = new Stopwatch();
            Timer.Start();
            NewGame();
            #region StartButton params
            MenuButtonStart = new Button(Width / 2 - 150, Height / 2 - 160, 300, 80);
            MenuButtonStart.SetStyles(new Texture("data/Styles/Default.png"), new Texture("data/Styles/Focused.png"), new Texture("data/Styles/Active.png"), Fonts.Arial);
            MenuButtonStart.InnerText = "Start";
            MenuButtonStart.SetStyles(new Color(255, 0, 0), new Color(0, 255, 0), new Color(0, 0, 255), 40);
            #endregion
            #region ExitButton params
            MenuButtonExit = new Button(Width / 2 - 150, Height / 2 - 70, 300, 80);
            MenuButtonExit.SetStyles(new Texture("data/Styles/Default.png"), new Texture("data/Styles/Focused.png"), new Texture("data/Styles/Active.png"), Fonts.Arial);
            MenuButtonExit.InnerText = "Exit";
            MenuButtonExit.SetStyles(new Color(255, 0, 0), new Color(0, 255, 0), new Color(0, 0, 255), 40);
            #endregion
        }
        public void Clear()
        {
            MainForm.Clear(Color.White);
        }
        public void Clear(Color cl)
        {
            MainForm.Clear(cl);
        }
        public void DrawMenu()
        {
            MainForm.Draw(Menu);
            DrawButton(ref MenuButtonStart);
            DrawButton(ref MenuButtonExit);
        }
        private void DrawButton(ref Button button)
        {
            Texture ButtonCurrentStyle = null;
            Color ButtonTextCurrentColor = new Color(0, 0, 0);
            if (button.status == ButtonStatus.Default)
            {
                ButtonCurrentStyle = button.styleDefault;
                ButtonTextCurrentColor = button.styleTextColorDefault;
            }
            if (button.status == ButtonStatus.Focused)
            {
                ButtonCurrentStyle = button.styleFocused;
                ButtonTextCurrentColor = button.styleTextColorFocused;
            }
            if (button.status == ButtonStatus.Active)
            {
                ButtonCurrentStyle = button.styleActive;
                ButtonTextCurrentColor = button.styleTextColorActive;
            }
            Sprite buttonsprite = new Sprite(ButtonCurrentStyle, new IntRect(button.PositionX, button.PositionY, button.Width, button.Height));
            buttonsprite.Position = new Vector2f(button.PositionX, button.PositionY);
            MainForm.Draw(buttonsprite);
            DrawText(button.InnerText, button.PositionX, button.PositionY, button.InnerTextSize, button.TextFont, ButtonTextCurrentColor);
        }
        public void DrawText(string s, int x, int y, int size, Font Font, Color cl)
        {
            Text TextOut = new Text(s, Font);
            TextOut.CharacterSize = (uint)size;
            TextOut.Color = cl;
            TextOut.Position = new Vector2f(x, y);
            MainForm.Draw(TextOut);
        }


        //BattleDraw and data
        //...

        private int NowMouseX = 100, NowMouseY = 100;
        public int CameraPosX = 0, CameraPosY = 0;
        private int SizeMapX, SizeMapY;
        private bool WasInit = false;
        private Stopwatch Timer;
        private int MainPlayer;

        bool CameraIsMoving = false;
        long LastMoveCamera = 0;

        private Dictionary<int, PlayerView> viewPlayers;

        public void DrawBattle(Dictionary<int, Player> Players, Dictionary<int, AArow> ArenaArrows, Dictionary<int, ADrop> ArenaDrops, Dictionary<int, APlayer> Aplayer,
            Dictionary<int, MPlayer> MapPlayers, Dictionary<int, MArrow> MapArrows, List<List<Square>> Field, Dictionary<int, MDrop> MapDrops)
        {
            if (!WasInit)
            {
                CameraPosX = CameraPosY = 0;
                SizeMapX = Field.Count;
                SizeMapY = Field[0].Count;
                WasInit = true;
            }
            Clear();
            RectangleShape Stone = new RectangleShape(new Vector2f(Map.Rwidth, Map.Rwidth));
            Stone.FillColor = Color.Magenta;
            RectangleShape Water = new RectangleShape(new Vector2f(Map.Rwidth, Map.Rwidth));
            Water.FillColor = Color.Blue;
            for (int i = 0; i < SizeMapX; i++)
            {
                for (int j = 0; j < SizeMapY; j++)
                {
                    if (Field[i][j].type == 1)
                    {
                        Stone.Position = new Vector2f(i * Map.Rwidth - CameraPosX, j * Map.Rwidth - CameraPosY);
                        MainForm.Draw(Stone);
                        //DrawText("#", i * Map.Rwidth - CameraPosX, j * Map.Rwidth - CameraPosY, 10, Fonts.Arial, Color.Black);
                    }
                    if (Field[i][j].type == 2)
                    {
                        Water.Position = new Vector2f(i * Map.Rwidth - CameraPosX, j * Map.Rwidth - CameraPosY);
                        MainForm.Draw(Water);
                        //DrawText("#", i * Map.Rwidth - CameraPosX, j * Map.Rwidth - CameraPosY, 10, Fonts.Arial, Color.Black);
                    }
                }
            }
            CircleShape plr = new CircleShape(Map.RPlayer);
            plr.FillColor = Color.Blue;
            foreach (var i in viewPlayers)
            {
                i.Value.x = (int)MapPlayers[i.Key].x;
                i.Value.y = (int)MapPlayers[i.Key].y;
                i.Value.Draw(this, plr);
            }
            CircleShape arr = new CircleShape(Map.RArrow);
            arr.FillColor = Color.Red;
            foreach (var i in MapArrows)
            {
                arr.Position = new Vector2f((int)i.Value.x - CameraPosX - Map.RArrow, (int)i.Value.y - CameraPosY - Map.RArrow);
                MainForm.Draw(arr);
                //code for color. one color - arrow. one color - magic
                //DrawText(">", (int)i.Value.x - CameraPosX, (int)i.Value.y - CameraPosY, 10, Fonts.Arial, Color.Black);
            }
            CircleShape drop = new CircleShape(Map.RDrop);
            drop.FillColor = Color.Magenta;
            foreach (var i in MapDrops)
            {
                drop.Position = new Vector2f((int)i.Value.x - CameraPosX - Map.RDrop, (int)i.Value.y - CameraPosY - Map.RDrop);
                MainForm.Draw(drop);
                //DrawText("d", (int)i.Value.x - CameraPosX, (int)i.Value.y - CameraPosY, 10, Fonts.Arial, Color.Black);
            }
            foreach (var i in MapDrops)
            {
                var it = Items.allItems[ArenaDrops[i.Key].id];
                if (it is ItemBow || it is Magic)
                    DrawText(it.Name, (int)i.Value.x - CameraPosX - 5, (int)i.Value.y - CameraPosY - 5, 10, Fonts.Arial, Color.Black);
                else if (it is Bottle)
                    DrawText(it.Name + "(" + ((Bottle)it).restore + ")", (int)i.Value.x - CameraPosX - 5, (int)i.Value.y - CameraPosY - 5, 10, Fonts.Arial, Color.Black);
                else if (it is Arrow)
                    DrawText(it.Name + "(" + ArenaDrops[i.Key].Count + ")", (int)i.Value.x - CameraPosX - 5, (int)i.Value.y - CameraPosY - 5, 10, Fonts.Arial, Color.Black);
            }

            /*var mp = Players[MainPlayer];
            DrawText("HP " + mp.Health, 10, 30, 30, Fonts.Arial, Color.Black);
            DrawText("Mana " + mp.inventory.getMana(), 10, 60, 30, Fonts.Arial, Color.Black);
            DrawText("Arrows " + mp.inventory.getArrowsAmount(), 10, 90, 30, Fonts.Arial, Color.Black);
            DrawText(Players[MainPlayer].getItemRight().Name, 700, 30, 30, Fonts.Arial, Color.Black);
            DrawText(Players[MainPlayer].inventory.getCurrentArrow().Name, 700, 60, 30, Fonts.Arial, Color.Black);
            */        
            //DrawText(CameraPosX.ToString(), 20, 20, 10, Fonts.Arial, Color.Black);
        }
        public void Pause()
        {
            Timer.Stop();
        }
        public void Run()
        {
            Timer.Start();
        }
        public void UpdateAnimation()
        {
            int DistanceToBoard = 60;
            double speed = 0.4;
            long nowTime = Timer.ElapsedMilliseconds;
            if (NowMouseX < DistanceToBoard)
            {
                if (CameraIsMoving)
                {
                    CameraPosX -= (int)((nowTime - LastMoveCamera) * speed);// * speed moving
                }
                LastMoveCamera = nowTime;
                CameraIsMoving = true;
            }
            if (NowMouseX > Width - DistanceToBoard)
            {
                if (CameraIsMoving)
                {
                    CameraPosX += (int)((nowTime - LastMoveCamera) * speed);// * speed moving
                }
                LastMoveCamera = nowTime;
                CameraIsMoving = true;
            }
            if (NowMouseY < DistanceToBoard)
            {
                if (CameraIsMoving)
                {
                    CameraPosY -= (int)((nowTime - LastMoveCamera) * speed);// * speed moving
                }
                LastMoveCamera = nowTime;
                CameraIsMoving = true;
            }
            if (NowMouseY > Height - DistanceToBoard)
            {
                if (CameraIsMoving)
                {
                    CameraPosY += (int)((nowTime - LastMoveCamera) * speed);// * speed moving
                }
                LastMoveCamera = nowTime;
                CameraIsMoving = true;
            }
            if (NowMouseX >= DistanceToBoard && NowMouseX <= Width - DistanceToBoard && NowMouseY >= DistanceToBoard && NowMouseY <= Height - DistanceToBoard)
                CameraIsMoving = false;
        }
        public void AddPlayer(int tag)
        {
            if (MainPlayer == -1)
                MainPlayer = tag;
            viewPlayers.Add(tag, new PlayerView(-1, -1));
        }
        public void RemovePlayer(int tag)
        {
            viewPlayers.Remove(tag);
        }
        public void AddDrop(int tag)
        {

        }
        public void RemoveDrop(int tag)
        {

        }
        public void AddArrow(int tag)
        {

        }
        public void RemoveArrow(int tag)
        {

        }
        public void MovePlayer(int tag, Tuple<double, double> vect)
        {
        }
        public void NewGame()
        {
            //Cleare all data
            WasInit = false;
            viewPlayers = new Dictionary<int, PlayerView>();
            MainPlayer = -1;
            Timer.Restart();
        }
        public Tuple<double, double> AngleByMousePos()
        {
            var mp = viewPlayers[MainPlayer];
            return Utily.MakePair<double>(NowMouseX - mp.x + CameraPosX, NowMouseY - mp.y + CameraPosY);
        }        
        public void Scroll(int i)
        {
            //if i == 1 - forward scroll. i == -1 - back scroll
        }
        
        //end BattleDraw and data

        public void OnMouseMove(ref MouseMoveEventArgs args)
        {
            MenuButtonStart.CheckFocusing(args.X, args.Y, ButtonStatus.Focused, ButtonStatus.Default);
            MenuButtonExit.CheckFocusing(args.X, args.Y, ButtonStatus.Focused, ButtonStatus.Default);
            NowMouseX = args.X;
            NowMouseY = args.Y;
        }
        public void OnMouseDown(ref MouseButtonEventArgs args)
        {
            MenuButtonStart.CheckFocusing(args.X, args.Y, ButtonStatus.Active, ButtonStatus.Focused);
            MenuButtonExit.CheckFocusing(args.X, args.Y, ButtonStatus.Active, ButtonStatus.Focused);
        }
        public void OnMouseUp(ref MouseButtonEventArgs args)
        {
            MenuButtonStart.CheckFocusing(args.X, args.Y, ButtonStatus.Focused, ButtonStatus.Active);
            MenuButtonExit.CheckFocusing(args.X, args.Y, ButtonStatus.Focused, ButtonStatus.Active);
        }
    }

    public class PlayerView
    {
        public int x { get; set; }
        public int y { get; set; }
        public PlayerView(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public void Draw(View view, CircleShape plr)
        {
            plr.Position = new Vector2f(x - view.CameraPosX - Map.RPlayer, y - view.CameraPosY - Map.RPlayer);
            view.MainForm.Draw(plr);
            //view.DrawText("||", x - view.CameraPosX, y - view.CameraPosY, 10, Fonts.Arial, Color.Black);
        }
    }
}
