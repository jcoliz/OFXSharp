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

            this.TransType                     = stmtTrn.RequireSingleElementChild  ( "TRNTYPE"       ) .RequireSingleTextChildNode().ParseEnum<OfxTransactionType>();
            this.Date                          = stmtTrn.RequireSingleElementChild  ( "DTPOSTED"      ) .RequireSingleTextChildNode().MaybeParseOfxDateTime();
            this.TransactionInitializationDate = stmtTrn.GetSingleElementChildOrNull( "DTUSER"        )?.RequireSingleTextChildNode().MaybeParseOfxDateTime();
            this.FundAvaliabilityDate          = stmtTrn.GetSingleElementChildOrNull( "DTAVAIL"       )?.RequireSingleTextChildNode().MaybeParseOfxDateTime();
            this.Amount                        = stmtTrn.RequireSingleElementChild  ( "TRNAMT"        ) .RequireSingleTextChildNode().RequireParseDecimal();
            this.TransactionId                 = stmtTrn.RequireSingleElementChild  ( "FITID"         ) .RequireSingleTextChildNode();

            this.IncorrectTransactionId        = stmtTrn.GetSingleElementChildOrNull( "CORRECTFITID"  )?.RequireSingleTextChildNode();
            this.TransactionCorrectionAction   = stmtTrn.GetSingleElementChildOrNull( "CORRECTACTION" )?.RequireSingleTextChildNode().TryParseEnum<TransactionCorrectionType>() ?? (TransactionCorrectionType?)null;

            this.ServerTransactionId           = stmtTrn.GetSingleElementChildOrNull( "SRVRTID"       )?.RequireSingleTextChildNode();
            this.CheckNum                      = stmtTrn.GetSingleElementChildOrNull( "CHECKNUM"      )?.RequireSingleTextChildNode();
            this.ReferenceNumber               = stmtTrn.GetSingleElementChildOrNull( "REFNUM"        )?.RequireSingleTextChildNode();
            this.Sic                           = stmtTrn.GetSingleElementChildOrNull( "SIC"           )?.RequireSingleTextChildNode();
            this.PayeeId                       = stmtTrn.GetSingleElementChildOrNull( "PAYEEID"       )?.RequireSingleTextChildNode();
            this.Name                          = stmtTrn.GetSingleElementChildOrNull( "NAME"          )?.RequireSingleTextChildNode();
            this.Memo                          = stmtTrn.GetSingleElementChildOrNull( "MEMO"          )?.RequireSingleTextChildNode();

            this.OriginalCurrency              = stmtTrn.GetSingleElementChildOrNull( "ORIGCURRENCY"  )?.RequireSingleTextChildNode();
            this.Currency                      = stmtTrn.GetSingleElementChildOrNull( "CURRENCY"      )?.RequireSingleTextChildNode();
            this.DefaultCurrency               = defaultCurrency;

            //If senders bank/credit card details avaliable, add
            if( stmtTrn.GetSingleElementChildOrNull( "BANKACCTTO" ) is XmlElement bankAcct )
            {
                this.TransactionSenderAccount = Account.FromXmlElement( bankAcct );
            }
            else if( stmtTrn.GetSingleElementChildOrNull( "CCACCTTO" ) is XmlElement creditCardAcct )
            {
                this.TransactionSenderAccount = Account.FromXmlElement( creditCardAcct );
            }
            else
            {
                this.TransactionSenderAccount = null;
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
