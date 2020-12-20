using Abot.Crawler;
using Abot.Poco;
using AngleSharp.Parser.Html;
using Leen.Practices.Mvvm;
using System;
using System.Collections.Generic;
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
            CrawlCommand = new RelayCommand(Crawl1);
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

        const string directory = "Articles";
        const string crawlTarget = "https://wemp.app/accounts/45bd407b-dcea-4be6-8e8f-7f39c975b448";

        private void Crawl()
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
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
                    var parentUri = crawledPage.ParentUri.ToString();
                    if (crawledPage.IsRoot || uri.StartsWith(crawlTarget + "?page=") || (uri.StartsWith("https://wemp.app/posts/") && parentUri.StartsWith(crawlTarget)))
                    {
                        decision.Allow = true;
                    }
                    else
                    {
                        decision.Allow = false;
                        decision.Reason = "skip pages";
                    }
                    return decision;
                });

                CrawlResult result = crawler.Crawl(new Uri(crawlTarget), cancellationTokenSource);

                if (result.ErrorOccurred)
                {
                    NextUrl = result.ErrorException.Message;
                    AddLog(string.Format("Crawl of {0} completed with error: {1}", result.RootUri.AbsoluteUri, result.ErrorException.Message));
                }
                else
                {
                    AddLog(string.Format("Crawl of {0} completed without error.", result.RootUri.AbsoluteUri));
                }
            }, null);
        }

        private void Crawl1()
        {
            var candidateIndexes = new List<string> { "2", "3", "7", "9", "11", "12", "13", "16", "17", "23", "24", "25", "27", "29", "30", "31", "32", "33", "34", "35", "38", "40" };
            ThreadPool.QueueUserWorkItem((state) =>
            {
                CrawTarget(crawlTarget);
                foreach (var index in candidateIndexes)
                {
                    var crawlingPage = crawlTarget + "?page=" + index;
                    CrawTarget(crawlingPage);
                }
            });
        }

        private void CrawTarget(string crawlingPage)
        {
            var candidateIndexes = new List<string> { "2", "3", "7", "9", "11", "12", "13", "16", "17", "23", "24", "25", "27", "29", "30", "31", "32", "33", "34", "35", "38", "40" };
            CrawlConfiguration crawlConfig = new CrawlConfiguration();
            crawlConfig.MaxConcurrentThreads = 10;
            crawlConfig.MaxPagesToCrawl = 10000;
            crawlConfig.CrawlTimeoutSeconds = 500;
            crawlConfig.HttpRequestTimeoutInSeconds = 500;
            crawlConfig.UserAgentString = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36";
            PoliteWebCrawler crawler = new PoliteWebCrawler(crawlConfig, null, null, null, null, null, null, null, null);
            crawler.PageCrawlStartingAsync += crawler_ProcessPageCrawlStarting;
            crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowed;
            crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompleted;
            crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowed; crawler.ShouldCrawlPage((crawledPage, crawledContext) =>
            {
                CrawlDecision decision = new CrawlDecision();
                var uri = crawledPage.Uri.ToString();
                var parentUri = crawledPage.ParentUri.ToString();
                var index = crawledPage.Uri.Query.Replace("?page=", "").Trim();

                if (crawledPage.IsRoot || (uri.StartsWith("https://wemp.app/posts/") && parentUri == crawlingPage))
                {
                    decision.Allow = true;
                }
                else
                {
                    decision.Allow = false;
                    decision.Reason = "skip pages";
                }
                return decision;
            });
            var result = crawler.Crawl(new Uri(crawlingPage), cancellationTokenSource);
            if (result.ErrorOccurred)
            {
                AddLog(string.Format("Page {0} crawling completed with error: {1}", result.RootUri.AbsoluteUri, result.ErrorException.Message));
            }
            else
            {
                AddLog(string.Format("Page {0} crawling completed without error.", result.RootUri.AbsoluteUri));
            }
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
            if (crawledPage.WebException != null)
            {
                AddLog(string.Format("Page {0} crawling failed {1}",
                    crawledPage.Uri.AbsoluteUri,
                    crawledPage.WebException.InnerException != null ? crawledPage.WebException.InnerException.Message : crawledPage.WebException.Message));
                return;
            }
            else if (crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
            {
                AddLog(string.Format("Page {0} crawling failed {1}", crawledPage.Uri.AbsoluteUri, crawledPage.HttpWebResponse.StatusCode));
                return;
            }
            else if (string.IsNullOrEmpty(crawledPage.Content.Text))
            {
                AddLog(string.Format("Page {0} has no content", crawledPage.Uri.AbsoluteUri));
                return;
            }

            AddLog(string.Format("Page {0} crawling succeeded", crawledPage.Uri.AbsoluteUri));

            if (crawledPage.IsRoot)
                return;

            var contentElement = crawledPage.AngleSharpHtmlDocument.QuerySelector("div.post__content");
            if (contentElement == null)
                return;

            var articleDate = crawledPage.AngleSharpHtmlDocument.QuerySelector("div.post__date").TextContent.Trim();
            var articleTitle = crawledPage.AngleSharpHtmlDocument.QuerySelector("h1.post__title").TextContent.Trim();
            var articleSource = "";
            var index = crawledPage.ParentUri.Query.Replace("?page=", "");
            var page = string.IsNullOrEmpty(index) ? 1 : int.Parse(index);
            var article = new WeixinArticle(page, articleTitle, articleSource, articleDate, contentElement.InnerHtml);
            lock (lockObject)
            {
                Articles.Add(article);
            }
            FileStream fileStream = null;
            try
            {
                var fileName = articleDate + " - " + articleTitle + ".html";
                fileName = fileName.Replace("|", "");
                fileName = fileName.Replace("<", "");
                fileName = fileName.Replace(">", "");
                fileName = fileName.Replace(":", "-");
                fileName = fileName.Replace("?", "");
                fileName = fileName.Replace("*", "");
                fileName = fileName.Replace("\"", "");
                fileName = fileName.Replace("\\", "");
                fileName = fileName.Replace("/", "");
                fileStream = new FileStream(Path.Combine(directory, fileName), FileMode.Create);
                var writer = new StreamWriter(fileStream);
                writer.Write(contentElement.InnerHtml);
            }
            catch (Exception error)
            {
                AddLog("Article file saving error：" + error.Message);
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }
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
        public WeixinArticle(int page, string title, string url, string date, string content)
        {
            Page = page;
            Title = title;
            OriginUrl = url;
            Date = date;
            Content = content;
        }

        public int Page { get; }

        public string Title { get; }

        public string OriginUrl { get; }

        public string Date { get; }

        public string Content { get; }
    }
}
