using System;
using System.IO;
using System.Net;
using System.Text;

namespace Demo.Windows
{
    class TingChinaKeySectionStreamRequestUriService : ISectionStreamRequstUriService
    {
        private readonly IKeySectionStreamRequstUriResponseParser parser;

        public TingChinaKeySectionStreamRequestUriService(IKeySectionStreamRequstUriResponseParser parser)
        {
            this.parser = parser;
        }

        public Uri GetStreamUri(Uri streamRequestUri)
        {
            var http = WebRequest.Create(streamRequestUri);
            var response = http.GetResponse();
            var stream = response.GetResponseStream();
            var sr = new StreamReader(stream, Encoding.UTF8);
            var content = sr.ReadToEnd();
            stream.Dispose();
            var key = parser.GetKey(content);
            var queries = streamRequestUri.Query.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);

            throw new NotImplementedException();
        }
    }
}
