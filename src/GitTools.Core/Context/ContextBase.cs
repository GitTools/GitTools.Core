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