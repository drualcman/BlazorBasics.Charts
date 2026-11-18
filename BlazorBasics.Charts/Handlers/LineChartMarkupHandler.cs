using System.Net;

namespace BlazorBasics.Charts.Handlers;
internal class LineChartMarkupHandler
{
    private readonly Func<LineChartPoint, string> PointPopup;
    private readonly Func<LineData, string> LegendLabel;

    public LineChartMarkupHandler(Func<LineChartPoint, string> pointPopup, Func<LineData, string> legendLabel)
    {
        PointPopup = pointPopup;
        LegendLabel = legendLabel;
    }

    internal MarkupString GetSelectedPointLabelMarkup(LineChartPoint selectedPoint)
    {
        StringBuilder builder = new StringBuilder();
        if(selectedPoint is not null)
        {
            if(PointPopup is null)
            {
                if(!string.IsNullOrEmpty(selectedPoint.Label))
                {
                    builder.Append("<span style='display: block; font-size: small; text-align: center;'>");
                    builder.Append(WebUtility.HtmlEncode(selectedPoint.Label));
                    builder.Append("</span>");
                }
                builder.Append("<strong style='display: block; font-size: medium; font-weight: bold; text-align: center;'>");
                builder.Append(WebUtility.HtmlEncode(selectedPoint.Value));
                builder.Append("</strong>");
            }
            else
                builder.Append(PointPopup(selectedPoint));
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
