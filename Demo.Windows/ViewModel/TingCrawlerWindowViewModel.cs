using Abot.Crawler;
using Abot.Poco;
using Leen.Practices.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Data;
using System.Windows.Input;

namespace Demo.Windows.ViewModel
{
    class TingCrawlerWindowViewModel : ViewModelBase
    {
        private ObservableCollection<Windows.OpenTradeInfo> sources = new ObservableCollection<Windows.OpenTradeInfo>();
        private ObservableCollection<BookViewModel> books = new ObservableCollection<BookViewModel>();
        private readonly object lockObject = new object();
        private readonly object booksLockObject = new object();
        readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private string nextUrl;

        public TingCrawlerWindowViewModel()
        {
            BindingOperations.EnableCollectionSynchronization(sources, lockObject);
            BindingOperations.EnableCollectionSynchronization(books, booksLockObject);
            CrawlCommand = new RelayCommand(Crawl);
            StopCrawlingCommand = new RelayCommand(StopCrawling);
        }

        private void StopCrawling()
        {
            cancellationTokenSource.Cancel();
        }

        public ObservableCollection<Windows.OpenTradeInfo> Sources
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

        public ObservableCollection<BookViewModel> Books
        {
            get { return books; }
            set
            {
                if (value != books)
                {
                    books = value;
                    RaisePropertyChanged(() => Books);
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
                    if (crawledPage.IsRoot || uri.StartsWith("http://www.tingchina.com/"))
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

                CrawlResult result = crawler.Crawl(new Uri("http://www.tingchina.com/"), cancellationTokenSource);

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
                lock (booksLockObject)
                {
                    var book = new BookViewModel(id, crawledPage.Uri);
                    Books.Add(book);
                }
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

                        lock (lockObject)
                        {
                            Sources.Add(new Windows.OpenTradeInfo(request));
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
            Console.WriteLine("Did not crawl the links on page {0} due to {1}", crawledPage.Uri.AbsoluteUri, e.DisallowedReason);
        }

        void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Console.WriteLine("Did not crawl page {0} due to {1}", pageToCrawl.Uri.AbsoluteUri, e.DisallowedReason);
        }
    }
}
