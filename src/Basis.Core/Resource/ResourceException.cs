using System;

namespace Basis.Resource
{
    public class ResourceException : BasisException
    {
        public ResourceException(string message) : base(message)
        {
        }

        public ResourceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
