using AventStack.ExtentReports;

namespace SeleniumFramework.Utilities;

/// <summary>
/// This class is all the features i use with ExtentTest but with highlighting 
/// </summary>
public class ReportClass
{
    public readonly ExtentTest logger;

    public ReportClass(string name, ExtentReports extent)
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
        text = "<font color = 'blue'>" + text + "<font/>";
        logger.Log(Status.Debug, text, provider);
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
        text = "<b><i><font color = 'red'>" + text + "<font/><i/><b/>";
        logger.Log(Status.Error, text, provider);
    }
}