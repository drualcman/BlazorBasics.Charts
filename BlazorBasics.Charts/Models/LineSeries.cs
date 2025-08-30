namespace BlazorBasics.Charts.Models;

internal class LineSeries
{
    public string Name { get; set; }
    public string Color { get; set; }
    public IEnumerable<ChartPoint> Values { get; set; }
    public string PointsString { get; set; }
    public ChartPoint MinPoint { get; }
    public ChartPoint MaxPoint { get; }

    public LineSeries(string name, string color, IEnumerable<ChartPoint> values, ChartPoint min, ChartPoint max)
    {
        Name = name;
        Color = color;
        Values = values;
        MinPoint = min;
        MaxPoint = max;
    }
}
