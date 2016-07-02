using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SFMLApp;
using Xunit;

namespace Tests
{
    public class Test
    {
        //[Fact]
        //public void ControlTest()
        //{
              //wait when dich go away
          //  var control = new Control(1024, 768);
           // control.UpDate(0);
            //control.UpDate(1000);
            //control.UpDate(40);
        //}
        [Fact]
        public void TestMap()
        {
            var map = new Map("data/Maps/bag.txt");
            bool IsFrame = true;
            for (int i = 0; i < map.Pheight; ++i)
                IsFrame = IsFrame && !(map.Field[0][i].isEmpty);
            Assert.True(IsFrame, "Bad left");
            for (int i = 0; i < map.Pheight; ++i)
                IsFrame = IsFrame && !(map.Field[map.Pwidth - 1][i].isEmpty);
            Assert.True(IsFrame, "Bad right");
            for (int i = 0; i < map.Pwidth; ++i)
                IsFrame = IsFrame && !(map.Field[i][0].isEmpty);
            Assert.True(IsFrame, "Bad top");
            for (int i = 0; i < map.Pwidth; ++i)
                IsFrame = IsFrame && !(map.Field[i][map.Pheight - 1].isEmpty);
            Assert.True(IsFrame, "Bad bottom");
            map.UpDate(10);
            map.AddDrop("Drop1", 10, 20);
            map.AddPlayer("Player1");
            map.SpawnPlayer("Player1", 10, 10);
            map.FirePlayer("Player1", "Arrow1", 10, 10);
            map.ShortUpDateArrow("Arrow1", 10);
            map.MovePlayer("Player1", new Tuple<double, double>(1, 2));
            map.NextEvent();
            map.UpDate(100);
            map.StopPlayer("Player1");
            map.UpDate(10);
            map.UpDate();
        }
        [Fact]
        public void TestEvents()
        {
            var map = new Map(1000, 5000);
            map.AddPlayer("p1");
            map.SpawnPlayer("p1", 10, 10);
            map.AddDrop("d1", 20, 20);
            map.MovePlayer("p1", new Tuple<double, double>(2, 2));
            map.UpDate(10);
            var ev = map.NextEvent();
            Assert.True(ev.Tag == MEvents.PlayerDrop);
            Assert.True(ev.Tag1 == "p1");
            Assert.True(ev.Tag2 == "d1");
        }
        [Fact]
        public void TestMapSave()
        {
            var map = new Map(1000,700);
            map.AddPlayer("p1");
            map.AddPlayer("p2");
            map.SpawnPlayer("p1", 10, 20);
            map.SpawnPlayer("p2",30,60);
            map.FirePlayer("p1", "a1", 1, 2);
            map.MovePlayer("p2", new Tuple<double, double>(-1,-2));
            map.UpDate(5);
            //map.SaveMap("D:/save.txt");
        }

        [Fact]
        public void UtilyTest()
        {
            Assert.True(25 == SFMLApp.Utily.Hypot2(3, 4), "Bad Hypot2(int)");
            Assert.True(SFMLApp.Utily.DoubleIsEqual(0.3, 0.1 + 0.2), "Bad double cmp");
            Assert.True(SFMLApp.Utily.DoubleIsEqual(5, SFMLApp.Utily.Hypot(3, 4)), "Bad Hypot");
            Assert.True(1000 == SFMLApp.Utily.GetTag(1000).Length, "bad tag");
        }

        [Fact]
        public void ArenaTest()
        {
            var arena = new Arena();
            arena.NewMap("bag");
            arena.AddPlayer("Tolya");
            arena.AddPlayer("prifio");
            arena.AddPlayer("aSh");
            arena.RemovePlayer("aSh");
            arena.MovePlayer("Tolya", new Tuple<double, double>(3, 4));
            arena.FirePlayer("prifio", new Tuple<double, double>(4, -3));
        }
    }
}
