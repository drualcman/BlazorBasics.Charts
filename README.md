[![Nuget](https://img.shields.io/nuget/v/BlazorBasics.Charts?style=for-the-badge)](https://www.nuget.org/packages/BlazorBasics.Charts)
[![Nuget](https://img.shields.io/nuget/dt/BlazorBasics.Charts?style=for-the-badge)](https://www.nuget.org/packages/BlazorBasics.Charts)

# Description
Create charts simples from data. You can create a Pie Chart, Columns Chart, Bar Chart or Stacked Bar Chart.
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
        bool showLegend = true)
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
        ShowLegend = showLegend;
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
    public bool ShowLegend { get; set; }
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
    public const double VERTICAL_LABEL_ANGLE = -90;

    public ColumnsBarChartParams(
        string backgroundColour = "#D3D3D3",
        int thickness = 20,
        int dimension = 100,
        bool showValues = false,
        IEnumerable<ChartColor> chartColours = null,
        int gap = 5,
        int maxWidth = 600,
        bool rotatedLabels = false,
        double labelRotationAngle = VERTICAL_LABEL_ANGLE,
        int labelFontSize = 12,
        ColumnLabelPlacement? labelPlacement = null,
        ColumnFillReference fillReference = ColumnFillReference.HighestValue)
    {
        BackgroundColour = backgroundColour;
        Thickness = thickness;
        Dimension = dimension;
        MaxWidth = maxWidth;
        ShowValues = showValues;
        Gap = gap;
        ChartColors = new(chartColours ?? ChartColourHelper
            .InitializeColours(256, 30));
        MaxWidth = maxWidth;
        RotatedLabels = rotatedLabels;
        LabelRotationAngle = labelRotationAngle;
        LabelFontSize = labelFontSize;
        LabelPlacement = labelPlacement;
        FillReference = fillReference;
    }

    public string BackgroundColour { get; init; }
    public int Thickness { get; init; }
    public int Dimension { get; init; }
    public int MaxWidth { get; init; }
    public int Gap { get; init; }
    public bool ShowValues { get; init; }
    public bool RotatedLabels { get; init; }
    public double LabelRotationAngle { get; init; }
    public int LabelFontSize { get; init; }
    public ColumnLabelPlacement? LabelPlacement { get; init; }
    public ColumnFillReference FillReference { get; init; }
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

### How much of the column or bar gets filled
`FillReference` decides what a completely filled shape means.

``` csharp
public enum ColumnFillReference
{
    HighestValue,     // default
    TotalOfAllValues
}
```
* `HighestValue`: the biggest value fills its shape completely and every other one is drawn against it. Use it to compare the values with each other. The empty part means nothing.
* `TotalOfAllValues`: the values are added together and each one fills its own share of that total, so a value worth a fifth of everything fills a fifth of its shape. Use it to read each value as part of the whole. The empty part is what the other values take.

`TotalOfAllValues` reads much better on `BarChartComponent` than on `ColumnChartComponent`, because a bar spreads the share along the whole width while a column only has its height, and a flat set of values leaves every column looking almost empty.

``` razor
<BarChartComponent Topics=Totals Parameters=BarParams />

@code {
    ColumnsBarChartParams BarParams =
        new(fillReference: ColumnFillReference.TotalOfAllValues);
}
```

### Where the labels go
`LabelPlacement` moves the label of every column or bar.

``` csharp
public enum ColumnLabelPlacement
{
    Bottom,
    Top,
    Left,
    Right
}
```
Left `null`, each chart keeps the placement it has always had: `Bottom` for `ColumnChartComponent` with horizontal labels, `Left` for `ColumnChartComponent` once the labels are rotated, and `Right` for `BarChartComponent`.

On **`ColumnChartComponent`**:
* `Bottom` and `Top` write the label outside the plotting area and the chart grows as much as the longest label needs, so nothing is ever cut.
* `Left` and `Right` run the label alongside its own column. The room a label has becomes the height of the chart instead of the width of the slot, so it is only shortened with an ellipsis when it does not fit in that height. This is the placement to use when the names are longer than the columns are wide.

On **`BarChartComponent`**:
* `Right` closes every row with its label, and `Left` reserves a column on the left and moves the bars across.
* `Top` and `Bottom` give the label a line of its own above or below its bar, with the full width to itself, so long names never need shortening. The rows grow to make room for it.

### Vertical labels on a column chart
`RotatedLabels` turns the labels of `ColumnChartComponent` by `LabelRotationAngle`. Negative angles read from bottom to top, positive ones from top to bottom, and `ColumnsBarChartParams.VERTICAL_LABEL_ANGLE` (-90) is fully vertical. `LabelFontSize` sets their size, in both charts.

``` razor
<ColumnChartComponent Topics=Totals Parameters=ColumnParams />

@code {
    ColumnsBarChartParams ColumnParams =
        new(thickness: 34, dimension: 240, rotatedLabels: true);
}
```
The rotation is only applied by `ColumnChartComponent`; on a horizontal bar chart it would not help, so `BarChartComponent` ignores it and uses `LabelPlacement` alone.

## Stacked Bar Chart
Draws every value as a slice of one single bar, so what is read is the share each value takes of the whole rather than how the values compare to each other. It is the data of a pie chart drawn as a bar.

``` razor
<StackedBarChartComponent Topics=Totals />

@code {
    IEnumerable<ChartSegment> Totals;

    protected override void OnInitialized()
    {
        Totals =
        [
            new ChartSegment { Name = "Tagum", Value = 1204 },
            new ChartSegment { Name = "Asuncion", Value = 486 },
            ...
        ];
    }
}
```
Also you can set some parameters
``` csharp
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

    public StackedBarOrientation Orientation { get; init; }
    public int Thickness { get; init; }
    public int Length { get; init; }
    public string BackgroundColour { get; init; }
    public bool ShowValues { get; init; }
    public StackedBarLabelSide LabelSide { get; init; }
    public StackedBarLabelAlignment LabelAlignment { get; init; }
    public int LabelFontSize { get; init; }
    public double MinimumLabelShare { get; init; }
    public double Total { get; init; }
    public List<ChartColor> ChartColors { get; set; }
    public int MaxColours => ChartColors.Count;
}

public enum StackedBarOrientation
{
    Vertical,     // one column, stacked from the top downwards
    Horizontal    // one bar, stacked from left to right
}

public enum StackedBarLabelSide
{
    Before,   // left of a vertical bar, above a horizontal one
    After,    // right of a vertical bar, below a horizontal one
    Inside    // over its own segment, in the contrasting colour of the segment
}

public enum StackedBarLabelAlignment
{
    Start,
    Middle,
    End
}
```
* `Thickness` is how wide a vertical bar is, or how tall a horizontal one is. `Length` is how long it runs in the direction it stacks in.
* The labels turn with the orientation and that is not configurable: they read horizontally beside a vertical bar and vertically above or below a horizontal one, which is the only way round they can be read in each case. `LabelSide` and `LabelAlignment` then work the same in both.
* `MinimumLabelShare` leaves out the label of any segment smaller than that share of the whole, given as a fraction, because there is no room to write one without it landing on its neighbours. The room reserved for the labels is measured only over the ones that are actually drawn, so leaving the small ones out also makes the chart smaller. Set it to zero to label every segment.
* `ShowValues` writes the value across the middle of its own segment, in the contrasting colour, and only when the segment is long enough to hold it. It is ignored when the names already go `Inside`, so the two never collide.
* `Total` is what the complete bar is worth. Left at zero the values are added together and the bar is always full; set to a known total, whatever the values do not account for stays as unfilled bar in `BackgroundColour`.

``` razor
<StackedBarChartComponent Topics=Totals Parameters=StackedParams />

@code {
    StackedBarChartParams StackedParams = new(
        StackedBarOrientation.Vertical,
        thickness: 46,
        length: 320,
        showValues: true,
        labelSide: StackedBarLabelSide.After,
        labelAlignment: StackedBarLabelAlignment.Start);
}
```
The chart asks for its own size, which for a vertical bar is narrow and tall. Its wrapper carries the `stacked-bar-chart` class plus `is-vertical` or `is-horizontal`, so stretching it, or leaving it alone, is up to the css of the page:
``` css
.my-card ::deep .stacked-bar-chart.is-horizontal svg { width: 100%; height: auto; }
.my-card ::deep .stacked-bar-chart.is-vertical svg { width: auto; max-width: 100%; }
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
This is aplicable for Pie Chart, Column Chart, Bar Chart and Stacked Bar Chart. Color can be rgb(0,0,0), HEX #000000 or hsl(1,80,40).

A `ChartColor` can also be given the colour of the text written over it, which is what the Stacked Bar Chart uses for the labels and values it writes inside a segment. With one colour only, the contrasting one is worked out for you.
``` csharp
new ChartColor("#2E3092", "#FFFFFF")   // background and text over it
new ChartColor("#2E3092")              // text colour worked out from the background
```

## Line Chart
Example about Line Chart.
``` razor
 <LineChartComponent Data=ChartData ParsingCulture=CultureInfo.InvariantCulture/>

@code {
    private LineChartData ChartData = new(new List<LineData>
    {
        new LineData("Line 1", "green", new List<string> { "10", "50", "15", "100", "20", "30", "25" }),
        new LineData("Line 2", "blue",  new List<string> { "0",  "5",  "50", "33",  "33", "8",  "12" }),
        new LineData("Line 3", "red",   new List<string> { "5",  "50", "10", "33",  "8",  "12", "15" })
    });
}
```
Example about Line Chart with event.
``` razor
 <LineChartComponent Data=ChartData OnLoading=CharLoading ParsingCulture=CultureInfo.InvariantCulture />

@code {
    private LineChartData ChartData = new(new List<LineData>
    {
        new LineData("Line 1", "green", new List<string> { "10", "50", "15", "100", "20", "30", "25" }),
        new LineData("Line 2", "blue",  new List<string> { "0",  "5",  "50", "33",  "33", "8",  "12" }),
        new LineData("Line 3", "red",   new List<string> { "5",  "50", "10", "33",  "8",  "12", "15" })
    });

    bool IsChartLoading;
    void CharLoading(bool state)
    {
        IsChartLoading = state;
    }
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
    bool showYLines = true
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
            if (rotationAngleXLabel < 0)
                throw new ArgumentException($"Must be positive", nameof(RotationAngleXLabel));
            if (rotationAngleXLabel > 90)
                throw new ArgumentException($"Must be less than 90", nameof(RotationAngleXLabel));
            return rotationAngleXLabel;
        }
    }
    public Func<string, string> FormatterLabelPopup => formatterLabelPopup;
    public Func<LineData, string> LegendLabel => legendLabel;
    public LineChartPointOptions PointOptions { get; } = pointOptions ?? new LineChartPointOptions();
    public int MaxPointPerLine => maxPointPerLine;
    public bool ShowLoading => showLoading;
    public bool ShowYLines => showYLines;
    public bool ShowXLines => showXLines;
}

public class LineChartPointOptions(
    bool visibleAllPoints = false,
    bool visibleMaxPoint = true,
    bool visibleMinPoint = true,
    bool visibleMaxPointLine = false,
    bool visibleMinPointLine = false
)
{
    public bool VisibleAllPoints => visibleAllPoints;
    public bool VisibleMaxPoint => visibleMaxPoint;
    public bool VisibleMinPoint => visibleMinPoint;
    public bool VisibleMaxPointLine => visibleMaxPointLine;
    public bool VisibleMinPointLine => visibleMinPointLine;
}
```
Then you can do
``` razor
 <LineChartComponent Data=ChartData Params=LineParams ParsingCulture=CultureInfo.InvariantCulture />

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
* RotationAngleXLabel must be between 0 and 90 degress to keep good view. Paramter outside will generate an ArgumentException.
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
## Column with lines
Example about ColumnWithLine Chart.
``` razor
 <ColumnWithLineChartComponent Data=ChartData />

@code {
    private ColumnWithLineChartData ChartData = new(new List<ColumnDataItem>
    {
        new("ENE",200000,184615) , 
        new("",0,0) { Label = "FEB", PrimaryValue = 300000, SecondaryValue = 245454 },  
        new("",0,0) { Label = "MAR", PrimaryValue = 400000, SecondaryValue = 289655 },  
        new("",0,0) { Label = "ABR", PrimaryValue = 500000, SecondaryValue = 319672 },  
        new("",0,0) { Label = "MAY", PrimaryValue = 600000, SecondaryValue = 452830 },  
        new("",0,0) { Label = "JUN", PrimaryValue = 700000, SecondaryValue = 466666 },  
    })
    {
         PrimaryLegend = "Active Users",
         SecondaryLegend = "Non Active Users",
         Title = "Total users" 
    };
}

```
Also you can set some parameters
``` csharp
public class ColumnWithLineChartParams
{
    public int Width { get; set; } = 800;
    public int Height { get; set; } = 500;
    public string BackgroundColor { get; set; } = "transparent";
    public int BarWidth { get; set; } = 15;
    public int MinBarHeight { get; set; } = 150;
    public int Spacing { get; set; } = 15;
    public string PrimaryColor { get; set; } = "#4e79a7";
    public string SecondaryColor { get; set; } = "#f28e2b";
    public string GrandTotalLineColor { get; set; } = "#e15759";
    public string PrimaryPercentageLineColor { get; set; } = "#59a84b";
    public string SecondaryPercentageLineColor { get; set; } = "#ed49ff";
    public bool ShowTitle { get; set; } = true;
    public bool ShowLegend { get; set; } = true;
    public bool ShowGranTotal { get; set; } = true;
    public bool ShowPrimaryValues { get; set; } = true;
    public bool ShowSecondaryValues { get; set; } = false;
    public Func<ColumnDataItem, string> BigTotalValueLabelFormatter { get; set; }
    public Func<ColumnDataItem, string> PrimaryValueLabelFormatter { get; set; }
    public Func<ColumnDataItem, string> SecondaryValueLabelFormatter { get; set; }
    public Func<ColumnDataItem, string> BottomLabelFormatter { get; set; }
    public Func<ColumnDataItem, string> TooltipFormatter { get; set; }
}
```
Then you can do
``` razor
  <ColumnWithLineChartComponent Data=ChartData Parameters=WithLineChartParams />
  @code {
    ColumnWithLineChartParams WithLineChartParams = new();

    protected override void OnInitialized()
    {
        WithLineChartParams.PrimaryColor = "green";
        WithLineChartParams.SecondaryColor = "blue";
    }

    private ColumnWithLineChartData ChartData = new(new List<ColumnDataItem>
    {
        new("ENE",200000,184615) , 
        new("",0,0) { Label = "FEB", PrimaryValue = 300000, SecondaryValue = 245454 },  
        new("",0,0) { Label = "MAR", PrimaryValue = 400000, SecondaryValue = 289655 },  
        new("",0,0) { Label = "ABR", PrimaryValue = 500000, SecondaryValue = 319672 },  
        new("",0,0) { Label = "MAY", PrimaryValue = 600000, SecondaryValue = 452830 },  
        new("",0,0) { Label = "JUN", PrimaryValue = 700000, SecondaryValue = 466666 },  
    })
    {
         PrimaryLegend = "Active Users",
         SecondaryLegend = "Non Active Users",
         Title = "Total users" 
    };
}
```

# Contributing
Contributions are welcome! Please feel free to submit a Pull Request.

- Fork the repository
- Create your feature branch (git checkout -b feature/AmazingFeature)
- Commit your changes (git commit -m 'Add some AmazingFeature')
- Push to the branch (git push origin feature/AmazingFeature)
- Open a Pull Request

# License
This project is licensed under the MIT License.

# Acknowledgments
- Built with love for the Blazor community
- Inspired by the need for simple, effective dashboard components
- Thanks to all contributors and users

# Made with love by DrUalcman

If you find this component useful, please consider giving it a star on GitHub!