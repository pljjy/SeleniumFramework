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
        driver.SendKeys(searchInput, search, false);
        driver.Submit(searchInput);
        driver.TakeScreenShot();
        driver.AssertElementIsPresent(By.XPath("//*[contains(text(), 'kitten')]"), true, false);
    }
}