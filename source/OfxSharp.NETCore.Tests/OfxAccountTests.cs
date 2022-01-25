using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using NUnit.Framework;

using static OfxSharp.Account;

namespace OfxSharp.NETCore.Tests
{
    public class OfxAccountTests
    {
        [Test]
        public void AccountNull()
        {
            var account = Account.FromXmlElementOrNull(null);
            Assert.IsNull( account );
        }

        [Test]
        public void CreditAccountNullThrows()
            => Assert.Throws<ArgumentNullException>( () => new CreditAccount(null) );

        [Test]
        public void BankAccountNullThrows()
            => Assert.Throws<ArgumentNullException>( () => new BankAccount( null ) );

        [Test]
        public void AccountOtherNameThrows()
        {
            var doc = new XmlDocument();
            var element = doc.CreateElement( "Other" );

            _ = Assert.Throws<OfxException>( () => Account.FromXmlElementOrNull( element ) );
        }
        [Test]
        public void AccountBankAccountTo()
        {
            var doc = new XmlDocument();
            var bankacctto = doc.CreateElement( "BANKACCTTO" );
            var accttype = doc.CreateElement( "ACCTTYPE" );
            _ = accttype.AppendChild( doc.CreateTextNode( "CHECKING" ) );
            _ = bankacctto.AppendChild( accttype );

            var account = Account.FromXmlElementOrNull( bankacctto );

            Assert.AreEqual( AccountType.BANK, account.AccountType );
        }
        [Test]
        public void AccountCcAccountFrom()
        {
            var doc = new XmlDocument();
            var bankacctto = doc.CreateElement( "CCACCTFROM" );
            var accttype = doc.CreateElement( "ACCTTYPE" );
            _ = accttype.AppendChild( doc.CreateTextNode( "CHECKING" ) );
            _ = bankacctto.AppendChild( accttype );

            var account = Account.FromXmlElementOrNull( bankacctto );

            Assert.AreEqual( AccountType.CC, account.AccountType );
        }
    }
}
