using AventStack.ExtentReports;
using NUnit.Framework;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumFramework.Utilities;

namespace SeleniumFramework.Source.CustomDriver;

/// <summary>
/// Use this to make a webdriver
/// 
/// </summary>
public class CustomDriver
{
    public readonly IWebDriver driver;
    public readonly ReportClass log;
    public readonly Lazy<IJavaScriptExecutor> js;
    // should save some time if there are a lot of tests 

    public CustomDriver(IWebDriver driver, ReportClass log)
    {
        this.driver = driver;
        this.log = log;
        js = new Lazy<IJavaScriptExecutor>((IJavaScriptExecutor)driver);
        // #define would go crazy with js.Value 🗣🗣🔥🥶 
    }

    #region Utility Methods

    /// <summary>
    /// shortcut for driver.Quit();
    /// </summary>
    public void Quit()
    {
        driver.Quit();
        log.Info("Driver quit");
    }

    /// <summary>
    /// Returns WebDriverWait with the needed timeout in seconds
    /// </summary>
    /// <param name="timeout"> timeout in seconds</param>
    /// <returns></returns>
    public WebDriverWait WebdriverWait(int timeout = 10)
    {
        return new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
    }

    public void IsInteractable(By locator, int timeout, bool softAssert = true)
    {
        try
        {
            WebdriverWait(timeout).Until(_ =>
                driver.FindElement(locator).Enabled && driver.FindElement(locator).Displayed);
            log.Debug($"Element <code>{locator}</code> is interactable");
        }
        catch (WebDriverTimeoutException)
        {
            if (!softAssert)
            {
                log.Error($"Element is not interactable after <b>{timeout}</b> seconds)" +
                          JsonReportText(new Dictionary<string, object>
                              { { "element", locator }, { "timeOutSeconds", timeout + "" } }));
                Assert.Fail($"Element is not interactable after <b>{timeout}</b> seconds)");
            }

            log.Warning($"Element is not interactable after <b>{timeout}</b> seconds)" +
                        JsonReportText(new Dictionary<string, object>
                            { { "element", locator }, { "timeOutSeconds", timeout + "" } }));
        }
    }

    /// <summary>
    /// Scrolls to the given element
    /// </summary>
    /// <param name="locator"></param>
    public void ScrollElementIntoView(By locator)
    {
        var _element = driver.FindElement(locator);
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
    /// <param name="locator"></param>
    /// <param name="attribute"></param>
    /// <param name="value"></param>
    public void JavaScriptSetAttribute(By locator, string attribute, string value)
    {
        var webElement = driver.FindElement(locator);
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
    /// Closes all the windows except the chosen(current by default) one
    /// </summary>
    /// <param name="homePage"></param>
    public void CloseOtherWindows(string? homePage = null)
    {
        homePage ??= driver.CurrentWindowHandle;
        var windows = driver.WindowHandles;
        foreach (var window in windows)
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

    private bool IsVisible(By locator)
    {
        try
        {
            return driver.FindElement(locator).Displayed;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
    }

    // TODO: add driver methods, but modify them with try catch, logging and other stuff if needed
    // submit, click

    #endregion

    #region Assertions

    /// <summary>
    /// Drags and drops element to another element
    /// TODO: rename to DragAndDrop and add parameter softAssert for assertion
    /// </summary>
    /// <param name="sourceElement"></param>
    /// <param name="targetElement"></param>
    /// <param name="severity"></param>
    /// <param name="screenShot"></param>
    public void DragAndDropAssertion(By sourceElement, By targetElement, Status severity = Status.Warning,
        bool screenShot = true)
    {
        var _sourceElement = driver.FindElement(sourceElement);
        var _targetElement = driver.FindElement(targetElement);
        string fromTo =
            $"<pre lang='json'><code><font color='red'>from: </font><font color='green'>'{sourceElement}'</font><br/>" +
            $"<font color='red'>to: </font><font color='green'>'{targetElement}'</font></code></pre>";
        try
        {
            new Actions(driver)
                .DragAndDrop(_sourceElement, _targetElement)
                .Perform();
            log.Pass($"Successfully Drag and Dropped" + fromTo);
        }
        catch
        {
            if (screenShot)
                log.BySeverity($"Couldn't Drag and Drop" + fromTo, severity, CaptureScreenshot());
            else
                log.BySeverity($"Couldn't Drag and Drop" + fromTo, severity);
        }
    }

    public void SendKeys(By locator, string value, bool softAssert = true, bool clear = true)
    {
        string reportText = JsonReportText(new Dictionary<string, object>
        {
            { "element", locator },
            { "value", value }
        });
        try
        {
            var input = driver.FindElement(locator);
            if (clear)
                input.Clear();

            input.SendKeys(value);
            log.Debug("Text entered to element" + reportText);
        }
        catch (ElementNotInteractableException)
        {
            if (!softAssert)
            {
                log.Error("Element is not interactable" + reportText, CaptureScreenshot());
                Assert.Fail("Couldn't interact with " + locator);
            }

            log.Warning("Element is not interactable" + reportText);
            Assert.Warn("Couldn't interact with " + locator);
        }
        catch (StaleElementReferenceException)
        {
            if (!softAssert)
            {
                log.Error("Element is stale. Unable to send text" + reportText, CaptureScreenshot());
                Assert.Fail($"Element \"{locator}\" is stale. Unable to send text");
            }

            log.Warning("Element is stale. Unable to send text" + reportText);
            Assert.Warn($"Element \"{locator}\" is stale. Unable to send text");
        }
        catch (NoSuchElementException)
        {
            if (!softAssert)
            {
                log.Error("No element found>" + reportText, CaptureScreenshot());
                Assert.Fail($"Element \"{locator}\" not found");
            }

            log.Warning("No element found" + reportText);
            Assert.Warn($"Element \"{locator}\" not found");
        }
        catch (Exception ex)
        {
            if (!softAssert)
            {
                log.Error($"{ex.Message}</br><b>Stack trace:</b><br/><code><pre lang='red'>" +
                          $"{ex.StackTrace?.Replace("\n", "<br/></code></pre>")}", CaptureScreenshot());
                Assert.Fail(ex.Message);
            }

            log.Warning($"{ex.Message}</br><b>Stack trace:</b><br/><code><pre lang='red'>" +
                        $"{ex.StackTrace?.Replace("\n", "<br/></code></pre>")}");
            Assert.Warn(ex.Message);
        }
    }

    public void AssertElementIsVisible(By locator, bool expected, bool softAssert = true)
    {
        bool visible = IsVisible(locator);
        string jsonText = JsonReportText(new Dictionary<string, object>
        {
            { "element", locator }
        });
        if (visible && expected)
        {
            log.Pass("Element <b>IS</b> visible and <b>SHOULD</b> be visible" + jsonText);
            if (!softAssert)
                Assert.Pass("Element IS visible and SHOULD be visible");
        }
        else if (visible && !expected)
        {
            if (!softAssert)
            {
                log.Error("Element <b>IS</b> visible and <b>SHOULD NOT</b> be visible" + jsonText, CaptureScreenshot());
                Assert.Fail("Element IS visible and SHOULD NOT be visible");
            }

            log.Warning("Element <b>IS</b> visible and <b>SHOULD NOT</b> be visible" + jsonText);
        }
        else if (!visible && expected)
        {
            if (!softAssert)
            {
                log.Error("Element <b>IS NOT</b> visible and <b>SHOULD</b> be visible" + jsonText, CaptureScreenshot());
                Assert.Fail("Element IS NOT visible and SHOULD BE visible");
            }

            log.Warning("Element <b>IS NOT</b> visible and <b>SHOULD</b> be visible" + jsonText);
        }
        else
        {
            log.Pass("Element <b>IS NOT</b> visible and <b>SHOULD NOT</b> be visible");
            if (!softAssert)
                Assert.Pass("Element IS NOT visible and SHOULD NOT be visible");
        }
    }

    //TODO: add more

    #endregion
}