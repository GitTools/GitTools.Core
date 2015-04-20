namespace GitTools.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class StringExtensionsFacts
    {
        [TestCase("/develop", false)]
        [TestCase("/master", false)]
        [TestCase("/pr/25", true)]
        [TestCase("/pull/25", true)]
        [TestCase("/pull-requests/25", true)]
        public void TheIsPullRequestMethod(string input, bool expectedValue)
        {
            var actualValue = input.IsPullRequest();
            
            Assert.AreEqual(expectedValue, actualValue);
        }
    }
}