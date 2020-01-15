using Abot.Crawler;
using Abot.Poco;
using Leen.Practices.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Data;
using System.Windows.Input;

namespace Demo.Windows.ViewModel
{
    class SimpleTraderCrawlerWindowViewModel : ViewModelBase
    {
        private ObservableCollection<OpenTradeInfo> sources = new ObservableCollection<OpenTradeInfo>();
        private ObservableCollection<string> crawlingLogs = new ObservableCollection<string>();
        private readonly object lockObject = new object();
        private readonly object logslockObject = new object();
        readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private string nextUrl;

        public SimpleTraderCrawlerWindowViewModel()
        {
            BindingOperations.EnableCollectionSynchronization(sources, lockObject);
            BindingOperations.EnableCollectionSynchronization(crawlingLogs, logslockObject);
            CrawlCommand = new RelayCommand(Crawl);
            StopCrawlingCommand = new RelayCommand(StopCrawling);
        }

        private void StopCrawling()
        {
            cancellationTokenSource.Cancel();
        }

        public ObservableCollection<OpenTradeInfo> Sources
        {
            get { return sources; }
            set
            {
                if (value != sources)
                {
                    sources = value;
                    RaisePropertyChanged(() => Sources);
                }
            }
        }

        public ObservableCollection<string> CrawlingLogs
        {
            get { return crawlingLogs; }
            set
            {
                if (value != crawlingLogs)
                {
                    crawlingLogs = value;
                    RaisePropertyChanged(() => CrawlingLogs);
                }
            }
        }

        public string NextUrl
        {
            get { return nextUrl; }
            set
            {
                if (nextUrl != value)
                {
                    nextUrl = value;
                    RaisePropertyChanged(() => NextUrl);
                }
            }
        }

        public ICommand CrawlCommand { get; set; }

        public ICommand StopCrawlingCommand { get; set; }

        private void Crawl()
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                PoliteWebCrawler crawler = new PoliteWebCrawler();
                crawler.PageCrawlStartingAsync += crawler_ProcessPageCrawlStarting;
                crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompleted;
                crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowed;
                crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowed;

                crawler.ShouldCrawlPage((crawledPage, crawledContext) =>
                {
                    CrawlDecision decision = new CrawlDecision();
                    var uri = crawledPage.Uri.ToString();
                    if (crawledPage.IsRoot || 
                    uri.StartsWith("https://www.simpletrader.net/signal/") ||
                    uri.StartsWith("https://www.simpletrader.net/forex-signals.html"))
                    {
                        decision.Allow = true;
                    }
                    else
                    {
                        decision.Allow = false;
                        decision.Reason = "Just noise pages!";
                    }
                    return decision;
                });

                CrawlResult result = crawler.Crawl(new Uri("https://www.simpletrader.net/forex-signals.html"), cancellationTokenSource);

                if (result.ErrorOccurred)
                {
                    NextUrl = result.ErrorException.Message;
                    CrawlingLogs.Add(string.Format("Crawl of {0} completed with error: {1}", result.RootUri.AbsoluteUri, result.ErrorException.Message));
                }
                else
                    CrawlingLogs.Add(string.Format("Crawl of {0} completed without error.", result.RootUri.AbsoluteUri));
                CrawlingLogs.Add("Crawling complete!");
            }, null);
        }
        void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
        {
            NextUrl = e.PageToCrawl.Uri.ToString();
            PageToCrawl pageToCrawl = e.PageToCrawl;
            CrawlingLogs.Add(string.Format("About to crawl link {0} which was found on page {1}", pageToCrawl.Uri.AbsoluteUri, pageToCrawl.ParentUri.AbsoluteUri));
        }

        void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;

            var crawledPageUri = crawledPage.Uri.ToString();
            //Crawling signal
            if (Regex.IsMatch(crawledPageUri, @"https://www.simpletrader.net/signal/(\d+)/*.*.html") ||
                Regex.IsMatch(crawledPageUri, @"https://www.simpletrader.net/signal/(\d+)//signal/(\d+)/*.*.html.html"))
            {
                var openTradeDom = crawledPage.CsQueryDocument.Document.GetElementById("openTrades");
                if (openTradeDom != null)
                {
                    var signalId = int.Parse(Regex.Match(crawledPageUri, @"(\d+)").Captures[0].Value);
                    var signalName = crawledPage.Uri.Segments[crawledPage.Uri.Segments.Length - 1].Replace(".html", "");
                    var openTradeInfoHeaders = new List<OpenTradeInfoKeys>();

                    foreach (var childElement in openTradeDom.ChildElements)
                    {
                        if (childElement.NodeName.ToLower() == "thead")
                        {
                            foreach (var tr in childElement.ChildElements)
                            {
                                foreach (var th in tr.ChildElements)
                                {
                                    var header = th.InnerText.Replace(" ", "");
                                    openTradeInfoHeaders.Add((OpenTradeInfoKeys)Enum.Parse(typeof(OpenTradeInfoKeys), header));
                                }
                            }
                        }

                        if (childElement.NodeName.ToLower() == "tbody")
                        {
                            foreach (var tr in childElement.ChildElements)
                            {
                                int index = 0;
                                var openTradeInfos = new Dictionary<OpenTradeInfoKeys, string>();
                                foreach (var td in tr.ChildElements)
                                {
                                    var header = openTradeInfoHeaders[index++];
                                    openTradeInfos.Add(header, td.InnerText);
                                }

                                var opentradeInfo = new OpenTradeInfo(signalId, signalName, openTradeInfos);
                                Sources.Add(opentradeInfo);
                            }
                        }
                    }
                }
            }

            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
                Console.WriteLine("Crawl of page failed {0}", crawledPage.Uri.AbsoluteUri);
            else
                Console.WriteLine("Crawl of page succeeded {0}", crawledPage.Uri.AbsoluteUri);

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
                Console.WriteLine("Page had no content {0}", crawledPage.Uri.AbsoluteUri);
        }

        void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            CrawlingLogs.Add(string.Format("Did not crawl the links on page {0} due to {1}", crawledPage.Uri.AbsoluteUri, e.DisallowedReason));
        }

        void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            CrawlingLogs.Add(string.Format("Did not crawl page {0} due to {1}", pageToCrawl.Uri.AbsoluteUri, e.DisallowedReason));
        }
    }

    enum OpenTradeInfoKeys
    {
        OpenTime,
        OpenPrice,
        Lots,
        Type,
        Pair,
        Profit,
        Pips,
        Comment,
    }

    class OpenTradeInfo : BindableBase
    {
        private readonly Dictionary<OpenTradeInfoKeys, string> _infos;

        public OpenTradeInfo(int id, string name, Dictionary<OpenTradeInfoKeys, string> infos)
        {
            SignalId = id;
            SignalName = name;
            _infos = infos;
        }

        public int SignalId { get; set; }

        public string SignalName { get; set; }

        public DateTime OpenTime
        {
            get { return DateTime.Parse(_infos[OpenTradeInfoKeys.OpenTime]); }
        }

        public double OpenPrice
        {
            get { return double.Parse(_infos[OpenTradeInfoKeys.OpenPrice]); }
        }

        public string Lots
        {
            get { return _infos[OpenTradeInfoKeys.Lots]; }
        }

        public string Type
        {
            get { return _infos[OpenTradeInfoKeys.Type]; }
        }

        public string Pair
        {
            get { return _infos[OpenTradeInfoKeys.Pair]; }
        }

        public double Profit
        {
            get { return double.Parse(_infos[OpenTradeInfoKeys.Profit]); }
        }

        public string Pips
        {
            get { return _infos[OpenTradeInfoKeys.Pips]; }
        }

        public string Comment
        {
            get { return _infos[OpenTradeInfoKeys.Comment]; }
        }
    }
}
