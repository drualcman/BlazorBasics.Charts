namespace BlazorBasics.Charts;

public partial class BarChartComponent
{
    private Random _random = new();
    private double MaxQuantity;
    private string Style;

    private string WrapperCss = "";
    [Parameter] public IEnumerable<ChartSegment> Topics { get; set; }
    [Parameter] public BarChartParams Parameters { get; set; } = new();

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; }

    protected override void OnParametersSet()
    {
        MaxQuantity = Topics.Any() ? Topics.Max(t => t.Value) : 0;
        if (Attributes is not null && Attributes.TryGetValue("class", out var css)) WrapperCss = css.ToString();
        Style = $"--Thickness: {Parameters.Thickness.ToString(CultureInfo.InvariantCulture)}px; " +
                $"--Dimension: {Parameters.Dimension.ToString(CultureInfo.InvariantCulture)}%; " +
                $"--BackgroundColour: {Parameters.BackgroundColour}; ";
        if (Attributes is not null && Attributes.TryGetValue("style", out var style))
        {
            Style += style.ToString();
            Attributes.Remove("style");
        }
    }

    int colourIndex = 0;
    private string GetColour(ChartSegment segment)
    {
        colourIndex++;
        if (colourIndex >= Parameters.MaxColours) colourIndex = 0;
        return string.IsNullOrEmpty(segment.ChartColor) ? Parameters.ChartColors[colourIndex].Background : segment.ChartColor;
    }

    private double GetPercentageWidth(double quantity)
    {
        return quantity / MaxQuantity * 100;
    }
}