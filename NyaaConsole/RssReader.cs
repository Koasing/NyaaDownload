using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using System.ServiceModel.Syndication;

namespace NyaaDownloader
{
    class RssReader
    {
        public string FeedUrl;

        public bool Read()
        {
            XmlReader reader = XmlReader.Create(FeedUrl);
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();


        }
    }
}
