using System;
using System.Globalization;
using System.Xml;

namespace OfxSharp
{
    /// <summary>&lt;STMTTRN&gt;</summary>
    public class Transaction
    {
        /// <summary></summary>
        /// <param name="stmtTrn">&lt;STMTTRN&gt;, a child of &lt;BANKTRANLIST&gt;</param>
        /// <param name="defaultCurrency"></param>
        public Transaction( XmlElement stmtTrn, string defaultCurrency )
        {
            if( stmtTrn is null ) throw new ArgumentNullException( nameof( stmtTrn ) );
            if( defaultCurrency is null ) throw new ArgumentNullException( nameof( defaultCurrency ) );

            _ = stmtTrn.AssertIsElement( "STMTTRN", parentElementName: "BANKTRANLIST" );

            //

            this.TransType                     = stmtTrn.RequireSingleElementChild  ( "TRNTYPE"       ) .Value.ParseEnum<OfxTransactionType>();
            this.Date                          = stmtTrn.RequireSingleElementChild  ( "DTPOSTED"      ) .Value.MaybeParseOfxDateTime();
            this.TransactionInitializationDate = stmtTrn.GetSingleElementChildOrNull( "DTUSER"        )?.Value.MaybeParseOfxDateTime();
            this.FundAvaliabilityDate          = stmtTrn.GetSingleElementChildOrNull( "DTAVAIL"       )?.Value.MaybeParseOfxDateTime();
            this.Amount                        = stmtTrn.RequireSingleElementChild  ( "TRNAMT"        ) .Value.RequireParseDecimal();
            this.TransactionId                 = stmtTrn.RequireSingleElementChild  ( "FITID"         ) .Value;

            this.IncorrectTransactionId        = stmtTrn.GetSingleElementChildOrNull( "CORRECTFITID"  )?.Value;
            this.TransactionCorrectionAction   = stmtTrn.GetSingleElementChildOrNull( "CORRECTACTION" )?.Value.TryParseEnum<TransactionCorrectionType>() ?? (TransactionCorrectionType?)null;

            this.ServerTransactionId           = stmtTrn.GetSingleElementChildOrNull( "SRVRTID"       )?.Value;
            this.CheckNum                      = stmtTrn.GetSingleElementChildOrNull( "CHECKNUM"      )?.Value;
            this.ReferenceNumber               = stmtTrn.GetSingleElementChildOrNull( "REFNUM"        )?.Value;
            this.Sic                           = stmtTrn.GetSingleElementChildOrNull( "SIC"           )?.Value;
            this.PayeeId                       = stmtTrn.GetSingleElementChildOrNull( "PAYEEID"       )?.Value;
            this.Name                          = stmtTrn.GetSingleElementChildOrNull( "NAME"          )?.Value;
            this.Memo                          = stmtTrn.GetSingleElementChildOrNull( "MEMO"          )?.Value;

            this.OriginalCurrency              = stmtTrn.GetSingleElementChildOrNull( "ORIGCURRENCY"  )?.Value;
            this.Currency                      = stmtTrn.GetSingleElementChildOrNull( "CURRENCY"      )?.Value;
            this.DefaultCurrency               = defaultCurrency;

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

        /// <summary>TRNTYPE</summary>
        public OfxTransactionType TransType { get; }

        /// <summary>DTPOSTED</summary>
        public DateTimeOffset? Date { get; }

        /// <summary>TRNAMT</summary>
        public decimal Amount { get; }

        /// <summary>FITID</summary>
        public string TransactionId { get; }

        /// <summary>NAME</summary>
        public string Name { get; }

        /// <summary>DTUSER</summary>
        public DateTimeOffset? TransactionInitializationDate { get; }

        /// <summary>DTAVAIL</summary>
        public DateTimeOffset? FundAvaliabilityDate { get; }

        /// <summary>MEMO</summary>
        public string Memo { get; }

        /// <summary>CORRECTFITID</summary>
        public string IncorrectTransactionId { get; }

        /// <summary>CORRECTACTION</summary>
        public TransactionCorrectionType? TransactionCorrectionAction { get; }

        /// <summary>SRVRTID</summary>
        public string ServerTransactionId { get; }

        /// <summary>CHECKNUM</summary>
        public string CheckNum { get; }

        /// <summary>REFNUM</summary>
        public string ReferenceNumber { get; }

        /// <summary>SIC</summary>
        public string Sic { get; }

        /// <summary>PAYEEID</summary>
        public string PayeeId { get; }

        /// <summary><c>BANKACCTTO</c> or <c>CCACCTTO</c> - or <see langword="null"/>.</summary>
        public Account TransactionSenderAccount { get; }

        /// <summary><c>ORIGCURRENCY</c></summary>
        public string OriginalCurrency { get; }

        /// <summary><c>CURRENCY</c></summary>
        public string Currency { get; }

        /// <summary><c>CURDEF</c> (defined outside of &lt;STMTTRN&gt;)</summary>
        public string DefaultCurrency { get; }

        /// <summary><c>CURRENCY</c> or <c>ORIGCURRENCY</c> - or the default currency string value passed into <see cref="Transaction"/>'s constructor.</summary>
        public string EffectiveCurrency => this.Currency ?? this.OriginalCurrency ?? this.DefaultCurrency;
    }
}
