using System;
using System.Globalization;
using System.Xml;

namespace OfxSharp
{
    public class Balance
    {
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

        public decimal LedgerBalance { get; set; }

        public DateTimeOffset? LedgerBalanceDate { get; set; }

        public decimal AvaliableBalance { get; set; }

        public DateTimeOffset? AvaliableBalanceDate { get; set; }
    }
}
