namespace GitTools
{
    using System;

    public interface IContext : IDisposable
    {
        bool IsHelp { get; set; }
        IRepositoryContext Repository { get; set; }
    }
}