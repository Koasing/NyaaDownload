using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using System.ServiceModel.Syndication;

using NLog;

namespace NyaaDownloader
{
    class RssReader
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public string BaseUrl;
        public string Keyword;
        public string DownloadFolder;
        public DateTimeOffset LastDownload;
        public string Description;

        public bool Read()
        {
            SyndicationFeed RssFeed = null;

            logger.Info("Try to retrieve RSS feed {0}{1}", BaseUrl, Keyword);

            try
            {
                XmlReader reader = XmlReader.Create(BuildRssFeedUrl());
                RssFeed = SyndicationFeed.Load(reader);
                reader.Close();
            }
            catch(Exception e)
            {
                logger.Error("Failed to retrieve RSS feed", e);
                return false;
            }

            logger.Info("Success. RSS Feed {0} ({1} items)", RssFeed.Title.Text, RssFeed.Items.Count());

            // process
            DateTimeOffset dtLast = LastDownload;
            foreach (SyndicationItem item in RssFeed.Items)
            {
                logger.Trace("RSS Item {0}", item.Title.Text);
                logger.Trace("Upload time (UTC) {0}", item.PublishDate.UtcDateTime);

                if(item.Links.Count > 0) {
                    logger.Trace("Download URL {0}", item.Links[0].Uri.AbsoluteUri);
                    if (dtLast < item.PublishDate)
                    {
                        // do Download
                        dtLast = item.PublishDate;
                    }
                }
            }

            logger.Trace("RSS Feed Last Download time : {0}", dtLast.UtcDateTime);
            LastDownload = dtLast;

            return true;
        }

        private string BuildRssFeedUrl()
        {
            // replace space with "+" character
            return BaseUrl + Keyword.Replace(' ', '+');
        }
    }
}
