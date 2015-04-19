namespace GitTools.Git
{
    using System;
    using Catel;
    using Catel.Logging;
    using LibGit2Sharp;

    public class RepositoryLoader
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static Repository GetRepo(string gitDirectory)
        {
            Argument.IsNotNull(() => gitDirectory);

            try
            {
                var repository = new Repository(gitDirectory);

                var branch = repository.Head;
                if (branch.Tip == null)
                {
                    Log.ErrorAndThrowException<GitToolsException>("No Tip found. Has repo been initialized?");
                }

                return repository;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("LibGit2Sharp.Core.NativeMethods") || ex.Message.Contains("FilePathMarshaler"))
                {
                    Log.ErrorAndThrowException<GitToolsException>("Restart of the process may be required to load an updated version of LibGit2Sharp.");
                }

                throw;
            }
        }
    }
}