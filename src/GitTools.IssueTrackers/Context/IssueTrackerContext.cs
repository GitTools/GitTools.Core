namespace GitTools.IssueTrackers
{
    public class IssueTrackerContext : DisposableWithChangeNotifications, IIssueTrackerContext
    {
        private string _server;
        private string _projectId;
        private string _diffUrlFormat;

        public IssueTrackerContext()
        {
            Authentication = new AuthenticationContext();
        }

        public string Server
        {
            get { return _server; }
            set
            {
                if (value == _server)
                {
                    return;
                }
                _server = value;
                RaisePropertyChanged();
            }
        }

        public string DiffUrlFormat
        {
            get { return _diffUrlFormat; }
            set
            {
                if (value == _diffUrlFormat)
                {
                    return;
                }
                _diffUrlFormat = value;
                RaisePropertyChanged();
            }
        }

        public string ProjectId
        {
            get { return _projectId; }
            set
            {
                if (value == _projectId)
                {
                    return;
                }
                _projectId = value;
                RaisePropertyChanged();
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