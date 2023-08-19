using MongoDB.Bson.IO;
using NUnit.Framework;
using RestSharp;
using SeleniumFramework.Source;
using SeleniumFramework.Utilities;
using JsonConvert = Newtonsoft.Json.JsonConvert;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

// ReSharper disable HeapView.BoxingAllocation

namespace SeleniumFramework.Tests.Api;

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
        string res = File.ReadAllText(GetProjectDirectory() + "/Tests/Api/TestData/SampleApiTData.json");
        apiVal.RespondBodyAssertion("https://reqres.in/api/users?page=2", res);
    }

    [Test]
    public void Test3()
    {
        
    }
    
}