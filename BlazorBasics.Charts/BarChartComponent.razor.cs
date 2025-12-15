namespace BlazorBasics.Charts;

public partial class BarChartComponent
{
    [Parameter] public IEnumerable<ChartSegment> Topics { get; set; }
    [Parameter] public ColumnsBarChartParams Parameters { get; set; } = new();

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; }

    MarkupString SvgMarkup = new();

    protected override void OnParametersSet()
    {
        SvgMarkup = new MarkupString(GenerateSvg());
    }

    public string GenerateSvg()
    {
        double maxQuantity = Topics.Any() ? Topics.Max(t => t.Value) : 0;

        double totalWidth = Parameters.Dimension;
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

        double totalHeight = Topics.Count() * (thickness + gap);

        StringBuilder svg = new StringBuilder();

        svg.AppendLine(
            $"<svg width=\"{totalWidth}\" height=\"{totalHeight}\" viewBox=\"0 0 {totalWidth} {totalHeight}\" xmlns=\"http://www.w3.org/2000/svg\">"
        );

        double y = 0;
        int colorIndex = 0;

        foreach (ChartSegment topic in Topics)
        {
            double percentage = maxQuantity > 0 ? topic.Value / maxQuantity : 0;
            double barWidth = barWidthTotal * percentage;

            string color = string.IsNullOrWhiteSpace(topic.ChartColor)
                ? Parameters.ChartColors[colorIndex].Background
                : topic.ChartColor;

            svg.AppendLine(
                SvgHelper.Rect(0, y, barWidthTotal, thickness, Parameters.BackgroundColour)
            );

            svg.AppendLine(
                SvgHelper.Rect(0, y, barWidth, thickness, color)
            );

            double textY = y + thickness - 5;

            if (Parameters.ShowValues)
            {
                double valueX = barWidthTotal + innerPadding;
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

            double labelX = totalWidth - innerPadding;
            svg.AppendLine(
                SvgHelper.Text(
                    topic.Name,
                    labelX,
                    textY,
                    "end",
                    12
                )
            );


            y += thickness + gap;

            colorIndex++;
            if (colorIndex >= Parameters.MaxColours)
                colorIndex = 0;
        }

        svg.AppendLine("</svg>");

        return svg.ToString();
    }
}