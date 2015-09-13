namespace GitTools
{
    public class RepositoryContext : DisposableWithChangeNotifications, IRepositoryContext
    {
        private string _url;
        private string _branch;
        private string _directory;

        public RepositoryContext()
        {
            Authentication = new AuthenticationContext();
        }

        public string Directory
        {
            get { return _directory; }
            set
            {
                if (value == _directory)
                {
                    return;
                }

                _directory = value;
                RaisePropertyChanged("Directory");
            }
        }

        public string Branch
        {
            get { return _branch; }
            set
            {
                if (value == _branch)
                {
                    return;
                }

                _branch = value;
                RaisePropertyChanged("Branch");
            }
        }

        public string Url
        {
            get { return _url; }
            set
            {
                if (value == _url)
                {
                    return;
                }

                _url = value;
                RaisePropertyChanged("Url");
            }
        }

        public IAuthenticationContext Authentication { get; private set; }

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            var authentication = Authentication;
            if (authentication != null)
            {
                authentication.Dispose();
            }
        }
    }
}