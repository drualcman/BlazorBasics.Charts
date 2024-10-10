namespace BlazorBasics.Charts.Models;

public class PieChartParams
{
    public PieChartParams(int width = 150, int height = 150,
        double saturation = 100.0, double luminosity = 50.0, int separationOffset = 30,
        double delayTime = 0, string title = "",
        IEnumerable<ChartColor> chartColours = null)
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
    }

    public int Width { get; init; }
    public int Height { get; init; }
    public double Saturation { get; init; }
    public double Luminosity { get; init; }
    public double DelayTime { get; init; }
    public int SeparationOffset { get; init; }
    public string Title { get; init; }
    public List<ChartColor> ChartColors { get; set; }
    public int MaxColours => ChartColors.Count;

}