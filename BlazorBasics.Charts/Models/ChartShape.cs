namespace BlazorBasics.Charts.Models;

/// <summary>
/// One rectangle of a chart, already placed and coloured.
/// </summary>
internal sealed class ChartShape
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public string Colour { get; set; }

    /// <summary>
    /// Which of the values the shape belongs to, or -1 when it is only decoration and nothing
    /// should happen when it is clicked.
    /// </summary>
    public int SegmentIndex { get; set; } = -1;
}
