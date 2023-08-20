using AventStack.ExtentReports;
using Newtonsoft.Json.Linq;
using RestSharp;
using SeleniumFramework.Utilities;

namespace SeleniumFramework.Source;

public static class ApiValidation
{
    /// <summary>
    ///     aompares response content to an expected one
    /// </summary>
    /// <param name="requestUrl"></param>
    /// <param name="expectedJson"></param>
    /// <param name="headers"></param>
    /// <param name="method"></param>
    /// <param name="jsonBody"></param>
    public static void PreciseResponseBodyAssertion(string requestUrl,
        string expectedJson,
        Dictionary<string, string>? headers = null,
        Method method = Method.Get,
        string? jsonBody = null)
    {
        var response = GetResponse(requestUrl, method, headers, jsonBody);

        if (EqualJsons(expectedJson, response.Content!))
        {
            ExtentManager.LogStep(
                $"Expected response:<pre lang='json' style='max-height: 700px; overflow-y: scroll; max-width: 1070px;'><code>{expectedJson}</code></pre>" +
                $"Actual response: <pre lang='json' style='max-height: 700px; overflow-y: scroll; max-width: 1070px;'><code>{response.Content}</code></pre><br/>",
                Status.Pass);
            Assert.Pass();
        }
        else
        {
            ExtentManager.LogStep(
                $"Expected response:<pre lang='json' style='max-height: 700px; overflow-y: scroll; max-width: 1070px;'><code>{expectedJson}</code></pre>" +
                $"Actual response: <pre lang='json' style='max-height: 700px; overflow-y: scroll; max-width: 1070px;'><code>{response.Content}</code></pre>",
                Status.Error);
            Assert.Fail();
        }
    }

    /// <summary>
    ///     asserts if response status is the same as expected
    /// </summary>
    /// <param name="requestUrl"></param>
    /// <param name="method"></param>
    /// <param name="expectedStatus"></param>
    /// <param name="headers"></param>
    /// <param name="jsonBody"></param>
    public static void SuccessAssert(string requestUrl,
        Method method,
        bool expectedStatus = true,
        Dictionary<string, string>? headers = null,
        string? jsonBody = null
    )
    {
        var response = GetResponse(requestUrl, method, headers, jsonBody);

        if (response.IsSuccessful == expectedStatus)
        {
            ExtentManager.LogStep(
                $"Expected success status - <b>{expectedStatus}</b><br/>Actual success status - <b>{response.IsSuccessful}</b><br/>",
                Status.Pass);
            ExtentManager.LogStep(
                $"Response body:<pre lang='json' style='max-height: 700px; overflow-y: scroll; max-width: 1070px;'><code>{response.Content}</pre></code>");
            Assert.Pass();
        }
        else
        {
            ExtentManager.LogStep(
                $"Expected success status - <b>{expectedStatus}</b><br/>Actual success status - <b>{response.IsSuccessful}</b><br/>",
                Status.Error);
            ExtentManager.LogStep(
                $"Response body:<pre lang='json' style='max-height: 700px; overflow-y: scroll; max-width: 1070px;'><code>{response.Content}</code></pre>");
            Assert.Fail();
        }
    }

    /// <summary>
    ///     checks if response contains an expected string
    /// </summary>
    /// <param name="requestUrl"></param>
    /// <param name="method"></param>
    /// <param name="expectedJson"></param>
    /// <param name="headers"></param>
    /// <param name="jsonBody"></param>
    public static void PartialResponseBodyAssert(string requestUrl,
        Method method,
        string expectedJson,
        Dictionary<string, string>? headers = null,
        string? jsonBody = null)
    {
        var response = GetResponse(requestUrl, method, headers, jsonBody);

        if (response.Content!.Contains(expectedJson))
        {
            ExtentManager.LogStep(
                $"Response: <pre lang='json' style='max-height: 700px; overflow-y: scroll; max-width: 1070px;'><code>{response.Content}</code></pre><br/>" +
                $"Response contains <pre lang='json' style='max-height: 700px; overflow-y: scroll; max-width: 1070px;'><code>{expectedJson}</code></pre>",
                Status.Pass);
            Assert.Pass();
        }
        else
        {
            ExtentManager.LogStep(
                $"Response: <pre lang='json' style='max-height: 700px; overflow-y: scroll; max-width: 1070px;'><code>{response.Content}</code></pre><br/>" +
                $"Response <b>DOES NOT</b> contain <pre lang='json' style='max-height: 700px; overflow-y: scroll; max-width: 1070px;'><code>{expectedJson}</code></pre>)",
                Status.Error);
            Assert.Fail();
        }
    }

    /// <summary>
    ///     makes an api request with provided info, returns the response
    ///     should be by other methods
    /// </summary>
    /// <param name="requestUrl"></param>
    /// <param name="method"></param>
    /// <param name="headers"></param>
    /// <param name="jsonBody"></param>
    /// <returns></returns>
    public static RestResponse GetResponse(string requestUrl,
        Method method,
        Dictionary<string, string>? headers = null,
        string? jsonBody = null)
    {
        var options = new RestClientOptions(requestUrl)
        {
            MaxTimeout = -1
        };
        var client = new RestClient(options);
        ExtentManager.LogStep("Initialized client");
        var request = new RestRequest("", method);
        ExtentManager.LogStep("Request method is " + method);
        if (headers != null)
            foreach (var pair in headers)
            {
                request.AddHeader(pair.Key, pair.Value);
                ExtentManager.LogStep($"Added header - {pair.Key}: '{pair.Value}'");
            }

        if (jsonBody is not null)
        {
            request.AddHeader("Content-Type", "application/json");
            request.AddStringBody(jsonBody, DataFormat.Json);
            ExtentManager.LogStep(
                $"Json body provided:<br/><pre lang='json' style='max-height: 700px; overflow-y: scroll; max-width: 1070px;'><code>{jsonBody}</code></pre>");
        }

        var response = client.Execute(request);
        ExtentManager.LogStep($"Response code - {response.StatusCode}: '{(int)response.StatusCode}'");

        return response;
    }

    public static bool EqualJsons(string json1, string json2)
    {
        return JToken.DeepEquals(JToken.Parse(json1), JToken.Parse(json2));
    }
}