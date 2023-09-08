using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace SeleniumFramework.Source.DriverAddons;

/// <summary>
///     Creates a chosen webdriver with some options
/// </summary>
public static class DriverFactory
{
    private static bool headless = true;

    /// <summary>
    ///     Returns a chosen webdriver with chosen options and some arguments
    /// </summary>
    /// <param name="eTypeDriver">type of the driver</param>
    /// <param name="implWait">implicit wait for the driver in seconds</param>
    /// <param name="_headless"></param>
    /// <param name="args">additional options arguments</param>
    /// <returns>webdriver with chosen options</returns>
    internal static IWebDriver GetBrowser(ETypeDriver eTypeDriver, int implWait = 5, bool _headless = true,
        string[]? args = null)
    {
        headless = _headless;

        IWebDriver driver;
        switch (eTypeDriver)
        {
            case ETypeDriver.Firefox:
                driver = GetFirefox(args);
                break;
            case ETypeDriver.Edge:
                driver = GetEdge(args);
                break;
            default:
                driver = GetChrome();
                break;
        }

        if (implWait >= 0) driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(implWait);

        return driver;
    }

    #region Private functions

    private static IWebDriver GetChrome(string[]? args = null)
    {
        new DriverManager().SetUpDriver(new ChromeConfig());
        var opts = new ChromeOptions();
        if (headless) opts.AddArgument("--headless");
        if (args is not null) opts.AddArguments(args);

        opts.AddArguments("--ignore-ssl-errors=yes",
            "--ignore-certificate-errors", "--window-size=1980,1080");
        var driver = new ChromeDriver(opts);
        driver.Manage().Window.Maximize();

        return driver;
    }

    private static IWebDriver GetFirefox(string[]? args = null)
    {
        new DriverManager().SetUpDriver(new FirefoxConfig());
        var opts = new FirefoxOptions();
        if (headless) opts.AddArgument("--headless");
        if (args is not null) opts.AddArguments(args);

        opts.AddArgument("--window-size=1980,1080");
        var driver = new FirefoxDriver();
        driver.Manage().Window.Maximize();

        return driver;
    }

    private static IWebDriver GetEdge(string[]? args = null)
    {
        new DriverManager().SetUpDriver(new EdgeConfig());
        var opts = new EdgeOptions();
        if (headless) opts.AddArgument("--headless");
        if (args is not null) opts.AddArguments(args);

        opts.AddArguments("--ignore-ssl-errors=yes",
            "--ignore-certificate-errors", "disable-gpu",
            "--disable-extensions");
        var driver = new EdgeDriver(opts);
        driver.Manage().Window.Maximize();

        return driver;
    }

    #endregion
}