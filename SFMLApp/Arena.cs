using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFMLApp
{
    public class Arena
    {
        public const int WaitRespawnDrop = 30000;
        public Map map { get; private set; }
        public Dictionary<int, Player> players { get; private set; }
        public Dictionary<int, AArow> Arrows { get; private set; }
        public Dictionary<int, ADrop> Drops { get; private set; }
        public Dictionary<int, APlayer> ArenaPlayer { get; private set; }
        private Stopwatch timer;
        Queue<Tuple<long, int>> DropForRespawn;

        public Arena()
        {
            ArenaPlayer = new Dictionary<int, APlayer>();
            players = new Dictionary<int, Player>();
            Arrows = new Dictionary<int, AArow>();
            Drops = new Dictionary<int, ADrop>();
            timer = new Stopwatch();
            DropForRespawn = new Queue<Tuple<long, int>>();
        }

        public void NewMap(string name)
        {
            map = new Map("./data/Maps/" + name + ".txt");
            foreach (var i in players)
            {
                map.AddPlayer(i.Key);
                players[i.Key].respawn();
            }
            Arrows.Clear();
            Drops.Clear();
            DropForRespawn.Clear();
            for (int i = 0; i < map.dropSpawners.Count; ++i)
            {
                int tag = Utily.GetTag();
                map.SpawnDrops(i, tag);
                Drops.Add(tag, new ADrop(map.dropSpawners[i].count, map.dropSpawners[i].id));
            }
            timer.Restart();
        }

        public void MovePlayer(int tag, Tuple<double, double> speed)
        {
            double m = players[tag].Speed();
            Tuple<double, double> NewVect = Utily.Normalizing(speed, players[tag].Speed());
            map.MovePlayer(tag, NewVect);
        }

        public int FirePlayer(int tag, Tuple<double, double> vect)
        {
            int dmg = players[tag].attack();
            if (dmg <= 0)
                return -1;
            Tuple<double, double> NewVect = Utily.Normalizing(vect, players[tag].ArrowSpeed());
            int arTag = Utily.GetTag();
            Arrows[arTag] = new AArow(tag, dmg, players[tag].inventory.getCurrentArrow().id);
            map.FirePlayer(tag, arTag, NewVect.Item1, NewVect.Item2);
            return arTag;
        }

        public void StopPlayer(int tag)
        {
            map.StopPlayer(tag);
        }

        public void Update()
        {
            map.UpDate();
            MEvent Event;
            while ((Event = map.NextEvent()) != null)
            {
                if (Event.Type == MEvents.PlayerArrow)
                {
                    int TagArrow = ((MEventArrowHit)Event).TagArrow;
                    int TagPlayer = ((MEventArrowHit)Event).TagPlayer;
                    players[TagPlayer].recieveDamage(Arrows[TagArrow].dmg);
                    if (players[TagPlayer].isDead())
                    {
                        ArenaPlayer[Arrows[TagArrow].creater].AddKill();
                        ArenaPlayer[TagPlayer].AddDeath();
                        int NewTag = Utily.GetTag();
                        map.SpawnDrops(NewTag, map.players[TagPlayer].x, map.players[TagPlayer].y);
                        map.SpawnPlayer(TagPlayer);
                        Drops.Add(NewTag, new ADrop(1, players[TagPlayer].rightHand));
                        players[TagPlayer].respawn();
                    }
                    Arrows.Remove(TagArrow);
                }
                if (Event.Type == MEvents.PlayerDrop)
                {
                    var drop = Drops[((MEventDrop)Event).TagDrop];
                    players[((MEventDrop)Event).TagPlayer].pickedUpItem(drop.id, drop.Count);
                    Drops.Remove(((MEventDrop)Event).TagDrop);
                    if (((MEventDrop)Event).BySpawner)
                    {
                        DropForRespawn.Enqueue(Utily.MakePair<long, int>(timer.ElapsedMilliseconds, ((MEventDrop)Event).NumSpawner));
                    }
                }
                if (Event.Type == MEvents.DestroyArrow)
                {
                    Arrows.Remove(((MEventDestroyArrow)Event).TagArrow);
                }
            }
            while (DropForRespawn.Count > 0 && DropForRespawn.Peek().Item1 + WaitRespawnDrop < timer.ElapsedMilliseconds)
            {
                int num = DropForRespawn.Dequeue().Item2;
                int tag = Utily.GetTag();
                map.SpawnDrops(num, tag);
                Drops.Add(tag, new ADrop(map.dropSpawners[num].count, map.dropSpawners[num].id));
            }
        }

        public bool TagIsUse(int tag)
        {
            return ArenaPlayer.ContainsKey(tag);
        }

        public int AddPlayer(string name)
        {
            int tag = Utily.GetTag();
            players[tag] = new Player();
            players[tag].respawn();
            ArenaPlayer[tag] = new APlayer(name);
            map.AddPlayer(tag);
            map.SpawnPlayer(tag);
            return tag;
        }
        public void RemovePlayer(int tag)
        {
            if (!TagIsUse(tag))
                return;
            map.RemovePlayer(tag);
            players.Remove(tag);
            ArenaPlayer.Remove(tag);
        }
        public void ChangeItem(int tagPlayer, int type)
        {
            if (type == 1)
                players[tagPlayer].NextItem();
            else
                players[tagPlayer].PrevItem();
        }
        public void ChangeArrow(int tagPlayer, int type)
        {
            if (type == 1)
                players[tagPlayer].NextArrow();
            else
                players[tagPlayer].PrevArrow();
        }
        public void Pause()
        {
            map.Pause();
            timer.Stop();
        }
        public void Run()
        {
            map.Run();
            timer.Start();
        }
        public Dictionary<int, string> GetAllInfo()
        {
            var ans = new Dictionary<int, string>();
            string mapinfo = map.GetInfo();
            var small = new Dictionary<int, string>();
            foreach (var i in players)
                small.Add(i.Key, i.Value.SmallString());
            string arenainfo = GetInfo();
            foreach (var i in players)
            {
                string now = mapinfo + "#" + arenainfo + "#";
                bool wasWrite = false;
                foreach (var j in players)
                {
                    if (j.Key != i.Key)
                    {
                        if (wasWrite)
                            now += ";" + j.Key + " " + small;
                        else
                        {
                            now += j.Key + " " + small;
                            wasWrite = false;
                        }
                    }
                }
                now += "#" + i.Value.LargeString();
                ans.Add(i.Key, now);
            }
            return ans;
        }
        public string GetInfo()
        {
            string ans = "";
            bool wasWrite = false;
            foreach (var j in Arrows)
            {
                if (map.arrows[j.Key].Exist)
                    if (wasWrite)
                        ans += "," + j.Key + " " + j.Value.GetInfo();
                    else
                    {
                        ans += j.Key + " " + j.Value.GetInfo();
                        wasWrite = true;
                    }
            }
            ans += "#";
            wasWrite = false;
            foreach (var j in Drops)
            {
                if (map.drops[j.Key].Exist)
                    if (wasWrite)
                        ans += "," + j.Key + " " + j.Value.GetInfo();
                    else
                    {
                        ans += j.Key + " " + j.Value.GetInfo();
                        wasWrite = true;
                    }
            }
            ans += "#";
            wasWrite = false;
            foreach (var j in ArenaPlayer)
            {
                if (map.players[j.Key].Exist)
                    if (wasWrite)
                        ans += "," + j.Key + " " + j.Value.GetInfo();
                    else
                    {
                        ans += j.Key + " " + j.Value.GetInfo();
                        wasWrite = true;
                    }
            }
            return ans;
        }
    }

    public class AArow
    {
        public int dmg { get; set; }
        public int creater { get; set; }
        public int id { get; private set; }

        public AArow(int crt, int dmg, int id)
        {
            creater = crt;
            this.dmg = dmg;
            this.id = id;
        }
        public string GetInfo()
        {
            return dmg + " " + id;
        }
    }

    public class ADrop
    {
        public int Count { get; private set; }
        public int id { get; private set; }
        public ADrop(int cnt, int id)
        {
            Count = cnt;
            this.id = id;
        }
        public string GetInfo()
        {
            return Count + " " + id;
        }
    }

    public class APlayer
    {
        public int Kill { get; private set; }
        public int Death { get; private set; }
        public string RealName;
        public APlayer(string name)
        {
            RealName = name;
            Kill = Death = 0;
        }
        public void AddKill()
        {
            ++Kill;
        }
        public void AddDeath()
        {
            ++Death;
        }
        public string GetInfo()
        {
            return Kill + " " + Death + " " + RealName;
        }
    }
}
