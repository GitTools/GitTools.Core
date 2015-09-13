namespace GitTools
{
    using System;

    public class GitToolsException : Exception
    {
        public GitToolsException(string messageFormat, params object[] args)
            : base(string.Format(messageFormat, args))
        {
        }
    }
}