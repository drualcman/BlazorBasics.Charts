namespace BlazorBasics.Charts.Handlers;
internal class LineChartCoordinatesHandler
{
    private readonly double MinX;
    private readonly double MaxX;
    private readonly double MinY;
    private readonly double MaxY;
    private readonly double PlotWidth;
    private readonly int MarginLeft;
    private readonly int MarginTop;
    private readonly double PlotHeight;
    private readonly CultureInfo ParsingCulture;

    public LineChartCoordinatesHandler(double minX, double maxX, double minY, double maxY, double plotWidth, int marginLeft, int marginTop, double plotHeight, CultureInfo parsingCulture)
    {
        MinX = minX;
        MaxX = maxX;
        MinY = minY;
        MaxY = maxY;
        PlotWidth = plotWidth;
        MarginLeft = marginLeft;
        MarginTop = marginTop;
        PlotHeight = plotHeight;
        ParsingCulture = parsingCulture;
    }

    internal ChartPoint GetCoordinates(ChartPoint point)
    {
        (int x, int y) cooredinates = SetCoordinated(point.X, point.Y);
        return new ChartPoint(cooredinates.x, cooredinates.y, point.Value);
    }

    internal string GetPoints(IEnumerable<ChartPoint> points)
    {
        StringBuilder result = new StringBuilder();
        IEnumerator<ChartPoint> enumerator = points.GetEnumerator();
        while (enumerator.MoveNext())
        {
            ChartPoint point = enumerator.Current;
            (int x, int y) cooredinates = SetCoordinated(point.X, point.Y);
            var line = $"{cooredinates.x.ToString(ParsingCulture)} {cooredinates.y.ToString(ParsingCulture)} ";
            result.Append(line);

        }
        return result.ToString();
    }

    private (int X, int Y) SetCoordinated(double xValue, double yValue)
    {
        double rangeX = MaxX - MinX;
        double rangeY = MaxY - MinY;

        // Avoid possible division by zero
        double normalizedX = Math.Abs(rangeX) < double.Epsilon ? 0.03 : (xValue - MinX) / rangeX;
        double normalizedY = Math.Abs(rangeY) < double.Epsilon ? 0.03 : (yValue - MinY) / rangeY;

        // Use rounding (not truncation) for more consistent pixel placement
        int x = MarginLeft + (int)Math.Round(normalizedX * PlotWidth);
        int y = MarginTop + (int)Math.Round((1.0 - normalizedY) * PlotHeight);
        return (x, y);
    }
}
