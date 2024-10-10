namespace BlazorBasics.Charts.Helpers;

public static class ChartMathHelpers
{
    public static double CalculatePercentage(double newValue, double originalValue)
    {
        if (originalValue == 0) throw new InvalidOperationException("Original value cannot be zero.");
        return Math.Round(newValue / originalValue * 100, 3);
    }

    public static int CalculatePercentageFromDateTimeToNow(DateTime startTime, DateTime endTime, int maxHours)
    {
        var hoursPassed = (endTime - startTime).TotalHours;
        var percentage = CalculatePercentage(hoursPassed, maxHours == 0 ? 1 : maxHours);
        var remainingTime = Math.Min(Math.Max(percentage, 0), 100);
        return (int)remainingTime;
    }
}