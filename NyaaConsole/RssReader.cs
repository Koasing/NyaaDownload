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
        public string BaseUrl;
        public string Keyword;
        public string DownloadFolder;
        public DateTimeOffset LastDownload;
        public string Description;

        private SyndicationFeed RssFeed;

        public bool Read()
        {
            try
            {
                XmlReader reader = XmlReader.Create(BuildRssFeedUrl());
                RssFeed = SyndicationFeed.Load(reader);
                reader.Close();
            }
            catch(Exception e)
            {
                // write log
                Console.WriteLine("OOPS: RSS Read failed... {0}", e);
                return false;
            }

            return true;
        }

        private string BuildRssFeedUrl()
        {
            // replace space with "+" character
            return BaseUrl + Keyword.Replace(' ', '+');
        }
    }
}
