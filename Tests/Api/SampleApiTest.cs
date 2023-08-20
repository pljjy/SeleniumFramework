using NUnit.Framework;
using RestSharp;
using SeleniumFramework.Source;
using SeleniumFramework.Utilities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

// ReSharper disable HeapView.BoxingAllocation

namespace SeleniumFramework.Tests.Api;

[Parallelizable(ParallelScope.Children)]
[Category("api")]
public class SampleApiTest : BaseTest
{
    public ApiValidation apiVal;

    [SetUp]
    public void SetUp()
    {
        apiVal = new ApiValidation(log);
        driverTest = false;
    }

    [Test]
    public void Test1()
    {
        apiVal.SuccessAssert("https://library-api.postmanlabs.com/books/",
            Method.Post,
            true,
            new Dictionary<string, string> { { "api-key", "postmanrulz" } },
            "{\r\n    \"title\":\"Adachi chungus\",\r\n    \"author\":\"me :p\",\r\n    \"genre\":\"amogus gameplay\",\r\n    \"yearPublished\":100\r\n}");
    }

    [Test]
    public void Test2()
    {
        string res = File.ReadAllText(projectDir + "/Tests/Api/TestData/SampleApiTData.json");
        apiVal.PreciseResponseBodyAssertion("https://reqres.in/api/users?page=2", res);
    }

    [Test]
    public void Test3()
    {
        string contains = "\"first_name\":\"Janet\"";
        apiVal.PartialResponseBodyAssert("https://reqres.in/api/users/2",
            Method.Get,
            contains);
    }
}