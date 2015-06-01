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

        [JsonProperty("description", Order=1)]
        public string Description;

        [JsonProperty("folder_prefix", Order=2)]
        public string FolderPrefix;

        [JsonProperty("inactive_days", Order=3)]
        public int InactiveDays = 14;

        [JsonProperty("feeds", Order=99)]
        public List<RssFeed> Feeds;
        
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
                    JsonTextWriter writer = new JsonTextWriter(file);

                    serializer.DateParseHandling = DateParseHandling.DateTimeOffset;
                    serializer.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;

                    // enable indent to generate human-readable output
                    serializer.Formatting = Formatting.Indented;
                    writer.Indentation = 2;
                    writer.IndentChar = ' ';

                    serializer.Serialize(writer, this);
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
