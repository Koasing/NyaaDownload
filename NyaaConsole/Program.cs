using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NyaaDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            RssConfig cfg = new RssConfig();

            cfg.Load("config.json");
            cfg.Save("config2.json");
        }
    }
}
