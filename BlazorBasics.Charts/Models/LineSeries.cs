namespace BlazorBasics.Charts.Models;

internal class LineSeries
{
    public string Name { get; set; }
    public string Color { get; set; }
    public IEnumerable<ChartPoint> Values { get; set; }

    public LineSeries(string name, string color, IEnumerable<ChartPoint> values)
    {
        Name = name;
        Color = string.IsNullOrEmpty(color) ? "black" : color;
        Values = values;
    }
}
