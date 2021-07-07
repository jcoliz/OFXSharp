using System;
using System.Xml;

namespace OfxSharp
{
    public class FinancialInstitution
    {
        public static FinancialInstitution FromXmlNode( XmlNode fiOrNull )
        {
            if( fiOrNull is null )
            {
                return null;
            }
            else
            {
                XmlElement fi = fiOrNull.AssertIsElement( "FI" );

                String orgName = fi.TryGetValue( "//ORG", out orgName ) ? orgName : null;
                String fIdText = fi.TryGetValue( "//FID", out fIdText ) ? fIdText : null;
                Int32? fId     = fIdText.TryParseInt32();

                return new FinancialInstitution( name: orgName, fId: fId );
            }
        }

        public FinancialInstitution( String name, Int32? fId )
        {
            this.Name = name;
            this.FId  = fId;
        }

        /// <summary>Can be null.<br />OFX/SIGNONMSGSRSV1/SONRS/FI/ORG</summary>
        public String Name { get; }

        /// <summary>Can be null.<br />OFX/SIGNONMSGSRSV1/SONRS/FI/FID</summary>
        public Int32? FId  { get; }
    }
}
