using FluentAssertions;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace OfxSharp.NETCore.Tests
{
#pragma warning disable IDE0058 // Expression value is never used

    public class OfxFileParseTests
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void Should_read_ITAU_statements()
        {
            OfxDocument ofx = OfxDocumentReader.FromSgmlFile( filePath: @"Files\itau.ofx" );

            ofx.HasSingleStatement( out SingleStatementOfxDocument ofxDocument ).Should().BeTrue();

            ofxDocument.Should().NotBeNull();
            ofxDocument.StatementStart.Should().Be(new DateTimeOffset(2013, 12,  5, 10, 0, 0, TimeSpan.FromHours(-3))); // "20131205100000[-03:EST]" -> 2013-12-05 10:00:00-03:00
            ofxDocument.StatementEnd  .Should().Be(new DateTimeOffset(2014,  2, 28, 10, 0, 0, TimeSpan.FromHours(-3))); // "20140228100000[-03:EST]" -> 2014-02-28 10:00:00-03:00

            Account.BankAccount acc = ofxDocument.Account.Should().BeOfType<Account.BankAccount>().Subject;
            acc.AccountId.Trim().Should().Be("9999999999");
            acc.BankId   .Trim().Should().Be("0341");

            ofxDocument.Transactions.Count.Should().Be(3);
            ofxDocument.Transactions.Sum(x => x.Amount).Should().Be(-644.44M);
        }

        [Test]
        public void Should_read_Santander_statements()
        {
            OfxDocument ofx = OfxDocumentReader.FromSgmlFile( filePath: @"Files\santander.ofx" );
            ofx.HasSingleStatement( out SingleStatementOfxDocument ofxDocument ).Should().BeTrue();

            ofxDocument.Should().NotBeNull();
            ofxDocument.StatementStart.Should().Be(new DateTimeOffset(2014, 2, 3, 18, 22, 51, TimeSpan.FromHours(-3))); // "20140203182251[-3:GMT]" -> 2014-02-03 18:22:51-03:00
            ofxDocument.StatementEnd  .Should().Be(new DateTimeOffset(2014, 2, 3, 18, 22, 51, TimeSpan.FromHours(-3))); // "20140203182251[-3:GMT]" -> 2014-02-03 18:22:51-03:00

            Account.BankAccount acc = ofxDocument.Account.Should().BeOfType<Account.BankAccount>().Subject;
            acc.AccountId.Trim().Should().Be("9999999999999");
            acc.BankId   .Trim().Should().Be("033");

            ofxDocument.Transactions.Count.Should().Be(3);
            ofxDocument.Transactions.Sum(x => x.Amount).Should().Be(-22566.44M);
        }

        [Test]
        public void Should_read_Bradseco_statements()
        {
            OfxDocument ofx = OfxDocumentReader.FromSgmlFile( filePath: @"Files\bradesco.ofx" );
            ofx.HasSingleStatement( out SingleStatementOfxDocument ofxDocument ).Should().BeTrue();

            ofxDocument.Should().NotBeNull();
            ofxDocument.StatementStart.Should().Be(new DateTimeOffset(2019, 5, 9, 12, 0, 0, TimeSpan.Zero ) ); // "20190509120000" -> 2019-05-09 12:00:00
            ofxDocument.StatementEnd  .Should().Be(new DateTimeOffset(2019, 5, 9, 12, 0, 0, TimeSpan.Zero ) ); // "20190509120000" -> 2019-05-09 12:00:00
            
            Account.BankAccount acc = ofxDocument.Account.Should().BeOfType<Account.BankAccount>().Subject;
            acc.AccountId.Trim().Should().Be("99999");
            acc.BankId   .Trim().Should().Be("0237");
            
            ofxDocument.Transactions.Count.Should().Be(3);
            ofxDocument.Transactions.Sum(x => x.Amount).Should().Be(200755M);
        }
    }
}
