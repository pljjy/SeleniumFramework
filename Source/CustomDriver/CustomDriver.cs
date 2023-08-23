using AventStack.ExtentReports;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumFramework.Utilities;

namespace SeleniumFramework.Source.CustomDriver;

/// <summary>
///     Use this to make a webdriver
/// </summary>
public class CustomDriver
{
    public readonly IWebDriver driver;

    public readonly Lazy<IJavaScriptExecutor> js;
    // should save some time if there are a lot of tests

    public CustomDriver(IWebDriver driver)
    {
        this.driver = driver;
        js = new Lazy<IJavaScriptExecutor>((IJavaScriptExecutor)driver);
    }

    #region Utility Methods

    /// <summary>
    ///     shortcut for driver.Quit();
    /// </summary>
    public void Quit()
    {
        driver.Quit();
    }

    /// <summary>
    ///     Returns WebDriverWait with the needed timeout in seconds
    /// </summary>
    /// <param name="timeout"> timeout in seconds</param>
    /// <returns></returns>
    public WebDriverWait WebdriverWait(int timeout = 10)
    {
        return new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
    }

    /// <summary>
    ///     Scrolls to the given element
    /// </summary>
    /// <param name="locator"></param>
    public void ScrollElementIntoView(By locator)
    {
        var element = driver.FindElement(locator);
        var scrollElementIntoMiddle =
            "var viewPortHeight = Math.max(document.documentElement.clientHeight, window.innerHeight || 0);"
            + "var elementTop = arguments[0].getBoundingClientRect().top;"
            + "window.scrollBy(0, elementTop-(viewPortHeight/2));";

        js.Value.ExecuteScript(scrollElementIntoMiddle, element);
    }

    /// <summary>
    ///     Changes attribute for the chosen element
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
    ///     Changes text for given element
    /// </summary>
    /// <param name="locator"></param>
    /// <param name="newText"></param>
    public void JavaScriptChangeInnerHTML(By locator, string newText)
    {
        var element = driver.FindElement(locator);
        js.Value.ExecuteScript($"arguments[0].innerHTML = '{newText}';", element);
    }

    /// <summary>
    ///     Closes all the windows except the chosen(current by default) one
    /// </summary>
    /// <param name="homePage"></param>
    public void CloseOtherWindows(string? homePage = null)
    {
        homePage ??= driver.CurrentWindowHandle;
        var windows = driver.WindowHandles;
        foreach (var window in windows)
            if (window != homePage)
            {
                driver.SwitchTo().Window(window);
                driver.Close();
            }
    }

    public bool IsVisible(By locator)
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

    public bool IsPresent(By locator)
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

    public bool IsInteractable(By locator, int timeout)
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
        ExtentManager.LogStep($"Navigated to <a href='{url}'>{url}</a>");
    }

    public void Click(By locator, int timeout = 10, bool softAssert = true)
    {
        if (IsInteractable(locator, timeout))
            try
            {
                driver.FindElement(locator).Click();
            }
            catch (ElementClickInterceptedException)
            {
                if (!softAssert)
                {
                    ExtentManager.LogStep("Element click is intercepted" + JsonReportText(new Dictionary<string, object>
                        { { "element", locator } }), Status.Error);
                    Assert.Fail("Element click is intercepted " + locator);
                }

                ExtentManager.LogStep("Element click is intercepted" +
                                      JsonReportText(new Dictionary<string, object> { { "element", locator } }));
                Assert.Warn("Element click is intercepted " + locator);
            }
    }

    public void Submit(By locator, bool debug = false)
    {
        driver.FindElement(locator).Submit();
        if (debug)
            ExtentManager.LogStep("Submitted element " + locator);
    }

    #endregion

    #region Assertions

    public void AssertElementIsInteractable(By locator, int timeout, bool softAssert = true)
    {
        try
        {
            WebdriverWait(timeout).Until(_ =>
                driver.FindElement(locator).Enabled && driver.FindElement(locator).Displayed);
            ExtentManager.LogStep($"Element <code>{locator}</code> is interactable");
        }
        catch (WebDriverTimeoutException)
        {
            if (!softAssert)
            {
                ExtentManager.LogStep($"Element is not interactable after <b>{timeout}</b> seconds)" +
                                      JsonReportText(new Dictionary<string, object>
                                          { { "element", locator }, { "timeOutSeconds", timeout + "" } }));
                Assert.Fail($"Element is not interactable after <b>{timeout}</b> seconds)");
            }

            ExtentManager.LogStep($"Element is not interactable after <b>{timeout}</b> seconds)" +
                                  JsonReportText(new Dictionary<string, object>
                                      { { "element", locator }, { "timeOutSeconds", timeout + "" } }));
        }
    }

    /// <summary>
    ///     Drags and drops element to another element
    /// </summary>
    /// <param name="sourceElement"></param>
    /// <param name="targetElement"></param>
    /// <param name="softAssert"></param>
    public void DragAndDrop(By sourceElement, By targetElement, bool softAssert = true)
    {
        var _sourceElement = driver.FindElement(sourceElement);
        var _targetElement = driver.FindElement(targetElement);
        var fromTo = JsonReportText(new Dictionary<string, object>
        {
            { "from", sourceElement },
            { "to:", targetElement }
        });
        try
        {
            new Actions(driver)
                .DragAndDrop(_sourceElement, _targetElement)
                .Perform();
            ExtentManager.LogStep("Successfully Drag and Dropped" + fromTo, Status.Pass);
        }
        catch
        {
            if (!softAssert)
            {
                ExtentManager.LogStep("Couldn't Drag and Drop" + fromTo, Status.Error);
                TakeScreenShot();
                Assert.Fail($"Couldn't Drag and Drop from {sourceElement} to {targetElement}");
            }

            ExtentManager.LogStep("Couldn't Drag and Drop" + fromTo, Status.Warning);
            Assert.Warn($"Couldn't Drag and Drop from {sourceElement} to {targetElement}");
        }
    }

    public void SendKeys(By locator, string value, bool softAssert = true, bool clear = true)
    {
        var reportText = JsonReportText(new Dictionary<string, object>
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
            ExtentManager.LogStep("Text entered to element" + reportText);
        }
        catch (ElementNotInteractableException)
        {
            if (!softAssert)
            {
                ExtentManager.LogStep("Element is not interactable" + reportText, Status.Error);
                TakeScreenShot();
                Assert.Fail("Couldn't interact with " + locator);
            }

            ExtentManager.LogStep("Element is not interactable" + reportText, Status.Warning);
            Assert.Warn("Couldn't interact with " + locator);
        }
        catch (StaleElementReferenceException)
        {
            if (!softAssert)
            {
                ExtentManager.LogStep("Element is stale. Unable to send text" + reportText, Status.Error);
                TakeScreenShot();
                Assert.Fail($"Element \"{locator}\" is stale. Unable to send text");
            }

            ExtentManager.LogStep("Element is stale. Unable to send text" + reportText, Status.Warning);
            Assert.Warn($"Element \"{locator}\" is stale. Unable to send text");
        }
        catch (NoSuchElementException)
        {
            if (!softAssert)
            {
                ExtentManager.LogStep("No element found>" + reportText, Status.Error);
                TakeScreenShot();
                Assert.Fail($"Element \"{locator}\" not found");
            }

            ExtentManager.LogStep("No element found" + reportText, Status.Warning);
            Assert.Warn($"Element \"{locator}\" not found");
        }
        catch (Exception ex)
        {
            if (!softAssert)
            {
                ExtentManager.LogStep($"{ex.Message}</br><b>Stack trace:</b><br/><code><pre lang='red'>" +
                                      $"{ex.StackTrace?.Replace("\n", "<br/></code></pre>")}",
                    Status.Error);
                TakeScreenShot();
                Assert.Fail(ex.Message);
            }

            ExtentManager.LogStep($"{ex.Message}</br><b>Stack trace:</b><br/><code><pre lang='red'>" +
                                  $"{ex.StackTrace?.Replace("\n", "<br/></code></pre>")}", Status.Warning);
            Assert.Warn(ex.Message);
        }
    }

    public void AssertElementIsVisible(By locator, bool expected, bool softAssert = true)
    {
        var visible = IsVisible(locator);
        var jsonText = JsonReportText(new Dictionary<string, object>
        {
            { "element", locator }
        });
        if (visible && expected)
        {
            ExtentManager.LogStep("Element <b>IS</b> visible and <b>SHOULD</b> be visible" + jsonText, Status.Pass);
            if (!softAssert)
                Assert.Pass("Element IS visible and SHOULD be visible");
        }
        else if (visible && !expected)
        {
            if (!softAssert)
            {
                ExtentManager.LogStep("Element <b>IS</b> visible and <b>SHOULD NOT</b> be visible" + jsonText,
                    Status.Error);
                TakeScreenShot();
                Assert.Fail("Element IS visible and SHOULD NOT be visible");
            }

            ExtentManager.LogStep("Element <b>IS</b> visible and <b>SHOULD NOT</b> be visible" + jsonText,
                Status.Warning);
        }
        else if (!visible && expected)
        {
            if (!softAssert)
            {
                ExtentManager.LogStep("Element <b>IS NOT</b> visible and <b>SHOULD</b> be visible" + jsonText,
                    Status.Error);
                TakeScreenShot();
                Assert.Fail("Element IS NOT visible and SHOULD BE visible");
            }

            ExtentManager.LogStep("Element <b>IS NOT</b> visible and <b>SHOULD</b> be visible" + jsonText,
                Status.Warning);
        }
        else
        {
            ExtentManager.LogStep("Element <b>IS NOT</b> visible and <b>SHOULD NOT</b> be visible", Status.Pass);
            if (!softAssert)
                Assert.Pass("Element IS NOT visible and SHOULD NOT be visible");
        }
    }

    public void AssertElementIsPresent(By locator, bool expected, bool softAssert = true)
    {
        var isPresent = IsPresent(locator);
        var jsonText = JsonReportText(new Dictionary<string, object>
        {
            { "element", locator }
        });
        if (isPresent && expected)
        {
            ExtentManager.LogStep("Element <b>IS</b> present and <b>SHOULD</b> be present" + jsonText, Status.Pass);
            if (!softAssert)
                Assert.Pass("Element IS present and SHOULD be present");
        }
        else if (isPresent && !expected)
        {
            if (!softAssert)
            {
                ExtentManager.LogStep("Element <b>IS</b> present and <b>SHOULD NOT</b> be present" + jsonText,
                    Status.Error);
                TakeScreenShot();
                Assert.Fail("Element IS present and SHOULD NOT be present");
            }

            ExtentManager.LogStep("Element <b>IS</b> present and <b>SHOULD NOT</b> be present" + jsonText,
                Status.Warning);
        }
        else if (!isPresent && expected)
        {
            if (!softAssert)
            {
                ExtentManager.LogStep("Element <b>IS NOT</b> present and <b>SHOULD</b> be present" + jsonText,
                    Status.Error);
                TakeScreenShot();
                Assert.Fail("Element IS NOT present and SHOULD BE present");
            }

            ExtentManager.LogStep("Element <b>IS NOT</b> present and <b>SHOULD</b> be present" + jsonText,
                Status.Warning);
        }
        else
        {
            ExtentManager.LogStep("Element <b>IS NOT</b> present and <b>SHOULD NOT</b> be present", Status.Pass);
            if (!softAssert)
                Assert.Pass("Element IS NOT present and SHOULD NOT be present");
        }
    }

    #endregion Assertions

    #region Screenshots

    public void TakeScreenShot(bool fullSize = true)
    {
        if (fullSize)
        {
            TakeFullPageScreenShot();
        }
        else
        {
            var extentTest = ExtentTestManager.GetTest();

            var testClassName = TestContext.CurrentContext.Test.ClassName!.Split('.').Last();
            var screenShotName = testClassName + "-" + TestContext.CurrentContext.Test.Name +
                                 RandomNum(3) + ".png";

            var screenShot = ((ITakesScreenshot)driver).GetScreenshot();
            var localPath = ExtentManager.reportFolder + screenShotName;
            screenShot.SaveAsFile(localPath, ScreenshotImageFormat.Png);

            var relPath = MakeRelative(localPath, ExtentManager.reportFolder);
            extentTest.Log(Status.Info, $"Screenshot is available at the end of the test, its name is: <br>" +
                                        $"<pre lang='json'><code>{screenShotName}</code></pre>" +
                                        extentTest.AddScreenCaptureFromPath(relPath));
        }
    }

    public string TakeFullPageScreenShot()
    {
        var extentTest = ExtentTestManager.GetTest();
        var testClassName = TestContext.CurrentContext.Test.ClassName!.Split('.').Last();
        var screenShotName = testClassName + "-" + TestContext.CurrentContext.Test.Name + RandomNum(3) + ".png";

        var html2canvasJs = File.ReadAllText(projectDir + "/html2canvas.js");
        js.Value.ExecuteScript(html2canvasJs);
        var generateScreenshotJS =
            @"
                var canvasImgContentDecoded;
                function genScreenshot () {
                html2canvas(document.body).then(function(canvas) {
                window.canvasImgContentDecoded = canvas.toDataURL(""image/png"");
                console.log(window.canvasImgContentDecoded);
                });
                }
                genScreenshot();";

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        js.Value.ExecuteScript(generateScreenshotJS);
        var encodedPngContent = new object();

        var getScreenShot = "return window.canvasImgContentDecoded;";

        wait.Until(
            _ =>
            {
                encodedPngContent = js.Value.ExecuteScript(getScreenShot);
                return encodedPngContent != null;
            });


        var pngContent = encodedPngContent.ToString()!;
        pngContent = pngContent.Replace("data:image/png;base64,", string.Empty);

        var localPath = ExtentManager.reportFolder + screenShotName;

        var fileSavePath = ExtentManager.reportFolder + screenShotName;
        File.WriteAllBytes(fileSavePath, Convert.FromBase64String(pngContent));
        var relPath = MakeRelative(localPath, ExtentManager.reportFolder);
        extentTest.Log(Status.Info, $"Screenshot is available at the end of the test, its name is: <br>" +
                                    $"<pre lang='json'><code>{screenShotName}</code></pre>" +
                                    extentTest.AddScreenCaptureFromPath(relPath));
        return relPath;
    }

    #endregion
}