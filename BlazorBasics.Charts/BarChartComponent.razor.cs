namespace BlazorBasics.Charts;

public partial class BarChartComponent
{
    [Parameter] public IEnumerable<ChartSegment> Topics { get; set; }
    [Parameter] public ColumnsBarChartParams Parameters { get; set; } = new();

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; }

    MarkupString SvgMarkup = new();

    /// <summary>
    /// The bar chart has always closed every row with its label, so that stays the default here.
    /// </summary>
    private ColumnLabelPlacement LabelPlacement =>
        Parameters.LabelPlacement ?? ColumnLabelPlacement.Right;

    protected override void OnParametersSet()
    {
        SvgMarkup = new MarkupString(GenerateSvg());
    }

    public string GenerateSvg()
    {
        double fillReference = FillReferenceValue();

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

        double totalHeight = Topics.Count() * rowHeight;

        StringBuilder svg = new StringBuilder();


        string svgWidth = Parameters.Dimension.ToString(CultureInfo.InvariantCulture) + "%";

        svg.AppendLine(
            $"<svg width=\"{svgWidth}\" height=\"{totalHeight}\" " +
            $"viewBox=\"0 0 {logicalWidth} {totalHeight}\" " +
            $"preserveAspectRatio=\"xMinYMin meet\" " +
            $"xmlns=\"http://www.w3.org/2000/svg\">"
        );

        double y = 0;
        int colorIndex = 0;

        foreach (ChartSegment topic in Topics)
        {
            double percentage = fillReference > 0 ? topic.Value / fillReference : 0;
            double barWidth = barWidthTotal * percentage;

            double barY = LabelPlacement == ColumnLabelPlacement.Top
                ? y + labelLineHeight
                : y;

            string color = string.IsNullOrWhiteSpace(topic.ChartColor)
                ? Parameters.ChartColors[colorIndex].Background
                : topic.ChartColor;

            svg.AppendLine(
                SvgHelper.Rect(barX, barY, barWidthTotal, thickness, Parameters.BackgroundColour)
            );

            svg.AppendLine(
                SvgHelper.Rect(barX, barY, barWidth, thickness, color)
            );

            double textY = barY + thickness - 5;

            if (Parameters.ShowValues)
            {
                double valueX = barX + barWidthTotal + innerPadding;
                svg.AppendLine(
                    SvgHelper.Text(
                        topic.Value.ToString(CultureInfo.InvariantCulture),
                        valueX,
                        textY,
                        "start",
                        12
                    )
                );
            }

            svg.AppendLine(LabelMarkup(topic.Name, y, barY, textY, totalWidth, innerPadding,
                labelWidth, thickness, labelFontSize));


            y += rowHeight;

            colorIndex++;
            if (colorIndex >= Parameters.MaxColours)
                colorIndex = 0;
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

    /// <summary>
    /// Top and Bottom give the label a line of its own, so it is never squeezed by the bar and
    /// long names do not need shortening; Left and Right keep it on the same line as the bar.
    /// </summary>
    private bool LabelIsOnItsOwnLine() =>
        LabelPlacement == ColumnLabelPlacement.Top ||
        LabelPlacement == ColumnLabelPlacement.Bottom;

    private string LabelMarkup(string label, double rowY, double barY, double textY,
        double totalWidth, double innerPadding, double labelWidth, double thickness, int fontSize)
    {
        string result;

        if (LabelPlacement == ColumnLabelPlacement.Top)
        {
            result = SvgHelper.Text(label, 0, rowY + fontSize, "start", fontSize);
        }
        else if (LabelPlacement == ColumnLabelPlacement.Bottom)
        {
            result = SvgHelper.Text(
                label, 0, barY + thickness + innerPadding + fontSize, "start", fontSize);
        }
        else if (LabelPlacement == ColumnLabelPlacement.Left)
        {
            result = SvgHelper.Text(label, labelWidth - innerPadding, textY, "end", fontSize);
        }
        else
        {
            result = SvgHelper.Text(label, totalWidth - innerPadding, textY, "end", fontSize);
        }

        return result;
    }
}
