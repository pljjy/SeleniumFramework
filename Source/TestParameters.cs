namespace SeleniumFramework.Source;

/// <summary>
///     Test parameters which are used via nunit3-console or dotnet test
/// </summary>
public static class TestParameters
{
    // USE THIS UNLESS DEBUGGING
    public static readonly string browser = TestContext.Parameters["browser"] ?? "chrome";
    public static readonly bool headless = bool.Parse(TestContext.Parameters["headless"] ?? "true");
    public static readonly int implicitWait = int.Parse(TestContext.Parameters["implicit-wait"] ?? "15");
}