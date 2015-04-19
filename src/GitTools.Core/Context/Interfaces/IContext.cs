// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContext.cs" company="GitTools">
//   Copyright (c) 2014 - 2015 GitTools. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitTools
{
    public interface IContext
    {
        bool IsHelp { get; set; }

        IRepositoryContext Repository { get; set; }
    }
}