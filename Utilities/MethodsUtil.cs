using Newtonsoft.Json.Linq;

namespace SeleniumFramework.Utilities;

/// <summary>
///     These are some methods that are frequently used throughout the code
/// </summary>
public class MethodsUtil
{
    private static readonly Random random = new();

    public static readonly string projectDir =
        Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent + "";

    public static readonly string pathToJsonFile = projectDir + "/Utilities/config.json";

    /// <summary>
    /// </summary>
    /// <param name="_filePath">Full file path (absolute)</param>
    /// <param name="_rootPath">Full base directory path (absolute)</param>
    /// <returns></returns>
    /// /// &lt;param name="_fromPath"&gt;&lt;/param&gt;
    public static string MakeRelative(string _filePath, string _rootPath)
    {
        var filePath = new Uri(_filePath, UriKind.Absolute);
        var rootPath = new Uri(_rootPath, UriKind.Absolute);

        var relPath = rootPath.MakeRelativeUri(filePath).ToString();
        return relPath;
    }

    /// <summary>
    ///     Returns string with length numbers, each number is random
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string RandomNum(int length)
    {
        const string chars = "0123456789";
        return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    ///     Removes all non number characters from string and returns what's left as an integer
    /// </summary>
    /// <param name="txt"></param>
    /// <returns></returns>
    public static int GetNumFromString(string txt)
    {
        return int.Parse(new string(txt.Where(char.IsDigit).ToArray()));
    }

    /// <summary>
    ///     Converts a .json file to a Dictionary
    /// </summary>
    /// <param name="_path"></param>
    /// <returns></returns>
    public static Dictionary<string, dynamic> JsonFileToDictionary(string _path)
    {
        return JObject.Parse(File.ReadAllText(_path)).ToObject<Dictionary<string, dynamic>>()!;
    }

    /// <summary>
    ///     Used by most methods with logging, adds a json to your report
    /// </summary>
    /// <param name="json"></param>
    /// <param name="keyColor"></param>
    /// <param name="valueColor"></param>
    /// <returns></returns>
    public static string JsonReportText(Dictionary<string, object> json, string keyColor = "red",
        string valueColor = "blue")
    {
        var res = "<br/><pre lang='json' style='max-height: 500px; overflow-y: scroll; max-width: 1070px;'><code>";
        for (var i = 0; i < json.Count; i++)
        {
            res += $"<font color='{keyColor}'>{json.ElementAt(i).Key}: </font>" +
                   $"<font color='{valueColor}'>'{json.ElementAt(i).Value}'</font>";
            if (i + 1 != json.Count)
                res += "<br/>";
        }

        return res + "</code></pre>";
    }
}