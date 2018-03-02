using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelGenTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            LevelGenTest testApp = new LevelGenTest();
            testApp.GameLoop();
        }
    }
}
