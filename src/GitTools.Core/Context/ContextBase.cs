// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextBase.cs" company="GitTools">
//   Copyright (c) 2014 - 2015 GitTools. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitTools
{
    public abstract class ContextBase : IContext
    {
        public ContextBase()
        {
            Repository = new RepositoryContext();
        }

        public bool IsHelp { get; set; }

        public IRepositoryContext Repository { get; set; }
    }
}