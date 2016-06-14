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
    public class Map
    {
        public static int RPlayer = 10;
        public static int RArrow = 5;
        Dictionary<string, MPlayer> players = new Dictionary<string, MPlayer>();
        Dictionary<string, MArrow> arrows = new Dictionary<string, MArrow>();
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
        public void StopPlayer(string Tag)
        {
            players[Tag].Speed = new Tuple<int, int>(0, 0);
        }
        public void MovePlayer(string Tag,Tuple<int,int> Speed)
        {
            players[Tag].Speed = Speed;
        }
    }
}
