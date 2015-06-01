using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using NLog;

namespace NyaaDownloader
{
    [JsonObject(MemberSerialization.OptIn)]
    class Config
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [JsonProperty(PropertyName = "db_file", Order = 1)]
        public string DBFile = "database.sqlite3";

        [JsonProperty(PropertyName="config_table")]
        public string ConfigTable = "config";

        [JsonProperty(PropertyName = "rss_feed_table")]
        public string RssFeedTable = "rssfeed";

        [JsonProperty(PropertyName = "inactive_days")]
        public int InactiveDays = 14;
        
        public bool Load(string Filename)
        {
            try
            {
                // load json
                using (StreamReader file = File.OpenText(Filename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Populate(file, this);
                }
                return true;
            }
            catch(Exception e)
            {
                logger.Error("Config load error: {0}", e);
                return false;
            }
        }

        public bool Save(string Filename)
        {
            try
            {
                // save json
                using (StreamWriter file = File.CreateText(Filename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, this);
                }
                return true;
            }
            catch(Exception e)
            {
                logger.Error("Config save error: {0}", e);
                return false;
            }
        }
    }
}
