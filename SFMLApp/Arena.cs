﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFMLApp
{
    public class Arena
    {
        private Map map;
        private Dictionary<string, Player> players;
        private SortedSet<string> PlayersTag;
        private Dictionary<string, AArow> Arrows;
        private Stopwatch timer;
        public Dictionary<string, int> kill { get; private set; }
        public Dictionary<string, int> death { get; private set; }
        public Dictionary<string, ADrop> drops { get; private set; }

        public Arena()
        {
            kill = new Dictionary<string, int>();
            death = new Dictionary<string, int>();
            players = new Dictionary<string, Player>();
            PlayersTag = new SortedSet<string>();
            Arrows = new Dictionary<string, AArow>();
            timer = new Stopwatch(); 
        }

        public void NewMap(string name)
        {
            map = new Map("./data/Maps/" + name + ".txt");
            foreach (string i in PlayersTag)
            {
                map.AddPlayer(i);
                players[i].respawn();
            }
            timer.Restart();
        }

        public void MovePlayer(string tag, Tuple<double, double> speed)
        {
            map.MovePlayer(tag, speed);
        }

        public void FirePlayer(string tag, Tuple<double, double> vect)
        {
            //need change vect to speed
            int dmg = players[tag].attack();
            if (dmg <= 0)
                return;
            string arTag = Utily.GetTag(10);
            Arrows[arTag] = new AArow(tag, dmg);
            map.FirePlayer(tag, arTag, vect.Item1, vect.Item2);
        }

        public void StopPlayer(string tag)
        {
            map.StopPlayer(tag);
        }

        public void Update()
        {
            map.UpDate();
            MEvent evnt;
            //need change
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

    public class AArow
    {
        public int dmg { get; set; }
        public string creater { get; set; }

        public AArow(string crt, int dmg)
        {
            creater = crt;
            this.dmg = dmg;
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