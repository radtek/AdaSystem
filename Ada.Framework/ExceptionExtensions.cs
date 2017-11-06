﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ada.Framework
{
    public static class ExceptionExtensions
    {
        public static bool IsFatal(this Exception ex)
        {
            return ex is StackOverflowException ||
                   ex is OutOfMemoryException ||
                   ex is AccessViolationException ||
                   ex is AppDomainUnloadedException ||
                   ex is ThreadAbortException ||
                   ex is SecurityException ||
                   ex is SEHException;
        }
    }
}
