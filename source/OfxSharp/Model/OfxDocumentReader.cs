using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

using Sgml;

namespace OfxSharp
{
    public static class OfxDocumentReader
    {
        private enum State
        {
            BeforeOfxHeader,
            InOfxHeader,
            StartOfOfxSgml
        }

        #region Non-async

        /// <summary></summary>
        /// <param name="filePath"></param>
        /// <param name="cultureOrNullForAutodetect">TODO: Use the &quot;LANGUAGE&quot; element to detect numeric formatting.</param>
        /// <returns></returns>
        public static OfxDocument FromSgmlFile( String filePath )//, CultureInfo cultureOrNullForAutodetect )
        {
            using( FileStream fs = new FileStream( filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, FileOptions.SequentialScan ) )
            {
                return FromSgmlFile( fs );
            }
        }

        public static OfxDocument FromSgmlFile( Stream stream )
        {
            using( StreamReader rdr = new StreamReader( stream ) )
            {
                return FromSgmlFile( reader: rdr );
            }
        }

        public static OfxDocument FromSgmlFile( TextReader reader )
        {
            if( reader is null ) throw new ArgumentNullException( nameof( reader ) );

            // Read the header:
            IReadOnlyDictionary<String,String> header = ReadOfxFileHeaderUntilStartOfSgml( reader );

            XmlDocument doc = ConvertSgmlToXml( reader );

            #if DEBUG
            String xmlDocString = doc.ToXmlString();
            #endif

            return OfxDocument.FromXmlElement( doc.DocumentElement );
        }

        private static IReadOnlyDictionary<String,String> ReadOfxFileHeaderUntilStartOfSgml( TextReader reader )
        {
            Dictionary<String,String> sgmlHeaderValues = new Dictionary<String,String>();

            //

            State state = State.BeforeOfxHeader;
            String line;

            while( ( line = reader.ReadLine() ) != null )
            {
                switch( state )
                {
                case State.BeforeOfxHeader:
                    if( line.IsSet() )
                    {
                        state = State.InOfxHeader;
                    }
                    break;

                case State.InOfxHeader:

                    if( line.IsEmpty() )
                    {
                        //state = State.StartOfOfxSgml;
                        return sgmlHeaderValues;
                    }
                    else
                    {
                        String[] parts = line.Split(':');
                        String name  = parts[0];
                        String value = parts[1];
                        sgmlHeaderValues.Add( name, value );
                    }

                    break;

                case State.StartOfOfxSgml:
                    throw new InvalidOperationException( "This state should never be entered." );
                }
            }

            throw new InvalidOperationException( "Reached end of OFX file without encountering end of OFX header." );
        }

        private static XmlDocument ConvertSgmlToXml( TextReader reader )
        {
            // Convert SGML to XML:
            try
            {
                SgmlDtd ofxSgmlDtd = ReadOfxSgmlDtd();

                SgmlReader sgmlReader = new SgmlReader();
                sgmlReader.WhitespaceHandling = WhitespaceHandling.None; // hmm, this doesn't work.
                // Hopefully the next update to `` will include my changes to support trimmed output: https://github.com/lovettchris/SgmlReader/issues/15
                sgmlReader.InputStream        = reader;
                sgmlReader.DocType            = "OFX"; // <-- This causes DTD magic to happen. I don't know where it gets the DTD from though.
                sgmlReader.Dtd                = ofxSgmlDtd;

                // https://stackoverflow.com/questions/1346995/how-to-create-a-xmldocument-using-xmlwriter-in-net
                XmlDocument doc = new XmlDocument(); 
                using( XmlWriter xmlWriter = doc.CreateNavigator().AppendChild() ) 
                { 
                    while( !sgmlReader.EOF )
                    {
                        xmlWriter.WriteNode( sgmlReader, defattr: true );
                    }
                }
                
                return doc;
            }
            catch( Exception ex )
            {
                throw;
            }
        }

        private static SgmlDtd ReadOfxSgmlDtd()
        {
            // Need to strip the DTD envelope, apparently...:  https://github.com/lovettchris/SgmlReader/issues/13#issuecomment-862666405
            String dtdText;
            using( FileStream fs = new FileStream( @"C:\git\forks\OFXSharp\source\Specifications\OFX1.6\ofx160.trimmed.dtd", FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096 ) )
            using( StreamReader rdr = new StreamReader( fs ) )
            {
                dtdText = rdr.ReadToEnd();
            }

            // Example cribbed from https://github.com/lovettchris/SgmlReader/blob/363decf083dd847d18c4c765cf0b87598ca491a0/SgmlTests/Tests-Logic.cs
            
            using( StringReader dtdReader = new StringReader( dtdText ) )
            {
                SgmlDtd dtd = SgmlDtd.Parse(
                    baseUri : null,
                    name    : "OFX",
                    input   : dtdReader,
                    subset  : "",
                    nt      : new NameTable(),
                    resolver: new DesktopEntityResolver()
                );

                return dtd;
            }
        }

        #endregion

        #region Async

        public static async Task<OfxDocument> FromSgmlFileAsync( Stream stream )
        {
            using( StreamReader rdr = new StreamReader( stream ) )
            {
                return await FromSgmlFileAsync( reader: rdr ).ConfigureAwait(false);
            }
        }

        public static async Task<OfxDocument> FromSgmlFileAsync( TextReader reader )
        {
            if( reader is null ) throw new ArgumentNullException( nameof( reader ) );

            // HACK: Honestly, it's easier just to buffer it all first:

            String text = await reader.ReadToEndAsync().ConfigureAwait(false);

            using( StringReader sr = new StringReader( text ) )
            {
                return FromSgmlFile( sr );
            }
        }

        #endregion
    }
}
