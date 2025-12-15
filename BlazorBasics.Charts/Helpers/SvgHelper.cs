namespace BlazorBasics.Charts.Helpers;

internal static class SvgHelper
{
    internal static string Text(string text, double x, double y, string anchor = "middle", int fontSize = 10)
    {
        return $"<text x=\"{x}\" y=\"{y}\" text-anchor=\"{anchor}\" font-size=\"{fontSize}\">{text}</text>";
    }

    internal static string Line(int x1, int y1, int x2, int y2)
    {
        return $"<line x1=\"{x1}\" y1=\"{y1}\" x2=\"{x2}\" y2=\"{y2}\" class=\"grid-line\" />";
    }

    internal static string Rect(double x, double y, double width, double thickness, string color)
    {
        return $"<rect x=\"{x}\" y=\"{y}\" width=\"{width}\" height=\"{thickness}\" fill=\"{color}\" />";
    }

    internal static string RotatedText(string text, int x, int y, double angleDegrees, int estimatedWidth)
    {
        double angleRadians = ChartMathHelpers.CalculateRadious(angleDegrees);

        // Horizontal offset depends on the width and the cosine of the angle
        double offsetX = estimatedWidth * 0.5 * Math.Cos(angleRadians);

        // If angle is negative, we shift to the left instead of right
        int xCorrected = x + (int)Math.Round(offsetX);

        return $"<text x=\"{xCorrected}\" y=\"{y}\" text-anchor=\"end\" transform=\"rotate({angleDegrees},{xCorrected},{y})\" font-size=\"12\">{text}</text>";
    }

}
