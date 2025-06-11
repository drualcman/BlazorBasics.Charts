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

    protected override void OnParametersSet()
    {
        if(!(_data?.Equals(Data) ?? false))
        {
            if(Attributes is not null && Attributes.TryGetValue("class", out var css))
                WrapperCss = css.ToString();
            Style = $"--background-color: {Parameters.BackgroundColor}; " +
                    $"--axis-stroke: {Parameters.AxisStroke}; " +
                    $"--axis-width: {Parameters.AxisWidth}; " +
                    $"--grid-line-stroke: {Parameters.GridLineStroke}; " +
                    $"--grid-line-width: {Parameters.GridWidth}; " +
                    $"--line-series-fill: {Parameters.LineSeriesFill}; " +
                    $"--line-series-width: {Parameters.LineSeriesWidth}; ";
            if(Attributes is not null && Attributes.TryGetValue("style", out var style))
            {
                Style += style.ToString();
                Attributes.Remove("style");
            }
            Width = Parameters.Width;
            Height = Parameters.Height;
            ChartData = new List<LineSeries>();
            List<double> allYValues = new List<double>();
            int maxValuesCount = Data.Data
            .Select(d => d.Values.Count())
            .DefaultIfEmpty(0)
            .Max();
            double minX = 1;
            double maxX = maxValuesCount;
            IEnumerator<LineData> lines = Data.Data.GetEnumerator();
            while(lines.MoveNext())
            {
                LineData input = lines.Current;
                List<ChartPoint> points = new List<ChartPoint>();
                int originalCount = input.Values.Count();
                List<string> valueList = input.Values.ToList();
                for(int j = 0; j < maxValuesCount; j++)
                {
                    double x = j + 1;
                    double yFallback = j + 1;
                    double y = j < originalCount ? double.TryParse(valueList[j], out double value) ? value : yFallback : 0;

                    allYValues.Add(y);
                    string label = j < originalCount ? valueList[j] : "0";
                    points.Add(new ChartPoint(x, y, label));
                }
                ChartData.Add(new LineSeries(input.Name, string.IsNullOrEmpty(input.Color) ? "" : input.Color, points));
            }
            if(Data.YLabels is not null && Data.YLabels.Any())
            {
                string longestLabel = Data.YLabels.OrderByDescending(label => label.Length).FirstOrDefault();
                double fontWidth = 6.5;
                MaxYLabelWidth = (int)(longestLabel.Length * fontWidth) + 10;
            }
            else
            {
                MaxYLabelWidth = 30;
            }
            double minY = allYValues.Count > 0 ? allYValues.Min() : 0;
            double maxY = allYValues.Count > 0 ? allYValues.Max() : 1;
            double yRange = maxY - minY;
            if(yRange == 0)
            {
                yRange = Math.Abs(maxY) * 0.1;
                if(yRange == 0)
                    yRange = 1;
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

            minY = Math.Floor(minY);
            maxY = Math.Ceiling(maxY);

            if(Data.XLabels is not null && Data.XLabels.Any())
            {
                int estimatedWidth = Data.XLabels.Max(label => label.Length) * 7;
                int totalLabels = Data.XLabels.Count();
                double spacing = totalLabels > 1 ? PlotWidth / (totalLabels - 1) : PlotWidth;
                NeedsRotation = Parameters.RotatedXLabels || estimatedWidth > spacing;

                if(NeedsRotation)
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

            double percentUnderZero = Math.Abs(minY) / (maxY - minY);
            int extraPixels = (int)(percentUnderZero * PlotHeight);
            ExtraBottomForRotatedLabels += extraPixels;
            LineChartCoordinatesHandler = new LineChartCoordinatesHandler(minX, maxX, minY, maxY, PlotWidth, MarginLeft, MarginTop, PlotHeight);
            LineChartXHandler = new LineChartXHandler(AxisGap, NeedsRotation, Parameters.RotationAngleXLabel, PlotWidth, maxX, MarginTop, MarginLeft, MarginBottom, Parameters.Height, Data.Data, Data.XLabels, ChartData, LineChartCoordinatesHandler);
            LineChartYHandler = new LineChartYHandler(AxisGap, maxY, minY, MarginTop, MarginRight, MarginLeft, MarginBottom, Parameters.Width, Parameters.Height, Parameters.StepsY, Data.YLabels);
            LineChartMarkupHandler = new LineChartMarkupHandler(Parameters.FormatterLabelPopup, Parameters.LegendLabel);
            LabelsX = LineChartXHandler.GetXLabels();
            LabelsY = LineChartYHandler.GetYLabels();
            _data = Data;
        }
    }

    void OnPointClick(ChartPoint selection)
    {
        if(SelectedPoint != null &&
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
        if(serie.Equals(SelectedSerie))
            SelectedSerie = null;
        else
            SelectedSerie = serie;
    }

    void CancelSelections()
    {
        SelectedPoint = null;
        SelectedSerie = null;
    }
}