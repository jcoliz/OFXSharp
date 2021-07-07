using System;
using System.Collections.Generic;
using System.Text;

namespace OfxSharp
{
    /// <summary>11.4.1.3 Status Codes</summary>
    public enum OfxStatusCode
    {
        Success              = 0,

        GeneralError         = 2000,
        GeneralAccountError  = 2002,
        AccountNotFound      = 2003,
        AccountClosed        = 2004,
        AccountNotAuthorized = 2005,
        DuplicateRequest     = 2019,
        InvalidDate          = 2020,
        InvalidDateRange     = 2027
    }
}
