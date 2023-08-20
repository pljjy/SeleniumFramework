using NUnit.Framework;
using SeleniumFramework.Utilities;
using static SeleniumFramework.Tests.WebDriver.Site.SampleDriverTestLocator;
using static SeleniumFramework.Tests.WebDriver.TestData.SimpleDriverTestTData;

namespace SeleniumFramework.Tests.WebDriver.Test;

[Category("driver")]
public class SampleDriverTest : BaseTest
{
    [Test]
    public void SampleDriverTest1()
    {
        driver.Get(url);
        driver.SendKeys(searchInput, search, softAssert: false);
        driver.Submit(searchInput);
        driver.AssertElementIsPresent(By.XPath("//*[contains(text(), 'kitten')]"), true, softAssert: false);
    }
}