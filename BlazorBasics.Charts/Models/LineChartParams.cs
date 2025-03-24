namespace BlazorBasics.Charts.Models;

public class LineChartParams(
    int width = 600,
    int height = 300,
    string backgroundColor = "transparent",
    string axisStroke = "black",
    int axisWidth = 2,
    string gridLineStroke = "#ddd",
    int gridWidth = 1,
    string lineSeriesFill = "none",
    int lineSeriesWidth = 1,
    int dotRadius = 4,
    int stepsY = 1,
    bool showX = true,
    bool showY = true,
    bool showLegend = true,
    bool rotatedXLabels = false,
    double rotationAngleXLabel = 45,
    Func<string, string> formatterLabelPopup = null,
    Func<LineData, string> legendLabel = null
    )
{
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
            if(rotationAngleXLabel < 0)
                throw new ArgumentException($"Must be positive", nameof(RotationAngleXLabel));
            if(rotationAngleXLabel > 90)
                throw new ArgumentException($"Must be less than 90", nameof(RotationAngleXLabel));
            return rotationAngleXLabel;
        }
    }
    public Func<string, string> FormatterLabelPopup => formatterLabelPopup;
    public Func<LineData, string> LegendLabel => legendLabel;

}
