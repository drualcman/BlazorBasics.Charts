namespace BlazorBasics.Charts.Models;

/// <summary>
/// Everything a chart made of rectangles needs drawn, worked out beforehand and kept apart from
/// how it is drawn. The same layout is written out as an svg string by GenerateSvg and rendered as
/// real markup by the component, and only the markup can carry the click events.
/// </summary>
internal sealed class ChartLayout
{
    public double Width { get; set; }

    public double Height { get; set; }

    /// <summary>
    /// What goes in the width attribute of the svg when it is not the plain width, so a chart can
    /// ask to be stretched to the element holding it with a percentage.
    /// </summary>
    public string CssWidth { get; set; }

    public string PreserveAspectRatio { get; set; } = "xMidYMid meet";

    public List<ChartShape> Shapes { get; } = [];

    public List<ChartText> Texts { get; } = [];

    public void AddShape(double x, double y, double width, double height, string colour,
        int segmentIndex) =>
        Shapes.Add(new ChartShape
        {
            X = x,
            Y = y,
            Width = width,
            Height = height,
            Colour = colour,
            SegmentIndex = segmentIndex
        });

    public void AddText(string content, double x, double y, string anchor, int fontSize,
        int segmentIndex, string colour = null, double? rotationAngle = null)
    {
        if (!string.IsNullOrEmpty(content))
        {
            Texts.Add(new ChartText
            {
                Content = content,
                X = x,
                Y = y,
                Anchor = anchor,
                FontSize = fontSize,
                Colour = colour,
                RotationAngle = rotationAngle,
                SegmentIndex = segmentIndex
            });
        }
    }
}
