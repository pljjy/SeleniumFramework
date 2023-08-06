using System.Text.RegularExpressions;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using SeleniumFramework.Source.DriverAddons;
using SeleniumFramework.Source.Extensions;

namespace SeleniumFramework.Utilities;

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

public class BaseTest
{
    private protected IWebDriver driver;
    private ExtentReports extent;
    private protected ReportClass report;
    private protected string nameClass;
    private protected Dictionary<string, dynamic> configs;

    public static string projectDir = GetProjectDirectory();

    #region SetUp

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        string reportsPath = projectDir + @"/Reports/";
        if (!Directory.Exists(reportsPath))
        {
            Directory.CreateDirectory(reportsPath);
            // html files are ignored by git so Report folder doesn't exist when cloned 
        }
        
        nameClass = GetType().ToString().Substring("Epam.TestCases.".Length);

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

        //information that will be shown in the html report
        configs = JsonFileToDictionary(pathToJsonFile);
        var systemInfo = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(configs["system-info"].ToString());

        if (systemInfo["show-browser-in-system-info"])
        {
            systemInfo.Remove("show-browser-in-system-info");
            extent.AddSystemInfo("Browser", configs["browser"]);
        }

        foreach (var info in systemInfo)
        {
            extent.AddSystemInfo(info.Key, info.Value);
        }
    }

    [SetUp]
    public void StartBrowser()
    {
        report = new ReportClass(TestContext.CurrentContext.Test.Name, extent);
        ETypeDriver webEType = ETypeDriver.Chrome;
        string browserName = Regex.Replace(configs["browser"], @"[_\s-]", "").ToLower();

        switch (browserName)
        {
            case "edge":
                webEType = ETypeDriver.Edge;
                break;
            case "firefox":
                webEType = ETypeDriver.Firefox; 
                break;
        }

        driver =  DriverFactory.GetBrowser(webEType, (int)configs["implicit-wait"], configs["headless"]);
        // json returns Int64 so it should be changed to Int32 manually
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
        catch(Exception e)
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
                report.Pass("Test passed successfully");
                break;
            case TestStatus.Skipped:
                report.Debug("Test skipped");
                break;
            case TestStatus.Warning:
                report.Warning("Test ended with a warning", driver.CaptureScreenshot());
                break;
            case TestStatus.Failed:
                report.Error("Test ended with an error", driver.CaptureScreenshot());
                break;
        }

        driver.Quit();
    }

    #endregion
}