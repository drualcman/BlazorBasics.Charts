namespace BlazorBasics.Charts;

public partial class ColumnChartComponent
{
    [Parameter] public IEnumerable<ChartSegment> Topics { get; set; }
    [Parameter] public ColumnsBarChartParams Parameters { get; set; } = new();

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; }

    private MarkupString SvgMarkup = new();
    private string WrapperCss = "";

    protected override void OnParametersSet()
    {
        if (Attributes is not null && Attributes.TryGetValue("class", out var css))
            WrapperCss = css.ToString();
        SvgMarkup = new MarkupString(GenerateSvg());
    }

    public string GenerateSvg()
    {
        double maxQuantity = Topics.Any() ? Topics.Max(t => t.Value) : 0;

        double columnWidth = Parameters.Thickness;
        double totalHeight = Parameters.Dimension;
        double totalWidth = Parameters.Dimension;

        double gap = Parameters.Gap;

        double valueAreaRatio = 0.15;
        double barAreaRatio = 0.70;
        double labelAreaRatio = 0.15;

        double valueAreaHeight = totalHeight * valueAreaRatio;
        double barAreaHeight = totalHeight * barAreaRatio;
        double labelAreaHeight = totalHeight * labelAreaRatio;

        int columnCount = Topics.Count();
        double slotWidth = totalWidth / columnCount;

        StringBuilder svg = new StringBuilder();

        svg.AppendLine(
            $"<svg width=\"{totalWidth}\" height=\"{totalHeight}\" viewBox=\"0 0 {totalWidth} {totalHeight}\" xmlns=\"http://www.w3.org/2000/svg\">"
        );

        int index = 0;
        int colorIndex = 0;

        foreach (ChartSegment topic in Topics)
        {
            double percentage = maxQuantity > 0 ? topic.Value / maxQuantity : 0;
            double barHeight = barAreaHeight * percentage;

            double columnX = (index * slotWidth) + (slotWidth / 2) - (columnWidth / 2);

            double barBottomY = valueAreaHeight + barAreaHeight;
            double barY = barBottomY - barHeight;

            string color = string.IsNullOrWhiteSpace(topic.ChartColor)
                ? Parameters.ChartColors[colorIndex].Background
                : topic.ChartColor;

            svg.AppendLine(
                SvgHelper.Rect(
                    columnX,
                    valueAreaHeight,
                    columnWidth,
                    barAreaHeight,
                    Parameters.BackgroundColour
                )
            );

            svg.AppendLine(
                SvgHelper.Rect(
                    columnX,
                    barY,
                    columnWidth,
                    barHeight,
                    color
                )
            );

            if (Parameters.ShowValues)
            {
                svg.AppendLine(
                    SvgHelper.Text(
                        topic.Value.ToString(CultureInfo.InvariantCulture),
                        columnX + columnWidth / 2,
                        valueAreaHeight - 5,
                        "middle",
                        12
                    )
                );
            }

            double labelY = barBottomY + gap + 10;

            svg.AppendLine(
                SvgHelper.Text(
                    topic.Name,
                    columnX + columnWidth / 2,
                    labelY,
                    "middle",
                    12
                )
            );

            index++;
            colorIndex++;
            if (colorIndex >= Parameters.MaxColours)
                colorIndex = 0;
        }

        svg.AppendLine("</svg>");

        return svg.ToString();
    }

}