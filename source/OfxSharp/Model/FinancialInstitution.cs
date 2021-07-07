using System;
using System.Xml;

namespace OfxSharp
{
    /// <summary>FI</summary>
    public class FinancialInstitution
    {
        public static FinancialInstitution FromXmlElementOrNull( XmlElement fiOrNull )
        {
            if( fiOrNull is null )
            {
                return null;
            }
            else
            {
                XmlElement fi = fiOrNull.AssertIsElement( "FI", parentElementName: "SONRS" );

                String orgName = fi.RequireSingleElementChild("ORG").RequireSingleTextChildNode();
                String fIdText = fi.RequireSingleElementChild("FID").RequireSingleTextChildNode();
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
