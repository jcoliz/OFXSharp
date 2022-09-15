using System;
using System.Collections.Generic;

namespace OfxSharp
{
    /// <summary>
    ///     Dictionary of message types and transaction types
    /// </summary>
    public class MessageResponseSets
    {
        public IDictionary<string, ResponseSet> getMessageResponseSets()
        {
            var knownSets = new Dictionary<string, ResponseSet>()
            {
                {"BANKMSGSRSV1", new ResponseSet {StatementTransactionRecord="STMTTRNRS", StatementRecord="STMTRS"} },
                {"CREDITCARDMSGSRSV1", new ResponseSet{StatementTransactionRecord="CCSTMTTRNRS", StatementRecord="CCSTMTRS"} }
            };
            return knownSets;
        }
    }

    public class ResponseSet
    {
        public string StatementTransactionRecord { get; set; }
        public string StatementRecord { get; set; }
    }
}
