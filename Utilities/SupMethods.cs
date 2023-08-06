using AventStack.ExtentReports;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace SeleniumFramework.Utilities;

/// <summary>
/// These are some methods that are frequently used throughout the code
/// </summary>
public class SupMethods
{
    public static readonly string pathToJsonFile = GetProjectDirectory() + @"/Utilities/config.json";

    public static int GetNumFromString(string txt)
    {
        return int.Parse(new string(txt.Where(char.IsDigit).ToArray()));
    }

    public static Dictionary<string, dynamic> JsonFileToDictionary(string _path)
    {
        return JObject.Parse(File.ReadAllText(_path)).ToObject<Dictionary<string, dynamic>>()!;
    }

    public static string GetProjectDirectory()
    {
        return Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent + "";
    }
}