namespace BlazorBasics.Charts.Models;

public class LineData
{
    public string Name { get; set; }
    public string Color { get; set; }
    public IEnumerable<string> Values { get; set; }

    public LineData(string name, string color, IEnumerable<string> values)
    {
        Name = name;
        Color = color;
        Values = values;
    }
}