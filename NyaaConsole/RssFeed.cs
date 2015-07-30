using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        [JsonProperty("description", Order = 1)]
        public string Description;

        [JsonProperty("keyword", Order = 2)]
        public string Keyword = "";

        [JsonProperty("folder", Order = 3)]
        public string DownloadFolder = ".";

        [JsonProperty("last_download", Order = 4)]
        public DateTimeOffset LastDownload = new DateTimeOffset();

        public bool Read(RssConfig Config)
        {
            // download RSS
            SyndicationFeed RssFeed = null;
            try
            {
                string FeedUrl = String.Format("{0}+{1}+{2}", Config.RssBaseUrl, Config.KeywordPrefix.Replace(' ', '+'), Keyword.Replace(' ', '+'));
                XmlReader reader = XmlReader.Create(FeedUrl);
                RssFeed = SyndicationFeed.Load(reader);
                reader.Close();
            }
            catch(Exception e)
            {
                logger.Warn("Retrive RSS failed: {0}+{1}+{2}", Config.RssBaseUrl, Config.KeywordPrefix, Keyword);
                logger.Debug(e);
                return false;
            }
            logger.Info("Retrieve RSS successful: {0}+{1}", Config.KeywordPrefix, Keyword);

            // create download folder
            string dlFolder = String.Format(@".\{0}\{1}", Config.FolderPrefix, DownloadFolder);
            try
            {
                // No need to check path existence because CreateDirectory does it.
                System.IO.Directory.CreateDirectory(dlFolder);
            }
            catch (Exception e)
            {
                logger.Warn("Cannot creat download folder: {0}", dlFolder);
                logger.Debug(e);
                return false;
            }
            logger.Info("Download folder: {0}", dlFolder);
            
            // process RSS
            DateTimeOffset dtLast = LastDownload;

            foreach (SyndicationItem item in RssFeed.Items)
            {
                if(item.Links.Count > 0)
                {
                    if (LastDownload < item.PublishDate)
                    {
                        logger.Info("Download {0}", item.Title.Text);

                        if (DownloadFile(item.Links[0].Uri, String.Format(@"{0}\{1}.torrent", dlFolder, item.Title.Text)) && dtLast < item.PublishDate)
                            dtLast = item.PublishDate;
                    }
                    else
                        logger.Trace("Already downloaded; skip {0}...", item.Title.Text);
                }
            }

            LastDownload = dtLast;

            return true;
        }

        private bool DownloadFile(Uri Remote, string Local)
        {
            try
            {
                // do Download
                WebClient __dl = new WebClient();
                __dl.DownloadFile(Remote, Local);

                logger.Trace("File download successful: {0} -> {1}", Remote.ToString(), Local);
                return true;
            }
            catch (Exception e)
            {
                logger.Warn("File download error: {0} -> {1}", Remote.ToString(), Local);
                logger.Debug(e);
                return false;
            }
        }
    }
}
