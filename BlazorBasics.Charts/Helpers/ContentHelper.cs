namespace BlazorBasics.Charts.Helpers;

public static class ContentHelper
{
    public static string ContentPath(Assembly assembly)
    {
        return $"_content/{assembly.GetName().Name}";
    }

    public static string ReplaceSpaceWithPlus(this string text)
    {
        string result;
        if (string.IsNullOrEmpty(text)) result = text;
        else result = text.Replace(' ', '+');
        return result;
    }
}