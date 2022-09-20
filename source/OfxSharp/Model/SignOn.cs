using System;
using System.Xml;

namespace OfxSharp
{
    public class SignOnResponse
    {
        public static SignOnResponse FromXmlElement( XmlElement signonMsgsrsV1 )
        {
            _ = signonMsgsrsV1.AssertIsElement( "SIGNONMSGSRSV1" );

            XmlElement signOnResponse = signonMsgsrsV1.RequireSingleElementChild( "SONRS"  );
            XmlElement status         = signOnResponse.RequireSingleElementChild( "STATUS" );

            return new SignOnResponse(
                statusCode    : status        .RequireSingleElementChildText( "CODE"     ).RequireParseInt32(),
                statusSeverity: status        .RequireSingleElementChildText( "SEVERITY" ),
                dtServer      : signOnResponse.RequireSingleElementChildText( "DTSERVER" ).RequireOptionalParseOfxDateTime(),
                language      : signOnResponse.RequireSingleElementChildText( "LANGUAGE" ),
                intuBid       : signOnResponse.GetSingleElementChildOrNull( "INTU.BID", allowDotsInElementName: true )?.RequireSingleTextChildNode() ?? null,
                intuUserid    : signOnResponse.GetSingleElementChildOrNull( "INTU.USERID", allowDotsInElementName: true )?.RequireSingleTextChildNode() ?? null,
                institution   : FinancialInstitution.FromXmlElementOrNull( signOnResponse.GetSingleElementChildOrNull("FI") )
            );
        }

        public SignOnResponse(
            Int32                statusCode,
            String               statusSeverity,
            DateTimeOffset?      dtServer,
            String               language,
            String               intuBid     = null,
            String               intuUserid  = null,
            FinancialInstitution institution = null
        )
        {
            this.StatusCode     = statusCode;
            this.StatusSeverity = statusSeverity;
            this.DTServer       = dtServer;
            this.Language       = language;
            this.IntuBid        = intuBid;
            this.IntuUserid     = intuUserid;
            this.Institution    = institution;
        }

        #region OFX 1.6 Required members

        /// <summary>Required.<br />OFX/SIGNONMSGSRSV1/SONRS/STATUS/CODE</summary>
        public int StatusCode { get; }

        /// <summary>Required.<br />OFX/SIGNONMSGSRSV1/SONRS/STATUS/SEVERITY</summary>
        public string StatusSeverity { get; }

        /// <summary>Required. All-zero (i.e. null) values accepted)<br />OFX/SIGNONMSGSRSV1/SONRS/DTSERVER</summary>
        public DateTimeOffset? DTServer { get; }

        /// <summary>Required.<br />OFX/SIGNONMSGSRSV1/SONRS/LANGUAGE</summary>
        public string Language { get; }

        #endregion

        #region OFX 1.6 Optional members and extensions

        /// <summary>Can be null.<br />OFX/SIGNONMSGSRSV1/SONRS/FI</summary>
        public FinancialInstitution Institution { get; }

        /// <summary>Intuit BankId (proprietary to Quicken/Quickbooks).<br />Can be null.<br />OFX/SIGNONMSGSRSV1/SONRS/INTU.BID</summary>
        public string IntuBid { get; }

        /// <summary>Intuit UserId (proprietary to Quicken/Quickbooks).<br />Can be null.<br />OFX/SIGNONMSGSRSV1/SONRS/INTU.USERID</summary>
        public string IntuUserid { get; }

        #endregion
    }
}
