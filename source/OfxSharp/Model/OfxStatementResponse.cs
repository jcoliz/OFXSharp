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
            var stmntElement = new MessageResponseSets().knownMessageResponseSets();
            string parentElement = null;
            if(stmntElement.ContainsKey(stmtrnrs.ParentNode.Name))
                parentElement = stmtrnrs.ParentNode.Name;
            var containerElement = stmntElement[parentElement].StatementTransaction;
            var itemElement = stmntElement[parentElement].StatementRecord;
            var bankRequest = stmntElement[parentElement].StatementRequest;
            _ = stmtrnrs.AssertIsElement( containerElement, parentElementName: parentElement );

            XmlElement stmtrs    = stmtrnrs.RequireSingleElementChild(itemElement);
            XmlElement transList = stmtrs  .RequireSingleElementChild("BANKTRANLIST");

            //

            String defaultCurrency = stmtrs.RequireSingleElementChildText("CURDEF");

            return new OfxStatementResponse(
                trnUid           : stmtrnrs.RequireSingleElementChildText("TRNUID").RequireParseInt32(),
                responseStatus   : OfxStatus.FromXmlElement( stmtrnrs.RequireSingleElementChild("STATUS") ),
                defaultCurrency  : defaultCurrency,
                accountFrom      : Account.FromXmlElementOrNull( stmtrs.GetSingleElementChildOrNull(bankRequest) ),
                transactionsStart: transList.RequireSingleElementChildText("DTSTART").RequireParseOfxDateTime(),
                transactionsEnd  : transList.RequireSingleElementChildText("DTEND"  ).RequireParseOfxDateTime(),
                transactions     : GetTransactions( transList, defaultCurrency ),
                ledgerBalance    : Balance.FromXmlElementOrNull( stmtrs.GetSingleElementChildOrNull("LEDGERBAL") ),
                availableBalance : Balance.FromXmlElementOrNull( stmtrs.GetSingleElementChildOrNull("AVAILBAL" ) )
            );
        }

        public static IEnumerable<Transaction> GetTransactions( XmlElement bankTranList, string defaultCurrency)
        {
            _ = bankTranList.AssertIsElement("BANKTRANLIST");

            foreach( XmlElement stmtTrn in bankTranList.GetChildNodes( "STMTTRN" ).Cast<XmlElement>() )
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
