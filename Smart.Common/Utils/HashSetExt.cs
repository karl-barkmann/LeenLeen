﻿using System.Collections.Generic;

namespace Leen.Common.Utils
{
    public class HashSetExt<T> : HashSet<T>
    {
        public bool this[T key]
        {
            get
            {
                return Contains(key);
            }
            set
            {
                if (value)
                {
                    Add(key);
                }
                else
                {
                    Remove(key);
                }
            }
        }
    }
}
