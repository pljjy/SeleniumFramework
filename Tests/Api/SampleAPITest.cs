using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using SeleniumFramework.Source;
using SeleniumFramework.Utilities;

// ReSharper disable HeapView.BoxingAllocation

namespace SeleniumFramework.Tests.Api;

public class SampleAPITest : BaseTest
{
    public ApiValidation apiVal;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        apiVal = new ApiValidation(log);
        driverTest = false;
    }
        
    [Test]
    public void Test1()
    {
        apiVal.ApiAssert("https://library-api.postmanlabs.com/books?genres=fiction",
            headers: new Dictionary<string, string> { { "api-key", "postmanrulz" } },
            method: Method.Post,
            jsonBody: "{\r\n    \"title\":\"Adachi chungus\",\r\n    \"author\":\"me :p\",\r\n    \"genre\":\"amogus gameplay\",\r\n    \"yearPublished\":100\r\n}");
        // didn't add expected json since it inclueds id and time which are unpredictable
        // TODO: make a method with partitial respond test and success test as a different test
    }

    [Test]
    [Ignore("")]
    public void Test2()
    {
        // apiVal.ApiAssert();
    }
}