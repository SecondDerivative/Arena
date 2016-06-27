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
        [Fact]
        public void ControlTest()
        {
            var control = new Control(1024, 768);
            control.UpDate(0);
            control.UpDate(1000);
            control.UpDate(40);
        }
        [Fact]
        public void TestMap()
        {
            var map = new Map(10000, 10000);
            map.UpDate(10);
            map.AddDrop("Drop1", 10, 20, Drops.arrows);
            map.AddPlayer("Player1");
            map.SpawnPlayer("Player1", 10, 10);
            map.FirePlayer("Player1", "Arrow1", new Tuple<double, double>(10, 10));
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
            map.AddDrop("d1", 20, 20, Drops.heal);
            map.MovePlayer("p1", new Tuple<double, double>(2, 2));
            map.UpDate(10);
            var ev = map.NextEvent();
            Assert.True(ev.Tag == MEvents.PlayerDrop);
            Assert.True(ev.Tag1 == "p1");
            Assert.True(ev.Tag2 == "d1");


        }
    }
}
