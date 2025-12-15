namespace BlazorBasics.Charts.Models;

public class ColumnsBarChartParams
{
    public ColumnsBarChartParams(
        string backgroundColour = "#D3D3D3",
        int thickness = 20,
        int dimension = 100,
        bool showValues = false,
        IEnumerable<ChartColor> chartColours = null,
        int gap = 5,
        int maxWidth = 600)
    {
        BackgroundColour = backgroundColour;
        Thickness = thickness;
        Dimension = dimension;
        MaxWidth = maxWidth;
        ShowValues = showValues;
        Gap = gap;
        ChartColors = new(chartColours ?? ChartColourHelper
            .InitializeColours(256, 30));
        MaxWidth = maxWidth;
    }

    public string BackgroundColour { get; init; }
    public int Thickness { get; init; }
    public int Dimension { get; init; }
    public int MaxWidth { get; init; }
    public int Gap { get; init; }
    public bool ShowValues { get; init; }
    public List<ChartColor> ChartColors { get; set; }
    public int MaxColours => ChartColors.Count;
}