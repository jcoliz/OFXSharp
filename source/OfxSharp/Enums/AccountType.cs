using System.ComponentModel;

namespace OfxSharp
{
    public enum AccountType
    {
        [Description("Bank Account")] BANK,

        [Description("Credit Card")] CC,

        [Description("Accounts Payable")] AP,

        [Description("Accounts Recievable")] AR,

        NA
    }
}