using System;
using System.Collections.Generic;

namespace OfxSharp
{
    public class OFXDocument
    {
        /// <summary>Raw value for <see cref="StatementStart"/>.</summary>
        public string StatementStartValue { get; set; }

        public DateTimeOffset? StatementStart { get; set; }

        /// <summary>Raw value for <see cref="StatementEnd"/>.</summary>
        public string StatementEndValue { get; set; }

        public DateTimeOffset? StatementEnd { get; set; }

        public AccountType AccType { get; set; }

        public string Currency { get; set; }

        public SignOn SignOn { get; set; }

        public Account Account { get; set; }

        public Balance Balance { get; set; }

        public List<Transaction> Transactions { get; set; }
    }
}
