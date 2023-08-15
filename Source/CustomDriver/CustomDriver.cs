using AventStack.ExtentReports;
using NUnit.Framework;
using OpenQA.Selenium.Interactions;
using SeleniumFramework.Utilities;

namespace SeleniumFramework.Source.CustomDriver;

/// <summary>
/// Use this to make a webdriver
/// 
/// </summary>
public class CustomDriver
{
    public readonly IWebDriver driver;
    public readonly ReportClass report;
    public readonly Lazy<IJavaScriptExecutor> js;
    // should save some time if there are a lot of tests 
    
    CustomDriver(IWebDriver driver, ReportClass report)
    {
        this.driver = driver;
        this.report = report;
        js = new Lazy<IJavaScriptExecutor>((IJavaScriptExecutor) driver);
        // #define would go crazy with js.Value 🗣🗣🔥🥶 
    }

    #region Fast access methods
    
    /// <summary>
    /// Scrolls to the given element
    /// </summary>
    /// <param name="element"></param>
    public void ScrollElementIntoView(By element)
    {
        var _element = driver.FindElement(element);
        string scrollElementIntoMiddle =
            "var viewPortHeight = Math.max(document.documentElement.clientHeight, window.innerHeight || 0);"
            + "var elementTop = arguments[0].getBoundingClientRect().top;"
            + "window.scrollBy(0, elementTop-(viewPortHeight/2));";

        js.Value.ExecuteScript(scrollElementIntoMiddle, _element);
    }

    public void ScrollElementIntoView(IWebElement _element)
    {
        string scrollElementIntoMiddle =
            "var viewPortHeight = Math.max(document.documentElement.clientHeight, window.innerHeight || 0);"
            + "var elementTop = arguments[0].getBoundingClientRect().top;"
            + "window.scrollBy(0, elementTop-(viewPortHeight/2));";

        js.Value.ExecuteScript(scrollElementIntoMiddle, _element);
    }

    /// <summary>
    /// Changes attribute for the chosen element
    /// </summary>
    /// <param name="element"></param>
    /// <param name="attribute"></param>
    /// <param name="value"></param>
    public void JavaScriptSetAttribute(By element, string attribute, string value)
    {
        var webElement = driver.FindElement(element);
        js.Value.ExecuteScript($"arguments[0].setAttribute('{attribute}', '{value}')", webElement);
    }

    public void JavaScriptSetAttribute(IWebDriver webElement, string attribute, string value)
    {
        js.Value.ExecuteScript($"arguments[0].setAttribute('{attribute}', '{value}')", webElement);
    }

    /// <summary>
    /// Changes text for given element
    /// </summary>
    /// <param name="webElement"></param>
    /// <param name="newText"></param>
    public void JavaScriptChangeInnerHTML(IWebElement webElement, string newText)
    {
        js.Value.ExecuteScript($"arguments[0].innerHTML = '{newText}';", webElement);
    }

    public void JavaScriptChangeInnerHTML(By element, string newText)
    {
        var webElement = driver.FindElement(element);
        js.Value.ExecuteScript($"arguments[0].innerHTML = '{newText}';", webElement);
    }


    /// <summary>
    /// Drags and drops element to another element 
    /// </summary>
    /// <param name="sourceElement"></param>
    /// <param name="targetElement"></param>
    public void DragAndDrop(By sourceElement, By targetElement)
    {
        var _sourceElement = driver.FindElement(sourceElement);
        var _targetElement = driver.FindElement(targetElement);
        new Actions(driver)
            .DragAndDrop(_sourceElement, _targetElement)
            .Perform();
    }

    /// <summary>
    /// Closes all the windows except the chosen(current by default) one
    /// </summary>
    /// <param name="homePage"></param>
    public void OtherWindowsClose(string? homePage = null)
    {
        homePage ??= driver.CurrentWindowHandle;
        var _windows = driver.WindowHandles;
        foreach (var window in _windows)
        {
            if (window != homePage)
            {
                driver.SwitchTo().Window(window);
                driver.Close();
            }
        }
    }

    /// <summary>
    /// Returns screenshot for extent reports
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public MediaEntityModelProvider CaptureScreenshot(string? fileName = null)
    {
        ITakesScreenshot ts = (ITakesScreenshot)driver;
        string screenshot = ts.GetScreenshot().AsBase64EncodedString;

        if (fileName == null)
        {
            var time = DateTime.Now;
            fileName = "Screenshot_" + time.ToString("h:mm:ss tt zz") + ".png";
        }

        return MediaEntityBuilder.CreateScreenCaptureFromBase64String(screenshot, fileName).Build();
    }

    #endregion

    #region Assertions



    #endregion
}