using ReadSharp.Ports.Sgml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace OfxSharp
{
    public static class OfxConsts
    {
        public const String SignOn = @"OFX/SIGNONMSGSRSV1/SONRS";

        public const String CCAccount = @"OFX/CREDITCARDMSGSRSV1/CCSTMTTRNRS/";

        public const String BankAccount = @"OFX/BANKMSGSRSV1/STMTTRNRS/";
    }

    public class OfxDocumentParser
    {
        public OfxDocument Import(Stream stream)
        {
            using ( StreamReader reader = new StreamReader(stream, Encoding.Default))
            {
                return this.Import(reader.ReadToEnd());
            }
        }

        public OfxDocument Import(string ofx)
        {
            return this.ParseOfxDocument(ofx);
        }

        private OfxDocument ParseOfxDocument(string ofxString)
        {
            //If OFX file in SGML format, convert to XML
            if (!this.IsXmlVersion(ofxString))
            {
                ofxString = this.SGMLToXML(ofxString);
            }

            return this.Parse(ofxString);
        }

        private OfxDocument Parse(string ofxString)
        {
            OfxDocument ofx = new OfxDocument { AccType = this.GetAccountType(ofxString) };

            //Load into xml document
            XmlDocument doc = new XmlDocument();
            doc.Load(new StringReader(ofxString));

            XmlNode currencyNode = doc.SelectSingleNode(this.GetXPath(ofx.AccType, OFXSection.CURRENCY));

            if (currencyNode != null)
            {
                ofx.Currency = currencyNode.FirstChild.RequireSingleTextChildNode();
            }
            else
            {
                throw new OfxParseException("Currency not found");
            }

            //Get sign on node from OFX file
            XmlNode signOnNode = doc.SelectSingleNode(OfxConsts.SignOn);

            //If exists, populate signon obj, else throw parse error
            if (signOnNode != null)
            {
                ofx.SignOn = new SignOn(signOnNode);
            }
            else
            {
                throw new OfxParseException("Sign On information not found");
            }

            //Get Account information for ofx doc
            XmlNode accountNode = doc.SelectSingleNode(this.GetXPath(ofx.AccType, OFXSection.ACCOUNTINFO));

            //If account info present, populate account object
            if (accountNode != null)
            {
                ofx.Account = new Account(accountNode, ofx.AccType);
            }
            else
            {
                throw new OfxParseException("Account information not found");
            }

            //Get list of transactions
            this.ImportTransations(ofx, doc);

            //Get balance info from ofx doc
            XmlNode ledgerNode = doc.SelectSingleNode(this.GetXPath(ofx.AccType, OFXSection.BALANCE) + "/LEDGERBAL");
            XmlNode avaliableNode = doc.SelectSingleNode(this.GetXPath(ofx.AccType, OFXSection.BALANCE) + "/AVAILBAL");

            //If balance info present, populate balance object
            // ***** OFX files from my bank don't have the 'avaliableNode' node, so i manage a 'null' situation
            if (ledgerNode != null) // && avaliableNode != null
            {
                ofx.Balance = new Balance(ledgerNode, avaliableNode);
            }
            else
            {
                throw new OfxParseException("Balance information not found");
            }

            return ofx;
        }

        /// <summary>
        ///     Returns the correct xpath to specified section for given account type
        /// </summary>
        /// <param name="type">Account type</param>
        /// <param name="section">Section of OFX document, e.g. Transaction Section</param>
        /// <exception cref="OfxException">Thrown in account type not supported</exception>
        private string GetXPath(AccountType type, OFXSection section)
        {
            string xpath, accountInfo;

            switch (type)
            {
                case AccountType.BANK:
                    xpath = OfxConsts.BankAccount;
                    accountInfo = "/BANKACCTFROM";
                    break;

                case AccountType.CC:
                    xpath = OfxConsts.CCAccount;
                    accountInfo = "/CCACCTFROM";
                    break;

                default:
                    throw new OfxException("Account Type not supported. Account type " + type);
            }

            switch (section)
            {
                case OFXSection.ACCOUNTINFO:
                    return xpath + accountInfo;

                case OFXSection.BALANCE:
                    return xpath;

                case OFXSection.TRANSACTIONS:
                    return xpath + "/BANKTRANLIST";

                case OFXSection.SIGNON:
                    return OfxConsts.SignOn;

                case OFXSection.CURRENCY:
                    return xpath + "/CURDEF";

                default:
                    throw new OfxException("Unknown section found when retrieving XPath. Section " + section);
            }
        }

        /// <summary>
        ///     Returns list of all transactions in OFX document
        /// </summary>
        /// <param name="doc">OFX document</param>
        /// <returns>List of transactions found in OFX document</returns>
        private void ImportTransations(OfxDocument ofxDocument, XmlDocument doc)
        {
            String xpath = this.GetXPath(ofxDocument.AccType, OFXSection.TRANSACTIONS);

            ofxDocument.StatementStartValue = doc.GetValue(xpath + "//DTSTART");
            ofxDocument.StatementStart      = ofxDocument.StatementStartValue.MaybeParseOfxDateTime();

            ofxDocument.StatementEndValue   = doc.GetValue(xpath + "//DTEND");
            ofxDocument.StatementEnd        = ofxDocument.StatementEndValue.MaybeParseOfxDateTime();

            XmlNodeList transactionNodes = doc.SelectNodes(xpath + "//STMTTRN");

            ofxDocument.Transactions = new List<Transaction>();

            foreach (XmlNode node in transactionNodes)
            {
                ofxDocument.Transactions.Add(new Transaction(node, ofxDocument.Currency));
            }
        }

        /// <summary>
        ///     Checks account type of supplied file
        /// </summaryof
        /// <param name="file">OFX file want to check</param>
        /// <returns>Account type for account supplied in ofx file</returns>
        private AccountType GetAccountType(string file)
        {
            if (file.IndexOf("<CREDITCARDMSGSRSV1>") != -1)
            {
                return AccountType.CC;
            }

            if (file.IndexOf("<BANKMSGSRSV1>") != -1)
            {
                return AccountType.BANK;
            }

            throw new OfxException("Unsupported Account Type");
        }

        /// <summary>
        ///     Check if OFX file is in SGML or XML format
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool IsXmlVersion(string file)
        {
            return file.IndexOf("OFXHEADER:100") == -1;
        }

        /// <summary>
        ///     Converts SGML to XML
        /// </summary>
        /// <param name="file">OFX File (SGML Format)</param>
        /// <returns>OFX File in XML format</returns>
        private string SGMLToXML(string file)
        {
            SgmlReader reader = new SgmlReader();

            //Inititialize SGML reader
            reader.InputStream = new StringReader( this.ParseHeader(file));
            reader.DocType = "OFX";

            StringWriter sw = new StringWriter();
            XmlTextWriter xml = new XmlTextWriter(sw);

            //write output of sgml reader to xml text writer
            while (!reader.EOF)
            {
                xml.WriteNode(reader, true);
            }

            //close xml text writer
            xml.Flush();
            xml.Close();

            String[] temp = sw.ToString().TrimStart().Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            return string.Join("", temp);
        }

        /// <summary>
        ///     Checks that the file is supported by checking the header. Removes the header.
        /// </summary>
        /// <param name="file">OFX file</param>
        /// <returns>File, without the header</returns>
        private string ParseHeader(string file)
        {
            //Select header of file and split into array
            //End of header worked out by finding first instance of '<'
            //Array split based of new line & carrige return
            String[] header = file.Substring(0, file.IndexOf('<'))
                .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            //Check that no errors in header
            this.CheckHeader(header);

            //Remove header
            return file.Substring(file.IndexOf('<')).Trim();
        }

        /// <summary>
        ///     Checks that all the elements in the header are supported
        /// </summary>
        /// <param name="header">Header of OFX file in array</param>
        private void CheckHeader(string[] header)
        {
            if (header[0] != "OFXHEADER:100")
            {
                throw new OfxParseException("Incorrect header format");
            }

            if (header[1] != "DATA:OFXSGML")
            {
                throw new OfxParseException("Data type unsupported: " + header[1] + ". OFXSGML required");
            }

            if (header[2] != "VERSION:102")
            {
                throw new OfxParseException("OFX version unsupported. " + header[2]);
            }

            if (header[3] != "SECURITY:NONE")
            {
                throw new OfxParseException("OFX security unsupported");
            }

            if (header[4] != "ENCODING:USASCII")
            {
                throw new OfxParseException("ASCII Format unsupported:" + header[4]);
            }

            if (header[5] != "CHARSET:1252")
            {
                throw new OfxParseException("Charecter set unsupported:" + header[5]);
            }

            if (header[6] != "COMPRESSION:NONE")
            {
                throw new OfxParseException("Compression unsupported");
            }

            if (header[7] != "OLDFILEUID:NONE")
            {
                throw new OfxParseException("OLDFILEUID incorrect");
            }
        }

        #region Nested type: OFXSection

        /// <summary>
        ///     Section of OFX Document
        /// </summary>
        private enum OFXSection
        {
            SIGNON,
            ACCOUNTINFO,
            TRANSACTIONS,
            BALANCE,
            CURRENCY
        }

        #endregion Nested type: OFXSection
    }
}
