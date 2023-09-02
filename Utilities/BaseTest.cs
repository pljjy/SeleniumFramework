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

public abstract class BaseTest
{
    private protected Dictionary<string, dynamic> configs;
    private protected CustomDriver driver;

    private protected bool driverTest = true;
    // declare driverTest in the child class as a new variable
    // nUnit uses 1 instance for all tests in testfixutres so constructor doesnt work

    #region SetUps

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        configs = JsonFileToDictionary(pathToJsonFile);
        ExtentTestManager.CreateParentTest(GetType().Name);
    }

    [SetUp]
    public void StartBrowser()
    {
        if (!driverTest) return;

        ETypeDriver webEType;
        switch (configs["browser"].ToLower())
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
        // json returns Int64 so it needs to be manually changed to Int32
        ExtentTestManager.CreateTest(TestContext.CurrentContext.Test.Name);
        driver = new CustomDriver(_driver);
    }

    #endregion

    #region TearDowns

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        ExtentManager.Instance.Flush();
    }

    [TearDown]
    public void TearDown()
    {
        if (!driverTest)
        {
            ExtentManager.FinishReport();
            return;
        }

        ExtentManager.FinishReport(driver);
        driver.Quit();
        ExtentManager.LogStep("Driver quit");
    }

    #endregion
}