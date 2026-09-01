namespace BlazorBasics.Charts.Models;

/// <summary>
/// The point of a line chart that was clicked, told in the terms the consumer gave the data in
/// rather than in the coordinates the chart drew it at.
/// </summary>
public class LineChartPoint
{
    public LineChartPoint(string seriesName, string colour, int index, string value, string label)
    {
        SeriesName = seriesName;
        Colour = colour;
        Index = index;
        Value = value;
        Label = label;
    }

    /// <summary>Name of the series the point belongs to.</summary>
    public string SeriesName { get; }

    public string Colour { get; }

    /// <summary>Position of the point inside the values of its series, counting from zero.</summary>
    public int Index { get; }

    /// <summary>The value as it was given, before being parsed into a number.</summary>
    public string Value { get; }

    /// <summary>
    /// The label of the X axis at that position, when the chart was given labels for it.
    /// </summary>
    public string Label { get; }
}
