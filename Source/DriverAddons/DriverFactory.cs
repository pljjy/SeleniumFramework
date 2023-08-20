using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

namespace SeleniumFramework.Source.DriverAddons;

/// <summary>
/// Creates a chosen webdriver with some options
/// </summary>
public class DriverFactory
{
    private static bool headless = true;
    public static readonly string driverPath = Directory.GetCurrentDirectory() + "/chromedriver.exe";

    /// <summary>
    /// Returns a chosen webdriver with chosen options and some arguments
    /// </summary>
    /// <param name="eTypeDriver">type of the driver</param>
    /// <param name="implWait">implicit wait for the driver in seconds</param>
    /// <param name="_headless"></param>
    /// <returns>webdriver with chosen options</returns>
    internal static IWebDriver GetBrowser(ETypeDriver eTypeDriver, int implWait = 5, bool _headless = true)
    {
        headless = _headless;

        IWebDriver driver;
        switch (eTypeDriver)
        {
            case ETypeDriver.Firefox:
                driver = GetFirefox();
                break;
            case ETypeDriver.Edge:
                driver = GetEdge();
                break;
            default:
                driver = GetChrome();
                break;
        }

        if (implWait >= 0)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(implWait);
        }

        return driver;
    }

    internal static ETypeDriver StringToETypeDriver(string value)
    {
        value = value.ToLower();
        switch (value)
        {
            case "edge": return ETypeDriver.Edge;
            case "firefox": return ETypeDriver.Firefox;
            default: return ETypeDriver.Chrome;
        }
    }

    #region Private functions

    private static IWebDriver GetChrome()
    {
        var opts = new ChromeOptions();
        if (headless) opts.AddArgument("--headless");
        opts.AddArguments("--ignore-ssl-errors=yes",
            "--ignore-certificate-errors", "--window-size=1980,1080");
        ChromeDriver driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(driverPath), opts);
        driver.Manage().Window.Maximize();

        return driver;
    }

    private static IWebDriver GetFirefox()
    {
        var opts = new FirefoxOptions();
        if (headless) opts.AddArgument("--headless");
        opts.AddArgument("--window-size=1980,1080");
        FirefoxDriver driver = new FirefoxDriver();
        driver.Manage().Window.Maximize();

        return driver;
    }

    private static IWebDriver GetEdge()
    {
        var opts = new EdgeOptions();
        if (headless) opts.AddArgument("--headless");
        opts.AddArguments("--ignore-ssl-errors=yes",
            "--ignore-certificate-errors", "disable-gpu",
            "--disable-extensions");
        EdgeDriver driver = new EdgeDriver(opts);
        driver.Manage().Window.Maximize();

        return driver;
    }

    #endregion
}