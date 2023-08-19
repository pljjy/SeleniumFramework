using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Configuration;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using SeleniumFramework.Source.CustomDriver;
using SeleniumFramework.Source.DriverAddons;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


/*
 *===========================================================
 * This class should be inherited by all classes with tests.
 * It setups the driver, setups ExtentReport variables
 * and flushes it into different files.
 * -----------------------------------------------------
 * Use report variable for logs, use
 * driver.CaptureScreenshot() as second parameter
 * to add a screenshot to the log using AsBase64EncodedString
 * which extent reports use
 *===========================================================
 */

[assembly: Parallelizable(ParallelScope.Fixtures)]

namespace SeleniumFramework.Utilities;

public class BaseTest
{
    private protected CustomDriver driver;
    private ExtentReports extent;
    private protected Reporter log;
    private protected string nameClass;
    private protected Dictionary<string, dynamic> configs;
    private protected bool driverTest = true;

    public static string projectDir = GetProjectDirectory();

    #region SetUp

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        configs = JsonFileToDictionary(pathToJsonFile);
        string reportsPath = projectDir + @"/Reports/";
        if (!Directory.Exists(reportsPath))
        {
            Directory.CreateDirectory(reportsPath);
            // html files are ignored by git so Report folder doesn't exist when cloned 
        }

        nameClass = GetType().ToString();
        var htmlReporter = new ExtentHtmlReporter(reportsPath)
        {
            Config =
            {
                DocumentTitle = "Epam tests",
                Theme = Theme.Standard
            }
        };

        extent = new ExtentReports();
        extent.AttachReporter(htmlReporter);
        //parse it to html
    }

    [SetUp]
    public void StartBrowser()
    {
        log = new Reporter(TestContext.CurrentContext.Test.Name, extent);
        ETypeDriver webEType;
        string browserName = configs["browser"].ToLower();
        switch (browserName)
        {
            case "edge":
                webEType = ETypeDriver.Edge;
                break;
            case "firefox":
                webEType = ETypeDriver.Firefox;
                break;
            default:
                webEType = ETypeDriver.Chrome;
                break;
        }

        IWebDriver _driver = DriverFactory.GetBrowser(webEType, (int)configs["implicit-wait"], configs["headless"]);
        // json returns Int64 so it should be manually changed to Int32
        driver = new CustomDriver(_driver, log);
    }

    #endregion

    #region TearDown

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        extent.Flush();
        try
        {
            File.Move(String.Format(projectDir + @"/Reports/index.html"),
                String.Format(projectDir + $@"/Reports/{DateTime.Now.ToString("yyyyMMdd_hhmm")}-{nameClass}.html"));
        }
        catch (Exception e)
        {
            Assert.Fail("Couldn't save reports\n\n" + e.Message + "\n" + e.StackTrace);
        }
    }

    [TearDown]
    public void TearDown()
    {
        var testStatus = TestContext.CurrentContext.Result.Outcome.Status;
        Thread.Sleep(500);
        switch (testStatus)
        {
            case TestStatus.Passed:
                log.Pass("Test passed successfully");
                break;
            case TestStatus.Skipped:
                log.Debug("Test skipped");
                break;
            case TestStatus.Warning:
                log.Warning("Test ended with a warning");
                break;
            case TestStatus.Failed:
                log.Error("Test ended with an error");
                break;
        }

        if (!driverTest) return;
        
        driver.Quit();
        log.Info("Driver quit");
    }

    #endregion
}