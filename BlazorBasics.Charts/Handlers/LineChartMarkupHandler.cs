namespace BlazorBasics.Charts.Handlers;
internal class LineChartMarkupHandler
{
    private readonly Func<string, string> FormatterLabelPopup;
    private readonly Func<LineData, string> LegendLabel;

    public LineChartMarkupHandler(Func<string, string> formatterLabelPopup, Func<LineData, string> legendLabel)
    {
        FormatterLabelPopup = formatterLabelPopup;
        LegendLabel = legendLabel;
    }

    internal MarkupString GetSelectedPointLabelMarkup(ChartPoint SelectedPoint)
    {
        StringBuilder builder = new StringBuilder();
        if(SelectedPoint is not null)
        {
            if(FormatterLabelPopup is null)
            {
                builder.Append($"<strong style='");
                builder.Append("font-size: medium;");
                builder.Append("font-weight: bold;");
                builder.Append("text-align: center;");
                builder.Append($"'>");
                builder.Append($"{SelectedPoint.Value}");
                builder.Append($"</strong>");
            }
            else
                builder.Append(FormatterLabelPopup(SelectedPoint.Value));
        }
        return new MarkupString(builder.ToString());
    }

    internal MarkupString GetLeyendLabelMarkup(LineData serie)
    {
        StringBuilder builder = new StringBuilder();
        if(LegendLabel is null)
        {
            builder.Append($"<div class=\"dot\" style=\"background-color: {serie?.Color ?? "black"};\"></div>");
            builder.Append($"<span>{serie?.Name}</span>");
        }
        else
            builder.Append(LegendLabel(serie));
        return new MarkupString(builder.ToString());
    }
}
