namespace GitTools
{
    using Logging;

    public static class IRepositoryContextExtensions
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        public static bool Validate(this IRepositoryContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Url))
            {
                Log.Error("Repository.Url is required");
                return false;
            }

            if (string.IsNullOrWhiteSpace(context.Branch))
            {
                Log.Error("Repository.Branch is required");
                return false;
            }

            return true;
        }
    }
}