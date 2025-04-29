namespace BlazorBasics.Charts.Models;

public class ColumnsBarChartParams
{
    public ColumnsBarChartParams(string backgroundColour = "#D3D3D3", int thickness = 20, int dimension = 100,
        bool showValues = false,
        IEnumerable<ChartColor> chartColours = null)
    {
        BackgroundColour = backgroundColour;
        Thickness = thickness;
        Dimension = dimension;
        ShowValues = showValues;
        ChartColors = new(chartColours ?? ChartColourHelper
            .InitializeColours(256, 30));
    }

    public string BackgroundColour { get; init; }
    public int Thickness { get; init; }
    public int Dimension { get; init; }
    public bool ShowValues { get; init; }
    public List<ChartColor> ChartColors { get; set; }
    public int MaxColours => ChartColors.Count;
}