﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SFMLApp
{
    public class Entity
    {
        public int r { get; private set; }
        public double x { get; set; }
        public double y { get; set; }
        public bool Exist { get; set; }
        private string Tag;
        public Entity(string Tag,double x,double y,int r)
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
        public Tuple<double, double> Speed;
        public MovableEntity(string Tag,double x,double y,int r):base(Tag,x,y,r){
            this.Speed = new Tuple<double, double>(0, 0);
        }
    }
    public class MPlayer : MovableEntity
    {
        public MPlayer(string Tag,double x,double y):base(Tag, x, y, Map.RPlayer)
        {}
    }
    public class MArrow : MovableEntity
    {
        public MArrow(string Tag,double x, double y):base(Tag, x, y, Map.RArrow)
        {}
    }
    public class Square
    {
        private int x, y;
        public bool isEmpty { get; private set; }
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
        public static int RDrop = 10;

        private Queue<MEvent> Q;
        private Stopwatch Timer;
        private int width, height;
        private int Pwidth, Pheight;
        private Dictionary<string, MPlayer> players;
        private Dictionary<string, MArrow> arrows;
        private List<List<Square>> Field;

        private Square getSquare(double x,double y)
        {
            return Field[(int)Math.Floor(x) / Rwidth][(int)Math.Floor(y) / Rwidth];
        }
        private Square getSquare(int x,int y)
        {
            return Field[x][y];
        }
        private bool IsEntityInSquare(Entity e)
        {
            List<Square> a = new List<Square>();
            int x = (int)Math.Floor(e.x) / Rwidth;
            int y = (int)Math.Floor(e.y) / Rwidth;
            a.Add(getSquare(x, y));
            a.Add(getSquare(x+1, y));
            a.Add(getSquare(x-1, y));
            a.Add(getSquare(x, y+1));
            a.Add(getSquare(x, y-1));
            a.Add(getSquare(x+1, y-1));
            a.Add(getSquare(x+1, y+1));
            a.Add(getSquare(x-1, y+1));
            a.Add(getSquare(x-1, y-1));

            bool ans = false;
            foreach (Square i in a)
            {
                if (!i.isEmpty)
                    ans = true;
            }
            return ans;
        }
        private bool IsEntityWillInSquare(Entity e,Tuple<double,double> Speed)
        {
            e.x += Speed.Item1;
            e.y += Speed.Item2;
            return IsEntityInSquare(e);
        }
        private Map(int width, int height)
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
            Q = new Queue<MEvent>();
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
            players[Tag].Speed = new Tuple<double, double>(0, 0);
        }
        public void MovePlayer(string Tag,Tuple<double,double> Speed)
        {
            players[Tag].Speed = Speed;
        }
        private bool IsSquareEmpty(double x,double y)
        {
            return Field[((int)Math.Floor(x)) / Rwidth][((int)Math.Floor(y)) / Rwidth].isEmpty;
        }
        private bool IsCrossEntity(Entity a,Entity b)
        {
            return (a.r+b.r)*(a.r+b.r)-(a.x-b.x)*(a.x - b.x)+(a.y-b.y)* (a.y - b.y)>=0;
        }
        private void ShortUpDatePlayer(string Tag, int Time)
        {
            MPlayer Pl = players[Tag];
            Tuple<double, double> Line = new Tuple<double, double>(Time * Pl.Speed.Item1 / 4, Time * Pl.Speed.Item2 / 4);
            if (IsEntityWillInSquare(Pl,Line))
            {
                StopPlayer(Tag);
                return;
            }
            players[Tag].x += Line.Item1;
            players[Tag].y += Line.Item2;
        }
        private void UpDatePlayer(string Tag,int Time)
        {
            if (players[Tag].Speed.Item1 == 0 && players[Tag].Speed.Item2 == 0)
                return;
            for(int i = 0; i < Time; ++i)
            {
                for(int j=0;j<4;++j)
                    ShortUpDatePlayer(Tag, 1);
            }
        }
        public void FirePlayer(string TagPlayer, string TagArrow, Tuple<double,double> Speed)
        {
            double x = Map.RPlayer / Math.Sqrt(1+Speed.Item2*Speed.Item2/(Speed.Item1*Speed.Item1));
            if (x * Speed.Item1 < 0)
                x = -x;
            double y = Speed.Item1 * x / Speed.Item2;
            arrows.Add(TagArrow, new MArrow(TagArrow, x, y));
        }
        private void ShortUpDateArrow(string Tag, int Time)
        {
            MArrow Ar = arrows[Tag];
            Tuple<double, double> Line = new Tuple<double, double>(Time * Ar.Speed.Item1 / 4, Time * Ar.Speed.Item2 / 4);
            if (IsEntityWillInSquare(Ar,Line))
            {
                //add event
                return;
            }
            arrows[Tag].x += Line.Item1;
            arrows[Tag].y += Line.Item2;
        }
        private void UpDateArrow(string Tag, int Time)
        {
            for (int i = 0; i < Time; ++i)
            {
                for(int j=0;j<4;++j)
                    ShortUpDateArrow(Tag, 1);
            }
        }
        public void UpDate()
        {

        }
    }
    public class MEvent
    {
        public MEvents Tag { get; private set; }
        public string Tag1 { get; private set; }
        public string Tag2 { get; private set; }
        public MEvent(MEvents Tag,string Tag1,string Tag2)
        {
            this.Tag = Tag;
            this.Tag1 = Tag1;
            this.Tag2 = Tag2;
        }
    }
    public enum MEvents
    {
        PlayerDrop,
        PlayerArrow
    }
    public class MDrop : Entity
    {
        public Drops drop;
        public MDrop(string Tag, double x, double y,Drops drop) : base(Tag, x, y, Map.RDrop)
        {
            this.drop = drop;
        }
    }
    public enum Drops
    {
        heal,
        arrows
    }
}