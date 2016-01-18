using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace CreateHtmlFakePages
{
    // Todo : create 8 parallel execution thread
    // todo : 1. use the main thread to check the timer.             
    //         2. to combine the data in all the threadLink files
    internal class LinkFetcher
    {
        private double _timeSpan;
        private int readListSize;
        private readonly int writeListSize = 10000;
        private string linkFile = "Links.txt";
        private int fileLinePointer = 0;
        List<string> readList = new List<string>();
        UniqueList<string> writeList = new UniqueList<string>();

        public LinkFetcher(int minutes, string startUrl)
        {
            Initialize(startUrl);

            _timeSpan = new TimeSpan(0, minutes, 0).TotalMilliseconds;

            var workerThread = new Thread(DoWork);
            workerThread.Start();

           Thread.Sleep((int)_timeSpan);
           workerThread.Abort();
           WriteListToFile();
        }

        private void DoWork()
        {
            while (true)
            {
                ConsumeReadList();
                ReFillReadList();
            }
        }

        private void ConsumeReadList()
        {
            foreach (var link in readList)
            {
                writeList.AddRange(FetchLinkList(link));
                if (writeList.Count > writeListSize)
                {
                    WriteListToFile();
                }
            }
        }

        private void ReFillReadList()
        {
            ConstructReadList();
            if (readList.Count == 0)
            {
                WriteListToFile();
                ConstructReadList();
            }
        }

        private void ConstructReadList()
        {
            readList.Clear();
            readList = File.ReadLines(linkFile).Skip(fileLinePointer).Take(readListSize).ToList();
            fileLinePointer += readList.Count;
        }

        private void WriteListToFile()
        {
            File.AppendAllLines(linkFile, writeList);
            writeList.Clear();
        }

        private void Initialize(string startUrl)
        {
            readList.Add(startUrl);
            readList.AddRange(FetchLinkList(startUrl));

            File.WriteAllLines(linkFile, readList);
            readListSize = readList.Count;
            fileLinePointer = readListSize;
            readList.RemoveAt(0);
        }

        private List<string> FetchLinkList(string url)
        {
            var httpClient = new HttpClient();
            var htmlData = httpClient.FetchHtmlData(url);
            return FilterValidLink(htmlData);
        }

        private List<string> FilterValidLink(string htmlData)
        {
            var linkFound = new List<string>();
            var pattern = @"<a\s+href\s*=\s*""(?<linkAddress>http:[^>|""]+)";
            var regex = new Regex(pattern);
            var linkAddresses = regex.Matches(htmlData);
            foreach ( Match link in linkAddresses)
            {
                linkFound.Add(link.Groups["linkAddress"].ToString());
            }
            return linkFound;
        }
    }
}