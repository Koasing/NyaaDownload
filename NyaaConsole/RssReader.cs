using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using System.ServiceModel.Syndication;

using NLog;
using System.Data.SQLite;

namespace NyaaDownloader
{
    class RssReader
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// SQLite RowID (Internal Field)
        /// </summary>
        private long RowId;
        
        public string BaseUrl;
        public string Keyword;
        public string DownloadFolder;
        public DateTimeOffset LastDownload;
        public string Description;

        public RssReader()
        {
            RowId = -1;
            
        }

        public bool Load(int RowId, SQLiteConnection SqlConnection)
        {
            SQLiteCommand command = SqlConnection.CreateCommand();

            command.CommandText = "SELECT * FROM RssReaders WHERE ROWID=@rowid";
            command.Parameters.AddWithValue("@rowid", RowId);

            SQLiteDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                // Parse result
                BaseUrl = (string)reader["baseurl"];
                Keyword = (string)reader["keyword"];
                DownloadFolder = (string)reader["folder"];
                Description = (string)reader["description"];

                // LastDownload time is UTC based (offset=0hr)
                LastDownload = new DateTimeOffset((DateTime)reader["lastdownload"], new TimeSpan(0, 0, 0));

                return true;
            }
            else
            {
                // there is no Row with given RowId.
                RowId = -1;
                return false;
            }
        }

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
