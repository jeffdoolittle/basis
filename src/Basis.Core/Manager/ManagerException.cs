using System;

namespace Basis.Manager
{
    public class ManagerException : BasisException
    {
        public ManagerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}