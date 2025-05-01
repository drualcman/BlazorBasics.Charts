namespace BlazorBasics.Charts.Models;
public class ColumnWithLineChartData
{
    public string Title { get; set; }
    public string PrimaryLegend { get; set; }
    public string SecondaryLegend { get; set; }
    public IEnumerable<ColumnDataItem> Data { get; set; }

    public ColumnWithLineChartData(IEnumerable<ColumnDataItem> data)
    {
        Data = data ?? throw new ArgumentNullException(nameof(data));
    }
}
