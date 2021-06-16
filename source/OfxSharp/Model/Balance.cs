using System;
using System.Globalization;
using System.Xml;

namespace OfxSharp
{
    /// <summary>LEDGERBAL, AVAILBAL</summary>
    public class Balance
    {
        public static Balance FromXmlElement( XmlNode elementOrNull )
        {
            if( elementOrNull is null )
            {
                return null;
            }

            XmlElement balanceElement = (XmlElement)elementOrNull;

            return new Balance(
                amount: elementOrNull.RequireNonemptyValue("BALAMT").RequireParseDecimal(),
                asOf  : elementOrNull.RequireNonemptyValue("DTASOF").RequireOptionalParseOfxDateTime()
            );
        }

        /*
        public Balance(XmlNode ledgerNode, XmlNode avaliableNode)
        {
            String tempLedgerBalance = ledgerNode.GetValue("//BALAMT");

            if (!string.IsNullOrEmpty(tempLedgerBalance))
            {
                this.LedgerBalance = Convert.ToDecimal(tempLedgerBalance, CultureInfo.InvariantCulture);
            }
            else
            {
                throw new OfxParseException("Ledger balance has not been set");
            }

            // ***** OFX files from my bank don't have the 'avaliableNode' node, so i manage a null situation
            if (avaliableNode == null)
            {
                this.AvaliableBalance = 0;

                // ***** this member veriable should be a nullable DateTimeOffset, declared as:
                // public DateTimeOffset? LedgerBalanceDate { get; set; }
                // and next line could be:
                // AvaliableBalanceDate = null;
                this.AvaliableBalanceDate = new DateTimeOffset();
            }
            else
            {
                String tempAvaliableBalance = avaliableNode.GetValue("//BALAMT");

                if (!string.IsNullOrEmpty(tempAvaliableBalance))
                {
                    this.AvaliableBalance = Convert.ToDecimal(tempAvaliableBalance, CultureInfo.InvariantCulture);
                }
                else
                {
                    throw new OfxParseException("Avaliable balance has not been set");
                }

                this.AvaliableBalanceDate = avaliableNode.GetValue("//DTASOF").MaybeParseOfxDateTime();
            }

            this.LedgerBalanceDate = ledgerNode.GetValue("//DTASOF").MaybeParseOfxDateTime();
        }
        */

        public Balance( Decimal amount, DateTimeOffset? asOf )
        {
            this.Amount = amount;
            this.AsOf   = asOf;
        }

        /// <BALAMT>BALAMT. Required.</summary>
        public decimal Amount { get; set; }

        /// <summary>DTASOF. Required. (UPDATE: I the OFX 1.6 spec says LEDGERBAL and AVAILBAL's DTASOF value is required, but the other &lt;BAL&gt; element does have an optional DTASOF, I'm unsure if bradesco is really returning &quot;00000000&quot; either intentionally or because they misunderstood the spec - or if the test data was masked to hide PII.</summary>
        public DateTimeOffset? AsOf { get; set; }
    }
}
