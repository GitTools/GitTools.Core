namespace GitTools.Git
{
    using Logging;

    public static class AuthenticationInfoExtensions
    {
        private static readonly ILog Log = LogProvider.GetLogger(typeof(AuthenticationInfoExtensions));

        public static bool IsEmpty(this AuthenticationInfo authenticationInfo)
        {
            if (authenticationInfo == null)
            {
                return true;
            }

            if (IsUsernameAndPasswordAuthentication(authenticationInfo))
            {
                return false;
            }

            if (IsTokenAuthentication(authenticationInfo))
            {
                return false;
            }

            return true;
        }

        public static bool IsUsernameAndPasswordAuthentication(this AuthenticationInfo authenticationInfo)
        {
            if (authenticationInfo == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(authenticationInfo.Username))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(authenticationInfo.Password))
            {
                return false;
            }

            return true;
        }

        public static bool IsTokenAuthentication(this AuthenticationInfo authenticationInfo)
        {
            if (authenticationInfo == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(authenticationInfo.Token))
            {
                return false;
            }

            return true;
        }
    }
}
