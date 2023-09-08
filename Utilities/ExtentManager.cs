using AventStack.ExtentReports;
using AventStack.ExtentReports.MarkupUtils;
using AventStack.ExtentReports.Reporter;
using NUnit.Framework.Interfaces;
using SeleniumFramework.Source;

namespace SeleniumFramework.Utilities;

public class ExtentManager
{
    private static readonly Lazy<ExtentReports> _lazy = new();
    public static string binDirectory = Directory.GetCurrentDirectory();
    public static readonly string reportFolder = binDirectory + @"\Reports\";

    static ExtentManager()
    {
        if (Directory.Exists(reportFolder) != true) Directory.CreateDirectory(reportFolder);
        var htmlReporter = new ExtentHtmlReporter(reportFolder);
        htmlReporter.LoadConfig(projectDir + "/extent-config.xml");
        Instance.AttachReporter(htmlReporter);
    }

    public static ExtentReports Instance => _lazy.Value;

    public static void FinishReport(CustomDriver driver)
    {
        var status = TestContext.CurrentContext.Result.Outcome.Status;
        var stacktrace = string.IsNullOrEmpty(TestContext.CurrentContext.Result.StackTrace)
            ? string.Empty
            : $"<pre>{TestContext.CurrentContext.Result.StackTrace}</pre>";
        var errorMessage = string.IsNullOrEmpty(TestContext.CurrentContext.Result.Message)
            ? string.Empty
            : $"<pre>{TestContext.CurrentContext.Result.Message}</pre>";
        var extentTest = ExtentTestManager.GetTest();
        Status logStatus;

        switch (status)
        {
            case TestStatus.Failed:
                logStatus = Status.Fail;
                driver.TakeScreenShot();
                extentTest.Log(logStatus, "Test error " + errorMessage);
                break;
            case TestStatus.Inconclusive:
                logStatus = Status.Warning;
                break;
            case TestStatus.Skipped:
                logStatus = Status.Skip;
                break;
            case TestStatus.Passed:
                logStatus = Status.Pass;
                break;
            default:
                logStatus = Status.Fail;
                driver.TakeScreenShot();
                extentTest.Log(logStatus, "Test error " + errorMessage);
                break;
        }

        extentTest.Log(logStatus, "Test ended with " + logStatus + stacktrace);
    }

    public static void FinishReport()
    {
        var status = TestContext.CurrentContext.Result.Outcome.Status;
        var stacktrace = string.IsNullOrEmpty(TestContext.CurrentContext.Result.StackTrace)
            ? string.Empty
            : $"<pre>{TestContext.CurrentContext.Result.StackTrace}</pre>";
        var errorMessage = string.IsNullOrEmpty(TestContext.CurrentContext.Result.Message)
            ? string.Empty
            : $"<pre>{TestContext.CurrentContext.Result.Message}</pre>";
        var extentTest = ExtentTestManager.GetTest();
        Status logStatus;

        switch (status)
        {
            case TestStatus.Failed:
                logStatus = Status.Fail;
                extentTest.Log(logStatus, "Test error " + errorMessage);
                break;

            case TestStatus.Inconclusive:
                logStatus = Status.Warning;
                break;

            case TestStatus.Skipped:
                logStatus = Status.Skip;
                break;
            case TestStatus.Passed:
                logStatus = Status.Pass;
                break;
            default:
                logStatus = Status.Fail;
                extentTest.Log(logStatus, "Test error " + errorMessage);
                break;
        }

        extentTest.Log(logStatus, "Test ended with " + logStatus + stacktrace);
    }

    public static void LogStep(string value, Status status = Status.Info)
    {
        ExtentTestManager.GetTest().Log(status, value);
    }

    public static void LogStep(IMarkup value, Status status = Status.Info)
    {
        ExtentTestManager.GetTest().Log(status, value);
    }
}