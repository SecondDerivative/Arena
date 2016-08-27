using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace SFMLApp
{
    public class Entity
    {
        public int r { get; private set; }
        public double x { get; set; }
        public double y { get; set; }
        public bool Exist { get; set; }
        public int Tag { get; private set; }
        public Entity(int Tag, double x, double y, int r)
        {
            this.r = r;
            this.x = x;
            this.y = y;
            this.Exist = true;
            this.Tag = Tag;
        }
        public static Entity Load(string s)
        {
            string[] args = s.Split().ToArray();
            Entity E = new Entity(int.Parse(args[0]), double.Parse(args[1]), double.Parse(args[2]), int.Parse(args[3]));
            return E;
        }
    }
    public class MovableEntity : Entity
    {
        public Tuple<double, double> Speed;
        public MovableEntity(int Tag, double x, double y, int r) : base(Tag, x, y, r)
        {
            this.Speed = new Tuple<double, double>(0, 0);
        }
        public MovableEntity(Entity E) : base(E.Tag, E.x, E.y, E.r)
        {
            this.Speed = new Tuple<double, double>(0, 0);
        }
        public MovableEntity Load(string s)
        {
            string[] args = s.Split().ToArray();
            string ss = args[0] + " " + args[1] + " " + args[2] + " " + args[3];
            Entity E = Load(ss);
            MovableEntity ME = new MovableEntity(E);
            ME.Speed = new Tuple<double, double>(double.Parse(args[4]), double.Parse(args[5]));
            return ME;
        }
    }
    public class MPlayer : MovableEntity
    {
        public MPlayer(int Tag, double x, double y) : base(Tag, x, y, Map.RPlayer)
        { }
        public static MPlayer Load(string save)
        {
            string[] args = save.Split().ToArray();
            bool tmp;
            bool Exist = Boolean.TryParse(args[0], out tmp);
            double x = double.Parse(args[1]), y = double.Parse(args[2]);
            Tuple<double, double> Speed = new Tuple<double, double>(double.Parse(args[3]), double.Parse(args[4]));
            MPlayer Pl = new MPlayer(Utily.GetTag(), x, y);
            Pl.Speed = Speed;
            Pl.Exist = Exist;
            return Pl;
        }
        public override string ToString()
        {
            return this.Tag + " " + this.Exist + " " + this.x + " " + this.y + " " + this.Speed.Item1 + " " + this.Speed.Item2;
        }
    }
    public class MArrow : MovableEntity
    {
        public MArrow(int Tag, double x, double y, double SpeedX, double SpeedY) : base(Tag, x, y, Map.RArrow)
        { this.Speed = new Tuple<double, double>(SpeedX, SpeedY); }
        public static MArrow Load(string save)
        {
            string[] args = save.Split().ToArray();
            int Tag = int.Parse(args[0]);
            bool tmp;
            bool Exist = Boolean.TryParse(args[1], out tmp);
            double x = double.Parse(args[2]), y = double.Parse(args[3]);
            MArrow Ar = new MArrow(Tag, x, y, double.Parse(args[4]), double.Parse(args[5]));
            Ar.Exist = Exist;
            return Ar;
        }
        public override string ToString()
        {
            return this.Tag + " " + this.Exist + " " + this.x + " " + this.y + " " + this.Speed.Item1 + " " + this.Speed.Item2;
        }
    }
    public class Square
    {
        public int x { get; private set; }
        public int y { get; private set; }
        public bool isEmpty { get; private set; }
        public Square(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.isEmpty = true;
        }
        public Square(int x, int y, bool b)
        {
            this.x = x;
            this.y = y;
            this.isEmpty = b;
        }
    }
    public class DropSpawner
    {
        public double x;
        public double y;
        public int id;
        public int count;
        public DropSpawner(double x,double y,int id,int count)
        {
            this.x = x;
            this.y = y;
            this.id = id;
            this.count = count;
        }
    }
    public class Map
    {
        public static int RPlayer = 10;
        public static int Rwidth = 10;
        public static int RArrow = 5;
        public static int RDrop = 10;

        private Queue<int> ForDelArrow;
        private List<DropSpawner> dropSpawners;
        private List<Tuple<double, double>> spawners;
        private Queue<MEvent> Q;
        private Stopwatch Timer;
        private int width, height;
        public int Pheight { get; private set; }
        public int Pwidth { get; private set; }
        public Dictionary<int, MPlayer> players { get; private set; }
        public Dictionary<int, MArrow> arrows { get; private set; }
        public List<List<Square>> Field { get; private set; }
        public Dictionary<int, MDrop> drops { get; private set; }

        public void SpawnDrops(int num)
        {

            foreach (var ds in dropSpawners)
            {
                Entity e = new Entity(0, ds.x, ds.y, RDrop);
                bool bol = true;
                foreach (var d in drops)
                {
                    if (IsCrossEntity(d.Value, e))
                    {
                        bol = false;
                    }
                }
                if (bol)
                {
                    int Tag = Utily.GetTag();
                    this.drops.Add(Tag, new MDrop(Tag, ds.x, ds.y, Reason.BySpawner));
                    return;
                }
            }
        }
        private void AddDropSpawner(double x, double y,int id,int count)
        {
            dropSpawners.Add(new DropSpawner(x,y,id,count));
        }
        public void SpawnDrops(int Tag, double x, double y)
        {
            this.drops.Add(Tag, new MDrop(Tag, x, y,Reason.ByPlayer));
        }
        private void AddSpawner(double x, double y)
        {
            spawners.Add(new Tuple<double, double>(x, y));
        }
        public void LoadMap(string path)
        {
            using (StreamReader sr = File.OpenText(path))
            {
                string s = "";
                List<string> args = new List<string>();
                while ((s = sr.ReadLine()) != null)
                {
                    args.Add(s);
                }
                int[] tmp = args[0].Split().Select(x => int.Parse(x)).ToArray();
                this.width = tmp[0]; this.height = tmp[1];
                int couPl = int.Parse(args[1]);
                this.players = new Dictionary<int, MPlayer>();
                for (int i = 2; i < couPl + 2; ++i)
                {
                    string stmp = args[i];
                    string[] Tmp = stmp.Split().ToArray();
                    this.players.Add(int.Parse(Tmp[0]), MPlayer.Load(stmp));
                }
                int couArr = int.Parse(args[couPl + 2]);
                this.arrows = new Dictionary<int, MArrow>();
                for (int i = couPl + 3; i < couPl + 3 + couArr; ++i)
                {
                    string stmp = args[i];
                    string[] Tmp = stmp.Split().ToArray();
                    this.arrows.Add(int.Parse(Tmp[0]), MArrow.Load(stmp));
                }
                int couDro = int.Parse(args[couArr + couPl + 3]);
                this.drops = new Dictionary<int, MDrop>();
                for (int i = couPl + 4 + couArr; i < couPl + 4 + couArr + couDro; ++i)
                {
                    string stmp = args[i];
                    string[] Tmp = stmp.Split().ToArray();
                    this.drops.Add(int.Parse(Tmp[0]), MDrop.Load(stmp));
                }
                int index = couPl + couArr + couDro + 4;
                tmp = args[index].Split().Select(x => int.Parse(x)).ToArray();
                this.Pwidth = tmp[0] + 2; this.Pheight = tmp[1] + 2;
                this.Field = new List<List<Square>>();
                for (int x = 0; x < this.Pwidth; ++x)
                    this.Field.Add(new List<Square>());

                for (int i = 0; i < this.Pwidth; i++)
                    this.Field[i].Add(new Square(i, 0, false));
                for (int i = 1; i < this.Pheight - 1; i++)
                {
                    this.Field[0].Add(new Square(0, i, false));
                    this.Field[this.Pwidth - 1].Add(new Square(this.Pwidth - 1, i, false));
                }
                bool tmpb;
                for (int y = 1; y < this.Pheight - 1; ++y)
                {
                    string line = args[y + index];
                    bool[] bol = line.Split().Select(x => bool.TryParse(x, out tmpb)).ToArray();
                    for (int x = 1; x < Pwidth - 1; ++x)
                        this.Field[x].Add(new Square(x, y, bol[x - 1]));
                }
                for (int i = 0; i < this.Pwidth; i++)
                    this.Field[i].Add(new Square(i, this.Pheight - 1, false));
            }
        }
        public void SaveMap(string path)
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(this.width + " " + this.height);
                sw.WriteLine(players.Count);
                foreach (var p in players)
                {
                    sw.WriteLine(p.Value.ToString());
                }
                sw.WriteLine(arrows.Count);
                foreach (var a in arrows)
                {
                    sw.WriteLine(a.Value.ToString());
                }
                sw.WriteLine(drops.Count);
                foreach (var d in drops)
                {
                    sw.WriteLine(d.Value.ToString());
                }
                sw.WriteLine(Pwidth + " " + Pheight);
                for (int y = 0; y < Pheight; ++y)
                {
                    for (int x = 0; x < Pwidth - 1; ++x)
                        sw.Write(Field[x][y].isEmpty + " ");
                    sw.WriteLine(Field[Pwidth - 1][y].isEmpty);
                }

            }
        }
        private double mul(double x1, double y1, double x2, double y2)
        {
            return (x1 * x2 + y1 * y2);
        }
        private double Length(double x, double y, double x1, double y1, double x2, double y2)
        {
            double len = ((y1 - y2) * x + (x2 - x1) * y + (x1 * y2 - x2 * y1)) * ((y1 - y2) * x + (x2 - x1) * y + (x1 * y2 - x2 * y1)) / ((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
            if (mul(x - x1, y - y1, x2 - x1, y2 - y1) < 0 || mul(x - x2, y - y2, x2 - x1, y2 - y1) < 0)
            {
                return Math.Min(Utily.Hypot2(x - x1, y - y1), Utily.Hypot2(x - x2, y - y2));
            }
            return len;
        }
        private bool IsCrossCircleSquare(double x, double y, double r, double x1, double y1, double x2, double y2)
        {
            double len1 = Length(x, y, x1, y1, x2, y1);
            double len2 = Length(x, y, x1, y1, x1, y2);
            double len3 = Length(x, y, x2, y2, x1, y2);
            double len4 = Length(x, y, x2, y2, x2, y1);
            double len = Math.Min(Math.Min(len1, len2), Math.Min(len3, len4));
            return len < r * r;
        }
        public MEvent NextEvent()
        {
            if (Q.Count == 0)
                return null;
            return Q.Dequeue();
        }
        //public void AddDrop(int Tag, double x, double y)
        //{
        //    drops.Add(Tag, new MDrop(Tag, x, y));
        //}
        private Square getSquare(double x, double y)
        {
            return Field[(int)Math.Floor(x) / Rwidth][(int)Math.Floor(y) / Rwidth];
        }
        private Square getSquare(int x, int y)
        {
            return Field[x][y];
        }
        private bool IsEntityInSquare(Entity e)
        {
            List<Square> a = new List<Square>();
            int x = (int)Math.Floor(e.x) / Rwidth;
            int y = (int)Math.Floor(e.y) / Rwidth;
            a.Add(getSquare(x, y));
            if (x + 1 < Pwidth)
                a.Add(getSquare(x + 1, y));
            if (x >= 1)
                a.Add(getSquare(x - 1, y));
            if (y + 1 < Pheight)
                a.Add(getSquare(x, y + 1));
            if (y >= 1)
                a.Add(getSquare(x, y - 1));
            if (x + 1 < Pwidth && y >= 1)
                a.Add(getSquare(x + 1, y - 1));
            if (x + 1 < Pwidth && y + 1 < Pheight)
                a.Add(getSquare(x + 1, y + 1));
            if (x > 1 && y + 1 < Pheight)
                a.Add(getSquare(x - 1, y + 1));
            if (x > 1 && y >= 1)
                a.Add(getSquare(x - 1, y - 1));

            bool ans = false;
            foreach (Square i in a)
            {
                if (IsCrossCircleSquare(e.x, e.y, Map.RPlayer, i.x * Map.Rwidth, i.y * Map.Rwidth, i.x * Map.Rwidth + Rwidth - 1, i.y * Map.Rwidth + Rwidth - 1) && !i.isEmpty)
                    ans = true;
            }
            return ans;
        }
        private bool IsEntityWillInSquare(Entity e, Tuple<double, double> Speed)
        {
            e.x += Speed.Item1;
            e.y += Speed.Item2;
            return IsEntityInSquare(e);
        }
        public Map(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.Pwidth = width / Rwidth;
            this.Pheight = height / Rwidth;
            this.players = new Dictionary<int, MPlayer>();
            this.arrows = new Dictionary<int, MArrow>();
            this.Field = new List<List<Square>>();
            this.drops = new Dictionary<int, MDrop>();
            this.spawners = new List<Tuple<double, double>>();
            this.dropSpawners = new List<DropSpawner>();
            for (int x = 0; x < Pwidth; ++x)
                this.Field.Add(new List<Square>());
            for (int x = 0; x < Pwidth; ++x)
                for (int y = 0; y < Pheight; ++y)
                    this.Field[x].Add(new Square(x, y));
            this.Timer = new Stopwatch();
            this.Q = new Queue<MEvent>();
            this.ForDelArrow = new Queue<int>();
        }
        public Map(string path)
        {

            this.Timer = new Stopwatch();
            this.Q = new Queue<MEvent>();
            this.ForDelArrow = new Queue<int>();
            this.spawners = new List<Tuple<double, double>>();
            this.dropSpawners = new List<DropSpawner>();
            LoadMap(path);
        }
        public void AddPlayer(int Tag)
        {
            players.Add(Tag, new MPlayer(Tag, 0, 0));
            players[Tag].Exist = false;
        }
        public void RemovePlayer(int tag)
        {
            players.Remove(tag);
        }
        public void SpawnPlayer(int Tag, int x, int y)
        {
            players[Tag].x = x;
            players[Tag].y = y;
            players[Tag].Exist = true;
        }
        public void SpawnPlayer(int Tag)
        {
            foreach (var p in spawners)
            {
                Entity e = new Entity(0, p.Item1, p.Item2, RPlayer);
                bool bol = true;
                foreach (var pl in players)
                {
                    if (IsCrossEntity(pl.Value, e))
                    {
                        bol = false;
                    }
                }
                if (bol)
                {
                    SpawnPlayer(Tag, (int)p.Item1, (int)p.Item2);
                    return;
                }
            }
        }
        public void StopPlayer(int Tag)
        {
            players[Tag].Speed = new Tuple<double, double>(0, 0);
        }
        public void MovePlayer(int Tag, Tuple<double, double> Speed)
        {
            players[Tag].Speed = Speed;
        }
        private bool IsSquareEmpty(double x, double y)
        {
            return Field[((int)Math.Floor(x)) / Rwidth][((int)Math.Floor(y)) / Rwidth].isEmpty;
        }
        public bool IsCrossEntity(Entity a, Entity b)
        {
            if (!a.Exist || !b.Exist)
                return false;
            return (a.r + b.r) * (a.r + b.r) - (a.x - b.x) * (a.x - b.x) - (a.y - b.y) * (a.y - b.y) >= 0;
        }
        private void ShortUpDatePlayer(int Tag, int Time)
        {
            MPlayer Pl = players[Tag];
            if (Pl.Speed.Item1 == 0 && Pl.Speed.Item2 == 0)
                return;
            Tuple<double, double> Line = new Tuple<double, double>(Time * Pl.Speed.Item1 / 4, Time * Pl.Speed.Item2 / 4);
            if (IsEntityWillInSquare(Pl, Line))
            {
                StopPlayer(Tag);
                return;
            }
            players[Tag].x += Line.Item1;
            players[Tag].y += Line.Item2;
            bool cross = false;
            MPlayer PL = players[Tag];
            foreach (var pl in players)
            {
                if (pl.Value.Tag != Tag && IsCrossEntity(pl.Value, PL))
                    cross = true;
            }
            if (cross)
            {
                players[Tag].x -= Line.Item1;
                players[Tag].y -= Line.Item2;
                return;
            }
            List<int> del = new List<int>();
            foreach (var d in drops)
            {
                if (IsCrossEntity(d.Value, Pl))
                {
                    Q.Enqueue(new MEventDrop(Tag, d.Value.Tag, false));//false need change
                    del.Add(d.Key);
                }
            }
            for (int i = 0; i < del.Count; ++i)
                drops.Remove(del[i]);
        }
        private void UpDatePlayer(int Tag, int Time)
        {
            if (players[Tag].Speed.Item1 == 0 && players[Tag].Speed.Item2 == 0)
                return;
            for (int i = 0; i < Time; ++i)
            {
                for (int j = 0; j < 4; ++j)
                    ShortUpDatePlayer(Tag, 1);
            }
        }
        public void FirePlayer(int TagPlayer, int TagArrow, double SpeedX, double SpeedY)
        {
            double abs = Math.Sqrt(SpeedX * SpeedX + SpeedY * SpeedY);
            double x = (RPlayer + RArrow + 1) * SpeedX / abs;
            double y = (RPlayer + RArrow + 1) * SpeedY / abs;
            arrows.Add(TagArrow, new MArrow(TagArrow, x + players[TagPlayer].x, y + players[TagPlayer].y, SpeedX, SpeedY));
        }
        public void ShortUpDateArrow(int Tag, int Time)
        {
            MArrow Ar = arrows[Tag];
            Tuple<double, double> Line = new Tuple<double, double>(Time * Ar.Speed.Item1 / 4.0, Time * Ar.Speed.Item2 / 4.0);
            if (IsEntityInSquare(Ar))
            {
                this.ForDelArrow.Enqueue(Tag);
                Q.Enqueue(new MEventDestroyArrow(Tag));
                return;
            }
            arrows[Tag].x += Line.Item1;
            arrows[Tag].y += Line.Item2;
            foreach (var p in players)
            {
                if (IsCrossEntity(p.Value, Ar))
                {
                    Q.Enqueue(new MEventArrowHit(p.Key, Tag));
                    this.ForDelArrow.Enqueue(Tag);
                    return;
                }
            }
            if (IsEntityInSquare(arrows[Tag]))
            {
                this.ForDelArrow.Enqueue(Tag);
            }
        }
        private void UpDateArrow(int Tag, int Time)
        {
            for (int i = 0; i < Time; ++i)
            {
                for (int j = 0; j < 4; ++j)
                    ShortUpDateArrow(Tag, 1);
            }
            if (IsEntityInSquare(arrows[Tag]))
            {
                arrows.Remove(Tag);
            }
        }
        public void UpDate()
        {
            int Time = (int)Timer.ElapsedMilliseconds;
            UpDate(Time);
            Timer.Restart();
        }
        public void UpDate(int Time)
        {
            for (int i = 0; i < 4 * Time; ++i)
            {
                foreach (var p in players)
                {
                    ShortUpDatePlayer(p.Key, 1);
                }
                foreach (var a in arrows)
                {
                    ShortUpDateArrow(a.Key, 1);
                }
                while (this.ForDelArrow.Count > 0)
                {
                    int s = ForDelArrow.Dequeue();
                    if (arrows.ContainsKey(s))
                        arrows.Remove(s);
                }
            }
        }
        public void Pause()
        {
            Timer.Stop();
        }
        public void Run()
        {
            Timer.Start();
        }
    }
    public class MDrop : Entity
    {
        public Reason reason;
        public MDrop(int Tag, double x, double y, Reason reason) : base(Tag, x, y, Map.RDrop)
        {
            this.reason = reason;
        }
        public override string ToString()
        {
            return this.Tag + " " + this.Exist + " " + this.x + " " + this.y;
        }
        public static MDrop Load(string save)
        {
            string[] args = save.Split().ToArray();
            int Tag = int.Parse(args[0]);
            bool tmp;
            bool Exist = Boolean.TryParse(args[1], out tmp);
            double x = double.Parse(args[2]), y = double.Parse(args[3]);
            MDrop Dr = new MDrop(Tag, x, y,Reason.ByPlayer);
            Dr.Exist = Exist;
            return Dr;
        }

    }
    public enum Reason
    {
        ByPlayer,
        BySpawner
    }
    public enum Drops
    {
        heal,
        arrows
    }

    public class MEvent
    {
        public MEvents Type { get; private set; }
        public MEvent(MEvents type)
        {
            this.Type = type;
        }
    }
    public class MEventDrop : MEvent
    {
        public int TagPlayer { get; private set; }
        public int TagDrop { get; private set; }//if Drop was spawned by spawner, this return number of spawner
        public bool BySpawner { get; private set; }
        public MEventDrop(int TagPlayer, int TagDrop, bool BySpawner) : base(MEvents.PlayerDrop)
        {
            this.BySpawner = BySpawner;
            this.TagDrop = TagDrop;
            this.TagPlayer = TagPlayer;
        }
    }
    public class MEventArrowHit : MEvent
    {
        public int TagArrow { get; private set; }
        public int TagPlayer { get; private set; }
        public MEventArrowHit(int TagArrow, int TagPlayer) : base(MEvents.PlayerArrow)
        {
            this.TagArrow = TagArrow;
            this.TagPlayer = TagPlayer;
        }
    }
    public class MEventDestroyArrow : MEvent
    {
        public int TagArrow { get; private set; }
        public MEventDestroyArrow(int TagArrow) : base(MEvents.DestroyArrow)
        {
            this.TagArrow = TagArrow;
        }
    }
    public enum MEvents
    {
        PlayerDrop,
        PlayerArrow,
        DestroyArrow
    }
}