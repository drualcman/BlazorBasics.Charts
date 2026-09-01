namespace BlazorBasics.Charts.Models;

/// <summary>
/// One piece of text of a chart, already placed and, when it needs to be, rotated.
/// </summary>
internal sealed class ChartText
{
    public string Content { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public string Anchor { get; set; }
    public int FontSize { get; set; }
    public string Colour { get; set; }
    public double? RotationAngle { get; set; }

    /// <summary>
    /// Which of the values the text belongs to, or -1 when it is only decoration and nothing
    /// should happen when it is clicked.
    /// </summary>
    public int SegmentIndex { get; set; } = -1;
}
