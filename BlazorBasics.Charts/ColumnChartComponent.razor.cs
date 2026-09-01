namespace BlazorBasics.Charts;

public partial class ColumnChartComponent
{
    [Parameter] public IEnumerable<ChartSegment> Topics { get; set; }
    [Parameter] public ColumnsBarChartParams Parameters { get; set; } = new();

    /// <summary>
    /// Raised with the value behind whatever was clicked, be it the column, the part of it that is
    /// still empty, or its label.
    /// </summary>
    [Parameter] public EventCallback<ChartSegment> OnClick { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; }

    private const double AverageCharacterWidthFactor = 0.55;
    private const double LabelAreaPadding = 10;
    private const double LabelGap = 10;
    private const double GlyphAscentFactor = 0.75;
    private const double GlyphDescentFactor = 0.25;

    private string WrapperCss = "";
    private List<ChartSegment> Segments = [];
    private ChartLayout Layout = new();

    /// <summary>
    /// Rotated labels default to running beside their column, where they have the height of the
    /// chart to spread over; horizontal ones stay under the columns, as they have always been.
    /// </summary>
    private ColumnLabelPlacement LabelPlacement =>
        Parameters.LabelPlacement ??
        (Parameters.RotatedLabels ? ColumnLabelPlacement.Left : ColumnLabelPlacement.Bottom);

    protected override void OnParametersSet()
    {
        if (Attributes is not null && Attributes.TryGetValue("class", out object css))
            WrapperCss = css.ToString();
        Segments = Topics is null ? [] : [.. Topics];
        Layout = BuildLayout(Segments);
    }

    public string GenerateSvg() =>
        SvgHelper.Document(BuildLayout(Topics is null ? [] : [.. Topics]));

    private string ClickableStyle => OnClick.HasDelegate ? "cursor: pointer;" : null;


    private Task SegmentClick(int segmentIndex) =>
        segmentIndex >= 0 && segmentIndex < Segments.Count && OnClick.HasDelegate
            ? OnClick.InvokeAsync(Segments[segmentIndex])
            : Task.CompletedTask;

    private ChartLayout BuildLayout(List<ChartSegment> topics)
    {
        double fillReference = FillReferenceValue(topics);

        double columnWidth = Parameters.Thickness;
        double totalHeight = Parameters.Dimension;
        double totalWidth = Parameters.MaxWidth;

        double valueAreaRatio = 0.15;
        double barAreaRatio = 0.70;
        double labelAreaRatio = 0.15;

        bool labelsBesideTheColumns = LabelsAreBesideTheColumns();

        double valueAreaHeight = totalHeight * valueAreaRatio;
        double barAreaHeight = labelsBesideTheColumns
            ? totalHeight * (barAreaRatio + labelAreaRatio)
            : totalHeight * barAreaRatio;
        double labelAreaHeight = labelsBesideTheColumns
            ? 0
            : LabelAreaHeight(topics, totalHeight * labelAreaRatio);

        double labelAreaOnTop =
            LabelPlacement == ColumnLabelPlacement.Top ? labelAreaHeight : 0;
        double labelAreaAtTheBottom =
            LabelPlacement == ColumnLabelPlacement.Bottom ? labelAreaHeight : 0;

        totalHeight = labelAreaOnTop + valueAreaHeight + barAreaHeight + labelAreaAtTheBottom;

        double barAreaTop = labelAreaOnTop + valueAreaHeight;
        double barBottomY = barAreaTop + barAreaHeight;

        int columnCount = topics.Count;
        double slotWidth = columnCount > 0 ? totalWidth / columnCount : totalWidth;
        double minGap = 5;
        columnWidth = Math.Min(columnWidth, slotWidth - minGap);

        ChartLayout layout = new ChartLayout
        {
            Width = totalWidth,
            Height = totalHeight,
            CssWidth = "100%",
            PreserveAspectRatio = "xMidYMin meet"
        };

        for (int index = 0; index < columnCount; index++)
        {
            ChartSegment topic = topics[index];

            double percentage = fillReference > 0 ? topic.Value / fillReference : 0;
            double barHeight = barAreaHeight * percentage;

            double columnX = (index * slotWidth) + (slotWidth / 2) - (columnWidth / 2);
            double barY = barBottomY - barHeight;

            layout.AddShape(columnX, barAreaTop, columnWidth, barAreaHeight,
                Parameters.BackgroundColour, index);

            string colour = string.IsNullOrWhiteSpace(topic.ChartColor)
                ? Parameters.ChartColors[index % Parameters.MaxColours].Background
                : topic.ChartColor;
            layout.AddShape(columnX, barY, columnWidth, barHeight, colour, index);

            if (Parameters.ShowValues)
            {
                layout.AddText(topic.Value.ToString(CultureInfo.InvariantCulture),
                    columnX + (columnWidth / 2), barAreaTop - 5, "middle", 12, index);
            }

            AddLabel(layout, topic.Name, index, columnX, columnWidth, labelAreaOnTop,
                barAreaHeight, barBottomY, totalWidth);
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

    private bool LabelsAreBesideTheColumns() =>
        LabelPlacement == ColumnLabelPlacement.Left ||
        LabelPlacement == ColumnLabelPlacement.Right;

    private void AddLabel(ChartLayout layout, string label, int index, double columnX,
        double columnWidth, double labelAreaOnTop, double barAreaHeight, double barBottomY,
        double totalWidth)
    {
        if (LabelsAreBesideTheColumns())
        {
            AddLabelBesideTheColumn(
                layout, label, index, columnX, columnWidth, barAreaHeight, barBottomY, totalWidth);
        }
        else if (LabelPlacement == ColumnLabelPlacement.Top)
        {
            AddLabelOutsideTheColumns(
                layout, label, index, columnX + (columnWidth / 2), labelAreaOnTop, false);
        }
        else
        {
            AddLabelOutsideTheColumns(
                layout, label, index, columnX + (columnWidth / 2), barBottomY + LabelGap, true);
        }
    }

    /// <summary>
    /// Label drawn above or below every column, on the area the chart grew to fit it.
    /// </summary>
    private void AddLabelOutsideTheColumns(ChartLayout layout, string label, int index,
        double columnCentreX, double y, bool growsDownwards)
    {
        if (Parameters.RotatedLabels)
        {
            double angleRadians = ChartMathHelpers.CalculateRadious(Parameters.LabelRotationAngle);
            double baselineX =
                columnCentreX - (Parameters.LabelFontSize * GlyphDescentFactor * Math.Sin(angleRadians));
            string anchor = ReadingGoesDownwards(angleRadians) == growsDownwards ? "start" : "end";

            layout.AddText(label, baselineX, y, anchor, Parameters.LabelFontSize, index,
                rotationAngle: Parameters.LabelRotationAngle);
        }
        else
        {
            layout.AddText(label, columnCentreX, y, "middle", Parameters.LabelFontSize, index);
        }
    }

    /// <summary>
    /// Label drawn alongside its own column, running from the base of the chart upwards, so the
    /// room it has is the height of the column and not the width of the slot.
    /// </summary>
    private void AddLabelBesideTheColumn(ChartLayout layout, string label, int index,
        double columnX, double columnWidth, double barAreaHeight, double barBottomY,
        double totalWidth)
    {
        bool onTheLeft = LabelPlacement == ColumnLabelPlacement.Left;
        // The label has to sit closer to its own column than to the one next to it, otherwise it
        // reads as belonging to the neighbour, so it hugs its column instead of using LabelGap.
        double besideGap = Parameters.LabelFontSize * GlyphDescentFactor;
        double leftEdge = columnX - besideGap;
        double rightEdge = columnX + columnWidth + besideGap;

        if (Parameters.RotatedLabels)
        {
            double angleRadians = ChartMathHelpers.CalculateRadious(Parameters.LabelRotationAngle);
            double ascent = Parameters.LabelFontSize * GlyphAscentFactor;
            double descent = Parameters.LabelFontSize * GlyphDescentFactor;
            bool glyphsGrowToTheLeft = Math.Sin(angleRadians) < 0;

            double baselineX = onTheLeft
                ? leftEdge - (glyphsGrowToTheLeft ? descent : ascent)
                : rightEdge + (glyphsGrowToTheLeft ? ascent : descent);

            // The first and the last column have no room outside the chart, so their label is
            // pulled back inside instead of being cut away by the viewBox.
            double roomOnTheLeft = glyphsGrowToTheLeft ? ascent : descent;
            double roomOnTheRight = glyphsGrowToTheLeft ? descent : ascent;
            baselineX = Math.Min(Math.Max(baselineX, roomOnTheLeft), totalWidth - roomOnTheRight);

            layout.AddText(ShortenToFit(label, barAreaHeight - LabelAreaPadding), baselineX,
                barBottomY, ReadingGoesDownwards(angleRadians) ? "end" : "start",
                Parameters.LabelFontSize, index,
                rotationAngle: Parameters.LabelRotationAngle);
        }
        else
        {
            layout.AddText(label, onTheLeft ? leftEdge : rightEdge, barBottomY,
                onTheLeft ? "end" : "start", Parameters.LabelFontSize, index);
        }
    }

    private static bool ReadingGoesDownwards(double angleRadians) => Math.Sin(angleRadians) > 0;

    private double LabelAreaHeight(List<ChartSegment> topics, double defaultHeight)
    {
        double result = defaultHeight;

        if (Parameters.RotatedLabels)
        {
            double longestLabelWidth = topics.Count > 0
                ? topics.Max(topic => EstimatedLabelWidth(topic.Name))
                : 0;

            double angleRadians = ChartMathHelpers.CalculateRadious(Parameters.LabelRotationAngle);
            double heightTakenByTheLabel =
                (Math.Abs(Math.Sin(angleRadians)) * longestLabelWidth) +
                (Math.Abs(Math.Cos(angleRadians)) * Parameters.LabelFontSize);

            result = Math.Max(defaultHeight, heightTakenByTheLabel + LabelAreaPadding);
        }

        return result;
    }

    private double EstimatedLabelWidth(string label) =>
        (label?.Length ?? 0) * Parameters.LabelFontSize * AverageCharacterWidthFactor;

    private string ShortenToFit(string label, double availableLength)
    {
        string result = label ?? string.Empty;

        if (EstimatedLabelWidth(result) > availableLength)
        {
            double characterWidth = Parameters.LabelFontSize * AverageCharacterWidthFactor;
            int charactersThatFit = (int)Math.Floor(availableLength / characterWidth) - 1;

            result = charactersThatFit > 0
                ? $"{result[..Math.Min(charactersThatFit, result.Length)]}…"
                : string.Empty;
        }

        return result;
    }
}
