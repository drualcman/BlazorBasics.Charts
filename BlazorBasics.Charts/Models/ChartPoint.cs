namespace BlazorBasics.Charts.Models;

internal class ChartPoint
{
    public double X { get; set; }
    public double Y { get; set; }
    public string Value { get; set; }

    public ChartPoint(double x, double y, string value)
    {
        X = x;
        Y = y;
        Value = value;
    }
}
