namespace BlazorBasics.Charts.Helpers;
internal static class SvgHelper
{

    internal static string CreateSvgText(string text, int x, int y, string anchor = "middle")
    {
        return $"<text x=\"{x}\" y=\"{y}\" text-anchor=\"{anchor}\" font-size=\"10\">{text}</text>";
    }

    internal static string CreateSvgLine(int x1, int y1, int x2, int y2)
    {
        return $"<line x1=\"{x1}\" y1=\"{y1}\" x2=\"{x2}\" y2=\"{y2}\" class=\"grid-line\" />";
    }

    internal static string CreateRotatedSvgText(string text, int x, int y, double angleDegrees, int estimatedWidth)
    {
        double angleRadians = ChartMathHelpers.CalculateRadious(angleDegrees);

        // Horizontal offset depends on the width and the cosine of the angle
        double offsetX = estimatedWidth * 0.5 * Math.Cos(angleRadians);

        // If angle is negative, we shift to the left instead of right
        int xCorrected = x + (int)Math.Round(offsetX);

        return $"<text x=\"{xCorrected}\" y=\"{y}\" text-anchor=\"end\" transform=\"rotate({angleDegrees},{xCorrected},{y})\" font-size=\"12\">{text}</text>";
    }

}
