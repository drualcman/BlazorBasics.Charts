[![Nuget](https://img.shields.io/nuget/v/BlazorBasics.Charts?style=for-the-badge)](https://www.nuget.org/packages/BlazorBasics.Charts)
[![Nuget](https://img.shields.io/nuget/dt/BlazorBasics.Charts?style=for-the-badge)](https://www.nuget.org/packages/BlazorBasics.Charts)

# Description
Simple SVG charts for Blazor, with no JavaScript. It works with Blazor WebAssembly, Blazor Server and Blazor Web Apps on .NET 8, 9, 10 and 11.

| Component | What it draws |
|---|---|
| `PieChartComponent` | Pie of percentages, with legend and hover labels |
| `ColumnChartComponent` | Vertical columns, one per value |
| `BarChartComponent` | Horizontal bars, one per value |
| `StackedBarChartComponent` | One single bar split into the share each value takes of the whole |
| `LineChartComponent` | One or more lines over the same X axis |
| `RingPercentageComponent` | A ring filled up to a percentage |
| `ColumnWithLineChartComponent` | Columns split in two values, with lines for their percentages |

# Getting started
Install the package:
```
dotnet add package BlazorBasics.Charts
```
Add the namespaces to `_Imports.razor`. The components live in `BlazorBasics.Charts` and their data and parameters in `BlazorBasics.Charts.Models`:
```razor
@using BlazorBasics.Charts
@using BlazorBasics.Charts.Models
```

## Upgrading to 1.4
Version 1.4 removes the options that never had any effect:
* `PieChartParams` no longer has `Saturation` and `Luminosity`, nor their constructor parameters. A `PieChartParams` built with positional arguments after `height` must name them now, for example `new PieChartParams(150, 150, separationOffset: 20)`.
* `ColumnWithLineChartParams` no longer has `Width` and `BackgroundColor`. Size the chart with `Height`, `BarWidth` and `Spacing`, and give it a background with the `style` or `class` of the component.

## What every chart shares
* **`Parameters`**: every component takes an optional `Parameters` object with its options. Left out, the defaults below are used.
* **`class` and `style`**: any `class` or `style` attribute given to a component is added to its wrapper, so the page can size and place it.
* **Size**: the charts are drawn in SVG. The sizes in the parameters are the size of the drawing, and the drawing scales to the width the page gives it, keeping its proportions. The exceptions are the pie and the ring, whose `Width` and `Height` are pixels.
* **Colours**: when a value does not bring its own colour, the chart picks one from `ChartColors`. See [Personalize the colours](#personalize-the-colours).
* **Clicks**: every chart made of parts says which one was clicked. See [Reacting to what the user clicks](#reacting-to-what-the-user-clicks).

## The data: ChartSegment
The pie, column, bar and stacked bar charts take their values as `ChartSegment`:
``` csharp
public class ChartSegment
{
    public string Name { get; set; }          // label of the value
    public double Value { get; set; }
    public string ChartColor { get; set; }    // optional, its own colour instead of the palette
    public string LabelColor { get; set; }    // pie only: colour of its text in the legend
    public bool IsSelected { get; set; }      // pie only: the slice that starts pulled out
    public Func<ChartSegment, string> SetTitleTopic { get; set; } // pie only: text of its label
}
```

# Pie Chart
Draws percentages. The values are read as percentages of the whole, so they must not add up to more than 100: if they do, the chart throws an `OverflowException`. Only one segment can have `IsSelected`, or it throws an `ArgumentException`.

``` razor
<PieChartComponent DataSource=GetPieSegments Parameters=PieParams />

@code {
    PieChartParams PieParams = new(title: "Requests");

    Task<IReadOnlyList<ChartSegment>> GetPieSegments()
    {
        IReadOnlyList<ChartSegment> segments =
        [
            new ChartSegment { Name = "Approved", Value = 62.5 },
            new ChartSegment { Name = "Pending", Value = 25 },
            new ChartSegment { Name = "Rejected", Value = 12.5 }
        ];
        return Task.FromResult(segments);
    }
}
```

### Component parameters
| Parameter | Type | What it does |
|---|---|---|
| `DataSource` | `Func<Task<IReadOnlyList<ChartSegment>>>` | Gives the segments. It is called once, when the chart is first drawn. |
| `Parameters` | `PieChartParams` | Options, see below. |
| `ChildContent` | `RenderFragment<ChartSegment>` | Template for each entry of the legend. When given, it replaces the default legend, which also leaves out `Title`. |
| `OnHover` | `Func<ChartSegment, string>` | Markup shown over the pie for the segment under the mouse. By default it is the text of the segment (see `SetTitleTopic`) or its value as a percentage. |
| `OnClick` | `EventCallback<ChartSegment>` | Raised when a slice or its legend entry is clicked. |

To change the data after the first draw, keep a reference to the component and call `UpdateData`:
``` razor
<PieChartComponent @ref=Pie DataSource=GetPieSegments />

@code {
    PieChartComponent Pie;

    void Refresh(IReadOnlyList<ChartSegment> segments) => Pie.UpdateData(segments);
}
```

### PieChartParams
``` csharp
new PieChartParams(
    int width = 150, int height = 150,
    int separationOffset = 15, int separationOnSelectOffset = 15,
    double delayTime = 0, string title = "",
    IEnumerable<ChartColor> chartColours = null,
    bool showLabels = false, double centerTextSeparationPercentage = 0.85,
    bool separateBiggerByDefault = true, bool showBiggestLabel = false,
    bool showLegend = true)
```
| Option | Default | What it does |
|---|---|---|
| `Width`, `Height` | 150, 150 | Size of the pie, in pixels. |
| `SeparationOffset` | 15 | Room left around the pie for a slice to move out. It also sets how far apart the hues of the default palette are. |
| `SeparationOnSelectOffset` | 15 | Added to `SeparationOffset`, it gives how far a hovered or selected slice moves out. |
| `DelayTime` | 0 | Milliseconds between one slice appearing and the next. With 0 the pie is drawn at once, without animation. |
| `Title` | `""` | First line of the legend. |
| `ChartColors` | generated | Palette of the slices, see [Personalize the colours](#personalize-the-colours). |
| `ShowLabels` | `false` | Writes the label of every slice over the pie at all times. |
| `CenterTextSeparationPercentage` | 0.85 | How far from the centre the labels are written, as a fraction of the radius. |
| `SeparateBiggerByDefault` | `true` | Pulls out the biggest slice while nothing is hovered. A segment with `IsSelected` is pulled out instead of the biggest one. |
| `ShowBiggestLabel` | `false` | Writes the label of the biggest slice while nothing is hovered. Ignored when `ShowLabels` is on. |
| `ShowLegend` | `true` | Shows the legend, with the name and the percentage of every slice. |

# Column and Bar Charts
`ColumnChartComponent` draws one vertical column per value and `BarChartComponent` one horizontal bar per value. Both take the same data and the same parameters.

``` razor
<ColumnChartComponent Topics=Totals />
<BarChartComponent Topics=Totals Parameters=BarParams />

@code {
    ColumnsBarChartParams BarParams = new(showValues: true);

    IEnumerable<ChartSegment> Totals =
    [
        new ChartSegment { Name = "Total Requests", Value = 120 },
        new ChartSegment { Name = "Approved", Value = 86 },
        new ChartSegment { Name = "Rejected", Value = 9 }
    ];
}
```

### Component parameters
| Parameter | Type | What it does |
|---|---|---|
| `Topics` | `IEnumerable<ChartSegment>` | The values, one column or bar each. |
| `Parameters` | `ColumnsBarChartParams` | Options, see below. |
| `OnClick` | `EventCallback<ChartSegment>` | Raised when a column or bar, the empty part of it or its label is clicked. |

### ColumnsBarChartParams
``` csharp
new ColumnsBarChartParams(
    string backgroundColour = "#D3D3D3",
    int thickness = 20,
    int dimension = 100,
    bool showValues = false,
    IEnumerable<ChartColor> chartColours = null,
    int gap = 5,
    int maxWidth = 600,
    bool rotatedLabels = false,
    double labelRotationAngle = ColumnsBarChartParams.VERTICAL_LABEL_ANGLE,
    int labelFontSize = 12,
    ColumnLabelPlacement? labelPlacement = null,
    ColumnFillReference fillReference = ColumnFillReference.HighestValue)
```
| Option | Default | Column chart | Bar chart |
|---|---|---|---|
| `BackgroundColour` | `#D3D3D3` | Colour of the empty part of each column. | Colour of the empty part of each bar. |
| `Thickness` | 20 | Width of each column. It is narrowed when the columns do not fit in `MaxWidth`. | Height of each bar. |
| `Dimension` | 100 | Height of the drawing. | Width of the chart, as a percentage of its container. |
| `MaxWidth` | 600 | Width of the drawing, shared out evenly between the columns. | Width of the drawing. |
| `Gap` | 5 | Not used: the columns are spread evenly. | Space between one bar and the next. |
| `ShowValues` | `false` | Writes the value above each column. | Writes the value after each bar. |
| `ChartColors` | generated | Palette, used for the values without their own `ChartColor`. | Same. |
| `RotatedLabels`, `LabelRotationAngle` | `false`, -90 | Rotate the labels, see [Vertical labels](#vertical-labels-on-a-column-chart). | Ignored. |
| `LabelFontSize` | 12 | Size of the labels. | Size of the labels. |
| `LabelPlacement` | `null` | See [Where the labels go](#where-the-labels-go). | Same. |
| `FillReference` | `HighestValue` | See [How much gets filled](#how-much-of-the-column-or-bar-gets-filled). | Same. |

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

# Stacked Bar Chart
Draws every value as a slice of one single bar, so what is read is the share each value takes of the whole rather than how the values compare to each other. It is the data of a pie chart drawn as a bar, but the values do not need to be percentages.

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

    IEnumerable<ChartSegment> Totals =
    [
        new ChartSegment { Name = "Tagum", Value = 1204 },
        new ChartSegment { Name = "Asuncion", Value = 486 }
    ];
}
```

### Component parameters
| Parameter | Type | What it does |
|---|---|---|
| `Topics` | `IEnumerable<ChartSegment>` | The values, one segment each. |
| `Parameters` | `StackedBarChartParams` | Options, see below. |
| `OnClick` | `EventCallback<ChartSegment>` | Raised when a segment or its label is clicked. |

### StackedBarChartParams
``` csharp
new StackedBarChartParams(
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
* `LabelFontSize` sets the size of the labels.
* `MinimumLabelShare` leaves out the label of any segment smaller than that share of the whole, given as a fraction, because there is no room to write one without it landing on its neighbours. The room reserved for the labels is measured only over the ones that are actually drawn, so leaving the small ones out also makes the chart smaller. Set it to zero to label every segment.
* `ShowValues` writes the value across the middle of its own segment, in the contrasting colour, and only when the segment is long enough to hold it. It is ignored when the names already go `Inside`, so the two never collide.
* `Total` is what the complete bar is worth. Left at zero the values are added together and the bar is always full; set to a known total, whatever the values do not account for stays as unfilled bar in `BackgroundColour`.

The chart asks for its own size, which for a vertical bar is narrow and tall. Its wrapper carries the `stacked-bar-chart` class plus `is-vertical` or `is-horizontal`, so stretching it, or leaving it alone, is up to the css of the page:
``` css
.my-card ::deep .stacked-bar-chart.is-horizontal svg { width: 100%; height: auto; }
.my-card ::deep .stacked-bar-chart.is-vertical svg { width: auto; max-width: 100%; }
```

# Personalize the colours
The pie, column, bar and stacked bar charts pick the colour of every value from their `ChartColors`, in order. Left out, a palette is generated. A `ChartSegment` with its own `ChartColor` keeps it.
``` razor
@code {
    static readonly List<ChartColor> ChartColours =
    [
        new("#2E3092"),
        new("#00AEEF"),
        new("#2E78E8"),
        new("#03B15E"),
        new("#F98316"),
        new("#D94D4D")
    ];

    ColumnsBarChartParams BarParams = new(chartColours: ChartColours);
    // or BarParams.ChartColors = ChartColours;
}
```
A colour can be written as `#000000`, `rgb(0,0,0)` or `hsl(1,80%,40%)`.

A `ChartColor` can also be given the colour of the text written over it, which is what the pie legend and the stacked bar use for the labels and values they write over a colour. With one colour only, the contrasting one is worked out for you.
``` csharp
new ChartColor("#2E3092", "#FFFFFF")   // background and text over it
new ChartColor("#2E3092")              // text colour worked out from the background
```

# Line Chart
Draws one or more series over the same X axis. The values are given as text and parsed with `ParsingCulture`.

``` razor
<LineChartComponent Data=ChartData Parameters=LineParams ParsingCulture=CultureInfo.InvariantCulture />

@code {
    LineChartParams LineParams = new(showLegend: true, pointOptions: new LineChartPointOptions(visibleAllPoints: true));

    LineChartData ChartData = new(
        [
            new LineData("Line 1", "green", ["10", "50", "15", "100", "20", "30", "25"]),
            new LineData("Line 2", "blue",  ["0",  "5",  "50", "33",  "33", "8",  "12"]),
            new LineData("Line 3", "red",   ["5",  "50", "10", "33",  "8",  "12", "15"])
        ],
        xLabels: ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"]);
}
```

### Component parameters
| Parameter | Type | What it does |
|---|---|---|
| `Data` | `LineChartData` | The series and the axis labels. |
| `Parameters` | `LineChartParams` | Options, see below. |
| `ParsingCulture` | `CultureInfo` | Culture used to read the values. Defaults to `CultureInfo.InvariantCulture`. |
| `OnLoading` | `EventCallback<bool>` | Raised with `true` when the chart starts working out a new set of data, and with `false` when it is drawn. |
| `OnPointClick` | `EventCallback<LineChartPoint>` | Raised when a dot is clicked. |
| `OnLegendClick` | `EventCallback<LineData>` | Raised when a series of the legend is clicked. |

The chart is worked out again only when `Data` is given a **new instance**. Changing the values inside the same `LineChartData`, or changing `Parameters` alone, does not redraw it.

### LineChartData and LineData
``` csharp
new LineChartData(
    IEnumerable<LineData> data,
    IEnumerable<string> xLabels = null,   // one label per value; when left out the values are numbered from 1
    IEnumerable<string> yLabels = null)   // labels of the Y axis, spread evenly from bottom to top

new LineData(string name, string color, IEnumerable<string> values)
```
Series with fewer values than the longest one are completed with zeros.

### LineChartParams
``` csharp
new LineChartParams(
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
    bool showYLines = true)
```
| Option | Default | What it does |
|---|---|---|
| `Width`, `Height` | 600, 300 | Size of the drawing. It scales to the width of its container. |
| `BackgroundColor` | `transparent` | Background of the chart. |
| `AxisStroke`, `AxisWidth` | `black`, 2 | Colour and width of the axes. |
| `GridLineStroke`, `GridWidth` | `black`, 1 | Colour and width of the grid lines. |
| `LineSeriesFill` | `none` | Fill under the lines. |
| `LineSeriesWidth` | 1 | Width of the lines. |
| `DotRadius` | 4 | Radius of the dots. |
| `StepsY` | 3 | How many steps the Y axis is divided in, when no `yLabels` are given. |
| `ShowX`, `ShowY` | `true` | Show the labels of each axis, together with their grid lines. |
| `ShowXLines` | `true` | Draws a **vertical** grid line at every X value. |
| `ShowYLines` | `true` | Draws a **horizontal** grid line at every step of the Y axis. |
| `ShowLegend` | `true` | Shows the legend under the chart. |
| `RotatedXLabels` | `false` | Rotates the X labels. They are also rotated on their own when they do not fit. When they do not fit even rotated, only five are written: the first, the last, the middle one and the two halfway between. |
| `RotationAngleXLabel` | 45 | Angle of the rotated X labels, from 0 to 90. Any other value throws an `ArgumentException`. |
| `FormatterLabelPopup` | `null` | Turns the value of a clicked dot into the markup of its popup. By default the value is written in bold. |
| `LegendLabel` | `null` | Turns a series into the markup of its legend entry. By default a dot of its colour and its name. |
| `PointOptions` | see below | Which dots are drawn. |
| `MaxPointPerLine` | 50 | Most dots drawn per series. A longer series keeps one value out of every few, plus the last one, so it draws faster; the X labels and their grid lines still follow every value. Set it to the number of values to draw them all. |
| `ShowLoading` | `true` | Shows a spinner while the chart is worked out. |

### LineChartPointOptions
``` csharp
new LineChartPointOptions(
    bool visibleAllPoints = false,
    bool visibleMaxPoint = true,
    bool visibleMinPoint = true,
    bool visibleMaxPointLine = false,
    bool visibleMinPointLine = false)
```
| Option | Default | What it does |
|---|---|---|
| `VisibleAllPoints` | `false` | Draws a dot at every value. |
| `VisibleMaxPoint`, `VisibleMinPoint` | `true` | When not every dot is drawn, draws the highest and the lowest dot of each series. |
| `VisibleMaxPointLine`, `VisibleMinPointLine` | `false` | Draws a dashed line across the chart at the highest value of all series (green) and at the lowest (red). |

### What the user can do with it
* Clicking a dot opens a popup with its value, written by `FormatterLabelPopup`. Clicking the popup closes it.
* Clicking a series in the legend leaves only that series on the chart, with all its dots. Clicking it again, or clicking anywhere else on the chart, brings every series back.

### A popup of your own
``` razor
<LineChartComponent Data=ChartData Parameters=LineParams />

@code {
    LineChartParams LineParams = new(formatterLabelPopup: value =>
        $"<div style='background-color: blue; color: white; padding: 6px 12px; border-radius: 6px;'><strong>{value}</strong></div>");
}
```

# Ring Percentage
A ring filled up to a percentage, with the percentage written in the middle.
``` razor
<RingPercentageComponent Percentage=69 />
<RingPercentageComponent Percentage=33 Parameters="new RingParams(width: 100, height: 100)" />
```

### Component parameters
| Parameter | Type | What it does |
|---|---|---|
| `Percentage` | `int` | How much of the ring is filled, from 0 to 100. |
| `Parameters` | `RingParams` | Options, see below. |

### RingParams
``` csharp
new RingParams(
    int width = 120, int height = 120,
    double fontPerspective = 3.5,
    string labelColor = "green",
    string fromColor = "#FFD700", string toColor = "#B22222",
    string circunferenceColour = "#eee",
    int strokeWidth = 10)
```
| Option | Default | What it does |
|---|---|---|
| `Width`, `Height` | 120, 120 | Size of the ring, in pixels. Zero leaves that side to the css of the page. |
| `FontSizeRatio` (`fontPerspective`) | 3.5 | Size of the percentage text, as `Width` divided by this ratio. Without a `Width`, `Height` is used, and without either, 120. |
| `LabelColor` | `green` | Colour of the percentage text. |
| `FromColor`, `ToColor` | `#FFD700`, `#B22222` | The filled part is a gradient from the first colour, at the top, to the second, at the bottom. |
| `CircunferenceColour` | `#eee` | Colour of the empty part of the ring. |
| `StrokeWidth` | 10 | Thickness of the ring. |

# Column with lines
Draws one column per item, split in a primary and a secondary value, with lines for the percentages:
* the **grand total** line places each item at the share it takes of the sum of all items;
* the **primary** and **secondary** lines place each item at the share its primary or secondary value takes of that item.

The columns are as tall as their total compared with the highest item.

``` razor
<ColumnWithLineChartComponent Data=ChartData Parameters=WithLineParams />

@code {
    ColumnWithLineChartParams WithLineParams = new() { PrimaryColor = "green", SecondaryColor = "blue" };

    ColumnWithLineChartData ChartData = new(
    [
        new("JAN", 200000, 184615),
        new("FEB", 300000, 245454),
        new("MAR", 400000, 289655)
    ])
    {
        Title = "Total users",
        PrimaryLegend = "Active Users",
        SecondaryLegend = "Non Active Users"
    };
}
```

### Component parameters
| Parameter | Type | What it does |
|---|---|---|
| `Data` | `ColumnWithLineChartData` | The items, the title and the names of the two values. |
| `Parameters` | `ColumnWithLineChartParams` | Options, see below. |
| `OnItemClick` | `EventCallback<ColumnDataItem>` | Raised when a column or a point of a line is clicked. |
| `OnPointClick` | `EventCallback<ColumnWithLinePoint>` | Raised when a point of a line is clicked, saying which line. |

### ColumnWithLineChartData and ColumnDataItem
``` csharp
public class ColumnWithLineChartData(IEnumerable<ColumnDataItem> data)
{
    public string Title { get; set; }
    public string PrimaryLegend { get; set; }     // name of the primary value
    public string SecondaryLegend { get; set; }   // name of the secondary value
    public IEnumerable<ColumnDataItem> Data { get; set; }
}

public class ColumnDataItem(string label, decimal primaryValue, decimal secondaryValue)
{
    public string Label { get; set; }
    public decimal PrimaryValue { get; set; }
    public decimal SecondaryValue { get; set; }
    public decimal Value => PrimaryValue + SecondaryValue;
}
```

### ColumnWithLineChartParams
Every option is a property with a setter, to be given with an object initializer.

| Option | Default | What it does |
|---|---|---|
| `Height` | 500 | Height of the drawing. The columns take half of it, but never less than `MinBarHeight`. |
| `MinBarHeight` | 150 | Least height of the columns. |
| `BarWidth` | 15 | Width of each column. |
| `Spacing` | 15 | Space between one column and the next. The width of the drawing follows from the number of items, `BarWidth` and `Spacing`, and it scales to its container. |
| `PrimaryColor`, `SecondaryColor` | `#4e79a7`, `#f28e2b` | Colours of the two parts of each column. |
| `GrandTotalLineColor` | `#e15759` | Colour of the grand total line. |
| `PrimaryPercentageLineColor`, `SecondaryPercentageLineColor` | `#59a84b`, `#ed49ff` | Colours of the primary and secondary lines. |
| `ShowTitle` | `true` | Writes `Title` above the chart. |
| `ShowLegend` | `true` | Shows the legend. |
| `ShowGranTotal` | `true` | Draws the grand total line, with its percentage written over every point. |
| `ShowPrimaryValues` | `true` | Draws the primary line. |
| `ShowSecondaryValues` | `false` | Draws the secondary line. |
| `BigTotalValueLabelFormatter` | `null` | Text over each point of the grand total line. By default its percentage. |
| `PrimaryValueLabelFormatter`, `SecondaryValueLabelFormatter` | `null` | Text over each point of the primary or secondary line. By default its percentage. |
| `BottomLabelFormatter` | `null` | Text under each column. By default the `Label` of the item. |
| `TooltipFormatter` | `null` | Markup of the tooltip shown when an item is clicked. By default its label and both values. |

### What the user can do with it
* Clicking a column or a point shows the tooltip of its item. Clicking a point also writes its value.
* Clicking the primary or the secondary entry of the legend writes the values over every point of that line. Clicking anywhere else on the chart hides them.

# Reacting to what the user clicks
Every chart made of parts tells you which one was clicked, so the page around it can react. The callback of each chart carries the type that chart works with, and nothing happens at all when no handler is given.

| Chart | Callback | What it carries | What is clickable |
|---|---|---|---|
| `PieChartComponent` | `OnClick` | `ChartSegment` | The slice and its entry in the legend |
| `ColumnChartComponent` | `OnClick` | `ChartSegment` | The column, the empty part of it and its label |
| `BarChartComponent` | `OnClick` | `ChartSegment` | The bar, the empty part of its row and its label |
| `StackedBarChartComponent` | `OnClick` | `ChartSegment` | Each segment and its label |
| `LineChartComponent` | `OnPointClick` | `LineChartPoint` | Every dot drawn, the highest and lowest ones included |
| `LineChartComponent` | `OnLegendClick` | `LineData` | Each series in the legend |
| `ColumnWithLineChartComponent` | `OnItemClick` | `ColumnDataItem` | The columns and the points of the lines |
| `ColumnWithLineChartComponent` | `OnPointClick` | `ColumnWithLinePoint` | The points of the lines, saying which line |

``` razor
<BarChartComponent Topics=Totals OnClick=Selected />
<StackedBarChartComponent Topics=Totals OnClick=Selected />
<LineChartComponent Data=ChartData OnPointClick=PointSelected OnLegendClick=SeriesSelected />

@code {
    void Selected(ChartSegment segment)
    {
        // segment.Name, segment.Value, segment.ChartColor ...
    }

    void PointSelected(LineChartPoint point)
    {
        // point.SeriesName, point.Index, point.Value, point.Label
    }

    void SeriesSelected(LineData series)
    {
        // series.Name and every value of that line
    }
}
```
The empty part of a column or of a bar answers the click with the same value as the filled part, so a small value is as easy to hit as a big one.

The charts that select something of their own keep doing it: the line chart still opens its popup on a point and still isolates a series from the legend, and the callback is raised as well as that, not instead of it.

``` csharp
public class LineChartPoint
{
    public string SeriesName { get; }
    public string Colour { get; }
    public int Index { get; }   // position in the series, counting from zero, also when MaxPointPerLine leaves values out
    public string Value { get; }  // the value as it was given, before being parsed
    public string Label { get; }  // the X axis label at that position, when there is one
}
```
A point of a `ColumnWithLineChartComponent` raises both of its callbacks, `OnItemClick` first and then `OnPointClick`; a column raises only the first. The points of the lines are drawn, and can therefore be clicked, only when `ShowGranTotal`, `ShowPrimaryValues` or `ShowSecondaryValues` are on, each one bringing its own line.

``` csharp
public class ColumnWithLinePoint
{
    public ColumnDataItem Item { get; }
    public int Index { get; }                 // position in the data, counting from zero
    public ColumnWithLineSeries Series { get; }  // GrandTotal, Primary or Secondary
    public string Percentage { get; }         // what the chart writes over that point
}
```

`RingPercentageComponent` has no callback: it is one whole ring, there are no parts to tell apart.

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
