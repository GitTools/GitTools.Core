namespace GitTools
{
    public abstract class ContextBase : Disposable, IContext
    {
        public ContextBase()
        {
            Repository = new RepositoryContext();
        }

        public bool IsHelp { get; set; }

        public IRepositoryContext Repository { get; set; }

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            var repository = Repository;
            if (repository != null)
            {
                repository.Dispose();
            }
        }
    }
}