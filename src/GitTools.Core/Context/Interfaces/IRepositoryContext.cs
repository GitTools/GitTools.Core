// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContext.cs" company="GitTools">
//   Copyright (c) 2014 - 2015 GitTools. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitTools
{
    public interface IRepositoryContext : IAuthenticationContext
    {
        string Directory { get; set; }

        string Branch { get; set; }

        string Url { get; set; }
    }
}