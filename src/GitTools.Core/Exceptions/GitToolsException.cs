// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitToolsException.cs" company="GitTools">
//   Copyright (c) 2014 - 2015 GitTools. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


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