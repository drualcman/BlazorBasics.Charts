namespace BlazorBasics.Charts;

public partial class ColumnWithLineChartComponent
{
    private double MaxTotal;
    private string Style;
    private string WrapperCss = "";
    [Parameter] public IEnumerable<ChartDoubleSegment> Topics { get; set; }
    [Parameter] public ColumnsBarChartParams Parameters { get; set; } = new();

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; }

    protected override void OnParametersSet()
    {
        MaxTotal = Topics.Any() ? Topics.Sum(t => t.GetTotal()) : 0;
        if(MaxTotal == 0)
            MaxTotal = 0.0000000001;
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
        return total == 0 ? 0 : (value / MaxTotal * 100);
    }

    private double GetPercentageHeight(ChartDoubleSegment segment, bool isPrimary)
    {
        double total = MaxTotal - segment.GetTotal();
        double percentage = 0;
        if(isPrimary)
        {
            percentage = segment.PrimaryValue;
        }
        else
        {
            percentage = total - segment.PrimaryValue;

        }
        return (percentage / total * 100) / MaxTotal * 100;
    }
}