using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public static OfxDocument ReadFile ( string filePath )
        {
            using( FileStream fs = new FileStream( filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, FileOptions.SequentialScan ) )
            {
                return ReadFile( fs );
            }

        }
        public static OfxDocument ReadFile ( Stream stream )
        {
            using( StreamReader reader = new StreamReader(stream) )
            {
                return ReadFile( reader: reader );
            }

        }
        public static OfxDocument ReadFile( StreamReader reader )
        {
            if( reader is null ) throw new ArgumentNullException( nameof( reader ) );
                
            var ofxheader = new char[100]; // 100 character span
            if (reader.Read(ofxheader, 0, 100) > 0 )
            {
                // This will restart streamreader from the beginning of the file.
                reader.DiscardBufferedData();
                reader.BaseStream.Seek( 0, SeekOrigin.Begin );

                string result = new string(ofxheader);
                    
                // If we don't find an OFX 1.0 header, then get XM directly
                if (result.IndexOf("OFXHEADER:100") == -1 )
                {
                    return FromXMLFile( reader: reader );
                } else
                {
                    return FromSgmlFile( reader: reader );
                }

            }
            // We should never get here unless something is wrong with the file.
            return null;

        }

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

        public static OfxDocument FromXMLFile ( String filePath )
        {
            using( FileStream fs = new FileStream( filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, FileOptions.SequentialScan ) )
            {
                return FromXMLFile( fs );
            }

        }

        public static OfxDocument FromXMLFile( Stream stream )
        {
            using( StreamReader rdr = new StreamReader( stream ) )
            {
                return FromXMLFile( reader: rdr );
            }
        }
        public static OfxDocument FromXMLFile( TextReader reader )
        {
            if( reader is null ) throw new ArgumentNullException( nameof( reader ) );

            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

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
            XmlDocument result = new XmlDocument();
            result.Load
            (
                new SgmlReader()
                {
                    WhitespaceHandling = WhitespaceHandling.None,
                    InputStream = reader,
                    DocType = "OFX",
                    Dtd = ReadOfxSgmlDtd()
                }
           );
                
           return result;
        }

        private static Stream OpenEmbeddedResource( string filename )
        {
            var assy = Assembly.GetExecutingAssembly();
            var names = assy.GetManifestResourceNames();

            var matching = names.Where(x => x.Contains(filename));
            if ( !matching.Any() )
                throw new FileNotFoundException( $"{filename} not found in assembly" );
            if ( matching.Skip(1).Any() )
                throw new FileNotFoundException( $"{filename} is ambiguous. Multiple found in assembly" );

            var name = matching.First();
            var stream = assy.GetManifestResourceStream(name);
            if (null == stream)
                throw new FileNotFoundException( $"{filename} not found in assembly" );

            return stream;
        }

        private static SgmlDtd ReadOfxSgmlDtd()
        {
            // Need to strip the DTD envelope, apparently...:  https://github.com/lovettchris/SgmlReader/issues/13#issuecomment-862666405
            String dtdText;
            using( Stream fs = OpenEmbeddedResource("ofx160.trimmed.dtd") )
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
