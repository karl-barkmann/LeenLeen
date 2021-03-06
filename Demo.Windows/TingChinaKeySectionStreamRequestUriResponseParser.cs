using Demo.Windows;
using System;
using System.Net;

public class TingChinaKeySectionStreamRequestUriResponseParser : IKeySectionStreamRequstUriResponseParser
{
    public string GetKey(string rawHtmlContent)
    {
        var keyIndex = "url[3]=";
        var realUrlIndex = rawHtmlContent.IndexOf(keyIndex);
        var realUrlIndex1 = rawHtmlContent.IndexOf(";", realUrlIndex);

        var realUrl = rawHtmlContent.Substring(realUrlIndex + keyIndex.Length + 2, realUrlIndex1 - realUrlIndex - keyIndex.Length - 3);
        string decodeUrl = System.Web.HttpUtility.UrlDecode(realUrl);
        var uri = new Uri(decodeUrl);
        return uri.Query.Substring(3, uri.Query.Length);
    }
}
