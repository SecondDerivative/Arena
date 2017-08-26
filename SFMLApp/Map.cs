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
        public MovableEntity(int Tag, double x, double y, int r)
            : base(Tag, x, y, r)
        {
            this.Speed = new Tuple<double, double>(0, 0);
        }
        public MovableEntity(Entity E)
            : base(E.Tag, E.x, E.y, E.r)
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
        public MPlayer(int Tag, double x, double y)
            : base(Tag, x, y, Map.RPlayer)
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
        public string getData()
        {
            StringBuilder ans = new StringBuilder();
            ans.Append(this.Tag);
            ans.Append(":");
            ans.Append(this.x);
            ans.Append(":");
            ans.Append(this.y);
            ans.Append(":");
            ans.Append(this.r);
            ans.AppendFormat(":{0}:{1}", Speed.Item1, Speed.Item2);
            return ans.ToString();
        }
    }
    public class MArrow : MovableEntity
    {
        public MArrow(int Tag, double x, double y, double SpeedX, double SpeedY)
            : base(Tag, x, y, Map.RArrow)
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
        public string getData()
        {
            StringBuilder ans = new StringBuilder();
            ans.Append(this.Tag);
            ans.Append(":");
            ans.Append(this.x);
            ans.Append(":");
            ans.Append(this.y);
            ans.Append(":");
            ans.Append(this.r);
            ans.AppendFormat(":{0}:{1}", Speed.Item1, Speed.Item2);
            return ans.ToString();
        }
    }
    public class Square
    {
        public int x { get; private set; }
        public int y { get; private set; }
        public int type { get; private set; }
        public Square(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.type = 0;
        }
        public Square(int x, int y, int t)
        {
            this.x = x;
            this.y = y;
            this.type = t;
        }
    }
    public class DropSpawner
    {
        public double x;
        public double y;
        public int id;
        public int count;
        public DropSpawner(double x, double y, int id, int count)
        {
            this.x = x;
            this.y = y;
            this.id = id;
            this.count = count;
        }
    }
    public class Map
    {
        public static int RPlayer = 4;
        public static int Rwidth = 8;
        public static int RArrow = 1;
        public static int RDrop = 4;

        public string Name;

        private Queue<int> ForDelArrow;
        public List<DropSpawner> dropSpawners { get; private set; }
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

        public string getData()
        {
            StringBuilder s = new StringBuilder();
            s.Append(Name);
            s.Append(";");
            bool iswrite = false;
            foreach (var item in drops)
            {
                if (item.Value.Exist)
                {
                    if (!iswrite)
                    {
                        iswrite = true;
                    }
                    else
                    {
                        s.Append(",");
                    }
                    s.Append(item.Value.getData());
                }
            }
            iswrite = false;
            s.Append(";");
            foreach (var item in arrows)
            {
                if (item.Value.Exist)
                {
                    if (!iswrite)
                    {
                        iswrite = true;
                    }
                    else
                    {
                        s.Append(",");
                    }
                    s.Append(item.Value.getData());
                }
            }
            s.Append(";");
            iswrite = false;
            foreach (var item in players)
            {
                if (item.Value.Exist)
                {
                    if (!iswrite)
                    {
                        iswrite = true;
                    }
                    else
                    {
                        s.Append(",");
                    }
                    s.Append(item.Value.getData());
                }
            }
            return s.ToString();
        }
        public void SpawnDrops(int num, int tag)
        {
            var ds = dropSpawners[num];
            this.drops.Add(tag, new MDrop(tag, ds.x, ds.y, num));
        }
        private void AddDropSpawner(double x, double y, int id, int count)
        {
            dropSpawners.Add(new DropSpawner(x, y, id, count));
        }
        public void SpawnDrops(int Tag, double x, double y)
        {
            this.drops.Add(Tag, new MDrop(Tag, x, y));
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
                int nowline = 4 + couArr + couDro + couPl;
                int countSpawn = int.Parse(args[nowline]);
                ++nowline;
                spawners = new List<Tuple<double, double>>();
                for (int i = 0; i < countSpawn; ++i)
                {
                    int x = int.Parse(args[nowline].Split()[0]), y = int.Parse(args[nowline].Split()[1]);
                    ++nowline;
                    spawners.Add(Utily.MakePair<double>(x, y));
                }
                int countSpawnDrop = int.Parse(args[nowline]);
                ++nowline;
                dropSpawners = new List<DropSpawner>();
                for (int i = 0; i < countSpawnDrop; ++i)
                {
                    int x = int.Parse(args[nowline].Split()[0]), y = int.Parse(args[nowline].Split()[1]);
                    int cnt = int.Parse(args[nowline].Split()[2]), id = int.Parse(args[nowline].Split()[3]);
                    ++nowline;
                    dropSpawners.Add(new DropSpawner(x, y, id, cnt));
                }
                tmp = args[nowline].Split().Select(x => int.Parse(x)).ToArray();
                this.Pwidth = tmp[0] + 2; this.Pheight = tmp[1] + 2;
                this.Field = new List<List<Square>>();
                for (int x = 0; x < this.Pwidth; ++x)
                    this.Field.Add(new List<Square>());

                for (int i = 0; i < this.Pwidth; i++)
                    this.Field[i].Add(new Square(i, 0, 1));
                for (int i = 1; i < this.Pheight - 1; i++)
                {
                    this.Field[0].Add(new Square(0, i, 1));
                    this.Field[this.Pwidth - 1].Add(new Square(this.Pwidth - 1, i, 1));
                }
                for (int y = 1; y < this.Pheight - 1; ++y)
                {
                    string line = args[y + nowline].Trim();
                    int[] agrs = line.Split().Select(x => int.Parse(x)).ToArray();
                    for (int x = 1; x < Pwidth - 1; ++x)
                        this.Field[x].Add(new Square(x, y, agrs[x - 1]));
                }
                for (int i = 0; i < this.Pwidth; i++)
                    this.Field[i].Add(new Square(i, this.Pheight - 1, 1));
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
                        sw.Write(Field[x][y].type + " ");
                    sw.WriteLine(Field[Pwidth - 1][y].type);
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
        private double GetDistToVertSegm(double x, double y1, double y2, double x3, double y3)
        {
            double dist = (x3 - x) * (x3 - x);
            if (y3 > y2)
                dist = (x - x3) * (x - x3) + (y2 - y3) * (y2 - y3);
            if (y3 < y1)
                dist = (x - x3) * (x - x3) + (y1 - y3) * (y1 - y3);
            return dist;
        }
        private bool IsCrossCircleSquare(double x, double y, double r, double x1, double y1, double x2, double y2)
        {
            if (y1 > y2)
                Utily.Swap<double>(ref y1, ref y2);
            if (x1 > x2)
                Utily.Swap<double>(ref x1, ref x2);
            double dist1 = GetDistToVertSegm(x1, y1, y2, x, y);
            double dist3 = GetDistToVertSegm(x2, y1, y2, x, y);
            double dist2 = GetDistToVertSegm(-y1, x1, x2, -y, x);
            double dist4 = GetDistToVertSegm(-y2, x1, x2, -y, x);
            double len = Math.Min(Math.Min(dist1, dist2), Math.Min(dist3, dist4));
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
        private bool IsEntityInSquare(Entity e, bool IsArrow)
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
                if (i.type != 0 && (!IsArrow || i.type != 2) && IsCrossCircleSquare(e.x, e.y, e.r, i.x * Map.Rwidth, i.y * Map.Rwidth, i.x * Map.Rwidth + Rwidth, i.y * Map.Rwidth + Rwidth))
                    ans = true;
            }
            return ans;
        }
        private bool IsEntityWillInSquare(Entity e, Tuple<double, double> Speed, bool IsArrow)
        {
            e.x += Speed.Item1;
            e.y += Speed.Item2;
            return IsEntityInSquare(e, IsArrow);
        }
        public Map(int width, int height)
        {
            this.Name = "";
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
            this.Name = path;
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
            List<Tuple<double, double> > ListOfGoodPlace = new List<Tuple<double, double> >();
            foreach (var p in spawners)
            {
                Entity e = new Entity(0, p.Item1, p.Item2, RPlayer);
                bool bol = true;
                foreach (var pl in players)
                    bol = bol && !IsCrossEntity(e, pl.Value);
                if (bol)
                    ListOfGoodPlace.Add(p);
            }
            int i = Utily.Next() % ListOfGoodPlace.Count;
            SpawnPlayer(Tag, (int)ListOfGoodPlace[i].Item1, (int)ListOfGoodPlace[i].Item2);
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
            return Field[((int)Math.Floor(x)) / Rwidth][((int)Math.Floor(y)) / Rwidth].type==0;
        }
        public bool IsCrossEntity(Entity a, Entity b)
        {
            if (!a.Exist || !b.Exist)
                return false;
            return (a.r + b.r) * (a.r + b.r) - (a.x - b.x) * (a.x - b.x) - (a.y - b.y) * (a.y - b.y) >= 0;
        }
        private void ShortUpDatePlayer(int Tag, int Time, Tuple<double, double> Line)
        {
            double originX = players[Tag].x, originY = players[Tag].y;
            MPlayer Pl = players[Tag];
            if (IsEntityWillInSquare(Pl, Line, false))
            {
                players[Tag].x = originX;
                players[Tag].y = originY;
                return;
            }
            bool cross = false;
            foreach (var pl in players)
            {
                cross = cross || (pl.Value.Tag != Tag && IsCrossEntity(pl.Value, Pl));
            }
            if (cross)
            {
                players[Tag].x = originX;
                players[Tag].y = originY;
                return;
            }
            List<int> del = new List<int>();
            foreach (var d in drops)
            {
                if (IsCrossEntity(d.Value, Pl))
                {
                    bool b = d.Value.reason == Reason.BySpawner;
                    if (d.Value.reason == Reason.BySpawner)
                        Q.Enqueue(new MEventDrop(Tag, d.Value.Tag, d.Value.NumSpawner));
                    else
                        Q.Enqueue(new MEventDrop(Tag, d.Value.Tag));
                    del.Add(d.Key);
                }
            }
            for (int i = 0; i < del.Count; ++i)
                drops.Remove(del[i]);
        }
        private void ShortUpDatePlayer(int Tag, int Time)
        {
            Tuple<double, double> Line = new Tuple<double, double>(Time * players[Tag].Speed.Item1 / 4, Time * players[Tag].Speed.Item2 / 4);
            ShortUpDatePlayer(Tag, Time, new Tuple<double, double>(0, Line.Item2));
            ShortUpDatePlayer(Tag, Time, new Tuple<double, double>(Line.Item1, 0));
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
            if (IsEntityInSquare(Ar, true))
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
                    Q.Enqueue(new MEventArrowHit(Tag, p.Key));
                    this.ForDelArrow.Enqueue(Tag);
                    return;
                }
            }
            if (IsEntityInSquare(arrows[Tag], true))
            {
                Q.Enqueue(new MEventDestroyArrow(Tag));
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
        public Reason reason { get; set; }
        public int NumSpawner { get; private set; }
        public MDrop(int Tag, double x, double y)
            : base(Tag, x, y, Map.RDrop)
        {
            this.reason = Reason.ByPlayer;
            NumSpawner = -1;
        }
        public MDrop(int Tag, double x, double y, int NumSpawner)
            : base(Tag, x, y, Map.RDrop)
        {
            this.reason = Reason.BySpawner;
            this.NumSpawner = NumSpawner;
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
            MDrop Dr = new MDrop(Tag, x, y);
            Dr.Exist = Exist;
            return Dr;
        }
        public string getData()
        {
            StringBuilder ans = new StringBuilder();
            ans.Append(this.Tag);
            ans.Append(":");
            ans.Append(this.x);
            ans.Append(":");
            ans.Append(this.y);
            ans.Append(":");
            ans.Append(this.r);
            return ans.ToString();
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
        public int NumSpawner { get; private set; }
        public bool BySpawner { get; private set; }
        public MEventDrop(int TagPlayer, int TagDrop)
            : base(MEvents.PlayerDrop)
        {
            this.BySpawner = false;
            NumSpawner = -1;
            this.TagDrop = TagDrop;
            this.TagPlayer = TagPlayer;
        }
        public MEventDrop(int TagPlayer, int TagDrop, int NumSpawner)
            : base(MEvents.PlayerDrop)
        {
            this.BySpawner = true;
            this.NumSpawner = NumSpawner;
            this.TagDrop = TagDrop;
            this.TagPlayer = TagPlayer;
        }
    }
    public class MEventArrowHit : MEvent
    {
        public int TagArrow { get; private set; }
        public int TagPlayer { get; private set; }
        public MEventArrowHit(int TagArrow, int TagPlayer)
            : base(MEvents.PlayerArrow)
        {
            this.TagArrow = TagArrow;
            this.TagPlayer = TagPlayer;
        }
    }
    public class MEventDestroyArrow : MEvent
    {
        public int TagArrow { get; private set; }
        public MEventDestroyArrow(int TagArrow)
            : base(MEvents.DestroyArrow)
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