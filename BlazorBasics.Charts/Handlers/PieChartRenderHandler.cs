namespace BlazorBasics.Charts.Handlers;

internal class PieChartRenderHandler
{
    private readonly PieChartAnglesHandler Handler;
    private readonly PieChartParams Params;

    public PieChartRenderHandler(PieChartParams parameters)
    {
        Handler = new PieChartAnglesHandler(parameters.SeparationOffset);
        Params = parameters;
    }

    public IReadOnlyList<PieSegment> Segments { get; private set; }
    public int TotalWith => Params.Width + Params.SeparationOffset;
    public int TotalHeight => Params.Height + Params.SeparationOffset;
    public bool HasData => Segments?.Any() ?? false;

    public string GetPieContainerStyle()
    {
        return
            $"width: {TotalWith.ToString(CultureInfo.InvariantCulture)}px;height: {TotalHeight.ToString(CultureInfo.InvariantCulture)}px;";
    }

    public void SetPie(IReadOnlyList<ChartSegment> pieSegments)
    {
        var segments = new List<PieSegment>();
        var totalPercentage = Math.Round(pieSegments.Sum(p => p.Value), 2);
        if(totalPercentage > 100.0)
            throw new OverflowException(
                $"{Params.Title} => Percentage sum can't be greater than 100%: {totalPercentage}.");
        if(pieSegments.Count(s => s.IsSelected) > 1)
            throw new ArgumentException("Only one pie can be selected.");
        double lastValue = 0;
        var totalPies = pieSegments.Count;
        for(var i = 0; i < totalPies; i++)
        {
            var value = lastValue + Math.Round(pieSegments[i].Value, 2);
            int colourIndex = i < Params.MaxColours ? i : 0;
            if(string.IsNullOrEmpty(pieSegments[i].ChartColor))
                pieSegments[i].ChartColor = Params.ChartColors[colourIndex].Background;
            if(string.IsNullOrEmpty(pieSegments[i].LabelColor))
                pieSegments[i].LabelColor = Params.ChartColors[colourIndex].Foreground;
            segments.Add(new PieSegment(
                i,
                pieSegments[i],
                Math.Round(value, 2),
                Math.Round(lastValue, 2)
            ));
            lastValue = value;
        }

        Handler.SetLargest(segments);
        Segments = segments;
    }

    public string CreateSvgPieSegment(PieSegment segment)
    {
        var total = Segments.Sum(s => s.Pie.Value);
        var startAngle = Segments.TakeWhile(s => s != segment).Sum(s => 360 * (s.Pie.Value / total));
        var endAngle = startAngle + 360 * (segment.Pie.Value / total);
        return CreateArcPath(Params.Width / 2, Params.Height / 2, Params.Width / 2, startAngle, endAngle);
    }

    private string CreateArcPath(double cx, double cy, double radius, double startAngle, double endAngle)
    {
        startAngle = Handler.DegreeToRadian(startAngle);
        endAngle = Handler.DegreeToRadian(endAngle);
        var x1 = cx + radius * Math.Cos(startAngle);
        var y1 = cy + radius * Math.Sin(startAngle);
        var x2 = cx + radius * Math.Cos(endAngle);
        var y2 = cy + radius * Math.Sin(endAngle);
        var largeArc = endAngle - startAngle > Math.PI ? 1 : 0;
        return
            $"M {GetStringWithTwoDecimals(cx)},{GetStringWithTwoDecimals(cy)} L {GetStringWithTwoDecimals(x1)},{GetStringWithTwoDecimals(y1)} A {GetStringWithTwoDecimals(radius)},{GetStringWithTwoDecimals(radius)} 0 {GetStringWithTwoDecimals(largeArc)} 1 {GetStringWithTwoDecimals(x2)},{GetStringWithTwoDecimals(y2 - 0.01)} Z";
    }

    private string GetStringWithTwoDecimals(double value)
    {
        return Math.Round(value, 2).ToString(CultureInfo.InvariantCulture);
    }

    public double GetHorizontal(double angle)
    {
        return Handler.CalculateOffsetX(angle);
    }

    public double GetHorizontal(double angle, double offset)
    {
        return offset * Math.Cos(Handler.DegreeToRadian(angle));
    }

    public double GetVertial(double angle)
    {
        return Handler.CalculateOffsetY(angle);
    }

    public double GetVertical(double angle, double offset)
    {
        return offset * Math.Sin(Handler.DegreeToRadian(angle));
    }


    public double CalculateMiddleAngle(PieSegment segment)
    {
        var startAngle = segment.TransparentPercentage / 100 * 360;
        var endAngle = segment.FillPercentage / 100 * 360;
        return (startAngle + endAngle) / 2;
    }
}