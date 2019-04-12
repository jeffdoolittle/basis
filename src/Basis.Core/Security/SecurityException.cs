using System;

namespace Basis.Security
{
    [Serializable]
    public class SecurityException : BasisException
    {
        public SecurityException(string message) : base(message)
        {
        }
    }
}