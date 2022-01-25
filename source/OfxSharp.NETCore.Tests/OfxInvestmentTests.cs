using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace OfxSharp.NETCore.Tests
{
    public class OfxInvestmentTests
    {
        [Test]
        public void LoadInvestment()
        {
            OfxDocument ofx = OfxDocumentReader.FromSgmlFile( filePath: "Files/investment.ofx" );

            Assert.IsNotNull( ofx );
        }
    }
}
