namespace BlazorBasics.Charts;

/// <summary>
/// Draws every value as a slice of one single bar, so what is read is the share each value takes
/// of the whole. The bar runs across or down depending on the orientation, and the labels turn
/// with it: horizontal beside a vertical bar, vertical above or below a horizontal one.
/// </summary>
public partial class StackedBarChartComponent
{
    [Parameter] public IEnumerable<ChartSegment> Topics { get; set; }
    [Parameter] public StackedBarChartParams Parameters { get; set; } = new();

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; }

    private const double AverageCharacterWidthFactor = 0.55;
    private const double GlyphAscentFactor = 0.75;
    private const double GlyphDescentFactor = 0.25;
    private const double LabelGap = 6;
    private const double LabelPadding = 8;
    private const double VerticalLabelAngle = -90;

    private MarkupString SvgMarkup = new();
    private string WrapperCss = "";

    private bool IsVertical => Parameters.Orientation == StackedBarOrientation.Vertical;

    private string OrientationCss => IsVertical ? "is-vertical" : "is-horizontal";

    protected override void OnParametersSet()
    {
        if (Attributes is not null && Attributes.TryGetValue("class", out object css))
            WrapperCss = css.ToString();
        SvgMarkup = new MarkupString(GenerateSvg());
    }

    public string GenerateSvg()
    {
        List<ChartSegment> segments = Topics is null ? [] : [.. Topics];
        double total = TotalOf(segments);

        double length = Parameters.Length;
        double thickness = Parameters.Thickness;
        double labelBand = LabelBandSize(segments, total);

        double barCrossStart = Parameters.LabelSide == StackedBarLabelSide.Before ? labelBand : 0;
        double crossSize = thickness + labelBand;

        double totalWidth = IsVertical ? crossSize : length;
        double totalHeight = IsVertical ? length : crossSize;

        StringBuilder svg = new StringBuilder();

        svg.AppendLine(
            $"<svg width=\"{Number(totalWidth)}\" height=\"{Number(totalHeight)}\" " +
            $"viewBox=\"0 0 {Number(totalWidth)} {Number(totalHeight)}\" " +
            $"preserveAspectRatio=\"xMidYMid meet\" xmlns=\"http://www.w3.org/2000/svg\">");

        svg.AppendLine(BarRect(0, length, barCrossStart, thickness, Parameters.BackgroundColour));

        double mainOffset = 0;

        for (int index = 0; index < segments.Count; index++)
        {
            ChartSegment segment = segments[index];
            ChartColor colour = Parameters.ChartColors[index % Parameters.MaxColours];
            string background = string.IsNullOrWhiteSpace(segment.ChartColor)
                ? colour.Background
                : segment.ChartColor;
            string foreground = string.IsNullOrWhiteSpace(segment.LabelColor)
                ? colour.Foreground
                : segment.LabelColor;

            double share = total > 0 ? segment.Value / total : 0;
            double mainSize = length * share;

            svg.AppendLine(BarRect(mainOffset, mainSize, barCrossStart, thickness, background));

            if (share >= Parameters.MinimumLabelShare)
            {
                svg.AppendLine(SegmentLabel(
                    segment.Name, mainOffset, mainSize, barCrossStart, thickness, foreground));
            }

            if (Parameters.ShowValues && Parameters.LabelSide != StackedBarLabelSide.Inside)
            {
                svg.AppendLine(SegmentValue(
                    segment.Value, mainOffset, mainSize, barCrossStart, thickness, foreground));
            }

            mainOffset += mainSize;
        }

        svg.AppendLine("</svg>");

        return svg.ToString();
    }

    private double TotalOf(List<ChartSegment> segments)
    {
        double valuesTotal = segments.Sum(segment => segment.Value);
        return Parameters.Total > 0 ? Parameters.Total : valuesTotal;
    }

    /// <summary>
    /// Room the labels need across the bar, which is the length of the longest one that is
    /// actually going to be drawn.
    /// </summary>
    private double LabelBandSize(List<ChartSegment> segments, double total)
    {
        double result = 0;

        if (Parameters.LabelSide != StackedBarLabelSide.Inside)
        {
            double longestLabel = segments
                .Where(segment => total <= 0 || segment.Value / total >= Parameters.MinimumLabelShare)
                .Select(segment => EstimatedTextWidth(segment.Name))
                .DefaultIfEmpty(0)
                .Max();

            result = longestLabel > 0 ? longestLabel + LabelPadding : 0;
        }

        return result;
    }

    private string BarRect(double mainStart, double mainSize, double crossStart, double thickness,
        string colour) =>
        IsVertical
            ? SvgHelper.Rect(crossStart, mainStart, thickness, mainSize, colour)
            : SvgHelper.Rect(mainStart, crossStart, mainSize, thickness, colour);

    private string SegmentLabel(string label, double mainStart, double mainSize, double crossStart,
        double thickness, string foreground)
    {
        int fontSize = Parameters.LabelFontSize;
        double ascent = fontSize * GlyphAscentFactor;
        double descent = fontSize * GlyphDescentFactor;
        bool inside = Parameters.LabelSide == StackedBarLabelSide.Inside;
        string colour = inside ? foreground : null;
        string result;

        if (IsVertical)
        {
            double baselineY = Parameters.LabelAlignment switch
            {
                StackedBarLabelAlignment.Start => mainStart + ascent,
                StackedBarLabelAlignment.End => mainStart + mainSize - descent,
                _ => mainStart + (mainSize / 2) + ((ascent - descent) / 2)
            };

            double baselineX = Parameters.LabelSide switch
            {
                StackedBarLabelSide.Before => crossStart - LabelGap,
                StackedBarLabelSide.After => crossStart + thickness + LabelGap,
                _ => crossStart + LabelGap
            };

            string anchor = Parameters.LabelSide == StackedBarLabelSide.Before ? "end" : "start";

            result = SvgHelper.Text(label, baselineX, baselineY, anchor, fontSize, colour);
        }
        else
        {
            double alignedMain = Parameters.LabelAlignment switch
            {
                StackedBarLabelAlignment.Start => mainStart + (fontSize * 0.5),
                StackedBarLabelAlignment.End => mainStart + mainSize - (fontSize * 0.5),
                _ => mainStart + (mainSize / 2)
            };

            // Rotated a quarter turn the glyphs grow towards the left of the baseline, so the
            // baseline is pushed back half the difference to leave the text centred on the point.
            double baselineX = alignedMain + ((ascent - descent) / 2);

            double baselineY = Parameters.LabelSide switch
            {
                StackedBarLabelSide.Before => crossStart - LabelGap,
                StackedBarLabelSide.After => crossStart + thickness + LabelGap,
                _ => crossStart + thickness - LabelGap
            };

            string anchor = Parameters.LabelSide == StackedBarLabelSide.After ? "end" : "start";

            result = SvgHelper.RotatedTextAt(
                label, baselineX, baselineY, VerticalLabelAngle, fontSize, anchor, colour);
        }

        return result;
    }

    /// <summary>
    /// Value written across the middle of its own segment, and only when the segment is long
    /// enough to hold it without spilling over the segments next to it.
    /// </summary>
    private string SegmentValue(double value, double mainStart, double mainSize, double crossStart,
        double thickness, string foreground)
    {
        int fontSize = Parameters.LabelFontSize;
        string text = value.ToString(CultureInfo.InvariantCulture);
        double textWidth = EstimatedTextWidth(text);
        double roomForTheTextWidth = IsVertical ? thickness : mainSize;
        double roomForTheTextHeight = IsVertical ? mainSize : thickness;
        string result = string.Empty;

        if (roomForTheTextWidth >= textWidth + LabelPadding && roomForTheTextHeight >= fontSize)
        {
            double centreMain = mainStart + (mainSize / 2);
            double centreCross = crossStart + (thickness / 2);
            double x = IsVertical ? centreCross : centreMain;
            double y = (IsVertical ? centreMain : centreCross) + (fontSize * 0.35);

            result = SvgHelper.Text(text, x, y, "middle", fontSize, foreground);
        }

        return result;
    }

    private double EstimatedTextWidth(string text) =>
        (text?.Length ?? 0) * Parameters.LabelFontSize * AverageCharacterWidthFactor;

    private static string Number(double value) =>
        value.ToString("0.####", CultureInfo.InvariantCulture);
}
