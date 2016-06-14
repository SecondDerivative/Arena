using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFMLApp
{
    public class Entity
    {
        public int x, y,r;
        public bool Exist;
        public string Tag;
        Entity(string Tag,int x,int y,int r)
        {
            this.r = r;
            this.x = x;
            this.y = y;
            this.Exist = true;
            this.Tag = Tag;
        }
    }
    public class MPlayer : Entity
    {
        public MPlayer(string Tag,int x,int y,int r)
        {}
    }
    public class MArrow : Entity
    {
        public MArrow(string Tag,int x,int y,int r)
        {}
    }
    public class Map
    {
        int RPlayer = 5;
        Dictionary<string, MPlayer> players = new Dictionary<string, MPlayer>();
        Dictionary<string, MArrow> arrows = new Dictionary<string, MArrow>();
        public void AddPlayer(string Tag)
        {
            players.Add(Tag, new MPlayer(Tag,0,0,RPlayer));
            players[Tag].Exist = false;
        }
        public void SpawnPlayer(string Tag,int x,int y)
        {
            players[Tag].x = x;
            players[Tag].y = y;
            players[Tag].Exist = true;
        }
        
    }
}
