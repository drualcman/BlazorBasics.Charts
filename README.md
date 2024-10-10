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
        double saturation = 100.0, double luminosity = 50.0, int separationOffset = 30,
        double delayTime = 0, string title = "",
        IEnumerable<ChartColor> chartColours = null)
    {
        Width = width;
        Height = height;
        Saturation = saturation;
        Luminosity = luminosity;
        SeparationOffset = separationOffset;
        DelayTime = delayTime;
        Title = title;
        ChartColors = new(chartColours ?? ChartColourHelper
            .InitializeColours(256, separationOffset));
    }

    public int Width { get; init; }
    public int Height { get; init; }
    public double Saturation { get; init; }
    public double Luminosity { get; init; }
    public double DelayTime { get; init; }
    public int SeparationOffset { get; init; }
    public string Title { get; init; }
    public List<ChartColor> ChartColors { get; set; }
    public int MaxColours => ChartColors.Count;

}
```
Then you can do that
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
Then you can do that
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
