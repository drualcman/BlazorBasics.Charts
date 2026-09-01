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

    /// <summary>
    /// Raised with the value behind the segment that was clicked, or behind its label.
    /// </summary>
    [Parameter] public EventCallback<ChartSegment> OnClick { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; }

    private const double AverageCharacterWidthFactor = 0.55;
    private const double GlyphAscentFactor = 0.75;
    private const double GlyphDescentFactor = 0.25;
    private const double LabelGap = 6;
    private const double LabelPadding = 8;
    private const double VerticalLabelAngle = -90;

    private string WrapperCss = "";
    private List<ChartSegment> Segments = [];
    private ChartLayout Layout = new();

    private bool IsVertical => Parameters.Orientation == StackedBarOrientation.Vertical;

    private string OrientationCss => IsVertical ? "is-vertical" : "is-horizontal";

    private string ClickableStyle => OnClick.HasDelegate ? "cursor: pointer;" : null;

    protected override void OnParametersSet()
    {
        if (Attributes is not null && Attributes.TryGetValue("class", out object css))
            WrapperCss = css.ToString();
        Segments = Topics is null ? [] : [.. Topics];
        Layout = BuildLayout(Segments);
    }

    public string GenerateSvg() =>
        SvgHelper.Document(BuildLayout(Topics is null ? [] : [.. Topics]));

    private Task SegmentClick(int segmentIndex) =>
        segmentIndex >= 0 && segmentIndex < Segments.Count && OnClick.HasDelegate
            ? OnClick.InvokeAsync(Segments[segmentIndex])
            : Task.CompletedTask;

    private ChartLayout BuildLayout(List<ChartSegment> segments)
    {
        double total = TotalOf(segments);

        double length = Parameters.Length;
        double thickness = Parameters.Thickness;
        double labelBand = LabelBandSize(segments, total);

        double barCrossStart = Parameters.LabelSide == StackedBarLabelSide.Before ? labelBand : 0;
        double crossSize = thickness + labelBand;

        ChartLayout layout = new ChartLayout
        {
            Width = IsVertical ? crossSize : length,
            Height = IsVertical ? length : crossSize,
            PreserveAspectRatio = "xMidYMid meet"
        };

        AddBarRect(layout, 0, length, barCrossStart, thickness, Parameters.BackgroundColour, -1);

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

            AddBarRect(layout, mainOffset, mainSize, barCrossStart, thickness, background, index);

            if (share >= Parameters.MinimumLabelShare)
            {
                AddSegmentLabel(layout, segment.Name, index, mainOffset, mainSize, barCrossStart,
                    thickness, foreground);
            }

            if (Parameters.ShowValues && Parameters.LabelSide != StackedBarLabelSide.Inside)
            {
                AddSegmentValue(layout, segment.Value, index, mainOffset, mainSize, barCrossStart,
                    thickness, foreground);
            }

            mainOffset += mainSize;
        }

        return layout;
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

    private void AddBarRect(ChartLayout layout, double mainStart, double mainSize,
        double crossStart, double thickness, string colour, int segmentIndex)
    {
        if (IsVertical)
        {
            layout.AddShape(crossStart, mainStart, thickness, mainSize, colour, segmentIndex);
        }
        else
        {
            layout.AddShape(mainStart, crossStart, mainSize, thickness, colour, segmentIndex);
        }
    }

    private void AddSegmentLabel(ChartLayout layout, string label, int index, double mainStart,
        double mainSize, double crossStart, double thickness, string foreground)
    {
        int fontSize = Parameters.LabelFontSize;
        double ascent = fontSize * GlyphAscentFactor;
        double descent = fontSize * GlyphDescentFactor;
        bool inside = Parameters.LabelSide == StackedBarLabelSide.Inside;
        string colour = inside ? foreground : null;

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

            layout.AddText(label, baselineX, baselineY, anchor, fontSize, index, colour);
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

            layout.AddText(label, baselineX, baselineY, anchor, fontSize, index, colour,
                VerticalLabelAngle);
        }
    }

    /// <summary>
    /// Value written across the middle of its own segment, and only when the segment is long
    /// enough to hold it without spilling over the segments next to it.
    /// </summary>
    private void AddSegmentValue(ChartLayout layout, double value, int index, double mainStart,
        double mainSize, double crossStart, double thickness, string foreground)
    {
        int fontSize = Parameters.LabelFontSize;
        string text = value.ToString(CultureInfo.InvariantCulture);
        double textWidth = EstimatedTextWidth(text);
        double roomForTheTextWidth = IsVertical ? thickness : mainSize;
        double roomForTheTextHeight = IsVertical ? mainSize : thickness;

        if (roomForTheTextWidth >= textWidth + LabelPadding && roomForTheTextHeight >= fontSize)
        {
            double centreMain = mainStart + (mainSize / 2);
            double centreCross = crossStart + (thickness / 2);
            double x = IsVertical ? centreCross : centreMain;
            double y = (IsVertical ? centreMain : centreCross) + (fontSize * 0.35);

            layout.AddText(text, x, y, "middle", fontSize, index, foreground);
        }
    }

    private double EstimatedTextWidth(string text) =>
        (text?.Length ?? 0) * Parameters.LabelFontSize * AverageCharacterWidthFactor;
}
