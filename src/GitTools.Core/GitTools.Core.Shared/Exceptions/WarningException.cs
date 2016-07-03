namespace GitTools
{
    using System;
#if !NETSTANDARD
    using System.Runtime.Serialization;
#endif

#if !NETSTANDARD
    [Serializable]
#endif
    public class WarningException : Exception
    {
        public WarningException(string message)
            : base(message)
        {
        }

#if !NETSTANDARD
        protected WarningException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}