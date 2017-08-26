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
            Assert.True(map.getData() == "data/Maps/bag.txt;;;", "1 getData wrong " + map.getData());
            bool IsFrame = true;
            for (int i = 0; i < map.Pheight; ++i)
                IsFrame = IsFrame && !(map.Field[0][i].type == 0);
            Assert.True(IsFrame, "Bad left");
            for (int i = 0; i < map.Pheight; ++i)
                IsFrame = IsFrame && !(map.Field[map.Pwidth - 1][i].type == 0);
            Assert.True(IsFrame, "Bad right");
            for (int i = 0; i < map.Pwidth; ++i)
                IsFrame = IsFrame && !(map.Field[i][0].type == 0);
            Assert.True(IsFrame, "Bad top");
            for (int i = 0; i < map.Pwidth; ++i)
                IsFrame = IsFrame && !(map.Field[i][map.Pheight - 1].type == 0);
            Assert.True(IsFrame, "Bad bottom");
            map.UpDate(10);
            map.SpawnDrops(1, 10, 20);
            map.AddPlayer(1);
            map.SpawnPlayer(1, 10, 10);
            map.SpawnDrops(2, 20, 10);
            map.AddPlayer(2);
            map.SpawnPlayer(2, 20, 20);
            map.FirePlayer(1, 1, 10, 10);
            map.NextEvent();
            map.UpDate(100);
            map.StopPlayer(1);
            map.UpDate(10);
            map.UpDate();
        }
        [Fact]
        public void TestEvents()
        {
            var map = new Map(1000, 5000);
            map.AddPlayer(0);
            map.SpawnPlayer(0, 10, 10);
            map.SpawnDrops(1, 20, 20);
            map.MovePlayer(0, new Tuple<double, double>(2, 2));
            map.UpDate(10);
            var ev = map.NextEvent();
            Assert.True(ev.Type == MEvents.PlayerDrop);
            var NewEv = (MEventDrop)ev;
            Assert.True(NewEv.TagPlayer == 0);
            Assert.True(NewEv.TagDrop == 1);
        }
        [Fact]
        public void TestMapSave()
        {
            var map = new Map(1000, 700);
            map.AddPlayer(1);
            map.AddPlayer(2);
            map.SpawnPlayer(1, 10, 20);
            map.SpawnPlayer(2, 30, 60);
            map.FirePlayer(1, 1, 1, 2);
            map.MovePlayer(2, new Tuple<double, double>(-1, -2));
            map.UpDate(5);
            //map.SaveMap("D:/save.txt");
        }

        [Fact]
        public void UtilyTest()
        {
            Assert.True(25 == SFMLApp.Utily.Hypot2(3, 4), "Bad Hypot2(int)");
            Assert.True(SFMLApp.Utily.DoubleIsEqual(0.3, 0.1 + 0.2), "Bad double cmp");
            Assert.True(SFMLApp.Utily.DoubleIsEqual(5, SFMLApp.Utily.Hypot(3, 4)), "Bad Hypot");
        }

        [Fact]
        public void PlayerTest()
        {
            var player = new Player();
            player.attack();
            player.pickedUpItem(Items.allItems[3]);
            player.takeItemLeft(player.inventory.getItem(3));
            player.attack();
            player.recieveDamage(100);
            player.isDead();
            player.respawn();
            var bottle = new HPBottle("bottle", 14, 60);
            bottle.Consume(player);
        }
        [Fact]
        public void ArenaTest()
        {
            var arena = new Arena();
            arena.NewMap("bag");
            int tag1 = arena.AddPlayer("Tolya");
            int tag2 = arena.AddPlayer("prifio");
            arena.MovePlayer(tag1, new Tuple<double, double>(3, 4));
            arena.FirePlayer(tag2, new Tuple<double, double>(4, -3));
        }
    }
}