using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
