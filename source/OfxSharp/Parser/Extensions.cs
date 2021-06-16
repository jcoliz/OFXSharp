using System;
using System.Collections.Generic;
using System.Globalization;
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

        public static TEnum? MaybeParseEnum<TEnum>(this string s)
            where TEnum : struct, Enum
        {
            return Enum.TryParse<TEnum>( s, ignoreCase: true, out TEnum value ) ? value : (TEnum?)null;
        }

        /// <summary> Returns value of specified node</summary>
        /// <param name="node">Node to look for specified node</param>
        /// <param name="xpath">XPath for node you want</param>
        public static string GetValue(this XmlNode node, string xpath)
        {
            // TODO: Be more rigorous about XmlNode types. `FirstChild` is meant to be the #text node, not an element node, right?
            // ...in which case, why not use `XmlNode.InnerText`?

			if( node.SelectSingleNode( xpath ) is XmlNode selectedNode )
            {
                if( selectedNode.FirstChild is XmlNode firstChild )
                {
                    return firstChild.Value;
                }
            }
                
            return string.Empty;
        }
	}
}
