using FluentAssertions;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace OfxSharp.NETCore.Tests
{
    public class OfxDateTimeTests
    {
        // We can't use NUnit's `[TestCase]` for parameterised tests because you can't use a DateTime or DateTimeOffset in a C# attribute ctor expression. *grumble...*

        [Test]
        public void OfxDateTime_should_parse_YYYYMMDD()
        {
            //           yyyyMMdd
            TestParse( @"19961005", new DateTimeOffset( year: 1996, month: 10, day: 5, hour: 0, minute: 0, second: 0, offset: TimeSpan.Zero ) );
            TestParse( @"20210102", new DateTimeOffset( year: 2021, month:  1, day: 2, hour: 0, minute: 0, second: 0, offset: TimeSpan.Zero ) );
        }

        [Test]
        public void OfxDateTime_should_parse_YYYYMMDDHHMMSS()
        {
            //           yyyyMMddHHmmSS
            TestParse( @"19961005132200", new DateTimeOffset( year: 1996, month: 10, day: 5, hour: 13, minute: 22, second: 0, offset: TimeSpan.Zero ) ); // Example date from the OFX 1.6 specification.
            TestParse( @"20210102030405", new DateTimeOffset( year: 2021, month:  1, day: 2, hour:  3, minute:  4, second: 5, offset: TimeSpan.Zero ) ); // This case is to ensure culture-invariant parsing due to ambiguous month+day.
        }

        [Test]
        public void OfxDateTime_should_parse_YYYYMMDDHHMMSS_XXX()
        {
            //           yyyyMMddHHmmSS.XXX
            TestParse( @"19961005132200.124", new DateTimeOffset( year: 1996, month: 10, day: 5, hour: 13, minute: 22, second: 0, millisecond: 124, offset: TimeSpan.Zero ) ); // Example date from the OFX 1.6 specification.
            TestParse( @"20210102030405.999", new DateTimeOffset( year: 2021, month:  1, day: 2, hour:  3, minute:  4, second: 5, millisecond: 999, offset: TimeSpan.Zero ) ); 
            TestParse( @"20210102030405.100", new DateTimeOffset( year: 2021, month:  1, day: 2, hour:  3, minute:  4, second: 5, millisecond: 100, offset: TimeSpan.Zero ) );
            TestParse( @"20210102030405.001", new DateTimeOffset( year: 2021, month:  1, day: 2, hour:  3, minute:  4, second: 5, millisecond:   1, offset: TimeSpan.Zero ) );
        }

        [Test]
        public void OfxDateTime_should_reject_YYYYMMDDHHMMSS_X()
        {
            //           yyyyMMddHHmmSS.XXX
            TestParseNull( @"19961005132200.01", expectedOK: false );
            TestParseNull( @"20210102030405.10", expectedOK: false ); 
            TestParseNull( @"20210102030405.1" , expectedOK: false );
            TestParseNull( @"20210102030405.0" , expectedOK: false );
        }

        [Test]
        public void OfxDateTime_should_accept_00000000000000_as_null()
        {
            //               yyyyMMddHHmmSS.XXX[zz:nom]
            TestParseNull( @"00000000000000.000[-8.00:PST]", expectedOK: true );

            //               yyyyMMddHHmmSS.XXX
            TestParseNull( @"00000000000000.000", expectedOK: true );

            //               yyyyMMddHHmmSS
            TestParseNull( @"00000000000000", expectedOK: true );

            //               yyyyMMdd
            TestParseNull( @"00000000", expectedOK: true );

            //               yyyyMMddHHmmSS.XXX[zz:nom]
            TestParseNull( @"00000000000000.000[0:PST]", expectedOK: true ); // Will this ever happen?
        }

        [Test]
        public void OfxDateTime_should_parse_YYYYMMDDHHMMSS_XXX_Offset_ZoneName()
        {
            //           yyyyMMddHHmmSS.XXX[zz:nom]
            TestParse( @"19961005132200.124[-5:EST]", new DateTimeOffset( year: 1996, month: 10, day: 5, hour: 13, minute: 22, second: 0, millisecond: 124, offset: TimeSpan.FromHours(-5) ) ); // Example date from the OFX 1.6 specification.
            TestParse( @"20210102030405.999[-5:EST]", new DateTimeOffset( year: 2021, month:  1, day: 2, hour:  3, minute:  4, second: 5, millisecond: 999, offset: TimeSpan.FromHours(-5) ) ); 
            TestParse( @"20210102030405.100[-5:EST]", new DateTimeOffset( year: 2021, month:  1, day: 2, hour:  3, minute:  4, second: 5, millisecond: 100, offset: TimeSpan.FromHours(-5) ) );
            TestParse( @"20210102030405.001[-5:EST]", new DateTimeOffset( year: 2021, month:  1, day: 2, hour:  3, minute:  4, second: 5, millisecond:   1, offset: TimeSpan.FromHours(-5) ) );

            TestParse( @"19961005132200.124[-12:IDL]", new DateTimeOffset( year: 1996, month: 10, day: 5, hour: 13, minute: 22, second: 0, millisecond: 124, offset: TimeSpan.FromHours(-12) ) );
            TestParse( @"20210102030405.999[-12:IDL]", new DateTimeOffset( year: 2021, month:  1, day: 2, hour:  3, minute:  4, second: 5, millisecond: 999, offset: TimeSpan.FromHours(-12) ) ); 
            TestParse( @"20210102030405.100[-12:IDL]", new DateTimeOffset( year: 2021, month:  1, day: 2, hour:  3, minute:  4, second: 5, millisecond: 100, offset: TimeSpan.FromHours(-12) ) );
            TestParse( @"20210102030405.001[-12:IDL]", new DateTimeOffset( year: 2021, month:  1, day: 2, hour:  3, minute:  4, second: 5, millisecond:   1, offset: TimeSpan.FromHours(-12) ) );

            TestParse( @"19961005132200.124[+12:Hmmmm]", new DateTimeOffset( year: 1996, month: 10, day: 5, hour: 13, minute: 22, second: 0, millisecond: 124, offset: TimeSpan.FromHours(12) ) );
            TestParse( @"20210102030405.999[+12:Hmmmm]", new DateTimeOffset( year: 2021, month:  1, day: 2, hour:  3, minute:  4, second: 5, millisecond: 999, offset: TimeSpan.FromHours(12) ) ); 
            TestParse( @"20210102030405.100[+12:Hmmmm]", new DateTimeOffset( year: 2021, month:  1, day: 2, hour:  3, minute:  4, second: 5, millisecond: 100, offset: TimeSpan.FromHours(12) ) );
            TestParse( @"20210102030405.001[+12:Hmmmm]", new DateTimeOffset( year: 2021, month:  1, day: 2, hour:  3, minute:  4, second: 5, millisecond:   1, offset: TimeSpan.FromHours(12) ) );

            // Is the "+" required for zero?
            TestParse( @"19961005132200.124[0:GMT]", new DateTimeOffset( year: 1996, month: 10, day: 5, hour: 13, minute: 22, second: 0, millisecond: 124, offset: TimeSpan.Zero ) );
            TestParse( @"20210102030405.999[0:GMT]", new DateTimeOffset( year: 2021, month:  1, day: 2, hour:  3, minute:  4, second: 5, millisecond: 999, offset: TimeSpan.Zero ) ); 
            TestParse( @"20210102030405.100[0:GMT]", new DateTimeOffset( year: 2021, month:  1, day: 2, hour:  3, minute:  4, second: 5, millisecond: 100, offset: TimeSpan.Zero ) );
            TestParse( @"20210102030405.001[0:GMT]", new DateTimeOffset( year: 2021, month:  1, day: 2, hour:  3, minute:  4, second: 5, millisecond:   1, offset: TimeSpan.Zero ) );
        }

        [Test]
        public void OfxDateTime_should_parse_YYYYMMDDHHMMSS_XXX_FractionalOffset_ZoneName()
        {
            //           yyyyMMddHHmmSS.XXX[zz.ff:nom]
            TestParse( @"19961005132200.124[-5.00:EST]", new DateTimeOffset( year: 1996, month: 10, day: 5, hour: 13, minute: 22, second: 0, millisecond: 124, offset: TimeSpan.FromHours(-5) ) );
            TestParse( @"20210102030405.999[-5.00:EST]", new DateTimeOffset( year: 2021, month:  1, day: 2, hour:  3, minute:  4, second: 5, millisecond: 999, offset: TimeSpan.FromHours(-5) ) ); 
            TestParse( @"20210102030405.100[-5.00:EST]", new DateTimeOffset( year: 2021, month:  1, day: 2, hour:  3, minute:  4, second: 5, millisecond: 100, offset: TimeSpan.FromHours(-5) ) );
            TestParse( @"20210102030405.001[-5.00:EST]", new DateTimeOffset( year: 2021, month:  1, day: 2, hour:  3, minute:  4, second: 5, millisecond:   1, offset: TimeSpan.FromHours(-5) ) );

            // Should "-5.50" be interpreted as 5h50m, or 5h30m? The OFX specification says "fractional" offset, which to me makes me think of decimal fractions, not Base-60 fractions.
            // But the ISO 8601 spec uses the same syntax/format for Base-60, so let's go with that. Also, this: https://www.timeanddate.com/time/time-zones-interesting.html

            TestParse( @"19961005132200.124[-5.15:EST]", new DateTimeOffset( year: 1996, month: 10, day: 5, hour: 13, minute: 22, second: 0, millisecond: 124, offset: TimeSpan.FromHours(-5).Subtract(TimeSpan.FromMinutes(15)) ) );
            TestParse( @"20210102030405.999[-5.30:EST]", new DateTimeOffset( year: 2021, month:  1, day: 2, hour:  3, minute:  4, second: 5, millisecond: 999, offset: TimeSpan.FromHours(-5).Subtract(TimeSpan.FromMinutes(30)) ) ); 
            TestParse( @"20210102030405.100[-5.45:EST]", new DateTimeOffset( year: 2021, month:  1, day: 2, hour:  3, minute:  4, second: 5, millisecond: 100, offset: TimeSpan.FromHours(-5).Subtract(TimeSpan.FromMinutes(45)) ) );
            TestParse( @"20210102030405.001[-5.50:EST]", new DateTimeOffset( year: 2021, month:  1, day: 2, hour:  3, minute:  4, second: 5, millisecond:   1, offset: TimeSpan.FromHours(-5).Subtract(TimeSpan.FromMinutes(50)) ) );
        }

        private static void TestParse( String input, DateTimeOffset expected )
        {
            DateTimeOffset actual = OfxDateTime.RequireParseOfxDateTime( input );
            _ = actual.Should().Be( expected );
        }

        private static void TestParseNull( String input, Boolean expectedOK )
        {
            Boolean ok = OfxDateTime.TryParseOfxDateTime( input, value: out DateTimeOffset? parsedValue, errorMessage: out _ );
            _ = ok.Should().Be( expectedOK );
            _ = parsedValue.Should().BeNull();
        }
    }
}
