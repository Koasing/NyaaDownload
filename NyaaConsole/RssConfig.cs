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
    [JsonObject]
    class RssConfig
    {
        [JsonIgnore]
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [JsonProperty("description", Order = 1)]
        public string Description;

        [JsonProperty("base_url", Order = 2)]
        public string RssBaseUrl = "";

        [JsonProperty("keyword_prefix", Order = 3)]
        public string KeywordPrefix = "";

        [JsonProperty("folder_prefix", Order = 4)]
        public string FolderPrefix = ".";

        [JsonProperty("inactive_days", Order = 5)]
        public int InactiveDays = 28;

        [JsonProperty("feeds", Order = 99)]
        public List<RssFeed> Feeds = new List<RssFeed>();
        
        public bool Load(string Filename)
        {
            try
            {
                // load json
                using (StreamReader file = File.OpenText(Filename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.DateParseHandling = DateParseHandling.DateTimeOffset;
                    serializer.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;

                    serializer.Populate(file, this);
                }

                logger.Debug("Config load successful: {0}", Filename);
                return true;
            }
            catch(Exception e)
            {
                logger.Warn("Config load error: {0}", Filename);
                logger.Debug(e);
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
                    JsonTextWriter writer = new JsonTextWriter(file);

                    serializer.DateParseHandling = DateParseHandling.DateTimeOffset;
                    serializer.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;

                    // enable indent to generate human-readable output
                    serializer.Formatting = Formatting.Indented;
                    writer.Indentation = 2;
                    writer.IndentChar = ' ';

                    serializer.Serialize(writer, this);
                }

                logger.Debug("Config save successful: {0}", Filename);
                return true;
            }
            catch(Exception e)
            {
                logger.Warn("Config save error: {0}", Filename);
                logger.Debug(e);
                return false;
            }
        }
    }
}
