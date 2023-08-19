using AventStack.ExtentReports;

namespace SeleniumFramework.Utilities;

/// <summary>
/// This class is all the features i use with ExtentTest but with highlighting 
/// </summary>
public class Reporter
{
    public readonly ExtentTest logger;

    public Reporter(string name, ExtentReports extent)
    {
        logger = extent.CreateTest(name);
    }

    public void Pass(string text, MediaEntityModelProvider? provider = null)
    {
        text = "<font color = 'green'>" + text + "<font/>";
        logger.Log(Status.Pass, text, provider);
    }

    public void Debug(string text, MediaEntityModelProvider? provider = null)
    {
        text = "<font color = 'gray'>" + text + "<font/>";
        logger.Log(Status.Debug, text, provider);
    }

    public void Info(string text, MediaEntityModelProvider? provider = null)
    {
        text = "<font color = '#026592'>" + text + "<font/>"; // #026592 - shape of blue
        logger.Log(Status.Info, text, provider);
    }

    public void Warning(string text, MediaEntityModelProvider? provider = null)
    {
        text = "<font color = 'yellow'>" + text + "<font/>";
        logger.Log(Status.Warning, text, provider);
    }

    public void Error(string text, MediaEntityModelProvider? provider = null)
    {
        text = "<font color = 'red'>" + text + "<font/>";
        logger.Log(Status.Error, text, provider);
    }

    public void Fatal(string text, MediaEntityModelProvider? provider = null)
    {
        text = "<b><i><font color = '#8b0000'>" + text + "<font/><i/><b/>"; // #8b0000 - dark red
        logger.Log(Status.Fatal, text, provider);
    }

    public void BySeverity(string text, Status severity, MediaEntityModelProvider? provider = null)
    {
        switch (severity)
        {
            case Status.Pass:
                Pass(text, provider);
                break;
            case Status.Debug:
                Debug(text, provider);
                break;
            case Status.Warning:
                Warning(text, provider);
                break;
            case Status.Error:
                Error(text, provider);
                break;
            case Status.Fatal:
                Fatal(text, provider);
                break;
            default:
                Info(text, provider);
                break;
        }
    }
}