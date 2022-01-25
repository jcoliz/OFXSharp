using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace OfxSharp
{
    public class OfxInvestmentStatementResponse
    {
        internal static OfxStatementResponse FromINVSTMTRS( XmlElement invstmtrs )
        {
            throw new NotImplementedException();
#if false
            _ = invstmtrs.AssertIsElement( "INVSTMTTRNRS", parentElementName: "INVSTMTMSGSRSV1" );

            XmlElement stmtrs    = invstmtrs.RequireSingleElementChild("STMTRS");
            XmlElement transList = stmtrs  .RequireSingleElementChild("BANKTRANLIST");
#endif
        }
    }
}
