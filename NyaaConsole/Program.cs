using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace NyaaDownloader
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static void parseParameter(string arg)
        {
            switch (arg[1])
            {
                case 'v': case 'V':
                    // reconfigure logger to log verbose log
                    foreach (var rule in LogManager.Configuration.LoggingRules)
                    {
                        if (rule.IsLoggingEnabledForLevel(LogLevel.Info))
                            rule.EnableLoggingForLevel(LogLevel.Trace);
                    }
                    LogManager.ReconfigExistingLoggers();
                    logger.Warn("Verbose Mode");
                    break;
                default:
                    break;
            }
            return;
        }

        static void Main(string[] args)
        {
            List<string> CfgList = new List<string>();

            foreach (string arg in args)
            {
                if (arg[0] == '-')
                    parseParameter(arg);
                else
                    CfgList.Add(arg);
            }

            // add default config
            if (CfgList.Count == 0)
                CfgList.Add("config.json");

            foreach (string cfgfile in CfgList)
            {
                if (!File.Exists(cfgfile))
                {
                    logger.Warn("Config file \"{0}\" is not found... continue.", cfgfile);
                    continue;
                }

                // make backup file
                try
                {
                    File.Copy(cfgfile, cfgfile + ".bak", true);
                }
                catch (Exception e)
                {
                    logger.Warn("Cannot make backup of {0}...", cfgfile);
                    logger.Warn("The file is not modified nor processed.");
                    logger.Debug(e);
                    continue;
                }

                // load RSS config
                RssConfig cfg = new RssConfig();
                if(!cfg.Load(cfgfile))
                {
                    logger.Warn("Config file \"{0}\" is not a valid config file... ignore it.");
                    continue;
                }

                foreach(RssFeed feed in cfg.Feeds)
                    feed.Read(cfg);

                if (!cfg.Save(cfgfile))
                {
                    logger.Warn("Cannot update config file \"{0}\".", cfgfile);
                }
            }
        }
    }
}
