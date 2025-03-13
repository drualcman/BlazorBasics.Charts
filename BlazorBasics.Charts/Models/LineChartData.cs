namespace BlazorBasics.Charts.Models;

public class LineChartData
{
    public IEnumerable<string> XLabels { get; set; }
    public IEnumerable<string> YLabels { get; set; }
    public IEnumerable<LineData> Data { get; set; }

    public LineChartData(IEnumerable<LineData> data, IEnumerable<string> xLabels = null, IEnumerable<string> yLabels = null)
    {
        XLabels = xLabels ?? [];
        YLabels = yLabels ?? [];
        Data = data ?? [];
    }
}
