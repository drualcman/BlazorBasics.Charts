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
    /// Same formatting, for the markup the components render themselves. Razor writes numbers with
    /// the culture of the thread, which in most of Europe would put a comma in the middle of every
    /// coordinate and break the svg, so every number written into markup goes through here.
    /// </summary>
    internal static string Number(double value) => Format(value);

    /// <summary>
    /// Transform attribute of a label that is rotated, and nothing at all for one that is not, so
    /// the attribute is left out of the markup instead of written empty.
    /// </summary>
    internal static string Rotation(ChartText text) =>
        text.RotationAngle.HasValue
            ? $"rotate({Format(text.RotationAngle.Value)},{Format(text.X)},{Format(text.Y)})"
            : null;

    /// <summary>
    /// Writes the whole of a layout as an svg document.
    /// </summary>
    internal static string Document(ChartLayout layout)
    {
        StringBuilder svg = new StringBuilder();

        svg.AppendLine(
            $"<svg width=\"{layout.CssWidth ?? Format(layout.Width)}\" height=\"{Format(layout.Height)}\" " +
            $"viewBox=\"0 0 {Format(layout.Width)} {Format(layout.Height)}\" " +
            $"preserveAspectRatio=\"{layout.PreserveAspectRatio}\" " +
            $"xmlns=\"http://www.w3.org/2000/svg\">");

        foreach (ChartShape shape in layout.Shapes)
        {
            svg.AppendLine(Rect(shape.X, shape.Y, shape.Width, shape.Height, shape.Colour));
        }

        foreach (ChartText text in layout.Texts)
        {
            svg.AppendLine(text.RotationAngle.HasValue
                ? RotatedTextAt(text.Content, text.X, text.Y, text.RotationAngle.Value,
                    text.FontSize, text.Anchor, text.Colour)
                : Text(text.Content, text.X, text.Y, text.Anchor, text.FontSize, text.Colour));
        }

        svg.AppendLine("</svg>");

        return svg.ToString();
    }

    /// <summary>
    /// Escapes text to make it safe inside an SVG <text> element
    /// </summary>
    private static string Escape(string text)
    {
        // SecurityElement.Escape correctly handles &, <, >, ", '
        return SecurityElement.Escape(text) ?? text;
    }

    internal static string Text(string text, double x, double y, string anchor = "middle", int fontSize = 10,
        string colour = null)
    {
        return $"<text x=\"{Format(x)}\" y=\"{Format(y)}\" text-anchor=\"{anchor}\" font-size=\"{fontSize}\"{Fill(colour)}>{Escape(text)}</text>";
    }

    private static string Fill(string colour) =>
        string.IsNullOrWhiteSpace(colour) ? string.Empty : $" fill=\"{colour}\"";

    internal static string Line(int x1, int y1, int x2, int y2)
    {
        // Integers do not need special formatting
        return $"<line x1=\"{x1}\" y1=\"{y1}\" x2=\"{x2}\" y2=\"{y2}\" class=\"grid-line\" />";
    }

    internal static string Rect(double x, double y, double width, double thickness, string color)
    {
        return $"<rect x=\"{Format(x)}\" y=\"{Format(y)}\" width=\"{Format(width)}\" height=\"{Format(thickness)}\" fill=\"{color}\" />";
    }

    /// <summary>
    /// Draws a text rotated around its anchor point. Negative angles read bottom to top and
    /// positive angles read top to bottom; in both cases the text grows away from the anchor,
    /// so the anchor is the edge closest to whatever the label belongs to.
    /// </summary>
    internal static string RotatedText(string text, double x, double y, double angleDegrees,
        double estimatedWidth, int fontSize)
    {
        double angleRadians = ChartMathHelpers.CalculateRadious(angleDegrees);
        double offsetAlongText = estimatedWidth * 0.5 * Math.Cos(angleRadians);
        double offsetAcrossText = fontSize * 0.25 * Math.Sin(angleRadians);
        double correctedX = x + offsetAlongText - offsetAcrossText;
        string anchor = angleDegrees > 0 ? "start" : "end";

        return RotatedTextAt(text, correctedX, y, angleDegrees, fontSize, anchor);
    }

    /// <summary>
    /// Draws a text rotated around the given point, with no centring of any kind: the point is
    /// exactly where the baseline starts or ends, depending on the anchor.
    /// </summary>
    internal static string RotatedTextAt(string text, double x, double y, double angleDegrees,
        int fontSize, string anchor, string colour = null)
    {
        return $"<text x=\"{Format(x)}\" y=\"{Format(y)}\" text-anchor=\"{anchor}\" " +
               $"transform=\"rotate({Format(angleDegrees)},{Format(x)},{Format(y)})\" " +
               $"font-size=\"{fontSize}\"{Fill(colour)}>{Escape(text)}</text>";
    }

    internal static string RotatedText(string text, int x, int y, double angleDegrees, int estimatedWidth) =>
        RotatedText(text, x, y, angleDegrees, estimatedWidth, 12);
    //{
    //    double angleRadians = ChartMathHelpers.CalculateRadious(angleDegrees);
    //    double offsetX = estimatedWidth * 0.5 * Math.Cos(angleRadians);
    //    int xCorrected = x + (int)Math.Round(offsetX);

    //    // Here we also format the doubles in the transform and escape the text
    //    return $"<text x=\"{xCorrected}\" y=\"{y}\" text-anchor=\"end\" " +
    //           $"transform=\"rotate({Format(angleDegrees)},{xCorrected},{y})\" " +
    //           $"font-size=\"12\">{Escape(text)}</text>";
    //}
}