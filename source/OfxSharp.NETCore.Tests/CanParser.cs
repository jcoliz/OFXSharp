using FluentAssertions;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace OfxSharp.NETCore.Tests
{
    public class CanParser
    {
        private readonly OFXDocumentParser parser = new OFXDocumentParser();

        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void CanParserItau()
        {
            var ofxFile = new FileStream(@"Files/itau.ofx", FileMode.Open);

            var ofxDocument = this.parser.Import(ofxFile);

            _ = ofxDocument.Should().NotBeNull();
            _ = ofxDocument.StatementStart.Should().Be(new DateTimeOffset(2013, 12,  5, 10, 0, 0, TimeSpan.FromHours(-3))); // "20131205100000[-03:EST]" -> 2013-12-05 10:00:00-03:00
            _ = ofxDocument.StatementEnd  .Should().Be(new DateTimeOffset(2014,  2, 28, 10, 0, 0, TimeSpan.FromHours(-3))); // "20140228100000[-03:EST]" -> 2014-02-28 10:00:00-03:00
            _ = ofxDocument.Account.AccountId.Trim().Should().Be("9999999999");
            _ = ofxDocument.Account.BankId.Trim().Should().Be("0341");
            _ = ofxDocument.Transactions.Count.Should().Be(3);
            _ = ofxDocument.Transactions.Sum(x => x.Amount).Should().Be(-644.44M);
        }

        [Test]
        public void CanParserSantander()
        {
            var ofxFile = new FileStream(@"Files/santander.ofx", FileMode.Open);

            var ofxDocument = this.parser.Import(ofxFile);

            _ = ofxDocument.Should().NotBeNull();
            _ = ofxDocument.StatementStart.Should().Be(new DateTimeOffset(2014, 2, 3, 18, 22, 51, TimeSpan.FromHours(-3))); // "20140203182251[-3:GMT]" -> 2014-02-03 18:22:51-03:00
            _ = ofxDocument.StatementEnd  .Should().Be(new DateTimeOffset(2014, 2, 3, 18, 22, 51, TimeSpan.FromHours(-3))); // "20140203182251[-3:GMT]" -> 2014-02-03 18:22:51-03:00
            _ = ofxDocument.Account.AccountId.Trim().Should().Be("9999999999999");
            _ = ofxDocument.Account.BankId.Trim().Should().Be("033");
            _ = ofxDocument.Transactions.Count.Should().Be(3);
            _ = ofxDocument.Transactions.Sum(x => x.Amount).Should().Be(-22566.44M);
        }

        [Test]
        public void CanParserBradesco()
        {
            var ofxFile = new FileStream(@"Files/bradesco.ofx", FileMode.Open);

            var ofxDocument = this.parser.Import(ofxFile);

            _ = ofxDocument.Should().NotBeNull();
            _ = ofxDocument.StatementStart.Should().Be(new DateTimeOffset(2019, 5, 9, 12, 0, 0, TimeSpan.Zero ) ); // "20190509120000" -> 2019-05-09 12:00:00
            _ = ofxDocument.StatementEnd  .Should().Be(new DateTimeOffset(2019, 5, 9, 12, 0, 0, TimeSpan.Zero ) ); // "20190509120000" -> 2019-05-09 12:00:00
            _ = ofxDocument.Account.AccountId.Should().Be("99999");
            _ = ofxDocument.Account.BankId.Should().Be("0237");
            _ = ofxDocument.Transactions.Count.Should().Be(3);
            _ = ofxDocument.Transactions.Sum(x => x.Amount).Should().Be(200755M);
        }
    }
}
