namespace BlazorBasics.Charts.Models;

public class PieChartParams
{
    public PieChartParams(int width = 150, int height = 150,
        double saturation = 100.0, double luminosity = 50.0, int separationOffset = 30,
        double delayTime = 0, string title = "",
        IEnumerable<ChartColor> chartColours = null,
        bool showLabels = false, double centerTextSeparationPercentage = 0.85)
    {
        Width = width;
        Height = height;
        Saturation = saturation;
        Luminosity = luminosity;
        SeparationOffset = separationOffset;
        DelayTime = delayTime;
        Title = title;
        ChartColors = new(chartColours ?? ChartColourHelper
            .InitializeColours(256, separationOffset));
        ShowLabels = showLabels;
        CenterTextSeparationPercentage = centerTextSeparationPercentage;
    }

    public int Width { get; init; }
    public int Height { get; init; }
    public double Saturation { get; init; }
    public double Luminosity { get; init; }
    public double DelayTime { get; init; }
    public int SeparationOffset { get; init; }
    public string Title { get; set; }
    public List<ChartColor> ChartColors { get; set; }
    public int MaxColours => ChartColors.Count;
    public bool ShowLabels { get; set; }
    public double CenterTextSeparationPercentage { get; set; }

}