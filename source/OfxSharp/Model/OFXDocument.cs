using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace OfxSharp
{
    public class OfxDocument
    {
        public static OfxDocument FromXmlElement( XmlElement ofxElement )
        {
            _ = ofxElement.AssertIsElement( "OFX" );

            XmlElement signOnMessageResponse = ofxElement.RequireSingleElementChild("SIGNONMSGSRSV1");
            XmlElement bankMessageResponse   = null;
            foreach( KeyValuePair<string, ResponseSet> msgsrsv in new MessageResponseSets().knownMessageResponseSets() )
            {
                bankMessageResponse = ofxElement.GetSingleElementChildOrNull( msgsrsv.Key );
                if( bankMessageResponse != null ) break;
            }
            if (bankMessageResponse == null )
                throw new InvalidOperationException( "This file has an uknown message response. Can not continue." );
            //XmlElement bankMessageResponse   = ofxElement.RequireSingleElementChild("BANKMSGSRSV1");

            return new OfxDocument(
                signOn    : SignOnResponse.FromXmlElement( signOnMessageResponse ),
                statements: GetStatements( bankMessageResponse )
            );
        }

        public static IEnumerable<OfxStatementResponse> GetStatements( XmlElement bankMessageResponse )
        {
            var stmntElement = new MessageResponseSets().knownMessageResponseSets();
            if (stmntElement.ContainsKey(bankMessageResponse.Name))
                _ = bankMessageResponse.AssertIsElement( bankMessageResponse.Name );

            foreach( XmlElement stmTrnResponse in bankMessageResponse.GetChildNodes(stmntElement[bankMessageResponse.Name].StatementTransaction) )
            {
                XmlElement stmtTrnRs = stmTrnResponse.AssertIsElement(stmntElement[bankMessageResponse.Name].StatementTransaction);

                yield return OfxStatementResponse.FromSTMTTRNRS( stmtTrnRs );
            }
        }

        public OfxDocument( SignOnResponse signOn, IEnumerable<OfxStatementResponse> statements )
        {
            this.SignOn = signOn?? throw new ArgumentNullException( nameof( signOn ) );

            this.Statements.AddRange( statements );
        }

        /// <summary>SIGNONMSGSRSV1. Required. Cannot be null.</summary>
        public SignOnResponse SignOn { get; set; }

        /// <summary>BANKMSGSRSV1/STMTTRNRS</summary>
        public List<OfxStatementResponse> Statements { get; } = new List<OfxStatementResponse>();

        //

        public Boolean HasSingleStatement( out SingleStatementOfxDocument doc )
        {
            if( this.Statements.Count == 1 )
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
