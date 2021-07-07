using System;
using System.Globalization;
using System.Xml;

namespace OfxSharp
{
    public class Transaction
    {
        public Transaction( XmlElement stmtTrn, string defaultCurrency )
        {
            this.TransType                     = stmtTrn.GetValue(".//TRNTYPE").ParseEnum<OfxTransactionType>();
            this.Date                          = stmtTrn.GetValue(".//DTPOSTED").MaybeParseOfxDateTime();
            this.TransactionInitializationDate = stmtTrn.GetValue(".//DTUSER").MaybeParseOfxDateTime();
            this.FundAvaliabilityDate          = stmtTrn.GetValue(".//DTAVAIL").MaybeParseOfxDateTime();

            try
            {
                this.Amount = Convert.ToDecimal(stmtTrn.GetValue(".//TRNAMT"), CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new OfxParseException("Transaction Amount unknown", ex);
            }

            try
            {
                this.TransactionId = stmtTrn.GetValue(".//FITID");
            }
            catch (Exception ex)
            {
                throw new OfxParseException("Transaction ID unknown", ex);
            }

            this.IncorrectTransactionId = stmtTrn.GetValue(".//CORRECTFITID");

            this.TransactionCorrectionAction = stmtTrn.GetValue(".//CORRECTACTION").TryParseEnum<TransactionCorrectionType>() ?? TransactionCorrectionType.NA;

            this.ServerTransactionId = stmtTrn.GetValue(".//SRVRTID");
            this.CheckNum            = stmtTrn.GetValue(".//CHECKNUM");
            this.ReferenceNumber     = stmtTrn.GetValue(".//REFNUM");
            this.Sic                 = stmtTrn.GetValue(".//SIC");
            this.PayeeId             = stmtTrn.GetValue(".//PAYEEID");
            this.Name                = stmtTrn.GetValue(".//NAME");
            this.Memo                = stmtTrn.GetValue(".//MEMO");

            //If differenct currency to CURDEF, populate currency
            if( stmtTrn.TryGetDescendant( ".//CURRENCY", out XmlElement currencyEl ))
            {
                this.Currency = currencyEl.Value;
            }
            else if( stmtTrn.TryGetDescendant( ".//ORIGCURRENCY", out XmlElement origCurrencyEl ) )
            {
                this.Currency = origCurrencyEl.Value;
            }
            else //If currency not different, set to CURDEF
            {
                this.Currency = defaultCurrency;
            }

            //If senders bank/credit card details avaliable, add
            if( stmtTrn.TryGetDescendant( ".//BANKACCTTO", out XmlElement bankAcct ) )
            {
                this.TransactionSenderAccount = Account.FromXmlElement( bankAcct );
            }
            else if( stmtTrn.TryGetDescendant( ".//CCACCTTO", out XmlElement creditCardAcct ) )
            {
                this.TransactionSenderAccount = Account.FromXmlElement( creditCardAcct );
            }
        }

        public OfxTransactionType TransType { get; }

        public DateTimeOffset? Date { get; }

        public decimal Amount { get; }

        public string TransactionId { get; }

        public string Name { get; }

        public DateTimeOffset? TransactionInitializationDate { get; }

        public DateTimeOffset? FundAvaliabilityDate { get; }

        public string Memo { get; }

        public string IncorrectTransactionId { get; }

        public TransactionCorrectionType TransactionCorrectionAction { get; }

        public string ServerTransactionId { get; }

        public string CheckNum { get; }

        public string ReferenceNumber { get; }

        public string Sic { get; }

        public string PayeeId { get; }

        /// <summary><c>BANKACCTTO</c> or <c>CCACCTTO</c> - or <see langword="null"/>.</summary>
        public Account TransactionSenderAccount { get; }

        /// <summary><c>CURRENCY</c> or <c>ORIGCURRENCY</c> - or the default currency string value passed into <see cref="Transaction"/>'s constructor.</summary>
        public string Currency { get; }
    }
}
