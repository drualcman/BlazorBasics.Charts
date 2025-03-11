namespace BlazorBasics.Charts;
public partial class RingPercentageComponent
{
    [Parameter] public int Percentage { get; set; }
    [Parameter] public RingParams Parameters { get; set; } = new();
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> Attributes { get; set; }

    string WrapperCss = "";
    string Style;
    protected override void OnParametersSet()
    {
        if(Attributes is not null && Attributes.TryGetValue("class", out object css))
            WrapperCss = css.ToString();
        Style = $"--Width: {(Parameters.Width > 0 ? Parameters.Width.ToString(CultureInfo.InvariantCulture) : "auto")}px; " +
                $"--Height: {(Parameters.Width > 0 ? Parameters.Height.ToString(CultureInfo.InvariantCulture) : "auto")}px; " +
                $"--LabelColor: {Parameters.LabelColor.ToString(CultureInfo.InvariantCulture)}; " +
                $"--FromColor: {Parameters.FromColor.ToString(CultureInfo.InvariantCulture)}; " +
                $"--ToColor: {Parameters.ToColor.ToString(CultureInfo.InvariantCulture)}; " +
                $"--BackgroundColor: {Parameters.CircunferenceColour.ToString(CultureInfo.InvariantCulture)}; " +
                $"--Percentage: {Percentage.ToString(CultureInfo.InvariantCulture)}; " +
                $"--Circumference: {CalculateCircumference().ToString(CultureInfo.InvariantCulture)}; " +
                $"--Offset: {CalculateOffset().ToString(CultureInfo.InvariantCulture)}; " +
                $"--StrokeWidth: {Parameters.StrokeWidth.ToString(CultureInfo.InvariantCulture)};";
        if(Attributes is not null && Attributes.TryGetValue("style", out object style))
            Style += style.ToString();
    }
    private double CalculateCircumference() => 2 * Math.PI * 52;

    private double CalculateOffset() => CalculateCircumference() - CalculateCircumference() * Percentage / 100 - CalculateCircumference() / 360.0;

    public string CalculateFontSize()
    {
        int fontSize = (int)(Parameters.Width / Parameters.FontSizeRatio);
        return $"{fontSize.ToString(CultureInfo.InvariantCulture)}px";
    }
}