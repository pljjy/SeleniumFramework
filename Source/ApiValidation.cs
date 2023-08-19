using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

    public void RespondBodyAssertion(string requestUrl,
        string expectedJson,
        Dictionary<string, string>? headers = null,
        Method method = Method.Get,
        string? jsonBody = null)

    {
        var options = new RestClientOptions(requestUrl)
        {
            MaxTimeout = -1
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
                log.Info($"Added header - {pair.Key}: '{pair.Value}'");
            }
        }

        if (jsonBody is not null)
        {
            request.AddHeader("Content-Type", "application/json");
            request.AddStringBody(jsonBody, DataFormat.Json);
            log.Info(
                $"Json body provided:<br/><pre lang='json' style='max-height: 700px; overflow-y: scroll; max-width: 1070px;'><code>{jsonBody}</code></pre>");
        }

        var response = client.Execute(request);
        log.Info($"Response code - {response.StatusCode}: '{(int)response.StatusCode}'");
        if (EqualJsons(expectedJson, response.Content))
        {
            log.Pass(
                $"Expected response:<pre lang='json' style='max-height: 700px; overflow-y: scroll; max-width: 1070px;'><code>{expectedJson}</code></pre>" +
                $"Actual response: <pre lang='json' style='max-height: 700px; overflow-y: scroll; max-width: 1070px;'><code>{response.Content}</code></pre><br/>");
            Assert.Pass();
        }
        else
        {
            log.Error(
                $"Expected response:<pre lang='json' style='max-height: 700px; overflow-y: scroll; max-width: 1070px;'><code>{expectedJson}</code></pre>" +
                $"Actual response: <pre lang='json' style='max-height: 700px; overflow-y: scroll; max-width: 1070px;'><code>{response.Content}</code></pre>");
            Assert.Fail();
        }
    }

    public void SuccessAssert(string requestUrl,
        Method method,
        bool expectedStatus = true,
        Dictionary<string, string>? headers = null,
        string? jsonBody = null
    )
    {
        var options = new RestClientOptions(requestUrl)
        {
            MaxTimeout = -1
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
                log.Info($"Added header - {pair.Key}: {pair.Value}");
            }
        }

        if (jsonBody is not null)
        {
            request.AddHeader("Content-Type", "application/json");
            request.AddStringBody(jsonBody, DataFormat.Json);
            log.Info(
                $"Json body provided:<br/><pre lang='json' style='max-height: 700px; overflow-y: scroll; max-width: 1070px;'><code>{jsonBody}</code></pre>");
        }

        var response = client.Execute(request);
        log.Info($"Response code - {response.StatusCode}: {(int)response.StatusCode}");
        if (response.IsSuccessful == expectedStatus)
        {
            log.Pass(
                $"Expected success status - <b>{expectedStatus}</b><br/>Actual success status - <b>{response.IsSuccessful}</b><br/>");
            log.Info(
                $"Response body:<pre lang='json' style='max-height: 700px; overflow-y: scroll; max-width: 1070px;'><code>{response.Content}</pre></code>");
            Assert.Pass();
        }
        else
        {
            log.Error(
                $"Expected success status - <b>{expectedStatus}</b><br/>Actual success status - <b>{response.IsSuccessful}</b><br/>");
            log.Info(
                $"Response body:<pre lang='json' style='max-height: 700px; overflow-y: scroll; max-width: 1070px;'><code>{response.Content}</code></pre>");
            Assert.Fail();
        }
    }

    public void PartialResponseBodyAssert(string requestUrl,
        Method method,
        Dictionary<string, string>? headers = null,
        string expectedJson = "No expected response",
        string? jsonBody = null)
    {
        
    }

    public static bool EqualJsons(string json1, string json2)
    {
        return JToken.DeepEquals(JToken.Parse(json1), JToken.Parse(json2));
    }
    // TODO: make a method with partitial respond test
}