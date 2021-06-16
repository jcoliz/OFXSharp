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
                if( selectedNode.FirstChild is XmlNode firstChild && firstChild.NodeType == XmlNodeType.Text )
                {
                    return firstChild.Value;
                }
            }
                
            return string.Empty;
        }

        public static Boolean TryGetValue(this XmlNode node, string xpath, out String value)
        {
            // TODO: Be more rigorous about XmlNode types. `FirstChild` is meant to be the #text node, not an element node, right?
            // ...in which case, why not use `XmlNode.InnerText`?

			if( node.SelectSingleNode( xpath ) is XmlNode selectedNode )
            {
                if( selectedNode.FirstChild is XmlNode firstChild && firstChild.NodeType == XmlNodeType.Text )
                {
                    value = firstChild.Value;
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
                        return value;
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

        public static XmlElement AssertIsElement( this XmlNode node, String elementName )
        {
            if( node is null ) throw new ArgumentNullException( nameof( node ) );
            
            if( node.NodeType != XmlNodeType.Element || node.LocalName != elementName )
            {
                throw new ArgumentException( message: "Expected an XML Element <{0}>, but encountered {1} <{2}> instead.".Fmt( node.NodeType, node.Name ) );
            }

            return (XmlElement)node;
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

        public static XmlElement RequireSingleElementChild(this XmlElement e, String childElementName)
        {
            // TODO: This method does not assert singleness, only that at least 1 exists. Hmm.

            String relativeXPath = "./" + childElementName;

            // Don't use `"childElementName"` as the XPath, that causes *all* elements in the document to be searched, even those that aren't descendants of `e`.
            // Yes, that's silly. See here: https://stackoverflow.com/questions/7692964/what-is-the-correct-use-of-xmlnode-selectsinglenodestring-xpath-in-c

            XmlNode child = e.SelectSingleNode( relativeXPath );
            if( child is XmlElement childElement && childElement.Name == childElementName && childElement.ParentNode == e )
            {
                return childElement;
            }
            else
            {
                throw new OfxException( message: "Couldn't find a child element <{0}> of <{1}>.".Fmt( childElementName, e.Name ) );
            }
        }

        public static XmlElement GetSingleElementChildOrNull(this XmlElement e, String childElementName)
        {
            String relativeXPath = "./" + childElementName;

            XmlNode child = e.SelectSingleNode( relativeXPath );
            if( child is XmlElement childElement && childElement.Name == childElementName && childElement.ParentNode == e )
            {
                return childElement;
            }
            else
            {
                return null;
            }
        }
	}
}
