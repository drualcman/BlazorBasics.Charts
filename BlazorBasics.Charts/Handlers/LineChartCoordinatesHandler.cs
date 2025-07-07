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

    public LineChartCoordinatesHandler(double minX, double maxX, double minY, double maxY, double plotWidth, int marginLeft, int marginTop, double plotHeight)
    {
        MinX = minX;
        MaxX = maxX;
        MinY = minY;
        MaxY = maxY;
        PlotWidth = plotWidth;
        MarginLeft = marginLeft;
        MarginTop = marginTop;
        PlotHeight = plotHeight;
    }

    internal ChartPoint GetCoordinates(ChartPoint point)
    {
        (int x, int y) cooredinates = SetCoordinated(point.X, point.Y);
        return new(cooredinates.x, cooredinates.y, point.Value);
    }

    internal string GetPoints(IEnumerable<ChartPoint> points)
    {
        StringBuilder result = new();
        IEnumerator<ChartPoint> enumerator = points.GetEnumerator();
        while(enumerator.MoveNext())
        {
            ChartPoint point = enumerator.Current;
            (int x, int y) cooredinates = SetCoordinated(point.X, point.Y);
            result.Append($"{cooredinates.x},{cooredinates.y} ");

        }
        return result.ToString();
    }

    private (int X, int Y) SetCoordinated(double xValue, double yValue)
    {
        double rangeX = MaxX - MinX;
        double rangeY = MaxY - MinY;

        //avoid possible division by zero
        double normalizedX = Math.Abs(rangeX) < double.Epsilon ? 0.03 : (xValue - MinX) / rangeX;
        double normalizedY = Math.Abs(rangeY) < double.Epsilon ? 0.03 : (yValue - MinY) / rangeY;

        int x = MarginLeft + (int)(normalizedX * PlotWidth);
        int y = MarginTop + (int)((1 - normalizedY) * PlotHeight);
        return (x, y);
    }
}
