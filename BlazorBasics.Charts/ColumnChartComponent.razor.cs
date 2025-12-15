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
        double totalWidth = Parameters.MaxWidth;

        double gap = Parameters.Gap;

        double valueAreaRatio = 0.15;
        double barAreaRatio = 0.70;
        double labelAreaRatio = 0.15;

        double valueAreaHeight = totalHeight * valueAreaRatio;
        double barAreaHeight = totalHeight * barAreaRatio;
        double labelAreaHeight = totalHeight * labelAreaRatio;

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

            double percentage = maxQuantity > 0 ? topic.Value / maxQuantity : 0;
            double barHeight = barAreaHeight * percentage;

            double columnX = (index * slotWidth) + (slotWidth / 2) - (columnWidth / 2);
            double barBottomY = valueAreaHeight + barAreaHeight;
            double barY = barBottomY - barHeight;

            // Draw background
            svg.AppendLine(SvgHelper.Rect(columnX, valueAreaHeight, columnWidth, barAreaHeight, Parameters.BackgroundColour));
            // Draw bar
            string color = string.IsNullOrWhiteSpace(topic.ChartColor) ? Parameters.ChartColors[index % Parameters.MaxColours].Background : topic.ChartColor;
            svg.AppendLine(SvgHelper.Rect(columnX, barY, columnWidth, barHeight, color));

            // Draw value
            if (Parameters.ShowValues)
                svg.AppendLine(SvgHelper.Text(topic.Value.ToString(CultureInfo.InvariantCulture), columnX + columnWidth / 2, valueAreaHeight - 5, "middle", 12));

            // Draw label
            double labelY = barBottomY + 10;
            svg.AppendLine(SvgHelper.Text(topic.Name, columnX + columnWidth / 2, labelY, "middle", 12));
        }


        svg.AppendLine("</svg>");

        return svg.ToString();
    }

}