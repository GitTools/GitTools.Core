namespace GitTools.Git
{
    public interface IRepositoryPreparer
    {
        bool IsPreparationRequired(IRepositoryContext context);
        string Prepare(IRepositoryContext context, TemporaryFilesContext temporaryFilesContext);
    }
}