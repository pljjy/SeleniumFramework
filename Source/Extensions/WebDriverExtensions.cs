using AventStack.ExtentReports;
using OpenQA.Selenium.Interactions;

namespace SeleniumFramework.Source.Extensions;

public static class WebDriverExtensions
{
    
    /// <summary>
    /// Fast access to js executor
    /// </summary>
    /// <param name="driver"></param>
    /// <returns></returns>
    public static IJavaScriptExecutor JavaScript(this IWebDriver driver)
    {
        return (IJavaScriptExecutor)driver;
    }

    /// <summary>
    /// Scrolls to the given element
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="element"></param>
    public static void ScrollElementIntoView(this IWebDriver driver, By element)
    {
        var _element = driver.FindElement(element);
        string scrollElementIntoMiddle =
            "var viewPortHeight = Math.max(document.documentElement.clientHeight, window.innerHeight || 0);"
            + "var elementTop = arguments[0].getBoundingClientRect().top;"
            + "window.scrollBy(0, elementTop-(viewPortHeight/2));";

        driver.JavaScript().ExecuteScript(scrollElementIntoMiddle, _element);
    }

    public static void ScrollElementIntoView(this IWebDriver driver, IWebElement _element)
    {
        string scrollElementIntoMiddle =
            "var viewPortHeight = Math.max(document.documentElement.clientHeight, window.innerHeight || 0);"
            + "var elementTop = arguments[0].getBoundingClientRect().top;"
            + "window.scrollBy(0, elementTop-(viewPortHeight/2));";

        driver.JavaScript().ExecuteScript(scrollElementIntoMiddle, _element);
    }

    /// <summary>
    /// Changes attribute for the chosen element
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="element"></param>
    /// <param name="attribute"></param>
    /// <param name="value"></param>
    public static void JavaScriptSetAttribute(this IWebDriver driver, By element, string attribute, string value)
    {
        var webElement = driver.FindElement(element);
        IJavaScriptExecutor executor = JavaScript(driver);
        executor.ExecuteScript($"arguments[0].setAttribute('{attribute}', '{value}')", webElement);
    }

    public static void JavaScriptSetAttribute(this IWebDriver driver, IWebDriver webElement, string attribute,
        string value)
    {
        IJavaScriptExecutor executor = JavaScript(driver);
        executor.ExecuteScript($"arguments[0].setAttribute('{attribute}', '{value}')", webElement);
    }

    /// <summary>
    /// Changes text for given element
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="webElement"></param>
    /// <param name="newText"></param>
    public static void JavaScriptChangeText(this IWebDriver driver, IWebElement webElement, string newText)
    {
        IJavaScriptExecutor executor = JavaScript(driver);
        executor.ExecuteScript($"arguments[0].innerHTML = '{newText}';", webElement);
    }

    public static void JavaScriptChangeText(this IWebDriver driver, By element, string newText)
    {
        var webElement = driver.FindElement(element);
        IJavaScriptExecutor executor = JavaScript(driver);
        executor.ExecuteScript($"arguments[0].innerHTML = '{newText}';", webElement);
    }


    /// <summary>
    /// Drags and drops element to another element 
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="sourceElement"></param>
    /// <param name="targetElement"></param>
    public static void DragAndDrop(this IWebDriver driver, By sourceElement, By targetElement)
    {
        var _sourceElement = driver.FindElement(sourceElement);
        var _targetElement = driver.FindElement(targetElement);
        var action = new Actions(driver);
        action.DragAndDrop(_sourceElement, _targetElement).Perform();
    }

    /// <summary>
    /// Closes all the windows except the chosen(current by default) one
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="homePage"></param>
    public static void OtherWindowsClose(this IWebDriver driver, string? homePage = null)
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
    /// <param name="driver"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static MediaEntityModelProvider CaptureScreenshot(this IWebDriver driver, string? fileName = null)
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
}