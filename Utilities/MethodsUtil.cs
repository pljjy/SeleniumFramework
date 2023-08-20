using Newtonsoft.Json.Linq;

namespace SeleniumFramework.Utilities;

/// <summary>
/// These are some methods that are frequently used throughout the code
/// </summary>
public class MethodsUtil
{
    public static readonly string projectDir =
        Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent + "";
    public static readonly string pathToJsonFile = projectDir + @"/Utilities/config.json";

    public static int GetNumFromString(string txt)
    {
        return int.Parse(new string(txt.Where(char.IsDigit).ToArray()));
    }

    public static Dictionary<string, dynamic> JsonFileToDictionary(string _path)
    {
        return JObject.Parse(File.ReadAllText(_path)).ToObject<Dictionary<string, dynamic>>()!;
    }

    /// <summary>
    /// Json used a lot for reports, if you need to change something about it, change it here
    /// </summary>
    /// <param name="json"></param>
    /// <param name="keyColor"></param>
    /// <param name="valueColor"></param>
    /// <returns></returns>
    public static string JsonReportText(Dictionary<string, object> json, string keyColor = "red",
        string valueColor = "blue")
    {
        string res = "<br/><pre lang='json' style='max-height: 500px; overflow-y: scroll; max-width: 1070px;'><code>";
        for (int i = 0; i < json.Count; i++)
        {
            res += $"<font color='{keyColor}'>{json.ElementAt(i).Key}: </font>" +
                   $"<font color='{valueColor}'>'{json.ElementAt(i).Value}'</font>";
            if (i + 1 != json.Count)
                res += "<br/>";
        }

        return res + "</code></pre>";
    }
}