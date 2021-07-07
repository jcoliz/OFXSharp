using System;
using System.Globalization;
using System.Xml;

namespace OfxSharp
{
    /// <summary>LEDGERBAL, AVAILBAL</summary>
    public class Balance
    {
        public static Balance FromXmlElementOrNull( XmlNode elementOrNull )
        {
            if( elementOrNull is null )
            {
                return null;
            }
            else
            {
                XmlElement ledgerbal = elementOrNull.AssertIsElementOneOf( "LEDGERBAL", "AVAILBAL" );

                return new Balance(
                    amount: ledgerbal.RequireSingleElementChild("BALAMT").RequireSingleTextChildNode().RequireParseDecimal(),
                    asOf  : ledgerbal.GetSingleElementChildOrNull("DTASOF").RequireSingleTextChildNode().RequireOptionalParseOfxDateTime()
                );
            }
        }

        public Balance( Decimal amount, DateTimeOffset? asOf )
        {
            this.Amount = amount;
            this.AsOf   = asOf;
        }

        /// <BALAMT>BALAMT. Required.</summary>
        public decimal Amount { get; }

        /// <summary>DTASOF. Required. (UPDATE: I the OFX 1.6 spec says LEDGERBAL and AVAILBAL's DTASOF value is required, but the other &lt;BAL&gt; element does have an optional DTASOF, I'm unsure if bradesco is really returning &quot;00000000&quot; either intentionally or because they misunderstood the spec - or if the test data was masked to hide PII.</summary>
        public DateTimeOffset? AsOf { get; }
    }
}
