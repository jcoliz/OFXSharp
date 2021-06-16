using System.ComponentModel;

namespace OfxSharp
{
    public enum BankAccountType
    {
        [Description("Checking Account")] CHECKING,

        [Description("Savings Account")] SAVINGS,

        [Description("Money Market Account")] MONEYMRKT,

        [Description("Line of Credit")] CREDITLINE,

        NA,

        [Description("Home Loan")] HOMELOAN
    }
}