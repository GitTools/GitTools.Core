namespace GitTools
{
    using System;
    using LibGit2Sharp;

    public static class TestValues
    {
        private static DateTimeOffset _simulatedTime = DateTimeOffset.Now.AddHours(-1);

        public static DateTimeOffset Now
        {
            get
            {
                _simulatedTime = _simulatedTime.AddMinutes(1);
                return _simulatedTime;
            }
        }

        public static Signature SignatureNow()
        {
            var dateTimeOffset = Now;
            return Signature(dateTimeOffset);
        }

        public static Signature Signature(DateTimeOffset dateTimeOffset)
        {
            return new Signature("A. U. Thor", "thor@valhalla.asgard.com", dateTimeOffset);
        }
    }
}