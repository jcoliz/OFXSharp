using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace OfxSharp
{
	internal static class Extensions
	{
		public static string Fmt(this string format, params object[] args)
		{
			return String.Format( CultureInfo.CurrentCulture, format, args );
		}

		public static string FmtInv(this string format, params object[] args)
		{
			return String.Format( CultureInfo.InvariantCulture, format, args );
		}

        public static bool IsSet(this string s)
		{
			return !String.IsNullOrWhiteSpace(s);
		}

        public static bool IsEmpty(this string s)
		{
			return String.IsNullOrWhiteSpace(s);
		}

        public static TEnum ParseEnum<TEnum>(this string s)
           where TEnum : struct, Enum
        {
            if( Enum.TryParse<TEnum>( s, ignoreCase: true, out TEnum value ) )
            {
                return value;
            }
            else
            {
                throw new FormatException( "Couldn't parse \"{0}\" as a {1} enum value.".Fmt( s, typeof(TEnum).Name ) );
            }
        }

        public static TEnum? TryParseEnum<TEnum>(this string s)
           where TEnum : struct, Enum
        {
            if( Enum.TryParse<TEnum>( s, ignoreCase: true, out TEnum value ) )
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        public static TEnum? MaybeParseEnum<TEnum>(this string s)
            where TEnum : struct, Enum
        {
            return Enum.TryParse<TEnum>( s, ignoreCase: true, out TEnum value ) ? value : (TEnum?)null;
        }

        public static Int32? TryParseInt32(this string s)
        {
            return Int32.TryParse( s, NumberStyles.Integer, CultureInfo.InvariantCulture, out Int32 value ) ? value : (Int32?)null;
        }

        public static Int32 RequireParseInt32(this string s)
        {
            return Int32.Parse( s, NumberStyles.Integer, CultureInfo.InvariantCulture );
        }

        public static Decimal RequireParseDecimal(this string s)
        {
            return Decimal.Parse( s, NumberStyles.Any, CultureInfo.InvariantCulture );
        }

        private static readonly XmlWriterSettings _xmlSettings = new XmlWriterSettings()
        {
            NewLineHandling = NewLineHandling.Replace,
            NewLineChars    = "\r\n",
            Indent          = true,
            IndentChars     = "\t",
            ConformanceLevel = ConformanceLevel.Document,
        };

        public static String ToXmlString( this XmlDocument doc )
        {
            String xmlString;

            using( StringWriter stringWriter = new StringWriter() )
            using( XmlWriter xmlTextWriter = XmlWriter.Create( stringWriter, _xmlSettings ) )
            {
                doc.WriteTo( xmlTextWriter );

                xmlTextWriter.Flush();

                xmlString = stringWriter.ToString();
            }

            return xmlString;
        }
	}
}
