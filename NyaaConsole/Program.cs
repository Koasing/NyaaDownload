using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using System.ServiceModel.Syndication;

namespace NyaaDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            RssReader test = new RssReader();

            test.BaseUrl = @"http://www.nyaa.se/?page=rss&term=";
            test.Keyword = @"Leopard Raws";

            test.LastDownload = new DateTimeOffset(2015,  5,  1,  0,  0,  0, new TimeSpan(0));

            test.Read();

            Console.WriteLine("Hello World!");
        }
    }
}
