using System;
using System.Runtime.Serialization;

namespace OfxSharp
{
    [Serializable]
    public class OfxException : Exception
    {
        public OfxException(string message) : base(message)
        {
        }
    }
}
