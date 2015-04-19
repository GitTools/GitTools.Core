namespace GitTools
{
    public interface IContext
    {
        bool IsHelp { get; set; }
        IRepositoryContext Repository { get; set; }
    }
}