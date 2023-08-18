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

    /// <summary>
    /// Changes text for given element
    /// </summary>
    /// <param name="element"></param>
    /// <param name="newText"></param>
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
        
        log.Info("Screenshot has been taken");
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

    public bool ElementIsPresent(By locator)
    {
        try
        {
            driver.FindElement(locator);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool ElementIsInteractable(By locator, int timeout)
    {
        try
        {
            WebdriverWait(timeout).Until(_ =>
                driver.FindElement(locator).Enabled && driver.FindElement(locator).Displayed);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void Get(string url)
    {
        driver.Url = url;
        log.Debug($"Navigated to <a href='{url}'>{url}</a>");
    }

    public void Click(By locator, int timeout = 10, bool softAssert = true)
    {
        if(ElementIsInteractable(locator, timeout))
            try
            {
                driver.FindElement(locator).Click();
            }
            catch (ElementClickInterceptedException)
            {
                if (!softAssert)
                {
                    log.Error("Element click is intercepted" + JsonReportText(new Dictionary<string, object>{{ "element", locator }}));
                    Assert.Fail("Element click is intercepted " + locator);
                }
                
                log.Warning("Element click is intercepted" + JsonReportText(new Dictionary<string, object>{{ "element", locator }}));
                Assert.Warn("Element click is intercepted " + locator);
            }
    }

    public void Submit(By locator, bool debug = false)
    {
        driver.FindElement(locator).Submit();
        if(debug)
            log.Debug("Submitted element " + locator);
    }

    #endregion

    #region Assertions

    public void AssertElementIsInteractable(By locator, int timeout, bool softAssert = true)
    {
        try
        {
            WebdriverWait(timeout).Until(_ =>
                driver.FindElement(locator).Enabled && driver.FindElement(locator).Displayed);
            log.Info($"Element <code>{locator}</code> is interactable");
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
    /// Drags and drops element to another element
    /// </summary>
    /// <param name="sourceElement"></param>
    /// <param name="targetElement"></param>
    /// <param name="softAssert"></param>
    public void DragAndDrop(By sourceElement, By targetElement, bool softAssert = true)
    {
        var _sourceElement = driver.FindElement(sourceElement);
        var _targetElement = driver.FindElement(targetElement);
        string fromTo = JsonReportText(new Dictionary<string, object>
        {
            { "from", sourceElement },
            { "to:", targetElement }
        });
        try
        {
            new Actions(driver)
                .DragAndDrop(_sourceElement, _targetElement)
                .Perform();
            log.Pass($"Successfully Drag and Dropped" + fromTo);
        }
        catch
        {
            if (!softAssert)
            {
                log.Error("Couldn't Drag and Drop" + fromTo, CaptureScreenshot());
                Assert.Fail($"Couldn't Drag and Drop from {sourceElement} to {targetElement}");
            }

            log.Warning("Couldn't Drag and Drop" + fromTo);
            Assert.Warn($"Couldn't Drag and Drop from {sourceElement} to {targetElement}");
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

    public void AssertElementIsPresent(By locator, bool expected, bool softAssert = true)
    {
        bool isPresent = ElementIsPresent(locator);
        string jsonText = JsonReportText(new Dictionary<string, object>
        {
            { "element", locator }
        });
        if (isPresent && expected)
        {
            log.Pass("Element <b>IS</b> present and <b>SHOULD</b> be present" + jsonText);
            if (!softAssert)
                Assert.Pass("Element IS present and SHOULD be present");
        }
        else if (isPresent && !expected)
        {
            if (!softAssert)
            {
                log.Error("Element <b>IS</b> present and <b>SHOULD NOT</b> be present" + jsonText, CaptureScreenshot());
                Assert.Fail("Element IS present and SHOULD NOT be present");
            }

            log.Warning("Element <b>IS</b> present and <b>SHOULD NOT</b> be present" + jsonText);
        }
        else if (!isPresent && expected)
        {
            if (!softAssert)
            {
                log.Error("Element <b>IS NOT</b> present and <b>SHOULD</b> be present" + jsonText, CaptureScreenshot());
                Assert.Fail("Element IS NOT present and SHOULD BE present");
            }

            log.Warning("Element <b>IS NOT</b> present and <b>SHOULD</b> be present" + jsonText);
        }
        else
        {
            log.Pass("Element <b>IS NOT</b> present and <b>SHOULD NOT</b> be present");
            if (!softAssert)
                Assert.Pass("Element IS NOT present and SHOULD NOT be present");
        }
    }

    #endregion
}