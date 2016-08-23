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
        public Map map { get; private set; }
        public Dictionary<int, Player> players { get; private set; }
        public Dictionary<int, AArow> Arrows { get; private set; }
        public Dictionary<int, ADrop> Drops { get; private set; }
        public Dictionary<int, APlayer> ArenaPlayer { get; private set; }
        private Stopwatch timer;
        private Dictionary<string, int> TagName;

        public Arena()
        {
            ArenaPlayer = new Dictionary<int, APlayer>();
            players = new Dictionary<int, Player>();
            Arrows = new Dictionary<int, AArow>();
            timer = new Stopwatch();
            TagName = new Dictionary<string, int>();
        }

        public void NewMap(string name)
        {
            map = new Map("./data/Maps/" + name + ".txt");
            foreach (var i in players)
            {
                map.AddPlayer(i.Key);
                players[i.Key].respawn();
            }
            timer.Restart();
        }

        public void MovePlayer(string name, Tuple<double, double> speed)
        {
            int tag = TagName[name];
            Tuple<double, double> NewVect = Utily.Normalizing(speed, players[tag].Speed());
            map.MovePlayer(TagName[name], NewVect);
        }

        public void FirePlayer(string name, Tuple<double, double> vect)
        {
            int tag = TagName[name];
            int dmg = players[tag].attack();
            if (dmg <= 0)
                return;
            //need change player.Speed() to Arrow.Speed()
            Tuple<double, double> NewVect = Utily.Normalizing(vect, players[tag].Speed());
            int arTag = Utily.GetTag();
            Arrows[arTag] = new AArow(tag, dmg, players[tag].inventory.getCurrentArrow().id);
            map.FirePlayer(tag, arTag, NewVect.Item1, NewVect.Item2);
        }

        public void StopPlayer(string name)
        {
            map.StopPlayer(TagName[name]);
        }

        public void Update()
        {
            map.UpDate();
            MEvent evnt;
            while ((evnt = map.NextEvent()) != null)
            {
                if (evnt.Tag == MEvents.PlayerArrow)
                {
                    players[evnt.Tag1].recieveDamage(Arrows[evnt.Tag2].dmg);
                    if (players[evnt.Tag1].isDead())
                    {
                        ArenaPlayer[Arrows[evnt.Tag2].creater].AddKill(); ;
                        ArenaPlayer[evnt.Tag1].AddDeath();
                    }
                    Arrows.Remove(evnt.Tag2);
                }
                if (evnt.Tag == MEvents.PlayerDrop)
                {
                    var drop = Drops[evnt.Tag2];
                    players[evnt.Tag1].pickedUpItem(drop.id, drop.Count);
                }
            }
            if (timer.ElapsedMilliseconds > 30000)
            {
                timer.Restart();
                //add drop to map
            }
        }

        public bool NickIsUse(string name)
        {
            return TagName.ContainsKey(name);
        }

        public void AddPlayer(string name)
        {
            if (NickIsUse(name))
                return;
            int tag = Utily.GetTag();
            players[tag] = new Player();
            players[tag].respawn();
            ArenaPlayer[tag] = new APlayer(name);
            TagName[name] = tag;
            map.AddPlayer(tag);
            map.SpawnPlayer(tag);
        }
        public void RemovePlayer(string name)
        {
            if (!NickIsUse(name))
                return;
            int tag = TagName[name];
            map.RemovePlayer(tag);
            players.Remove(tag);
            ArenaPlayer.Remove(tag);
            TagName.Remove(name);
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
    }

    public struct AArow
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
    }
}
