using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using SeleniumFramework.Utilities;
// ReSharper disable HeapView.BoxingAllocation

namespace SeleniumFramework.Tests.Api;

public class SampleAPITest : BaseTest
{
    [Test]
    public async Task SampleTestApi()
    {
        var options = new RestClientOptions("https://api.github.com")
        {
            MaxTimeout = -1,
        };
        var client = new RestClient(options);
        var request = new RestRequest("/", Method.Get);
        log.Info("Request sent" + JsonReportText(new Dictionary<string, object>
        {
            {"requestUrl", options.BaseUrl + request.Resource},
            {"requestMethod", request.Method}
        }));
        RestResponse response = await client.ExecuteAsync(request);
        Dictionary<string, string>? json =
            JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content ??"{'error': 'NO RESPONSE FOUND'}");
        
        Assert.That(json!["user_url"], Is.EqualTo("https://api.github.com/users/{user}"));
    }
}