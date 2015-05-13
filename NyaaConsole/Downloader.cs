using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace NyaaDownloader
{
    public class DownloadItem
    {
        public string Url;
        public string LocalPath;
    }

    public class Downloader
    {
        private ConcurrentQueue<DownloadItem> DownloadQueue;
        private bool StopSignal;

        // Constructor
        public Downloader()
        {
            DownloadQueue = new ConcurrentQueue<DownloadItem>();
            StopSignal = false;
        }

        // add queue
        public bool QueueItem(DownloadItem item)
        {
            if (item != null && item.Url != null && item.LocalPath != null)
            {
                DownloadQueue.Enqueue(item);
                return true;
            }
            else
            {
                return false;
            }
        }

        // send stop signal
        public void Interrupt()
        {
            StopSignal = true;
        }

        // main loop
        public void Loop()
        {
            StopSignal = false;

            // do loop while
            while (!StopSignal)
            {
                DownloadItem item;
                if (DownloadQueue.TryDequeue(out item))
                {
                    // successfully dequeued; try to download
                    try
                    {
                        WebClient __dl = new WebClient();
                        __dl.DownloadFile(item.Url, item.LocalPath);
                    }
                    catch(ArgumentNullException e)
                    {
                        Console.WriteLine("OOPS: Download URI shoudld not be null.");
                    }
                    catch(WebException e)
                    {
                        Console.WriteLine("WARN: Failed to download \"{0}\" to \"{1}\".", item.Url, item.LocalPath);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("OOPS: Something wrong. {0}", e.ToString());
                    }
                }
                else
                {
                    // queue is empty... sleep a while
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
