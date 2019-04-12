using System;

namespace Basis.Inversion
{
    public class InversionException : Exception
    {
        public InversionException()
        {
        }

        public InversionException(string message) : base(message)
        {
        }

        public InversionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
