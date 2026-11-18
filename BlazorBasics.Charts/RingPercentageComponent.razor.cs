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
        Style = $"--Width: {CssSize(Parameters.Width)}; " +
                $"--Height: {CssSize(Parameters.Height)}; " +
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

    private const int DEFAULT_SIZE = 120;

    private readonly string GradientId = $"progress-ring-gradient-{Guid.NewGuid():N}";

    private static string CssSize(int pixels) =>
        pixels > 0 ? $"{pixels.ToString(CultureInfo.InvariantCulture)}px" : "auto";

    private int FontReferenceSize =>
        Parameters.Width > 0 ? Parameters.Width : Parameters.Height > 0 ? Parameters.Height : DEFAULT_SIZE;

    public string CalculateFontSize()
    {
        int fontSize = (int)(FontReferenceSize / Parameters.FontSizeRatio);
        return $"{fontSize.ToString(CultureInfo.InvariantCulture)}px";
    }
}