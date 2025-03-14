[![Nuget](https://img.shields.io/nuget/v/BlazorBasics.Charts?style=for-the-badge)](https://www.nuget.org/packages/BlazorBasics.Charts)
[![Nuget](https://img.shields.io/nuget/dt/BlazorBasics.Charts?style=for-the-badge)](https://www.nuget.org/packages/BlazorBasics.Charts)

# Description
Create charts simples from data. You can create a Pie Chart, Columns Chart or Bar Chart.
# How to use simple way
Import the name space adding to _Imports.razor this line:
```
@using BlazorBasics.Charts
```
## Pie Chart
Example about Pie Chart. Values can't be more than 100% about total values.
``` razor
<PieChartComponent DataSource=GetPieSegments />

@code {
    Task<IReadOnlyList<ChartSegment>> GetPieSegments()
    {
        IReadOnlyList<ChartSegment> segments = new();
        ...
        return Task.FromResult(segments);
    } 
}
```
Also you can set some parameters
``` csharp
public class PieChartParams
{
    public PieChartParams(int width = 150, int height = 150,
        double saturation = 100.0, double luminosity = 50.0,
        int separationOffset = 15, int separationOnSelectOffset = 15,
        double delayTime = 0, string title = "",
        IEnumerable<ChartColor> chartColours = null,
        bool showLabels = false, double centerTextSeparationPercentage = 0.85,
        bool separateBiggerByDefault = true, bool showBiggestLabel = false,
        bool showLegent = true)
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
        ShowLegent = showLegent;
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
    public bool ShowLegent { get; set; }
}
```
Then you can do
``` razor
<PieChartComponent DataSource=GetPieSegments Parameters=PieParams />

@code {
    PieChartParams PieParams = new PieChartParams(title: "Totals");

    Task<IReadOnlyList<ChartSegment>> GetPieSegments()
    {
        IReadOnlyList<ChartSegment> segments = new();
        ...
        return Task.FromResult(segments);
    } 
}
```

## Column or Bar Chart
Example about Column or Bar Chart.
``` razor
 <ColumnChartComponent Topics=Totals />

@code {
    IEnumerable<ChartSegment> Totals;

    protected override void OnInitialized()
    {
        Totals =
        [
            new ChartSegment
            {
                Value = "1",
                Name = "Total Requests"
            },
            ...
        ];
    }
}
```
Also you can set some parameters
``` csharp
public class ColumnsBarChartParams
{
    public ColumnsBarChartParams(string backgroundColour = "#D3D3D3", int thickness = 20, int dimension = 100,
        bool showValues = true,
        IEnumerable<ChartColor> chartColours = null)
    {
        BackgroundColour = backgroundColour;
        Thickness = thickness;
        Dimension = dimension;
        ShowValues = showValues;
        ChartColors = new(chartColours ?? ChartColourHelper
            .InitializeColours(256, 30));
    }

    public string BackgroundColour { get; init; }
    public int Thickness { get; init; }
    public int Dimension { get; init; }
    public bool ShowValues { get; init; }
    public List<ChartColor> ChartColors { get; set; }
    public int MaxColours => ChartColors.Count;
}
```
Then you can do
``` razor
<BarChartComponent Topics=Totals Parameters=BarParams />

@code {
    ColumnsBarChartParams BarParams = new ColumnsBarChartParams();

    IEnumerable<ChartSegment> Totals;

    protected override void OnInitialized()
    {
        Totals =
        [
            new ChartSegment
            {
                Value = "1",
                Name = "Total Requests"
            },
            ...
        ];
    }
}
```

### Personalize Charts
All chart automatic set the colours depending in how many items have the data. But also you can personalize colours in the charts using the property ChartColours.
``` Razor
@code {
    ColumnsBarChartParams BarParams = new ColumnsBarChartParams();

    List<ChartColor> ChartColours =
       [
            new("#2E3092"),
            new("#00AEEF"),
            new("#EDF5FF"),
            new("#2E78E8"),
            new("#0C5FBA"),
            new("#D1E7FF"),
            new("#03B15E"),
            new("#E6FFEA"),
            new("#CEEED3"),
            new("#F98316"),
            new("#FFF2DA"),
            new("#FFD8B1"),
            new("#D94D4D"),
            new("#FFE7E7"),
            new("#FFCFCF")
       ];

    protected override void OnInitialized()
    {
        BarParams = new ColumnsBarChartParams(chartColours: ChartColours);
        // or BarParams.ChartColors = ChartColours;
    }
}
```
This is aplicable for Pie Chart, Column Chart and Bar Chart. Color can be rgb(0,0,0), HEX #000000 or hsl(1,80,40).

## Line Chart
Example about Line Chart.
``` razor
 <LineChart Data=ChartData />

@code {
    private LineChartData ChartData = new(new List<LineData>
    {
        new LineData("Line 1", "green", new List<string> { "10", "50", "15", "100", "20", "30", "25" }),
        new LineData("Line 2", "blue",  new List<string> { "0",  "5",  "50", "33",  "33", "8",  "12" }),
        new LineData("Line 3", "red",   new List<string> { "5",  "50", "10", "33",  "8",  "12", "15" })
    });
}
```
Also you can set some parameters
``` csharp
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
    bool showLegend = true,
    Func<string, string> formaterLabelPopup = null,
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
    public int DotRadio => dotRadio;
    public int StepsY => stepsY;
    public bool ShowX => showX;
    public bool ShowY => showY;
    public bool ShowLegend => showLegend;
    public Func<string, string> FormaterLabelPopup => formaterLabelPopup;
    public Func<LineData, string> LegendLabel => legendLabel;
}
```
Then you can do
``` razor
 <LineChart Data=ChartData Params=LineParams />

@code {
    LineChartParams LineParams = new LineChartParams();

    private LineChartData ChartData = new(new List<LineData>
    {
        new LineData("Line 1", "green", new List<string> { "10", "50", "15", "100", "20", "30", "25" }),
        new LineData("Line 2", "blue",  new List<string> { "0",  "5",  "50", "33",  "33", "8",  "12" }),
        new LineData("Line 3", "red",   new List<string> { "5",  "50", "10", "33",  "8",  "12", "15" })
    });

    protected override void OnInitialized()
    {
        LineParams= new(formaterLabelPopup: GetSelectedPointLabelMarkup);
    }

    private string GetSelectedPointLabelMarkup(string value)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("<div style='");
        builder.Append("pointer-events: none;");
        builder.Append("background-color: blue;");
        builder.Append("box-shadow: 0 2px 6px rgba(0,0,0,0.2);");
        builder.Append("padding: 6px 12px;");
        builder.Append("border-radius: 6px;");
        builder.Append("text-align: center;");
        builder.Append("'>");
        builder.Append($"<strong  style='");
        builder.Append("color: white;");
        builder.Append("font-size: medium;");
        builder.Append("font-weight: bold;");
        builder.Append($"'>");
        builder.Append($"{value}");
        builder.Append($"</strong>");
        builder.Append("</div>");
        return builder.ToString();
    }
}
```
## Ring Percentage
Example about Line Chart.
``` razor
 <RingPercentageComponent Percentage=69 />
```
Also you can set some parameters
``` csharp
public class RingParams
{
    public int Width { get; init; }
    public int Height { get; init; }
    public double FontSizeRatio { get; init; }
    public string LabelColor { get; init; }
    public string FromColor { get; init; }
    public string ToColor { get; init; }
    public string CircunferenceColour { get; init; }
    public int StrokeWidth { get; init; }

    public RingParams(int width = 0, int height = 0, double fontPerspective = 3.5, string labelColor = "green",
        string fromColor = "#FFD700", string toColor = "#B22222", string circunferenceColour = "#eee", int strokeWidth = 10)
    {
        Width = width;
        Height = height;
        FontSizeRatio = fontPerspective;
        LabelColor = labelColor;
        FromColor = fromColor;
        ToColor = toColor;
        CircunferenceColour = circunferenceColour;
        StrokeWidth = strokeWidth;
    }
}
```
Then you can do
``` razor
 <RingPercentageComponent Percentage=33 Parameters="new RingParams(width: 100)" />
```
