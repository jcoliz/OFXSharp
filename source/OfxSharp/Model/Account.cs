using System;
using System.Xml;

namespace OfxSharp
{
    /// <summary>11.3.1 Banking Account &lt;BANKACCTFROM&gt; and &lt;BANKACCTTO&gt;</summary>
    public abstract class Account
    {
        public static Account FromXmlElementOrNull( XmlElement accountElementOrNull )
        {
            if( accountElementOrNull is null )
            {
                return null;
            }
            else if( accountElementOrNull.Name == "BANKACCTFROM" )
            {
                return new BankAccount( accountElementOrNull );
            }
            else if( accountElementOrNull.Name == "BANKACCTTO" )
            {
                return new BankAccount( accountElementOrNull );
            }
            else if( accountElementOrNull.Name == "CCACCTFROM" )
            {
                return new CreditAccount( accountElementOrNull );
            }
            else
            {
                throw new OfxException( message: "Expected null, <BANKACCTFROM>, <BANKACCTTO>, or <CCACCTFROM>, but encountered: <{0}>".Fmt( accountElementOrNull.Name ) );
            }
        }

        private Account( XmlElement accountElement, AccountType type )
        {
            if( accountElement is null ) throw new ArgumentNullException( nameof( accountElement ) );

            this.AccountType = type;
            this.AccountId   = accountElement.GetSingleElementChildTextOrNull( "ACCTID"  );
            this.AccountKey  = accountElement.GetSingleElementChildTextOrNull( "ACCTKEY" );
        }

        /// <summary><c>ACCTID</c>. Can be null.</summary>
        public string      AccountId   { get; }

        /// <summary><c>ACCTKEY</c></summary>
        public string      AccountKey  { get; }

        /// <summary>Varies based on <c>BANKACCTFROM</c> or <c>BANKACCTTO</c> or <c>CCACCTFROM</c></summary>
        public AccountType AccountType { get; }

        #region Subtypes

        public sealed class BankAccount : Account
        {
            public BankAccount( XmlElement bankAccountElement )
                : base( bankAccountElement, AccountType.BANK )
            {
                if( bankAccountElement is null ) throw new ArgumentNullException( nameof( bankAccountElement ) );

                this.BankId          = bankAccountElement.GetSingleElementChildTextOrNull( "BANKID"   );
                this.BranchId        = bankAccountElement.GetSingleElementChildTextOrNull( "BRANCHID" );
                this.BankAccountType = bankAccountElement.RequireSingleElementChildText  ( "ACCTTYPE" ).ParseEnum<BankAccountType>();
            }

            /// <summary>BANKID</summary>
            public string BankId { get; }

            /// <summary>BRANCHID</summary>
            public string BranchId { get; }

            /// <summary>ACCTTYPE</summary>
            public BankAccountType BankAccountType { get; }
        }

        public sealed class CreditAccount : Account
        {
            public CreditAccount( XmlElement creditAccountElement )
                : base( creditAccountElement, AccountType.CC )
            {
                if( creditAccountElement is null ) throw new ArgumentNullException( nameof( creditAccountElement ) );
            }
        }

        #endregion
    }

}
