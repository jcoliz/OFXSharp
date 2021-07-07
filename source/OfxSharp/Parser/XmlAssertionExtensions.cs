using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace OfxSharp
{
	internal static class XmlAssertionExtensions
	{
        public static XmlElement AssertIsElementOneOf( this XmlNode node, params String[] elementNames )
        {
            if( node is null ) throw new ArgumentNullException( nameof( node ) );

            if( node is XmlElement element && node.NodeType == XmlNodeType.Element )
            {
                if( elementNames.Any( name => name.Equals( element.LocalName ) ) )
                {
                    return element;
                }
                else
                {
                    throw new ArgumentException( message: "Expected an XML Element one-of <{0}>, but encountered {1} <{2}> instead.".Fmt( String.Join( " ", elementNames ), node.NodeType, node.LocalName ) );
                }
            }
            else
            {
                throw new ArgumentException( message: "Expected an XML Element one-of <{0}>, but encountered {1} <{2}> instead.".Fmt( String.Join( " ", elementNames ), node.NodeType, node.LocalName ) );
            }
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
    }
}
