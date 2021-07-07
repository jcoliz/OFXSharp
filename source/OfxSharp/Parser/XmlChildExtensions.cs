using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace OfxSharp
{
#if XPATH_CHEAT_SHEET

    XPath               | CSS                          | https://www.w3.org/TR/1999/REC-xpath-19991116/ - 2.5 Abbreviated Syntax
    --------------------+------------------------------+-----------------------------------------------------
    `.`                 | `:scope`                     | selects the context node
    `para`              | `:scope > para`              | selects the <para> element children of the context node
    `para[1]`           | `:scope > para:nth-child(1)` | selects the <para> element children of the context node
    `para[last()]`      | `:scope > para:last-child`   | selects the last <para> child of the context node
    `*/para`            |                              | selects all <para> grandchildren of the context node
    `//para`            | `para`                       | selects all the <para> descendants of the document root and thus selects all <para> elements in the same document as the context node
    `/para`             | `:root > para`               | selects the <para> element children of the root node
    `.//para`           | `:scope para`                | selects the <para> element descendants of the context node
    `..`                |                              | selects the parent of the context node

    // Question: Is `./para` valid XPath then?

#endif

    internal static class XmlChildExtensions
    {
        private static readonly Char[] _xpathSyntaxChars = new[]
        {
            '.', '*', '/', '[', ']'
        };

        public static XmlElement RequireSingleElementChild( this XmlElement element, String childElementName )
        {
            if( element is null ) throw new ArgumentNullException( nameof( element ) );
            if( String.IsNullOrWhiteSpace( childElementName ) ) throw new ArgumentException( message: "Value cannot be null/empty/whitespace.", paramName: nameof(childElementName) );
            if( childElementName.IndexOfAny( _xpathSyntaxChars ) > -1 ) throw new ArgumentException( message: "Value cannot contain XPath syntax characters.", paramName: nameof(childElementName) );

            //

            XmlNode child = element.SelectSingleNode( childElementName );
            if( child is XmlElement childElement && childElement.LocalName == childElementName && childElement.ParentNode == element )
            {
                return childElement;
            }
            else
            {
                throw new OfxException( message: "Couldn't find a child element <{0}> of <{1}>.".Fmt( childElementName, element.LocalName ) );
            }
        }

        public static XmlElement GetSingleElementChildOrNull( this XmlElement element, String childElementName )
        {
            if( element is null ) throw new ArgumentNullException( nameof( element ) );
            if( String.IsNullOrWhiteSpace( childElementName ) ) throw new ArgumentException( message: "Value cannot be null/empty/whitespace.", paramName: nameof(childElementName) );
            if( childElementName.IndexOfAny( _xpathSyntaxChars ) > -1 ) throw new ArgumentException( message: "Value cannot contain XPath syntax characters.", paramName: nameof(childElementName) );

            //

            XmlNode child = element.SelectSingleNode( childElementName );
            if( child is XmlElement childElement )
            {
                if( childElement.LocalName == childElementName )
                {
                    return childElement;
                }
                else
                {
                    throw new OfxException( message: "Expected a single <{0}> element child - or null - but encountered <{1}> instead.".Fmt( childElementName, child.LocalName ) );
                }
            }
            else if( child is null )
            {
                // OK
                return null;
            }
            else
            {
                // Unexpected
                throw new OfxException( message: "Expected a single <{0}> element child - or null - but encountered {1} <{2}> instead.".Fmt( childElementName, child.NodeType, child.LocalName ) );
            }
        }

        public static IEnumerable<XmlElement> GetChildNodes( this XmlElement element, String childElementName )
        {
            if( element is null ) throw new ArgumentNullException( nameof( element ) );
            if( String.IsNullOrWhiteSpace( childElementName ) ) throw new ArgumentException( message: "Value cannot be null/empty/whitespace.", paramName: nameof(childElementName) );
            if( childElementName.IndexOfAny( _xpathSyntaxChars ) > -1 ) throw new ArgumentException( message: "Value cannot contain XPath syntax characters.", paramName: nameof(childElementName) );

            //

            return element.SelectNodes( childElementName ).Cast<XmlElement>();
        }
    }
}
