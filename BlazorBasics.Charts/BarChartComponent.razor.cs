namespace BlazorBasics.Charts;

public partial class BarChartComponent
{
    [Parameter] public IEnumerable<ChartSegment> Topics { get; set; }
    [Parameter] public ColumnsBarChartParams Parameters { get; set; } = new();

    /// <summary>
    /// Raised with the value behind whatever was clicked, be it the bar, the part of the row that
    /// is still empty, or its label.
    /// </summary>
    [Parameter] public EventCallback<ChartSegment> OnClick { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; }

    private string WrapperCss = "";
    private List<ChartSegment> Segments = [];
    private ChartLayout Layout = new();

    /// <summary>
    /// The bar chart has always closed every row with its label, so that stays the default here.
    /// </summary>
    private ColumnLabelPlacement LabelPlacement =>
        Parameters.LabelPlacement ?? ColumnLabelPlacement.Right;

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

    private ChartLayout BuildLayout(List<ChartSegment> topics)
    {
        double fillReference = FillReferenceValue(topics);

        double logicalWidth = Parameters.MaxWidth;
        double totalWidth = logicalWidth;
        double thickness = Parameters.Thickness;
        double gap = Parameters.Gap;

        double barRatio = 0.75;
        double textRatio = 0.25;

        double valueRatio = 0.30;
        double labelRatio = 0.70;

        double barWidthTotal = totalWidth * barRatio;
        double textWidthTotal = totalWidth * textRatio;

        double valueWidth = textWidthTotal * valueRatio;
        double labelWidth = textWidthTotal * labelRatio;

        double innerPadding = 5;

        int labelFontSize = Parameters.LabelFontSize;
        bool labelOnItsOwnLine = LabelIsOnItsOwnLine();
        bool labelBeforeTheBar = LabelPlacement == ColumnLabelPlacement.Left;

        double labelLineHeight = labelOnItsOwnLine ? labelFontSize + innerPadding : 0;
        double rowHeight = thickness + gap + labelLineHeight;

        double barX = 0;
        if (labelOnItsOwnLine)
        {
            barWidthTotal = totalWidth - (Parameters.ShowValues ? valueWidth : 0);
        }
        else if (labelBeforeTheBar)
        {
            barX = labelWidth;
            barWidthTotal = totalWidth - labelWidth - (Parameters.ShowValues ? valueWidth : 0);
        }

        ChartLayout layout = new ChartLayout
        {
            Width = logicalWidth,
            Height = topics.Count * rowHeight,
            CssWidth = Parameters.Dimension.ToString(CultureInfo.InvariantCulture) + "%",
            PreserveAspectRatio = "xMinYMin meet"
        };

        double y = 0;
        int colorIndex = 0;

        for (int index = 0; index < topics.Count; index++)
        {
            ChartSegment topic = topics[index];

            double percentage = fillReference > 0 ? topic.Value / fillReference : 0;
            double barWidth = barWidthTotal * percentage;

            double barY = LabelPlacement == ColumnLabelPlacement.Top ? y + labelLineHeight : y;

            string colour = string.IsNullOrWhiteSpace(topic.ChartColor)
                ? Parameters.ChartColors[colorIndex].Background
                : topic.ChartColor;

            layout.AddShape(barX, barY, barWidthTotal, thickness, Parameters.BackgroundColour, index);
            layout.AddShape(barX, barY, barWidth, thickness, colour, index);

            double textY = barY + thickness - 5;

            if (Parameters.ShowValues)
            {
                layout.AddText(topic.Value.ToString(CultureInfo.InvariantCulture),
                    barX + barWidthTotal + innerPadding, textY, "start", 12, index);
            }

            AddLabel(layout, topic.Name, index, y, barY, textY, totalWidth, innerPadding,
                labelWidth, thickness, labelFontSize);

            y += rowHeight;

            colorIndex++;
            if (colorIndex >= Parameters.MaxColours)
                colorIndex = 0;
        }

        return layout;
    }

    private double FillReferenceValue(List<ChartSegment> topics)
    {
        double result = 0;

        if (topics.Count > 0)
        {
            result = Parameters.FillReference == ColumnFillReference.TotalOfAllValues
                ? topics.Sum(topic => topic.Value)
                : topics.Max(topic => topic.Value);
        }

        return result;
    }

    /// <summary>
    /// Top and Bottom give the label a line of its own, so it is never squeezed by the bar and
    /// long names do not need shortening; Left and Right keep it on the same line as the bar.
    /// </summary>
    private bool LabelIsOnItsOwnLine() =>
        LabelPlacement == ColumnLabelPlacement.Top ||
        LabelPlacement == ColumnLabelPlacement.Bottom;

    private void AddLabel(ChartLayout layout, string label, int index, double rowY, double barY,
        double textY, double totalWidth, double innerPadding, double labelWidth, double thickness,
        int fontSize)
    {
        if (LabelPlacement == ColumnLabelPlacement.Top)
        {
            layout.AddText(label, 0, rowY + fontSize, "start", fontSize, index);
        }
        else if (LabelPlacement == ColumnLabelPlacement.Bottom)
        {
            layout.AddText(label, 0, barY + thickness + innerPadding + fontSize, "start",
                fontSize, index);
        }
        else if (LabelPlacement == ColumnLabelPlacement.Left)
        {
            layout.AddText(label, labelWidth - innerPadding, textY, "end", fontSize, index);
        }
        else
        {
            layout.AddText(label, totalWidth - innerPadding, textY, "end", fontSize, index);
        }
    }
}
