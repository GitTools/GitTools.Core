namespace GitTools.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class IAuthenticationContextExtensionsFacts
    {
        [TestCase("user", "password", null, false)]
        [TestCase(null, null, "token", false)]
        [TestCase(null, null, null, true)]
        public void TheIsEmptyMethod(string username, string password, string token, bool expectedValue)
        {
            var authenticationContext = new AuthenticationContext
            {
                Username = username,
                Password = password,
                Token = token
            };

            Assert.AreEqual(expectedValue, authenticationContext.IsEmpty());
        }

        [TestCase("user", "password", null, true)]
        [TestCase(null, null, "token", false)]
        [TestCase(null, null, null, false)]
        public void TheIsUsernameAndPasswordAuthenticationMethod(string username, string password, string token, bool expectedValue)
        {
            var authenticationContext = new AuthenticationContext
            {
                Username = username,
                Password = password,
                Token = token
            };

            Assert.AreEqual(expectedValue, authenticationContext.IsUsernameAndPasswordAuthentication());
        }

        [TestCase("user", "password", null, false)]
        [TestCase(null, null, "token", true)]
        [TestCase(null, null, null, false)]
        public void TheIsTokenAuthenticationMethod(string username, string password, string token, bool expectedValue)
        {
            var authenticationContext = new AuthenticationContext
            {
                Username = username,
                Password = password,
                Token = token
            };

            Assert.AreEqual(expectedValue, authenticationContext.IsTokenAuthentication());
        }
    }
}