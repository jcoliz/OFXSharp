using System;
using System.Xml;

namespace OfxSharp
{
    public class SignOn
    {
        public SignOn(XmlNode node)
        {
            this.StatusCode = Convert.ToInt32(node.GetValue("//CODE"));
            this.StatusSeverity = node.GetValue("//SEVERITY");
            this.DtServer = node.GetValue("//DTSERVER").MaybeParseOfxDateTime();
            this.Language = node.GetValue("//LANGUAGE");
            this.IntuBid = node.GetValue("//INTU.BID");
        }

        public string StatusSeverity { get; set; }

        public DateTimeOffset? DtServer { get; set; }

        public int StatusCode { get; set; }

        public string Language { get; set; }

        public string IntuBid { get; set; }
    }
}
