using Abot.Crawler;
using Abot.Poco;
using Leen.Practices.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Input;

namespace Demo.Windows.ViewModel
{
    class NgaCrawlerWindowViewModel : ViewModelBase
    {
        readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private string nextUrl;
        private ObservableCollection<string> _titles;

        public NgaCrawlerWindowViewModel()
        {
            CrawlCommand = new RelayCommand(Crawl);
            StopCrawlingCommand = new RelayCommand(StopCrawling);
        }

        private void StopCrawling()
        {
            cancellationTokenSource.Cancel();
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

        public ObservableCollection<string> Titles
        {
            get { return _titles; }
            set
            {
                SetProperty(ref _titles, value, () => Titles);
            }
        }

        private void Crawl()
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                CrawlConfiguration crawlConfig = new CrawlConfiguration();
                crawlConfig.CrawlTimeoutSeconds = 100;
                crawlConfig.MaxConcurrentThreads = 10;
                crawlConfig.MaxPagesToCrawl = 1000;
                crawlConfig.UserAgentString = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36";
                PoliteWebCrawler crawler = new PoliteWebCrawler(crawlConfig, null, null, null, null, null, null, null, null);
                crawler.PageCrawlStartingAsync += crawler_ProcessPageCrawlStarting;
                crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompleted;
                crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowed;
                crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowed;
                crawler.ShouldCrawlPage((crawledPage, crawledContext) =>
                {
                    CrawlDecision decision = new CrawlDecision();
                    var uri = crawledPage.Uri.ToString();
                    if (uri.StartsWith("http://nga.178.com/read.php?tid=17681878&_ff=-7"))
                    {
                        decision.Allow = true;
                    }
                    else
                    {
                        decision.Allow = false;
                        decision.Reason = "Just erge pages!";
                    }
                    return decision;
                });

                CrawlResult result = crawler.Crawl(new Uri("http://nga.178.com/read.php?tid=17681878&_ff=-7"), cancellationTokenSource);

                if (result.ErrorOccurred)
                {
                    NextUrl = result.ErrorException.Message;
                    Console.WriteLine("Crawl of {0} completed with error: {1}", result.RootUri.AbsoluteUri, result.ErrorException.Message);
                }
                else
                    Console.WriteLine("Crawl of {0} completed without error.", result.RootUri.AbsoluteUri);
                Console.ReadLine();
            }, null);
        }

        void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
        {
            NextUrl = e.PageToCrawl.Uri.ToString();
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Console.WriteLine("About to crawl link {0} which was found on page {1}", pageToCrawl.Uri.AbsoluteUri, pageToCrawl.ParentUri.AbsoluteUri);
        }

        void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;

            var crawledPageUri = crawledPage.Uri.ToString();
            //Crawling books
            if (Regex.IsMatch(crawledPageUri, @"http://www.tingchina.com/*.*/disp_(\d+).htm"))
            {
                int id = Int32.Parse(Regex.Match(crawledPageUri, @"(\d+)").Captures[0].Value);
            }

            //Crawling book sections
            if (Regex.IsMatch(crawledPageUri, @"http://www.tingchina.com/*.*/(\d+)/play_(\d+)_(\d+)\.htm"))
            {
                var csQuery = crawledPage.CsQueryDocument.Find("iframe");
                foreach (var query in csQuery)
                {
                    if (query.Name == "playmedia")
                    {
                        var playSrc = query.GetAttribute("src");
                        var request = String.Format("http://{0}{1}", crawledPage.Uri.Host, playSrc);
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
            Console.WriteLine("Did not crawl the links on page {0} due to {1}", crawledPage.Uri.AbsoluteUri, e.DisallowedReason);
        }

        void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Console.WriteLine("Did not crawl page {0} due to {1}", pageToCrawl.Uri.AbsoluteUri, e.DisallowedReason);
        }
    }
}
