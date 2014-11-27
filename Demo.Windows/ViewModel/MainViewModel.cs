using Abot.Crawler;
using Abot.Poco;
using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Data;
using System.Windows.Input;

namespace Demo.Windows.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<SoundPlayerSourceViewModel> sources = new ObservableCollection<SoundPlayerSourceViewModel>();
        private readonly object lockObject = new object();
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private string nextUrl;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}

            BindingOperations.EnableCollectionSynchronization(sources, lockObject);
            CrawlCommand = new GalaSoft.MvvmLight.Command.RelayCommand(Crawl);
            StopCrawlingCommand = new GalaSoft.MvvmLight.Command.RelayCommand(StopCrawling);
        }

        private void StopCrawling()
        {
            cancellationTokenSource.Cancel();
        }

        public ObservableCollection<SoundPlayerSourceViewModel> Sources
        {
            get { return sources; }
            set
            {
                if (value != sources)
                {
                    sources = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string NextUrl
        {
            get { return nextUrl; }
            set
            {
                if(nextUrl!=value)
                {
                    nextUrl = value;
                    RaisePropertyChanged();
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
                    if (crawledPage.IsRoot || uri.StartsWith("http://www.tingchina.com/erge/"))
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
                    Console.WriteLine("Crawl of {0} completed with error: {1}", result.RootUri.AbsoluteUri, result.ErrorException.Message);
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

            if (Regex.IsMatch(crawledPage.Uri.ToString(), @"http://www.tingchina.com/erge/(\d+)/play_(\d+)_(\d+)\.htm"))
            {

                var csQuery = crawledPage.CsQueryDocument.Find("iframe");
                foreach (var query in csQuery)
                {
                    if(query.Name=="playmedia")
                    {
                        var playSrc = query.GetAttribute("src");
                        var request = String.Format("http://{0}{1}", crawledPage.Uri.Host, playSrc);

                        lock (lockObject)
                        {
                            Sources.Add(new SoundPlayerSourceViewModel(request));
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