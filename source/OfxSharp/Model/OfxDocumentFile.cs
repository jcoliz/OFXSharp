using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace OfxSharp
{
    /// <summary>Combines <see cref="OfxDocument"/> with <see cref="FileInfo"/>. Implicitly convertible to <see cref="OfxSharp.OfxDocument"/>.</summary>
    public class OfxDocumentFile
    {
        /// <summary>Returns a cached instance of <c>Encoding.GetEncoding( codepage: 1252 )</c>.</summary>
        /// https://stackoverflow.com/questions/37870084/net-core-doesnt-know-about-windows-1252-how-to-fix
        public static Encoding Windows1252 { get; } = CodePagesEncodingProvider.Instance.GetEncoding( 1252 );

        /// <summary></summary>
        /// <param name="ofxFileInfo"></param>
        /// <param name="encoding">Can be null. When null this defaults to <see cref="Windows1252"/>.</param>
        /// <param name="cancellationToken">Not currently used. Will be used after <see cref="StreamReader"/> supports it. See https://github.com/dotnet/runtime/issues/20824</param>
        public static async Task<OfxDocumentFile> ReadFileAsync( FileInfo ofxFileInfo, Encoding encoding = null, CancellationToken cancellationToken = default )
        {
            if( ofxFileInfo is null ) throw new ArgumentNullException( nameof( ofxFileInfo ) );
            
            if( encoding is null )
            {
                encoding = Windows1252;
            }

            // HACK: Simpler for now...
            String fileText;
            using( FileStream fs = new FileStream( path: ofxFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 1 * 1024 * 1024, useAsync: true ) )
            using( StreamReader rdr = new StreamReader( fs, encoding ) )
            {
                fileText = await rdr.ReadToEndAsync().ConfigureAwait(false); // Grumble: https://github.com/dotnet/runtime/issues/20824 // "Add CancellationToken to StreamReader.Read* methods"
            }

            using( StringReader fileTextStringReader = new StringReader( fileText ) )
            {
                OfxDocument ofxDoc = OfxDocumentReader.FromSgmlFile( fileTextStringReader );
                return new OfxDocumentFile( ofxDoc, ofxFileInfo );
            }
        }

        public static OfxDocumentFile ReadFile( FileInfo ofxFileInfo, Encoding encoding = null )
        {
            if( ofxFileInfo is null ) throw new ArgumentNullException( nameof( ofxFileInfo ) );
            
            if( encoding is null )
            {
                encoding = Windows1252;
            }

            // HACK: Simpler for now...
            String fileText = System.IO.File.ReadAllText( ofxFileInfo.FullName, encoding );

            using( StringReader fileTextStringReader = new StringReader( fileText ) )
            {
                OfxDocument ofxDoc = OfxDocumentReader.FromSgmlFile( fileTextStringReader );
                return new OfxDocumentFile( ofxDoc, ofxFileInfo );
            }
        }

        public static implicit operator OfxDocument( OfxDocumentFile self )
        {
            return self.OfxDocument;
        }

        public OfxDocumentFile( OfxDocument ofxDocument, FileInfo file )
        {
            this.OfxDocument = ofxDocument ?? throw new ArgumentNullException( nameof( ofxDocument ) );
            this.File        = file        ?? throw new ArgumentNullException( nameof( file ) );
        }

        public OfxDocument OfxDocument { get; }
        public FileInfo    File        { get; }

        // Not for now...
        /*
        public Byte[] ComputeHash()
        {

        }
        */
    }
}
