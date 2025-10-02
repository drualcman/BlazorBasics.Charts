using System.Collections.Concurrent;

namespace BlazorBasics.Charts;
public partial class LineChartComponent
{
    private const int AxisGap = 10;
    private const double AxisYPaddingRatio = 0.05;
    private const double AxisXPaddingRatio = 0.05;
    private const int MarginRight = 20;
    private const int MarginTop = 20;
    private string Style;
    private string WrapperCss = "";

    [Parameter] public LineChartData Data { get; set; }
    [Parameter] public LineChartParams Parameters { get; set; } = new();
    [Parameter] public CultureInfo ParsingCulture { get; set; } = CultureInfo.InvariantCulture;
    [Parameter] public EventCallback<bool> OnLoading { get; set; } = new();

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; }

    int Width = 0;
    int ViewBoxWidth => Width;
    int Height = 0;
    int ViewBoxHeight => Height;
    int MarginLeft => MaxYLabelWidth + AxisGap;
    int MarginBottom => ExtraBottomForRotatedLabels;
    double PlotWidth => ViewBoxWidth - MarginLeft - MarginRight;
    double PlotHeight => ViewBoxHeight - MarginTop - MarginBottom;
    int ExtraBottomForRotatedLabels = 0;

    List<LineSeries> ChartData;
    int MaxYLabelWidth = 0;
    LineSeries SelectedSerie;
    ChartPoint SelectedPoint;
    bool NeedsRotation;
    LineChartCoordinatesHandler LineChartCoordinatesHandler;
    LineChartXHandler LineChartXHandler;
    LineChartYHandler LineChartYHandler;
    LineChartMarkupHandler LineChartMarkupHandler;
    IEnumerable<MarkupString> LabelsX = [];
    IEnumerable<MarkupString> LabelsY = [];
    LineChartData _data;
    bool IsLoading;
    double? GlobalMinY;
    double? GlobalMaxY;

    protected override void OnParametersSet()
    {
        if (!ReferenceEquals(_data, Data))
        {
            if (!IsLoading)
            {
                IsLoading = true;
                SelectedSerie = null;
                SelectedPoint = null;
                if (Attributes is not null && Attributes.TryGetValue("class", out var css))
                    WrapperCss = css.ToString();
                Style = $"--background-color: {Parameters.BackgroundColor}; " +
                        $"--axis-stroke: {Parameters.AxisStroke}; " +
                        $"--axis-width: {Parameters.AxisWidth}; " +
                        $"--grid-line-stroke: {Parameters.GridLineStroke}; " +
                        $"--grid-line-width: {Parameters.GridWidth}; " +
                        $"--line-series-fill: {Parameters.LineSeriesFill}; " +
                        $"--line-series-width: {Parameters.LineSeriesWidth}; ";
                if (Attributes is not null && Attributes.TryGetValue("style", out var style))
                {
                    Style += style.ToString();
                    Attributes.Remove("style");
                }
                Width = Parameters.Width;
                Height = Parameters.Height;
            }
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!ReferenceEquals(_data, Data))
        {
            _data = Data;
            if (IsLoading)
            {
                if (OnLoading.HasDelegate)
                    await OnLoading.InvokeAsync(IsLoading);
                await Task.Yield();

                ConcurrentBag<double> allYValuesConcurrent = new ConcurrentBag<double>();
                ConcurrentBag<LineSeries> chartDataConcurrent = new ConcurrentBag<LineSeries>();

                int maxValuesCount = Data.Data
                    .Select(d => d.Values.Count())
                    .DefaultIfEmpty(0)
                    .Max();

                double minX = 1;
                double maxX = maxValuesCount;

                await Task.Run(() =>
                {

                    Parallel.ForEach(Data.Data, input =>
                    {
                        List<ChartPoint> points = new List<ChartPoint>();
                        int originalCount = input.Values.Count();
                        List<string> valueList = input.Values.ToList();

                        for (int j = 0; j < maxValuesCount; j++)
                        {
                            double x = j + 1;
                            double yFallback = j + 1;
                            double y = j < originalCount
                                ? double.TryParse(valueList[j], NumberStyles.Any, ParsingCulture, out double value) ? value : yFallback
                                : 0;
                            allYValuesConcurrent.Add(y);
                            string label = j < originalCount ? valueList[j] : "0";
                            points.Add(new ChartPoint(x, y, label));
                        }

                        List<ChartPoint> reducedPoints = ReduceResolution(points, Parameters.MaxPointPerLine);
                        (ChartPoint min, ChartPoint max) = GetMinMax(reducedPoints);

                        chartDataConcurrent.Add(new LineSeries(
                            input.Name,
                            string.IsNullOrEmpty(input.Color) ? "" : input.Color,
                            reducedPoints,
                            min,
                            max
                        ));
                    });
                });

                ChartData = chartDataConcurrent.ToList();
                List<double> allYValues = allYValuesConcurrent.ToList();

                if (Data.YLabels is not null && Data.YLabels.Any())
                {
                    string longestLabel = Data.YLabels.OrderByDescending(label => label.Length).FirstOrDefault();
                    double fontWidth = 6.5;
                    MaxYLabelWidth = (int)(longestLabel.Length * fontWidth) + 10;
                }
                else
                {
                    MaxYLabelWidth = 30;
                }

                // Compute raw min/max and pad
                double minY = allYValues.Count > 0 ? allYValues.Min() : 0;
                double maxY = allYValues.Count > 0 ? allYValues.Max() : 1;
                double yRange = maxY - minY;

                if (yRange == 0)
                {
                    yRange = Math.Abs(maxY) * 0.1;
                    if (yRange == 0)
                    {
                        yRange = 1;
                    }

                    maxY += yRange / 2;
                    minY -= yRange / 2;
                }
                else
                {
                    double yPadding = yRange * AxisYPaddingRatio;
                    maxY += yPadding;
                    minY -= yPadding;
                }

                double xPadding = (maxX - minX) * AxisXPaddingRatio;
                maxX += xPadding;
                minX -= xPadding;

                // Round to whole numbers for initial range (handler can refine)
                minY = Math.Floor(minY);
                maxY = Math.Ceiling(maxY);

                // --- IMPORTANT: compute X-label rotation and ExtraBottomForRotatedLabels BEFORE creating YHandler
                if (Data.XLabels is not null && Data.XLabels.Any())
                {
                    int estimatedWidth = Data.XLabels.Max(label => label.Length) * 7;
                    int totalLabels = Data.XLabels.Count();
                    double spacing = totalLabels > 1 ? PlotWidth / (totalLabels - 1) : PlotWidth;
                    NeedsRotation = Parameters.RotatedXLabels || estimatedWidth > spacing;

                    if (NeedsRotation)
                    {
                        double angleRad = ChartMathHelpers.CalculateRadious(Parameters.RotationAngleXLabel);
                        int fontSize = 12;
                        double rotatedHeight = estimatedWidth * Math.Sin(angleRad) + fontSize * Math.Cos(angleRad);
                        ExtraBottomForRotatedLabels = (int)Math.Ceiling(rotatedHeight) + AxisGap * 2;
                    }
                    else
                    {
                        ExtraBottomForRotatedLabels = AxisGap * 3;
                    }
                }
                else
                {
                    ExtraBottomForRotatedLabels = AxisGap * 3;
                }

                // Account for percent under zero (works with the final PlotHeight that uses ExtraBottomForRotatedLabels)
                double percentUnderZero = 0;
                if (minY < 0)
                {
                    percentUnderZero = Math.Abs(minY) / (maxY - minY);
                }
                int extraPixels = (int)(percentUnderZero * PlotHeight);
                ExtraBottomForRotatedLabels += extraPixels;

                minY = allYValues.Any() ? allYValues.Min() : 0.0;
                maxY = allYValues.Any() ? allYValues.Max() : 1.0;

                // Now create Y handler using the final MarginBottom (ExtraBottomForRotatedLabels)
                LineChartYHandler = new LineChartYHandler(
                    AxisGap,
                    maxY,
                    minY,
                    MarginTop,
                    MarginRight,
                    MarginLeft,
                    ExtraBottomForRotatedLabels,
                    Parameters.Width,
                    Parameters.Height,
                    Parameters.StepsY,
                    Data.YLabels,
                    Parameters.ShowYLines,
                    ParsingCulture
                );

                // Now create coordinates handler with the same final minY/maxY and final plot sizes
                LineChartCoordinatesHandler = new LineChartCoordinatesHandler(minX, maxX, minY, maxY, PlotWidth, MarginLeft, MarginTop, PlotHeight, ParsingCulture);
                LineChartXHandler = new LineChartXHandler(AxisGap, NeedsRotation, Parameters.RotationAngleXLabel, PlotWidth, maxX, MarginTop, MarginLeft, ExtraBottomForRotatedLabels, Parameters.Height, Data.Data, Data.XLabels, ChartData, LineChartCoordinatesHandler, Parameters.ShowXLines, ParsingCulture);
                LineChartMarkupHandler = new LineChartMarkupHandler(Parameters.FormatterLabelPopup, Parameters.LegendLabel);

                Console.WriteLine($"[DEBUG] PlotWidth={PlotWidth}, PlotHeight={PlotHeight}, MarginBottom={MarginBottom}, ExtraBottomForRotatedLabels={ExtraBottomForRotatedLabels}, minY={minY}, maxY={maxY}");

                await Task.WhenAll(
                    Task.Run(() =>
                    {
                        LabelsY = LineChartYHandler.GetYLabels();

                        // Read possibly adjusted min/max from the Y handler (handler can refine ticks and adjusted range)
                        minY = LineChartYHandler.MinY;
                        maxY = LineChartYHandler.MaxY;
                        foreach (LineSeries serie in ChartData)
                        {
                            serie.PointsString = LineChartCoordinatesHandler.GetPoints(serie.Values);
                        }
                    }),
                    Task.Run(() =>
                    {
                        if (Parameters.PointOptions.VisibleMaxPointLine)
                        {
                            ChartPoint globalMaxPoint = null;
                            foreach (LineSeries serie in ChartData)
                            {
                                if (serie.MaxPoint is not null)
                                {
                                    if (globalMaxPoint is null || serie.MaxPoint.Y > globalMaxPoint.Y)
                                        globalMaxPoint = serie.MaxPoint;
                                }
                            }

                            if (globalMaxPoint is not null)
                                GlobalMaxY = LineChartCoordinatesHandler.GetCoordinates(globalMaxPoint).Y;
                        }
                        else
                            GlobalMaxY = null;
                    }),
                    Task.Run(() =>
                    {
                        if (Parameters.PointOptions.VisibleMinPointLine)
                        {
                            ChartPoint globalMinPoint = null;
                            foreach (LineSeries serie in ChartData)
                            {
                                if (serie.MinPoint is not null)
                                {
                                    if (globalMinPoint is null || serie.MinPoint.Y < globalMinPoint.Y)
                                        globalMinPoint = serie.MinPoint;
                                }
                            }

                            if (globalMinPoint is not null)
                                GlobalMinY = LineChartCoordinatesHandler.GetCoordinates(globalMinPoint).Y;
                        }
                        else
                            GlobalMinY = null;
                    }),
                    Task.Run(() => LabelsX = LineChartXHandler.GetXLabels())
                );

                IsLoading = false;
                if (OnLoading.HasDelegate)
                    await OnLoading.InvokeAsync(IsLoading);
            }
        }
    }


    (ChartPoint Min, ChartPoint Max) GetMinMax(IReadOnlyList<ChartPoint> values)
    {
        if (values == null || values.Count == 0)
        {
            return (null, null);
        }

        ChartPoint min = values[0];
        ChartPoint max = values[0];

        for (int i = 1; i < values.Count; i++)
        {
            ChartPoint current = values[i];
            if (current.Y < min.Y)
            {
                min = current;
            }
            if (current.Y > max.Y)
            {
                max = current;
            }
        }

        return (min, max);
    }

    void OnPointClick(ChartPoint selection)
    {
        if (SelectedPoint != null &&
           SelectedPoint.X == selection.X &&
           SelectedPoint.Y == selection.Y)
        {
            SelectedPoint = null;
        }
        else
        {
            SelectedPoint = selection;
        }
    }

    void OnSelectLegend(LineSeries serie)
    {
        SelectedPoint = null;
        if (serie.Equals(SelectedSerie))
            SelectedSerie = null;
        else
            SelectedSerie = serie;
    }

    void CancelSelections()
    {
        SelectedPoint = null;
        SelectedSerie = null;
    }

    string GetPolilyneSerieKey(LineSeries serie)
    {
        string key = $"P{serie.Name}|{Parameters.Width}x{Parameters.Height}";
        return key;
    }

    string GetCircleSerieKey(LineSeries serie, int p)
    {
        string key = $"C{p}_{serie.Name}|{Parameters.Width}x{Parameters.Height}";
        return key;
    }

    private List<ChartPoint> ReduceResolution(List<ChartPoint> points, int maxPoints)
    {
        int count = points.Count;
        if (count <= maxPoints)
            return points;

        double step = (double)count / maxPoints;
        List<ChartPoint> reduced = new List<ChartPoint>();

        for (int i = 0; i < count; i += (int)Math.Ceiling(step))
        {
            reduced.Add(points[i]);
        }

        if (reduced[^1].X != points[^1].X)
            reduced.Add(points[^1]);

        return reduced;
    }
}
