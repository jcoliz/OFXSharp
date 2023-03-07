using System;
using System.Collections.Generic;

namespace OfxSharp
{
    /// <summary>
    ///     Dictionary of message types and transaction types
    /// </summary>
    public class MessageResponseSets
    {
        public IDictionary<string, ResponseSet> knownMessageResponseSets()
        {
            var knownSets = new Dictionary<string, ResponseSet>()
            {
                {
                    "BANKMSGSRSV1", 
                    new ResponseSet
                    {
                        StatementTransaction="STMTTRNRS",
                        StatementRecord="STMTRS",
                        StatementRequest="BANKACCTFROM"
                    }
                },
                {
                    "CREDITCARDMSGSRSV1", 
                    new ResponseSet
                    {
                        StatementTransaction="CCSTMTTRNRS",
                        StatementRecord="CCSTMTRS",
                        StatementRequest="CCACCTFROM"
                    } 
                },
            };

            return knownSets;
        }
    }

    public class ResponseSet
    {
        public string StatementTransaction { get; set; }
        public string StatementRecord { get; set; }
        public string StatementRequest { get; set; }
    }
}
