using System;
using System.Xml;

namespace OfxSharp
{
	public static class OFXHelperMethods
    {
        /// <summary>Converts string representation of AccountInfo to enum <see cref="BankAccountType"/></summary>
        /// <param name="bankAccountType">representation of AccountInfo</param>
        public static BankAccountType GetBankAccountType(this string bankAccountType)
        {
            try
            {
                return (BankAccountType)Enum.Parse(typeof(BankAccountType), bankAccountType, true);
            }
            catch (Exception)
            {
                return BankAccountType.NA;
            }
        }

        /// <summary> Returns value of specified node</summary>
        /// <param name="node">Node to look for specified node</param>
        /// <param name="xpath">XPath for node you want</param>
        /// <returns></returns>
        public static string GetValue(this XmlNode node, string xpath)
        {
			XmlNode tempNode = node.SelectSingleNode(xpath);

            if (tempNode != null && tempNode.FirstChild != null)
			{
                return tempNode.FirstChild.Value;
			}
                
            return string.Empty;
        }
    }
}