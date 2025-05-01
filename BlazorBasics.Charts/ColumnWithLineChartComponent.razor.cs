namespace BlazorBasics.Charts;

public partial class ColumnWithLineChartComponent
{
    private double TotalValue;
    private double MaxValue;
    private string Style;
    private string WrapperCss = "";
    [Parameter] public IEnumerable<ChartDoubleSegment> Topics { get; set; }
    [Parameter] public ColumnsBarChartParams Parameters { get; set; } = new();

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; }

    protected override void OnParametersSet()
    {
        TotalValue = Topics.Any() ? Topics.Sum(t => t.Value) : 0;
        if(TotalValue == 0)
            TotalValue = 0.0000000001;
        MaxValue = Topics.Any() ? Topics.Max(t => t.Value) : 0;
        if(MaxValue == 0)
            MaxValue = 0.0000000001;
        if(Attributes is not null && Attributes.TryGetValue("class", out var css))
            WrapperCss = css.ToString();

        Style = $"--Thickness: {Parameters.Thickness.ToString(CultureInfo.InvariantCulture)}px; " +
                $"--Dimension: {Parameters.Dimension.ToString(CultureInfo.InvariantCulture)}px; " +
                $"--BackgroundColour: {Parameters.BackgroundColour}; ";
        if(Attributes is not null && Attributes.TryGetValue("style", out var style))
        {
            Style += style.ToString();
            Attributes.Remove("style");
        }
    }

    int colourIndex = 0;
    private string GetColour(ChartDoubleSegment segment, bool isPrimary)
    {
        colourIndex++;
        if(colourIndex >= Parameters.MaxColours)
            colourIndex = 0;
        string result;
        if(isPrimary)
            result = string.IsNullOrEmpty(segment.PrimaryChartColor) ? Parameters.ChartColors[colourIndex].Background : segment.PrimaryChartColor;
        else
            result = string.IsNullOrEmpty(segment.SecondaryChartColor) ? Parameters.ChartColors[colourIndex].Background : segment.SecondaryChartColor;
        return result;
    }

    private double GetPercentageHeight(double value, double total)
    {
        return total == 0 ? 0 : (value / TotalValue * 100);
    }

    private double GetPercentageHeight(ChartDoubleSegment segment, bool isPrimary)
    {
        double value = isPrimary ? segment.PrimaryPercentage : segment.SecondaryPercentage;
        double container = segment.Value / TotalValue * 100;
        double portion = value / segment.Value * 100;
        double result = container * portion / 100;
        return result;
    }
}