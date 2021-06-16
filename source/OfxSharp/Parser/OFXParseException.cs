using System;
using System.Runtime.Serialization;

namespace OfxSharp
{
    [Serializable]
    public class OfxParseException : OfxException
    {
        public OfxParseException()
        {
        }

        public OfxParseException(string message) : base(message)
        {
        }

        public OfxParseException(string message, Exception inner) : base(message, inner)
        {
        }

        protected OfxParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
