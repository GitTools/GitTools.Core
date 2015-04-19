using Catel.Logging;
using NUnit.Framework;

[SetUpFixture]
public class GlobalInitialization
{
    [SetUp]
    public static void SetUp()
    {
#if DEBUG
        LogManager.AddDebugListener(true);
#endif
    }
}