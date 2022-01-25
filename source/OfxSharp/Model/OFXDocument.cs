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

            var result = new OfxDocument(
                signOn    : SignOnResponse.FromXmlElement( signOnMessageResponse ),
                statements: GetStatements( bankMessageResponse )
            );

            XmlElement invMessageResponse     = ofxElement.GetSingleElementChildOrNull("INVSTMTMSGSRSV1");
            if ( ! ( invMessageResponse is null ))
                result.InvestementStatements = GetInvestmentStatements( invMessageResponse );

            return result;
        }

        public static IEnumerable<OfxStatementResponse> GetStatements( XmlElement bankMessageResponse )
            => bankMessageResponse?
                .AssertIsElement( "BANKMSGSRSV1" )
                .GetChildNodes( "STMTTRNRS" )
                .Select
                (
                    n => OfxStatementResponse.FromSTMTTRNRS( n.AssertIsElement( "STMTTRNRS" ) )
                )
                ?? Enumerable.Empty<OfxStatementResponse>();

        private static IEnumerable<OfxInvestmentStatementResponse> GetInvestmentStatements( XmlElement invMessageResponse )
        {
            return Enumerable.Empty<OfxInvestmentStatementResponse>();
        }

        public OfxDocument( SignOnResponse signOn, IEnumerable<OfxStatementResponse> statements )
        {
            this.SignOn = signOn ?? throw new ArgumentNullException( nameof( signOn ) );

            this.Statements = new List<OfxStatementResponse>( statements );

            this.InvestementStatements = Enumerable.Empty<OfxInvestmentStatementResponse>();
        }

        /// <summary>SIGNONMSGSRSV1. Required. Cannot be null.</summary>
        public SignOnResponse SignOn { get; private set; }

        /// <summary>BANKMSGSRSV1/STMTTRNRS</summary>
        public IEnumerable<OfxStatementResponse> Statements { get; private set; }

        public IEnumerable<OfxInvestmentStatementResponse> InvestementStatements { get; private set; }


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
