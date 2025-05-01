namespace BlazorBasics.Charts;

public partial class ColumnWithLineChartComponent
{
    private List<LinePoints> linePoints = new();

    [Parameter]
    public List<ChartDataItem> Data { get; set; } = new();
    private int MaxTotal => Data.Any() ? Data.Max(x => x.ActiveUsers + x.NonActiveUsers) : 0;

    protected override void OnAfterRender(bool firstRender)
    {
        if(firstRender)
        {
            linePoints.Clear();
            StateHasChanged();
        }
    }

}
public class LinePoints
{
    public double X1 { get; set; }
    public double Y1 { get; set; }
    public double X2 { get; set; }
    public double Y2 { get; set; }
}

public class ChartDataItem
{
    public string Month { get; set; } = string.Empty;
    public int ActiveUsers { get; set; }
    public int NonActiveUsers { get; set; }
}