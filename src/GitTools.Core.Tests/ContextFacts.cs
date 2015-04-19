namespace GitTools.Tests
{
    using NUnit.Framework;

    public class ContextFacts
    {
        [TestFixture]
        public class TheDefaultValues
        {
            [TestCase]
            public void SetsRightDefaultValues()
            {
                var context = new Context();

                //Assert.AreEqual("Release", context.ConfigurationName);
                Assert.IsFalse(context.IsHelp);
            }
        }
    }
}