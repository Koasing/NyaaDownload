using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using System.ServiceModel.Syndication;

using Newtonsoft.Json;
using NLog;

namespace NyaaDownloader
{
    [JsonObject]
    class RssFeed
    {
        [JsonIgnore]
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [JsonProperty("baseurl", Order=1)]
        public string BaseUrl;

        [JsonProperty("keyword", Order=2)]
        public string Keyword;

        [JsonProperty("folder", Order=3)]
        public string DownloadFolder;

        [JsonProperty("last_download", Order=4)]
        public DateTimeOffset LastDownload;

        [JsonProperty("description", Order=99)]
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
