namespace BlazorBasics.Charts.Models;

/// <summary>
/// Options of a chart that draws every value as a slice of one single bar, so what is read is the
/// share each value takes of the whole rather than how the values compare to each other.
/// </summary>
public class StackedBarChartParams
{
    public StackedBarChartParams(
        StackedBarOrientation orientation = StackedBarOrientation.Horizontal,
        int thickness = 40,
        int length = 600,
        string backgroundColour = "#D3D3D3",
        IEnumerable<ChartColor> chartColours = null,
        bool showValues = false,
        StackedBarLabelSide labelSide = StackedBarLabelSide.After,
        StackedBarLabelAlignment labelAlignment = StackedBarLabelAlignment.Start,
        int labelFontSize = 12,
        double minimumLabelShare = 0.03,
        double total = 0)
    {
        Orientation = orientation;
        Thickness = thickness;
        Length = length;
        BackgroundColour = backgroundColour;
        ChartColors = new(chartColours ?? ChartColourHelper.InitializeColours(256, 30));
        ShowValues = showValues;
        LabelSide = labelSide;
        LabelAlignment = labelAlignment;
        LabelFontSize = labelFontSize;
        MinimumLabelShare = minimumLabelShare;
        Total = total;
    }

    public StackedBarOrientation Orientation { get; init; }

    /// <summary>How wide a vertical bar is, or how tall a horizontal one is.</summary>
    public int Thickness { get; init; }

    /// <summary>How long the bar is along the direction it stacks in.</summary>
    public int Length { get; init; }

    /// <summary>
    /// Colour of the part of the bar no value accounts for, which only shows up when
    /// <see cref="Total"/> is bigger than the values added together.
    /// </summary>
    public string BackgroundColour { get; init; }

    /// <summary>
    /// Writes the value of every segment inside it, in the contrasting colour, whenever the
    /// segment is long enough to hold it. Ignored when the names already go inside.
    /// </summary>
    public bool ShowValues { get; init; }

    public StackedBarLabelSide LabelSide { get; init; }

    public StackedBarLabelAlignment LabelAlignment { get; init; }

    public int LabelFontSize { get; init; }

    /// <summary>
    /// Segments smaller than this share of the whole are drawn without a label, because there is
    /// no room to write one without it landing on top of its neighbours. Given as a fraction, so
    /// 0.03 leaves out anything under three per cent. Zero labels every segment.
    /// </summary>
    public double MinimumLabelShare { get; init; }

    /// <summary>
    /// What the complete bar is worth. Left at zero the values are added together and the bar is
    /// always full; set to a known total, the values that are not among the segments stay as
    /// unfilled bar.
    /// </summary>
    public double Total { get; init; }

    public List<ChartColor> ChartColors { get; set; }

    public int MaxColours => ChartColors.Count;
}
