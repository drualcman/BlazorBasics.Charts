namespace BlazorBasics.Charts.Handlers;
internal class LineChartXHandler
{
    private readonly int AxisGap;
    private readonly double RotationAngleXLabel;
    private readonly double PlotWidth;
    private readonly double MaxX;
    private readonly int MarginTop;
    private readonly int MarginLeft;
    private readonly int MarginBottom;
    private readonly int Height;
    private readonly IEnumerable<LineData> Data;
    private readonly IEnumerable<string> XLabels;
    private readonly List<LineSeries> ChartData;
    private readonly LineChartCoordinatesHandler LineChartCoordinatesHandler;
    private bool NeedsRotation;
    private bool ShowLines;
    private readonly CultureInfo ParsingCulture;

    public LineChartXHandler(int axisGap, bool needsRotation, double rotationAngleXLabel,
        double plotWidth, double maxX, int marginTop, int marginLeft, int marginBottom,
        int height, IEnumerable<LineData> data, IEnumerable<string> xLabels, List<LineSeries> chartData,
        LineChartCoordinatesHandler lineChartCoordinatesHandler, bool showLines, CultureInfo parsingCulture)
    {
        AxisGap = axisGap;
        NeedsRotation = needsRotation;
        RotationAngleXLabel = rotationAngleXLabel;
        PlotWidth = plotWidth;
        MaxX = maxX;
        MarginTop = marginTop;
        MarginLeft = marginLeft;
        MarginBottom = marginBottom;
        Height = height;
        Data = data;
        XLabels = xLabels;
        ChartData = chartData;
        LineChartCoordinatesHandler = lineChartCoordinatesHandler;
        ShowLines = showLines;
        ParsingCulture = parsingCulture;
    }

    internal IEnumerable<MarkupString> GetXLabels()
    {
        List<string> textLabels = new List<string>();
        List<string> gridLines = new List<string>();
        List<(int x, int y, int rotatedY, int estimatedWidth, string label)> positions = new();
        CalculateLabelPositions(positions, gridLines);
        AdjustLabelDisplay(positions);
        int i = 0;
        int labelCount = positions.Count - 1;
        foreach ((int x, int y, int rotatedY, int estimatedWidth, string label) in positions)
        {
            string textSvg = (i > 0 && i < labelCount) && NeedsRotation
                ? SvgHelper.CreateRotatedSvgText(label, x, rotatedY, RotationAngleXLabel, estimatedWidth)
                : SvgHelper.CreateSvgText(label, x, y, "middle");

            textLabels.Add(textSvg);
            i++;
        }
        return gridLines.Select((line, i) => (MarkupString)(line + (i < textLabels.Count ? textLabels[i] : "")));
    }

    void CalculateLabelPositions(List<(int x, int y, int rotatedY, int estimatedWidth, string label)> positions, List<string> gridLines)
    {
        positions.Clear();
        gridLines.Clear();
        List<string> customLabels = XLabels?.ToList();
        bool hasCustomLabels = customLabels != null && customLabels.Any();
        int labelCount = hasCustomLabels ? customLabels.Count : (int)Math.Ceiling(MaxX);
        double spacing = labelCount > 1 ? (double)PlotWidth / (labelCount - 1) : PlotWidth;
        for (int i = 0; i < labelCount; i++)
        {
            double percent = labelCount == 1 ? 0 : (double)i / (labelCount - 1);
            int x = MarginLeft + (int)(percent * PlotWidth);
            string label = (i + 1).ToString(ParsingCulture);
            if (hasCustomLabels)
                label = customLabels[i];
            var data = Data.First();
            if (data.Values.Count() == labelCount)
            {
                List<ChartPoint> point = ChartData
                    .FirstOrDefault(p => p.Name == data.Name && p.Color == (string.IsNullOrEmpty(data.Color) ? "black" : data.Color))
                    ?.Values.ToList();

                if (point is not null && (i < point.Count))
                {
                    ChartPoint selection = LineChartCoordinatesHandler.GetCoordinates(point[i]);
                    x = (int)Math.Ceiling(selection.X) - AxisGap;
                }
            }
            int xLabel = x + AxisGap;
            int fontSize = 12;
            int estimatedWidth = label.Length * 7;
            int yBase = Height - MarginBottom;
            int yLabel = yBase + (int)(AxisGap * 2.5);
            double angleRad = ChartMathHelpers.CalculateRadious(RotationAngleXLabel);
            double rotatedHeight = estimatedWidth * Math.Sin(angleRad) + fontSize * Math.Cos(angleRad);
            int rotatedY = yBase + AxisGap + (int)Math.Ceiling(rotatedHeight);
            positions.Add((xLabel, yLabel, rotatedY, estimatedWidth, label));
            string gridLine = ShowLines ? SvgHelper.CreateSvgLine(xLabel, MarginTop - (int)(AxisGap * 1.5), xLabel, Height - MarginBottom + AxisGap) : string.Empty;
            gridLines.Add(gridLine);
        }
    }

    void AdjustLabelDisplay(List<(int x, int y, int rotatedY, int e, string label)> positions)
    {
        if (positions.Count > 2)
        {
            int estimatedWidth = positions.Max(p => p.label.Length) * 7;
            int labelCount = positions.Count;
            double spacing = labelCount > 1 ? (double)PlotWidth / (labelCount - 1) : PlotWidth;
            if (NeedsRotation)
            {
                double angleRad = ChartMathHelpers.CalculateRadious(RotationAngleXLabel);
                double rotatedWidth = estimatedWidth * Math.Cos(angleRad);

                if (rotatedWidth > spacing)
                {
                    List<(int x, int y, int rotatedY, int e, string label)> reducedPositions = [positions[0]];

                    int middleIndex = (labelCount - 1) / 2;
                    int betweenFirstAndMiddle = (middleIndex) / 2;
                    reducedPositions.Add(positions[betweenFirstAndMiddle]);

                    reducedPositions.Add(positions[middleIndex]);

                    int betweenMiddleAndLast = middleIndex + (labelCount - 1 - middleIndex) / 2;
                    reducedPositions.Add(positions[betweenMiddleAndLast]);

                    reducedPositions.Add(positions[labelCount - 1]);

                    positions.Clear();
                    positions.AddRange(reducedPositions);
                    NeedsRotation = false;
                }
            }
        }
    }
}
