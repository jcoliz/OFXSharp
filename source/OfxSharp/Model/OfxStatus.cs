using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace OfxSharp
{
    /// <summary>STATUS</summary>
    public class OfxStatus
    {
        public static OfxStatus FromXmlElement( XmlElement statusElement )
        {
            _ = statusElement.AssertIsElement("STATUS");

            return new OfxStatus(
                code    : (OfxStatusCode)statusElement.RequireNonemptyValue("//CODE").RequireParseInt32(),
                severity: statusElement.RequireNonemptyValue("//SEVERITY")
            );
        }

        public OfxStatus( OfxStatusCode code, String severity )
        {
            this.Code     = code;
            this.Severity = severity ?? throw new ArgumentNullException( nameof( severity ) );
        }

        /// <summary>STATUS/CODE</summary>
        public OfxStatusCode Code { get; }

        /// <summary>STATUS/SEVERITY</summary>
        public String Severity { get; }
    }
}
