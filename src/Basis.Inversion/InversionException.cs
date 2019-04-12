using System;

namespace Basis.Inversion
{
    [Serializable]
    public class InversionException : BasisException
    {
        public InversionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
