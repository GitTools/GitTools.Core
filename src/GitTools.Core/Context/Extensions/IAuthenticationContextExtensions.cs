namespace GitTools
{
    public static class IAuthenticationContextExtensions
    {
        public static bool IsEmpty(this IAuthenticationContext authenticationContext)
        {
            if (authenticationContext == null)
            {
                return true;
            }

            if (IsUsernameAndPasswordAuthentication(authenticationContext))
            {
                return false;
            }

            if (IsTokenAuthentication(authenticationContext))
            {
                return false;
            }

            return true;
        }

        public static bool IsUsernameAndPasswordAuthentication(this IAuthenticationContext authenticationContext)
        {
            if (authenticationContext == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(authenticationContext.Username))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(authenticationContext.Password))
            {
                return false;
            }

            return true;
        }

        public static bool IsTokenAuthentication(this IAuthenticationContext authenticationContext)
        {
            if (authenticationContext == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(authenticationContext.Token))
            {
                return false;
            }

            return true;
        }
    }
}