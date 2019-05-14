using System;

namespace Demo.Windows
{
    class DefaultSectionStreamRequestUriService : ISectionStreamRequstUriService
    {
        public Uri GetStreamUri(Uri streamRequestUri)
        {
            return streamRequestUri;
        }
    }
}
