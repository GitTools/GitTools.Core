// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryPreparer.cs" company="GitTools">
//   Copyright (c) 2014 - 2015 GitTools. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitTools.Git
{
    public interface IRepositoryPreparer
    {
        bool IsPreparationRequired(IRepositoryContext context);
        string Prepare(IRepositoryContext context, TemporaryFilesContext temporaryFilesContext);
    }
}