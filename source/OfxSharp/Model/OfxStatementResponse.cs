using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace OfxSharp
{
    /// <summary>Flattened view of STMTTRNRS and STMTRS. 11.4.1.2 Response &lt;STMTRS&gt;<br />
    /// &quot;The <STMTRS> response must appear within a &lt;STMTTRNRS&gt; transaction wrapper.&quot; (the &quot;transaction&quot; refers to the OFX request/response transaction - not a bank transaction).</summary>
    public class OfxStatementResponse
    {
        public static OfxStatementResponse FromSTMTTRNRS( XmlElement stmtrnrs )
        {
            _ = stmtrnrs.AssertIsElement( "STMTTRNRS", parentElementName: "BANKMSGSRSV1" );

            XmlElement stmtrs    = stmtrnrs.RequireSingleElementChild("STMTRS");
            XmlElement transList = stmtrs  .RequireSingleElementChild("BANKTRANLIST");

            //

            String defaultCurrency = stmtrs.RequireNonemptyValue("CURDEF");

            return new OfxStatementResponse(
                trnUid           : stmtrnrs.RequireNonemptyValue("TRNUID").RequireParseInt32(),
                responseStatus   : OfxStatus.FromXmlElement( stmtrnrs.RequireSingleElementChild("STATUS") ),
                defaultCurrency  : defaultCurrency,
                accountFrom      : Account.FromXmlElement( stmtrs.GetSingleElementChildOrNull("BANKACCTFROM") ),
                transactionsStart: transList.RequireNonemptyValue("DTSTART").RequireParseOfxDateTime(),
                transactionsEnd  : transList.RequireNonemptyValue("DTEND"  ).RequireParseOfxDateTime(),
                transactions     : GetTransactions( transList, defaultCurrency ),
                ledgerBalance    : Balance.FromXmlElement( stmtrs.SelectSingleNode("LEDGERBAL") ),
                availableBalance : Balance.FromXmlElement( stmtrs.SelectSingleNode("AVAILBAL" ) )
            );
        }

        public static IEnumerable<Transaction> GetTransactions( XmlElement bankTranList, string defaultCurrency)
        {
            _ = bankTranList.AssertIsElement("BANKTRANLIST");

            foreach( XmlElement stmtTrn in bankTranList.SelectNodes("./STMTTRN").Cast<XmlElement>() )
            {
                yield return new Transaction( stmtTrn: stmtTrn, defaultCurrency );
            }
        }

        public OfxStatementResponse(
            Int32                    trnUid,
            OfxStatus                responseStatus,
            String                   defaultCurrency,
            Account                  accountFrom,
            DateTimeOffset           transactionsStart,
            DateTimeOffset           transactionsEnd,
            IEnumerable<Transaction> transactions,
            Balance                  ledgerBalance,
            Balance                  availableBalance
        )
        {
            this.OfxTransactionUniqueId = trnUid;
            this.ResponseStatus         = responseStatus   ?? throw new ArgumentNullException( nameof( responseStatus ) );
            this.DefaultCurrency        = defaultCurrency  ?? throw new ArgumentNullException( nameof( defaultCurrency ) );
            this.AccountFrom            = accountFrom      ?? throw new ArgumentNullException( nameof( accountFrom ) );
            this.TransactionsStart      = transactionsStart;
            this.TransactionsEnd        = transactionsEnd;
            this.LedgerBalance          = ledgerBalance    ?? throw new ArgumentNullException( nameof( ledgerBalance ) );
            this.AvailableBalance       = availableBalance;

            this.Transactions.AddRange( transactions );
        }

        /// <summary>STMTTRNRS/TRNUID (OFX Request/Response Transaction ID - this is unrelated to bank transactions).</summary>
        public Int32 OfxTransactionUniqueId { get; }

        /// <summary>STMTTRNRS/STATUS</summary>
        public OfxStatus ResponseStatus { get; }

        /// <summary>STMTTRNRS/STMTRS/CURDEF</summary>
        public String DefaultCurrency { get; }

        /// <summary>STMTTRNRS/STMTRS/BANKACCTFROM</summary>
        public Account AccountFrom { get; }

        /// <summary>STMTTRNRS/STMTRS/BANKTRANLIST/DTSTART</summary>
        public DateTimeOffset TransactionsStart { get; }

        /// <summary>STMTTRNRS/STMTRS/BANKTRANLIST/DTEND</summary>
        public DateTimeOffset TransactionsEnd   { get; }

        /// <summary>STMTTRNRS/STMTRS/BANKTRANLIST</summary>
        public List<Transaction> Transactions { get; } = new List<Transaction>();

        /// <summary>STMTTRNRS/STMTRS/LEDGERBAL. Required.</summary>
        public Balance LedgerBalance { get; }

        /// <summary>STMTTRNRS/STMTRS/AVAILBAL. Optional. Can be null.</summary>
        public Balance AvailableBalance { get; }
    }
}
