﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextFacts.cs" company="GitTools">
//   Copyright (c) 2014 - 2015 GitTools. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace GitTools.Tests
{
    using System;
    using Catel.Test;
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

        //[TestFixture]
        //public class TheValidateContextMethod
        //{
        //    [TestCase]
        //    public void ThrowsExceptionForMissingSolutionDirectory()
        //    {
        //        var context = new Context();

        //        ExceptionTester.CallMethodAndExpectException<GitLinkException>(() => context.ValidateContext());
        //    }

        //    [TestCase]
        //    public void ThrowsExceptionForMissingConfigurationName()
        //    {
        //        var context = new Context(new ProviderManager())
        //        {
        //            SolutionDirectory = @"c:\source\GitLink",
        //            ConfigurationName = string.Empty
        //        };

        //        ExceptionTester.CallMethodAndExpectException<GitLinkException>(() => context.ValidateContext());
        //    }

        //    [TestCase]
        //    public void ThrowsExceptionForMissingTargetUrl()
        //    {
        //        var context = new Context(new ProviderManager())
        //        {
        //            SolutionDirectory = @"c:\source\GitLink",
        //        };

        //        ExceptionTester.CallMethodAndExpectException<GitLinkException>(() => context.ValidateContext());
        //    }

        //    [TestCase]
        //    public void SucceedsForValidContext()
        //    {
        //        var context = new Context(new ProviderManager())
        //        {
        //            SolutionDirectory = @"c:\source\GitLink",
        //            TargetUrl = "https://github.com/CatenaLogic/GitLink",
        //        };

        //        // should not throw
        //        context.ValidateContext();
        //    }

        //    [TestCase(@".", @"c:\")]
        //    [TestCase(@"C:\MyDirectory\", @"C:\MyDirectory\")]
        //    [TestCase(@"C:\MyDirectory\..", @"C:\")]
        //    [TestCase(@"C:\MyDirectory\..\TestDirectory\", @"C:\TestDirectory\")]
        //    public void ExpandsDirectoryIfRequired(string solutionDirectory, string expectedValue)
        //    {
        //        Environment.CurrentDirectory = @"c:\";

        //        var context = new Context(new ProviderManager());
        //        context.TargetUrl = "https://github.com/CatenaLogic/GitLink.git";
        //        context.SolutionDirectory = solutionDirectory;

        //        context.ValidateContext();

        //        Assert.AreEqual(expectedValue, context.SolutionDirectory);
        //    }
        //}
    }
}