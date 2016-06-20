using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SFMLApp
{
    public class Entity
    {
        public int x, y,r;
        public bool Exist;
        public string Tag;
        public Entity(string Tag,int x,int y,int r)
        {
            this.r = r;
            this.x = x;
            this.y = y;
            this.Exist = true;
            this.Tag = Tag;
        }
    }
    public class MovableEntity : Entity
    {
        public Tuple<int, int> Speed;
        public MovableEntity(string Tag,int x,int y,int r):base(Tag,x,y,r){
            this.Speed = new Tuple<int, int>(0, 0);
        }
    }
    public class MPlayer : MovableEntity
    {
        public MPlayer(string Tag,int x,int y):base(Tag, x, y, Map.RPlayer)
        {}
    }
    public class MArrow : MovableEntity
    {
        public MArrow(string Tag,int x,int y):base(Tag, x, y, Map.RArrow)
        {}
    }
    public class Square
    {
        public int x, y;
        public bool isEmpty;
        public Square(int x,int y)
        {
            this.x = x;
            this.y = y;
            this.isEmpty = false;
        }
        public Square(int x, int y,bool b)
        {
            this.x = x;
            this.y = y;
            this.isEmpty = b;
        }
    }
    public class Map
    {
        public static int RPlayer = 10;
        public static int Rwidth = 32;
        public static int RArrow = 5;

        

        public Stopwatch Timer;
        public int width, height;
        public int Pwidth, Pheight;
        Dictionary<string, MPlayer> players;
        Dictionary<string, MArrow> arrows;
        List<List<Square>> Field;

        public Map(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.Pwidth = width / Rwidth;
            this.Pheight = height / Rwidth;
            this.players = new Dictionary<string, MPlayer>();
            this.arrows = new Dictionary<string, MArrow>();
            this.Field = new List<List<Square>>();
            for (int x = 0; x < Pheight; ++x)
                this.Field.Add(new List<Square>());
            for(int y = 0; y < Pheight; ++y)
                for(int x = 0; x < Pwidth; ++x)
                {
                    this.Field[y].Add(new Square(x,y));
                }
            Timer = new Stopwatch();
            Timer.Start();
        }
        public void AddPlayer(string Tag)
        {
            players.Add(Tag, new MPlayer(Tag,0,0));
            players[Tag].Exist = false;
        }
        public void SpawnPlayer(string Tag,int x,int y)
        {
            players[Tag].x = x;
            players[Tag].y = y;
            players[Tag].Exist = true;
        }
        private void StopPlayer(string Tag)
        {
            players[Tag].Speed = new Tuple<int, int>(0, 0);
        }
        public void MovePlayer(string Tag,Tuple<int,int> Speed)
        {
            players[Tag].Speed = Speed;
        }
        private bool IsEmpty(int x,int y)
        {
            return Field[x / Rwidth][y / Rwidth].isEmpty;
        }
        private bool IsCrossEntity(Entity a,Entity b)
        {
            return (a.r+b.r)*(a.r+b.r)>=(a.x-b.x)*(a.x - b.x)+(a.y-b.y)* (a.y - b.y);
        }
        private void ShortUpDatePlayer(string Tag, int Time)
        {
            MPlayer Pl = players[Tag];
            Tuple<int, int> Line = new Tuple<int, int>(Time * Pl.Speed.Item1, Time * Pl.Speed.Item2);
            if (!IsEmpty(Pl.x + Line.Item1, Pl.y + Line.Item1))
                return;
            players[Tag].x += Line.Item1;
            players[Tag].y += Line.Item2;
        }
        public void UpDatePlayer(string Tag,int Time)
        {
            for(int i = 0; i < Time; ++i)
            {
                ShortUpDatePlayer(Tag, 1);
            }
        }
    }
}
