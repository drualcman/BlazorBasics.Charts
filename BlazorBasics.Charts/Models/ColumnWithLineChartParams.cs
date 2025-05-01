namespace BlazorBasics.Charts.Models;
public class ColumnWithLineChartParams
{
    public int Width { get; set; } = 800;
    public int Height { get; set; } = 500;
    public string BackgroundColor { get; set; } = "transparent";
    public int BarWidth { get; set; } = 75;
    public int Spacing { get; set; } = 15;
    public int Margin { get; set; } = 15;
    public string PrimaryColor { get; set; } = "#4e79a7";
    public string SecondaryColor { get; set; } = "#f28e2b";
    public string GrandTotalLineColor { get; set; } = "#e15759";
    public string PrimaryPercentageLineColor { get; set; } = "#59a84b";
    public string SecondaryPercentageLineColor { get; set; } = "#ed49ff";
    public bool ShowTitle { get; set; } = true;
    public bool ShowLegend { get; set; } = true;
    public Func<ColumnDataItem, string> BigTotalValueLabelFormatter { get; set; }
    public Func<ColumnDataItem, string> PrimaryValueLabelFormatter { get; set; }
    public Func<ColumnDataItem, string> SecondaryValueLabelFormatter { get; set; }
    public Func<ColumnDataItem, string> BottomLabelFormatter { get; set; }
    public Func<ColumnDataItem, string> TooltipFormatter { get; set; }
}
