using Abot.Crawler;
using Abot.Poco;
using CsQuery;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Demo.Crawler
{
    class Program
    {
        static void Main(string[] args)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

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
        }

        static void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Console.WriteLine("About to crawl link {0} which was found on page {1}", pageToCrawl.Uri.AbsoluteUri, pageToCrawl.ParentUri.AbsoluteUri);
        }

        static void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
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

                        var http = WebRequest.Create(request);
                        var response = http.GetResponse();
                        var stream = response.GetResponseStream();
                        var sr = new StreamReader(stream);
                        var content = sr.ReadToEnd();
                        stream.Dispose();
                        var realUrlIndex = content.IndexOf("url[3]=");
                        var realUrlIndex1 = content.IndexOf(";", realUrlIndex);

                        var realUrl = content.Substring(realUrlIndex, realUrlIndex1 - realUrlIndex);
                        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!   " + realUrl + "   !!!!!!!!!!!!!!!!!!!!");
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

        static void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            Console.WriteLine("Did not crawl the links on page {0} due to {1}", crawledPage.Uri.AbsoluteUri, e.DisallowedReason);
        }

        static void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Console.WriteLine("Did not crawl page {0} due to {1}", pageToCrawl.Uri.AbsoluteUri, e.DisallowedReason);
        }
    }
}
