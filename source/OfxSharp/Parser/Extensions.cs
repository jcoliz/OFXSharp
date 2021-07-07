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

        public static Boolean TryGetDescendant( this XmlElement element, string xpath, out XmlElement descendantElement )
        {
            XmlNode desc = element.SelectSingleNode( xpath );
            if( desc is XmlElement descEl )
            {
                descendantElement = descEl;
                return true;
            }
            else
            {
                descendantElement = default;
                return false;
            }
        }

        public static Boolean TryGetValue(this XmlNode node, string xpath, out String value)
        {
            // TODO: Be more rigorous about XmlNode types. `FirstChild` is meant to be the #text node, not an element node, right?
            // ...in which case, why not use `XmlNode.InnerText`?

			if( node.SelectSingleNode( xpath ) is XmlNode selectedNode )
            {
                if( selectedNode.FirstChild is XmlNode firstChild && firstChild.NodeType == XmlNodeType.Text )
                {
                    value = firstChild.Value?.Trim();
                    return true;
                }
            }
                
            value = default;
            return false;
        }

        public static String RequireNonemptyValue(this XmlNode node, string xpath)
        {
			if( node.SelectSingleNode( xpath ) is XmlNode selectedNode )
            {
                if( selectedNode.FirstChild is XmlNode firstChild && firstChild.NodeType == XmlNodeType.Text )
                {
                    String value = firstChild.Value;
                    if( value.IsSet() )
                    {
                        return value.Trim();
                    }
                    else
                    {
                        throw new OfxException( message: "The XML node matching XPath \"{0}\" has an empty #text node first-child.".Fmt( xpath ) );
                    }
                }
                else
                {
                    throw new OfxException( message: "The XML node matching XPath \"{0}\" does not have a #text node first-child.".Fmt( xpath ) );
                }
            }
            else
            {
                throw new OfxException( message: "No XML node matched the XPath \"{0}\".".Fmt( xpath ) );
            }
        }

        public static String GetOptionalValue(this XmlNode node, string xpath)
        {
			if( node.SelectSingleNode( xpath ) is XmlNode selectedNode )
            {
                if( selectedNode.FirstChild is XmlNode firstChild && firstChild.NodeType == XmlNodeType.Text )
                {
                    return firstChild.Value;
                }
            }
                
            return null;
        }

        public static XmlElement AssertIsElement( this XmlNode node, String elementName, String parentElementName = null )
        {
            if( node is null ) throw new ArgumentNullException( nameof( node ) );

            if( node is XmlElement element && node.NodeType == XmlNodeType.Element )
            {
                if( element.LocalName == elementName )
                {
                    if( parentElementName is null || ( element.ParentNode.LocalName == parentElementName ) )
                    {
                       return element;
                    }
                    else
                    {
                        throw new ArgumentException( message: "Expected XML Element <{0}> to have a parent element <{1}>, but encountered parent <{2}> instead.".Fmt( node.LocalName, parentElementName, node.ParentNode?.LocalName ?? "(null)" ) );
                    }
                }
                else
                {
                    throw new ArgumentException( message: "Expected an XML Element <{0}>, but encountered {1} <{2}> instead.".Fmt( elementName, node.NodeType, node.LocalName ) );
                }
            }
            else
            {
                throw new ArgumentException( message: "Expected an XML Element <{0}>, but encountered {1} <{2}> instead.".Fmt( elementName, node.NodeType, node.LocalName ) );
            }
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
