using System;
using System.Diagnostics;

namespace Basis
{
    [Serializable]
    public class BasisException : Exception
    {
        public BasisException(string message) : base(message)
        {
        }

        public BasisException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public override string StackTrace => this.GetStackTraceWithoutHiddenMethods();

        public override string ToString()
        {
            return this.GetToStringWithoutHiddenMethods();
        }
    }
}
