using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
