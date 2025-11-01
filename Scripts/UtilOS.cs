
namespace Util;

public static class UtilOS
{
    public static Dictionary<string, string> Args { get; } = [];

    static UtilOS()
    {
        var args = OS.GetCmdlineArgs();
        string lastKey = "";
        foreach (var item in args)
        {
            if (item.StartsWith('-'))
            {
                lastKey = item.ToLower().Replace("-", "");
                Args.Add(lastKey, "");
            }
            else
                Args[lastKey] = item;
        }
    }

}
