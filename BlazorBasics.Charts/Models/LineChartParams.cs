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
    int dotRadio = 4,
    int stepsY = 1,
    bool showX = true,
    bool showY = true,
    Func<string, string> formaterLabelPopup = null,
    Func<LineData, string> leyendLabel = null
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
    public int DotRadio => dotRadio;
    public int StepsY => stepsY;
    public bool ShowX => showX;
    public bool ShowY => showY;
    public Func<string, string> FormaterLabelPopup => formaterLabelPopup;
    public Func<LineData, string> LegendLabel => leyendLabel;
}
