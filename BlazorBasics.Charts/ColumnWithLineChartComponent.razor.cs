namespace BlazorBasics.Charts;

public partial class ColumnWithLineChartComponent
{
    private List<LinePoints> linePoints = new();

    [Parameter] public List<ChartDataItem> Data { get; set; } = new();

    // Constantes para el layout
    const int margin = 40;
    const int chartHeight = 300;
    const int barWidth = 50;
    const int spacing = 30;

    // Calcular valores
    int MaxColumnTotal => Data.Max(x => x.ActiveUsers + x.NonActiveUsers);
    int GrandTotal => Data.Sum(x => x.ActiveUsers + x.NonActiveUsers);
    List<LinePoints> GrandTotalPoints = new List<LinePoints>();
    List<LinePoints> ActivePercentagePoints = new List<LinePoints>();
    int TotalWidth => (Data.Count * (barWidth + spacing)) + margin;

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
    public double X { get; set; }
    public double Y { get; set; }

    public LinePoints(double x, double y)
    {
        X = x;
        Y = y;
    }
}

public class ChartDataItem
{
    public string Month { get; set; } = string.Empty;
    public int ActiveUsers { get; set; }
    public int NonActiveUsers { get; set; }
}