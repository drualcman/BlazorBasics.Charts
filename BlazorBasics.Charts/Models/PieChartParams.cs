namespace BlazorBasics.Charts.Models;

public class PieChartParams
{
    public PieChartParams(int width = 150, int height = 150,
        double saturation = 100.0, double luminosity = 50.0,
        int separationOffset = 15, int separationOnSelectOffset = 15,
        double delayTime = 0, string title = "",
        IEnumerable<ChartColor> chartColours = null,
        bool showLabels = false, double centerTextSeparationPercentage = 0.85,
        bool separateBiggerByDefault = true, bool showBiggestLabel = false,
        bool showLegend = true)
    {
        Width = width;
        Height = height;
        Saturation = saturation;
        Luminosity = luminosity;
        SeparationOffset = separationOffset;
        SeparationOnSelectOffset = separationOnSelectOffset;
        DelayTime = delayTime;
        Title = title;
        ChartColors = new(chartColours ?? ChartColourHelper
            .InitializeColours(256, separationOffset));
        ShowLabels = showLabels;
        CenterTextSeparationPercentage = centerTextSeparationPercentage;
        SeparateBiggerByDefault = separateBiggerByDefault;
        ShowBiggestLabel = showBiggestLabel;
        ShowLegend = showLegend;
    }

    public int Width { get; init; }
    public int Height { get; init; }
    public double Saturation { get; init; }
    public double Luminosity { get; init; }
    public double DelayTime { get; init; }
    public int SeparationOffset { get; init; }
    public int SeparationOnSelectOffset { get; init; }
    public string Title { get; set; }
    public List<ChartColor> ChartColors { get; set; }
    public int MaxColours => ChartColors.Count;
    public bool ShowLabels { get; set; }
    public double CenterTextSeparationPercentage { get; set; }
    public bool SeparateBiggerByDefault { get; init; }
    public bool ShowBiggestLabel { get; set; }
    public bool ShowLegend { get; set; }
}