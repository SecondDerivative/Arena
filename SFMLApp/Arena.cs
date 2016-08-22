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
        public Dictionary<string, Player> players { get; private set; }
        public Dictionary<string, AArow> Arrows { get; private set; }
        public Dictionary<string, ADrop> drops { get; private set; }
        private Stopwatch timer;
        public Dictionary<string, int> kill { get; private set; }
        public Dictionary<string, int> death { get; private set; }

        public Arena()
        {
            kill = new Dictionary<string, int>();
            death = new Dictionary<string, int>();
            players = new Dictionary<string, Player>();
            Arrows = new Dictionary<string, AArow>();
            timer = new Stopwatch(); 
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

        public void MovePlayer(string tag, Tuple<double, double> speed)
        {
            map.MovePlayer(tag, speed);
        }

        public void FirePlayer(string tag, Tuple<double, double> vect)
        {
            int dmg = players[tag].attack();
            if (dmg <= 0)
                return;
            Tuple<double, double> NewVect = Utily.Normalizing(vect, players[tag].Speed());
            string arTag = Utily.GetTag(10);
            //need change RightHand to CurrentArrow
            Arrows[arTag] = new AArow(tag, dmg, players[tag].rightHand);
            map.FirePlayer(tag, arTag, NewVect.Item1, NewVect.Item2);
        }

        public void StopPlayer(string tag)
        {
            map.StopPlayer(tag);
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
                        ++kill[Arrows[evnt.Tag2].creater];
                        ++death[evnt.Tag1];
                    }
                    Arrows.Remove(evnt.Tag2);
                }
                if (evnt.Tag == MEvents.PlayerDrop)
                {
                    var drop = drops[evnt.Tag2];
                    players[evnt.Tag1].pickedUpItem(drop.id, drop.Count);
                }
            }
            if (timer.ElapsedMilliseconds > 30000)
            {
                timer.Restart();
                //add drop to map
            }
        }

        public void AddPlayer(string tag)
        {
            kill[tag] = death[tag] = 0;
            players[tag] = new Player();
            players[tag].respawn();
            map.AddPlayer(tag);
            map.SpawnPlayer(tag);
        }
        public void RemovePlayer(string tag)
        {
            map.RemovePlayer(tag);
            players.Remove(tag);
            kill.Remove(tag);
            death.Remove(tag);
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
        public string creater { get; set; }
        public int id { get; private set; }

        public AArow(string crt, int dmg, int id)
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
}
