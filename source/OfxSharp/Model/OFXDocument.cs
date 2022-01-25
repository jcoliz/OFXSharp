using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace OfxSharp
{
    public class OfxDocument
    {
        public static OfxDocument FromXmlElement( XmlElement ofxElement )
        {
            _ = ofxElement.AssertIsElement( "OFX" );

            XmlElement signOnMessageResponse = ofxElement.RequireSingleElementChild("SIGNONMSGSRSV1");
            XmlElement bankMessageResponse   = ofxElement.GetSingleElementChildOrNull("BANKMSGSRSV1");

            return new OfxDocument(
                signOn    : SignOnResponse.FromXmlElement( signOnMessageResponse ),
                statements: GetStatements( bankMessageResponse )
            );
        }

        public static IEnumerable<OfxStatementResponse> GetStatements( XmlElement bankMessageResponse )
        {
            if( bankMessageResponse is null )
                return Enumerable.Empty<OfxStatementResponse>();

            return bankMessageResponse
                .AssertIsElement( "BANKMSGSRSV1" )
                .GetChildNodes( "STMTTRNRS" )
                .Select
                (
                    n => OfxStatementResponse.FromSTMTTRNRS( n.AssertIsElement( "STMTTRNRS" ) )
                );
        }

        public OfxDocument( SignOnResponse signOn, IEnumerable<OfxStatementResponse> statements )
        {
            this.SignOn = signOn ?? throw new ArgumentNullException( nameof( signOn ) );

            this.Statements = new List<OfxStatementResponse>( statements );
        }

        /// <summary>SIGNONMSGSRSV1. Required. Cannot be null.</summary>
        public SignOnResponse SignOn { get; set; }

        /// <summary>BANKMSGSRSV1/STMTTRNRS</summary>
        public IEnumerable<OfxStatementResponse> Statements { get; } = Enumerable.Empty<OfxStatementResponse>();

        //

        public Boolean HasSingleStatement( out SingleStatementOfxDocument doc )
        {
            if( this.Statements.Count() == 1 )
            {
                doc = new SingleStatementOfxDocument( this );
                return true;
            }
            else
            {
                doc = default;
                return false;
            }
        }
    }
}
