using System;

namespace Basis
{
    public class BasisException : Exception
    {
        public BasisException()
        {
        }

        public BasisException(string message) : base(message)
        {
        }

        public BasisException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
