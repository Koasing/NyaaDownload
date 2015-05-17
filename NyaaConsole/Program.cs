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
            XmlReader reader = XmlReader.Create(@"http://leopard-raws.org/rss.php");
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();

            Console.WriteLine("Hello World!");
        }
    }
}
