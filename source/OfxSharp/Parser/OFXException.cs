using System;
using System.Runtime.Serialization;

namespace OfxSharp
{
    [Serializable]
    public class OFXException : Exception
    {
        public OFXException()
        {
        }

        public OFXException(string message) : base(message)
        {
        }

        public OFXException(string message, Exception inner) : base(message, inner)
        {
        }

        protected OFXException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}