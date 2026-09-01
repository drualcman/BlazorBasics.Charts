namespace BlazorBasics.Charts;

public partial class ColumnChartComponent
{
    [Parameter] public IEnumerable<ChartSegment> Topics { get; set; }
    [Parameter] public ColumnsBarChartParams Parameters { get; set; } = new();

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; }

    private const double AverageCharacterWidthFactor = 0.55;
    private const double LabelAreaPadding = 10;
    private const double LabelGap = 10;
    private const double GlyphAscentFactor = 0.75;
    private const double GlyphDescentFactor = 0.25;

    private MarkupString SvgMarkup = new();
    private string WrapperCss = "";

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
        SvgMarkup = new MarkupString(GenerateSvg());
    }

    public string GenerateSvg()
    {
        double fillReference = FillReferenceValue();

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
            : LabelAreaHeight(totalHeight * labelAreaRatio);

        double labelAreaOnTop =
            LabelPlacement == ColumnLabelPlacement.Top ? labelAreaHeight : 0;
        double labelAreaAtTheBottom =
            LabelPlacement == ColumnLabelPlacement.Bottom ? labelAreaHeight : 0;

        totalHeight = labelAreaOnTop + valueAreaHeight + barAreaHeight + labelAreaAtTheBottom;

        double barAreaTop = labelAreaOnTop + valueAreaHeight;
        double barBottomY = barAreaTop + barAreaHeight;

        int columnCount = Topics.Count();
        double slotWidth = totalWidth / columnCount;
        double minGap = 5;
        columnWidth = Math.Min(columnWidth, slotWidth - minGap);

        StringBuilder svg = new StringBuilder();

        svg.AppendLine(
            $"<svg width=\"100%\" height=\"{totalHeight}\" viewBox=\"0 0 {totalWidth} {totalHeight}\" xmlns=\"http://www.w3.org/2000/svg\" preserveAspectRatio=\"xMidYMin meet\">"
        );

        for (int index = 0; index < columnCount; index++)
        {
            ChartSegment topic = Topics.ElementAt(index);

            double percentage = fillReference > 0 ? topic.Value / fillReference : 0;
            double barHeight = barAreaHeight * percentage;

            double columnX = (index * slotWidth) + (slotWidth / 2) - (columnWidth / 2);
            double barY = barBottomY - barHeight;

            // Draw background
            svg.AppendLine(SvgHelper.Rect(columnX, barAreaTop, columnWidth, barAreaHeight, Parameters.BackgroundColour));
            // Draw bar
            string color = string.IsNullOrWhiteSpace(topic.ChartColor) ? Parameters.ChartColors[index % Parameters.MaxColours].Background : topic.ChartColor;
            svg.AppendLine(SvgHelper.Rect(columnX, barY, columnWidth, barHeight, color));

            // Draw value
            if (Parameters.ShowValues)
                svg.AppendLine(SvgHelper.Text(topic.Value.ToString(CultureInfo.InvariantCulture), columnX + columnWidth / 2, barAreaTop - 5, "middle", 12));

            // Draw label
            svg.AppendLine(LabelMarkup(
                topic.Name, columnX, columnWidth, labelAreaOnTop, barAreaTop, barAreaHeight,
                barBottomY, totalWidth));
        }


        svg.AppendLine("</svg>");

        return svg.ToString();
    }

    private double FillReferenceValue()
    {
        double result = 0;

        if (Topics.Any())
        {
            result = Parameters.FillReference == ColumnFillReference.TotalOfAllValues
                ? Topics.Sum(topic => topic.Value)
                : Topics.Max(topic => topic.Value);
        }

        return result;
    }

    private bool LabelsAreBesideTheColumns() =>
        LabelPlacement == ColumnLabelPlacement.Left ||
        LabelPlacement == ColumnLabelPlacement.Right;

    private string LabelMarkup(string label, double columnX, double columnWidth,
        double labelAreaOnTop, double barAreaTop, double barAreaHeight, double barBottomY,
        double totalWidth)
    {
        string result;

        if (LabelsAreBesideTheColumns())
        {
            result = LabelBesideTheColumn(
                label, columnX, columnWidth, barAreaHeight, barBottomY, totalWidth);
        }
        else if (LabelPlacement == ColumnLabelPlacement.Top)
        {
            result = LabelOutsideTheColumns(label, columnX + (columnWidth / 2), labelAreaOnTop, false);
        }
        else
        {
            result = LabelOutsideTheColumns(
                label, columnX + (columnWidth / 2), barBottomY + LabelGap, true);
        }

        return result;
    }

    /// <summary>
    /// Label drawn above or below every column, on the area the chart grew to fit it.
    /// </summary>
    private string LabelOutsideTheColumns(string label, double columnCentreX, double y,
        bool growsDownwards)
    {
        string result;

        if (Parameters.RotatedLabels)
        {
            double angleRadians = ChartMathHelpers.CalculateRadious(Parameters.LabelRotationAngle);
            double baselineX =
                columnCentreX - (Parameters.LabelFontSize * GlyphDescentFactor * Math.Sin(angleRadians));
            string anchor = ReadingGoesDownwards(angleRadians) == growsDownwards ? "start" : "end";

            result = SvgHelper.RotatedTextAt(
                label, baselineX, y, Parameters.LabelRotationAngle, Parameters.LabelFontSize, anchor);
        }
        else
        {
            result = SvgHelper.Text(label, columnCentreX, y, "middle", Parameters.LabelFontSize);
        }

        return result;
    }

    /// <summary>
    /// Label drawn alongside its own column, running from the base of the chart upwards, so the
    /// room it has is the height of the column and not the width of the slot.
    /// </summary>
    private string LabelBesideTheColumn(string label, double columnX, double columnWidth,
        double barAreaHeight, double barBottomY, double totalWidth)
    {
        bool onTheLeft = LabelPlacement == ColumnLabelPlacement.Left;
        // The label has to sit closer to its own column than to the one next to it, otherwise it
        // reads as belonging to the neighbour, so it hugs its column instead of using LabelGap.
        double besideGap = Parameters.LabelFontSize * GlyphDescentFactor;
        double leftEdge = columnX - besideGap;
        double rightEdge = columnX + columnWidth + besideGap;
        string result;

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
            baselineX = Math.Min(
                Math.Max(baselineX, roomOnTheLeft), totalWidth - roomOnTheRight);

            result = SvgHelper.RotatedTextAt(
                ShortenToFit(label, barAreaHeight - LabelAreaPadding),
                baselineX, barBottomY, Parameters.LabelRotationAngle, Parameters.LabelFontSize,
                ReadingGoesDownwards(angleRadians) ? "end" : "start");
        }
        else
        {
            result = SvgHelper.Text(label, onTheLeft ? leftEdge : rightEdge, barBottomY,
                onTheLeft ? "end" : "start", Parameters.LabelFontSize);
        }

        return result;
    }

    private static bool ReadingGoesDownwards(double angleRadians) => Math.Sin(angleRadians) > 0;

    private double LabelAreaHeight(double defaultHeight)
    {
        double result = defaultHeight;

        if (Parameters.RotatedLabels)
        {
            double longestLabelWidth = Topics.Any()
                ? Topics.Max(topic => EstimatedLabelWidth(topic.Name))
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
