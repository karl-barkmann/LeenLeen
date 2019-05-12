//Copyright \x00a9 GalaSoft Laurent Bugnion 2009-2012

using System;

namespace Leen.Practices.ServiceLocation
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public sealed class PreferredConstructorAttribute : Attribute
    {
    }
}

