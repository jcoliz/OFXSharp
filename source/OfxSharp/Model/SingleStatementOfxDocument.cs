using System;
using System.Collections.Generic;
using System.Linq;

namespace OfxSharp
{
    public class SingleStatementOfxDocument
    {
        public SingleStatementOfxDocument( OfxDocument ofx )
        {
            this.Ofx             = ofx ?? throw new ArgumentNullException( nameof( ofx ) );

            this.SingleStatement = ofx.Statements.Single();
        }

        public OfxDocument Ofx { get; }

        public OfxStatementResponse SingleStatement { get; }

        public DateTimeOffset             StatementStart => this.SingleStatement.TransactionsStart;
        public DateTimeOffset             StatementEnd   => this.SingleStatement.TransactionsEnd;
        public Account                    Account        => this.SingleStatement.AccountFrom;
        public IReadOnlyList<Transaction> Transactions   => this.SingleStatement.Transactions;
    }
}
