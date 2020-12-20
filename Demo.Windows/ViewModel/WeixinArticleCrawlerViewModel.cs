using Abot.Crawler;
using Abot.Poco;
using AngleSharp.Parser.Html;
using Leen.Practices.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Demo.Windows.ViewModel
{
    class WeixinArticleCrawlerViewModel : ViewModelBase
    {
        private string nextUrl;
        private readonly object lockObject = new object();
        private readonly object logslockObject = new object();
        private ObservableCollection<WeixinArticle> _articles = new ObservableCollection<WeixinArticle>();
        private ObservableCollection<string> crawlingLogs = new ObservableCollection<string>();
        readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public WeixinArticleCrawlerViewModel()
        {
            CrawlCommand = new RelayCommand(Crawl);
            StopCrawlingCommand = new RelayCommand(StopCrawling);
            BindingOperations.EnableCollectionSynchronization(_articles, lockObject);
            BindingOperations.EnableCollectionSynchronization(crawlingLogs, logslockObject);
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

        public ObservableCollection<WeixinArticle> Articles
        {
            get { return _articles; }
            set
            {
                SetProperty(ref _articles, value, () => Articles);
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

        private void Crawl()
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                CrawlConfiguration crawlConfig = new CrawlConfiguration();
                crawlConfig.MaxConcurrentThreads = 10;
                crawlConfig.MaxPagesToCrawl = 10000;
                crawlConfig.UserAgentString = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36";
                PoliteWebCrawler crawler = new PoliteWebCrawler(crawlConfig, null, null, null, null, null, null, null, null);
                crawler.PageCrawlStartingAsync += crawler_ProcessPageCrawlStarting;
                crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompleted;
                crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowed;
                crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowed;
                crawler.ShouldCrawlPage((crawledPage, crawledContext) =>
                {
                    CrawlDecision decision = new CrawlDecision();
                    var uri = crawledPage.Uri.ToString();
                    var index = crawledPage.Uri.Query.Replace("?page=", "");
                    if (crawledPage.IsRoot || (uri.StartsWith("https://wemp.app/accounts/45bd407b-dcea-4be6-8e8f-7f39c975b448?page=") && int.Parse(index) >= 9) ||
                    uri.StartsWith("https://wemp.app/posts/"))
                    {
                        decision.Allow = true;
                    }
                    else
                    {
                        decision.Allow = false;
                        decision.Reason = "Just skip pages!";
                    }
                    return decision;
                });

                CrawlResult result = crawler.Crawl(new Uri("https://wemp.app/accounts/45bd407b-dcea-4be6-8e8f-7f39c975b448?page=9"), cancellationTokenSource);

                if (result.ErrorOccurred)
                {
                    NextUrl = result.ErrorException.Message;
                    AddLog(string.Format("Crawl of {0} completed with error: {1}", result.RootUri.AbsoluteUri, result.ErrorException.Message));
                }
                else
                    AddLog(string.Format("Crawl of {0} completed without error.", result.RootUri.AbsoluteUri));
                Console.ReadLine();
            }, null);
        }

        void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
        {
            NextUrl = e.PageToCrawl.Uri.ToString();
            PageToCrawl pageToCrawl = e.PageToCrawl;
            AddLog(string.Format("About to crawl link {0} which was found on page {1}", pageToCrawl.Uri.AbsoluteUri, pageToCrawl.ParentUri.AbsoluteUri));
        }

        void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;

            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
            {
                AddLog(string.Format("Crawl of page failed {0}", crawledPage.Uri.AbsoluteUri));
                return;
            }
            else if (string.IsNullOrEmpty(crawledPage.Content.Text))
            {
                AddLog(string.Format("Page had no content {0}", crawledPage.Uri.AbsoluteUri));
                return;
            }

            AddLog(string.Format("Crawl of page succeeded {0}", crawledPage.Uri.AbsoluteUri));
            if (crawledPage.IsRoot)
                return;

            var query = crawledPage.AngleSharpHtmlDocument.QuerySelector("div.post__content");
            if (query == null)
                return;

            var title = crawledPage.AngleSharpHtmlDocument.QuerySelector("title");
            var index = title.TextContent.IndexOf("- lip师兄投资屋");
            var articleSource = "";
            var articleTitle = title.TextContent.Substring(0, index + 1);
            var article = new WeixinArticle(articleTitle, articleSource, query.InnerHtml);
            lock (lockObject)
            {
                Articles.Add(article);
            }
            FileStream fileStream = null;
            try
            {
                articleTitle.Replace("|", "");
                articleTitle.Replace("<", "");
                articleTitle.Replace(">", "");
                articleTitle.Replace(":", "");
                articleTitle.Replace("?", "");
                articleTitle.Replace("*", "");
                articleTitle.Replace("\"", "");
                articleTitle.Replace("\\", "");
                articleTitle.Replace("/", "");
                fileStream = new FileStream(articleTitle + ".html", FileMode.Create);
                var writer = new StreamWriter(fileStream);
                writer.Write(query.InnerHtml);
            }
            catch (Exception error)
            {

            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }

            //UIService.BeginInvokeIfNeeded(() =>
            //{
            //    WebBrowser browser = new WebBrowser();
            //    browser.Source = crawledPage.Uri;
            //    browser.LoadCompleted += Browser_LoadCompleted;
            //});
        }

        private async void Browser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            WebBrowser browser = sender as WebBrowser;
            browser.LoadCompleted -= Browser_LoadCompleted;
            var parser = new HtmlParser();
            //var document = await parser.ParseAsync(htmlSourceCode);
        }

        void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            AddLog(string.Format("Did not crawl the links on page {0} due to {1}", crawledPage.Uri.AbsoluteUri, e.DisallowedReason));
        }

        void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            AddLog(string.Format("Did not crawl page {0} due to {1}", pageToCrawl.Uri.AbsoluteUri, e.DisallowedReason));
        }

        private void AddLog(string log)
        {
            lock (logslockObject)
            {
                CrawlingLogs.Add(log);
            }
        }
    }

    class WeixinArticle
    {
        public WeixinArticle(string title, string url, string content)
        {
            Title = title;
            OriginUrl = url;
            Content = content;
        }

        public string Title { get; }

        public string OriginUrl { get; }

        public string Content { get; }
    }
}
