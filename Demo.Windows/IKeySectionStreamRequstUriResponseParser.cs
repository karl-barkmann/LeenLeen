﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Windows
{
    interface IKeySectionStreamRequstUriResponseParser
    {
        string GetKey(string rawHtmlContent);
    }
}
