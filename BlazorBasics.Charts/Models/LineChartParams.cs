namespace BlazorBasics.Charts.Models;

public class LineChartParams(
    int width = 600,
    int height = 300,
    string backgroundColor = "transparent",
    string axisStroke = "black",
    int axisWidth = 2,
    string gridLineStroke = "black",
    int gridWidth = 1,
    string lineSeriesFill = "none",
    int lineSeriesWidth = 1,
    int dotRadius = 4,
    int stepsY = 3,
    bool showX = true,
    bool showY = true,
    bool showLegend = true,
    bool rotatedXLabels = false,
    double rotationAngleXLabel = 45,
    Func<string, string> formatterLabelPopup = null,
    Func<LineData, string> legendLabel = null,
    LineChartPointOptions pointOptions = null,
    int maxPointPerLine = 50,
    bool showLoading = true,
    bool showXLines = true,
    bool showYLines = true,
    Func<LineChartPoint, string> pointPopup = null
    )
{
    private readonly Func<LineChartPoint, string> ChosenPointPopup = ChoosePointPopup(formatterLabelPopup, pointPopup);

    public int Width => width;
    public int Height => height;
    public string BackgroundColor => backgroundColor;
    public string AxisStroke => axisStroke;
    public int AxisWidth => axisWidth;
    public string GridLineStroke => gridLineStroke;
    public int GridWidth => gridWidth;
    public string LineSeriesFill => lineSeriesFill;
    public int LineSeriesWidth => lineSeriesWidth;
    public int DotRadius => dotRadius;
    public int StepsY => stepsY;
    public bool ShowX => showX;
    public bool ShowY => showY;
    public bool ShowLegend => showLegend;
    public bool RotatedXLabels => rotatedXLabels;
    public double RotationAngleXLabel
    {
        get
        {
            if (rotationAngleXLabel < 0)
                throw new ArgumentException($"Must be positive", nameof(RotationAngleXLabel));
            if (rotationAngleXLabel > 90)
                throw new ArgumentException($"Must be less than 90", nameof(RotationAngleXLabel));
            return rotationAngleXLabel;
        }
    }
    [Obsolete("Use pointPopup, which receives the whole LineChartPoint. FormatterLabelPopup will be removed in the next version with breaking changes.")]
    public Func<string, string> FormatterLabelPopup { get; } = formatterLabelPopup;
    public Func<LineChartPoint, string> PointPopup => ChosenPointPopup;
    public Func<LineData, string> LegendLabel => legendLabel;
    public LineChartPointOptions PointOptions { get; } = pointOptions ?? new LineChartPointOptions();
    public int MaxPointPerLine => maxPointPerLine;
    public bool ShowLoading => showLoading;
    public bool ShowYLines => showYLines;
    public bool ShowXLines => showXLines;

    private static Func<LineChartPoint, string> ChoosePointPopup(
        Func<string, string> valuePopup, Func<LineChartPoint, string> pointPopup)
    {
        if (valuePopup is not null && pointPopup is not null)
        {
            throw new ArgumentException(
                "Use either pointPopup or the obsolete formatterLabelPopup, not both.", nameof(pointPopup));
        }

        Func<LineChartPoint, string> chosenPopup = pointPopup;
        if (valuePopup is not null)
        {
            chosenPopup = point => valuePopup(point.Value);
        }

        return chosenPopup;
    }
}
