using System;
using System.Runtime.Serialization;

namespace OfxSharp
{
    [Serializable]
    public class OfxException : Exception
    {
        public OfxException()
        {
        }

        public OfxException(string message) : base(message)
        {
        }

        public OfxException(string message, Exception inner) : base(message, inner)
        {
        }

        protected OfxException(SerializationInfo info,StreamingContext context) : base(info, context)
        {
        }
    }
}
