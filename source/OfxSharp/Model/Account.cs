using System;
using System.Xml;

namespace OfxSharp
{
    /// <summary>11.3.1 Banking Account &lt;BANKACCTFROM&gt; and &lt;BANKACCTTO&gt;</summary>
    public class Account
    {
        public static Account FromXmlElement( XmlNode accountElementOrNull )
        {
            if( accountElementOrNull is null )
            {
                return null;
            }
            else if( accountElementOrNull.Name == "BANKACCTFROM" )
            {
                return new Account( node: accountElementOrNull, type: AccountType.BANK );
            }
            else if( accountElementOrNull.Name == "BANKACCTTO" )
            {
                return new Account( node: accountElementOrNull, type: AccountType.BANK );
            }
            else if( accountElementOrNull.Name == "CCACCTFROM" )
            {
                return new Account( node: accountElementOrNull, type: AccountType.CC );
            }
            else
            {
                throw new OfxException( message: "Unrecognized or unsupported account element: <{0}>".Fmt( accountElementOrNull.Name ) );
            }
        }

        public Account(XmlNode node, AccountType type)
        {
            this.AccountType = type;

            this.AccountId = node.GetValue("//ACCTID")?.Trim();
            this.AccountKey = node.GetValue("//ACCTKEY")?.Trim();

            switch ( this.AccountType )
            {
                case AccountType.BANK:
                this.InitializeBank(node);
                    break;

                case AccountType.AP:
                this.InitializeAp(node);
                    break;

                case AccountType.AR:
                this.InitializeAr(node);
                    break;
            }
        }

        public string      AccountId   { get; set; }
        public string      AccountKey  { get; set; }
        public AccountType AccountType { get; set; }

        /// <summary>Initializes information specific to bank</summary>
        private void InitializeBank(XmlNode node)
        {
            this.BankId = node.GetValue("//BANKID");
            this.BranchId = node.GetValue("//BRANCHID");

            //Get Bank Account Type from XML
            String bankAccountType = node.GetValue("//ACCTTYPE");

            //Check that it has been set
            if (string.IsNullOrEmpty(bankAccountType))
            {
                throw new OfxParseException("Bank Account type unknown");
            }

            //Set bank account enum
            this._bankAccountType = bankAccountType.ParseEnum<BankAccountType>();
        }

        #region Bank Only

        private BankAccountType _bankAccountType = BankAccountType.NA;

        public string BankId { get; set; }

        public string BranchId { get; set; }

        public BankAccountType BankAccountType
        {
            get => this.AccountType == AccountType.BANK ? this._bankAccountType : BankAccountType.NA;
            set => this._bankAccountType = this.AccountType == AccountType.BANK ? value : BankAccountType.NA;
        }

        #endregion Bank Only

        #region Account types not supported

        private void InitializeAp(XmlNode node)
        {
            throw new OfxParseException("AP Account type not supported");
        }

        private void InitializeAr(XmlNode node)
        {
            throw new OfxParseException("AR Account type not supported");
        }

        #endregion Account types not supported
    }
}
