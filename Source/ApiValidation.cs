using NUnit.Framework;
using RestSharp;
using SeleniumFramework.Utilities;

namespace SeleniumFramework.Source;

public class ApiValidation
{
    private Reporter log;

    public ApiValidation(Reporter log)
    {
        this.log = log;
    }

    public void ApiAssert(string requestUrl,
        bool expectedStatus = true,
        string expectedJson = "No expected response",
        Dictionary<string, string>? headers = null,
        Method method = Method.Get,
        string? jsonBody = null,
        int maxTimeOut = -1)

    {
        var options = new RestClientOptions(requestUrl)
        {
            MaxTimeout = maxTimeOut
        };
        var client = new RestClient(options);
        log.Info("Initialized client");
        var request = new RestRequest("", method);
        log.Info("Request method is " + method);
        if (headers != null)
        {
            foreach (var pair in headers)
            {
                request.AddHeader(pair.Key, pair.Value);
                log.Info($"Added header {pair.Key}: '{pair.Value}'");
            }
        }

        if (jsonBody is not null)
        {
            request.AddHeader("Content-Type", "application/json");
            request.AddStringBody(jsonBody, DataFormat.Json);
            log.Info(
                $"Added json body:<br/><pre lang='json'><code>{jsonBody}</code></pre>");
        }

        var response = client.Execute(request);
        log.Info($"Response code - {response.ResponseStatus}: {response.StatusCode}");
        if (response.IsSuccessful == expectedStatus || expectedJson == response.Content)
        {
            log.Pass(
                $"Expected success status - <b>{expectedStatus}</b>;<br/>Actual success status - <b>{response.IsSuccessful}</b><br/>" +
                $"Actual response: <pre lang='json' style='max-height: 700px; overflow-y: scroll; max-width: 1070px;'><code>{response.Content}</code></pre>" +
                $"Expected response:<pre lang='json' style='max-height: 700px; overflow-y: scroll; max-width: 1070px;'><code>{expectedJson}</code></pre>");
            Assert.Pass();
        }
        else
        {
            log.Error(
                $"Expected success status - <b>{expectedStatus}</b>;<br/>Actual success status - <b>{response.IsSuccessful}</b><br/>" +
                $"Actual response: <pre lang='json' style='max-height: 700px; overflow-y: scroll; max-width: 1070px;'><code>{response.Content}</code></pre>" +
                $"Expected response:<pre lang='json' style='max-height: 700px; overflow-y: scroll; max-width: 1070px;'><code>{expectedJson}</code></pre>");
            Assert.Fail();
        }
        
        
    }
}