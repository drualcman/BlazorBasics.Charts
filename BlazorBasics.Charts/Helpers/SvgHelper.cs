using System.Security;

namespace BlazorBasics.Charts.Helpers;

internal static class SvgHelper
{
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    /// <summary>
    /// Formats a double for SVG: maximum 4 decimal places, removes unnecessary zeros, uses decimal point
    /// </summary>
    private static string Format(double value)
    {
        // "0.####" → shows up to 4 decimal places but removes unnecessary ones (e.g. 450 → "450", 38.3333 → "38.3333")
        return value.ToString("0.####", Invariant);
    }

    /// <summary>
    /// Escapes text to make it safe inside an SVG <text> element
    /// </summary>
    private static string Escape(string text)
    {
        // SecurityElement.Escape correctly handles &, <, >, ", '
        return SecurityElement.Escape(text) ?? text;
    }

    internal static string Text(string text, double x, double y, string anchor = "middle", int fontSize = 10)
    {
        return $"<text x=\"{Format(x)}\" y=\"{Format(y)}\" text-anchor=\"{anchor}\" font-size=\"{fontSize}\">{Escape(text)}</text>";
    }

    internal static string Line(int x1, int y1, int x2, int y2)
    {
        // Integers do not need special formatting
        return $"<line x1=\"{x1}\" y1=\"{y1}\" x2=\"{x2}\" y2=\"{y2}\" class=\"grid-line\" />";
    }

    internal static string Rect(double x, double y, double width, double thickness, string color)
    {
        return $"<rect x=\"{Format(x)}\" y=\"{Format(y)}\" width=\"{Format(width)}\" height=\"{Format(thickness)}\" fill=\"{color}\" />";
    }

    internal static string RotatedText(string text, int x, int y, double angleDegrees, int estimatedWidth)
    {
        double angleRadians = ChartMathHelpers.CalculateRadious(angleDegrees);
        double offsetX = estimatedWidth * 0.5 * Math.Cos(angleRadians);
        int xCorrected = x + (int)Math.Round(offsetX);

        // Here we also format the doubles in the transform and escape the text
        return $"<text x=\"{xCorrected}\" y=\"{y}\" text-anchor=\"end\" " +
               $"transform=\"rotate({Format(angleDegrees)},{xCorrected},{y})\" " +
               $"font-size=\"12\">{Escape(text)}</text>";
    }
}