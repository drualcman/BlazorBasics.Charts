namespace BlazorBasics.Charts.Models;
public class RingParams
{
    public int Width { get; init; }
    public int Height { get; init; }
    public double FontSizeRatio { get; init; }
    public string LabelColor { get; init; }
    public string FromColor { get; init; }
    public string ToColor { get; init; }
    public string CircunferenceColour { get; init; }
    public int StrokeWidth { get; init; }

    public RingParams(int width = 0, int height = 0, double fontPerspective = 3.5, string labelColor = "green",
        string fromColor = "#FFD700", string toColor = "#B22222", string circunferenceColour = "#eee", int strokeWidth = 10)
    {
        Width = width;
        Height = height;
        FontSizeRatio = fontPerspective;
        LabelColor = labelColor;
        FromColor = fromColor;
        ToColor = toColor;
        CircunferenceColour = circunferenceColour;
        StrokeWidth = strokeWidth;
    }
}
