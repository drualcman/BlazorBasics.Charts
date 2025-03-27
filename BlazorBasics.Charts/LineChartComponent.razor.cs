namespace BlazorBasics.Charts;
public partial class LineChartComponent
{
    private const int AxisGap = 10;
    private const double AxisYPaddingRatio = 0.05;
    private const double AxisXPaddingRatio = 0.05;
    private const int MarginRight = 20;
    private const int MarginTop = 20;

    [Parameter] public LineChartData Data { get; set; }
    [Parameter] public LineChartParams Parameters { get; set; } = new();

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
    double MinX;
    double MaxX;
    double MinY;
    double MaxY;
    LineSeries SelectedSerie;
    ChartPoint SelectedPoint;
    bool NeedsRotation;

    protected override void OnParametersSet()
    {
        Width = Parameters.Width;
        Height = Parameters.Height;
        ChartData = new List<LineSeries>();
        List<double> allYValues = new List<double>();
        int maxValuesCount = Data.Data
        .Select(d => d.Values.Count())
        .DefaultIfEmpty(0)
        .Max();
        MinX = 1;
        MaxX = maxValuesCount;
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
        MinY = allYValues.Count > 0 ? allYValues.Min() : 0;
        MaxY = allYValues.Count > 0 ? allYValues.Max() : 1;
        double yRange = MaxY - MinY;
        if(yRange == 0)
        {
            yRange = Math.Abs(MaxY) * 0.1;
            if(yRange == 0)
                yRange = 1;
            MaxY += yRange / 2;
            MinY -= yRange / 2;
        }
        else
        {
            double yPadding = yRange * AxisYPaddingRatio;
            MaxY += yPadding;
            MinY -= yPadding;
        }
        double xPadding = (MaxX - MinX) * AxisXPaddingRatio;
        MaxX += xPadding;
        MinX -= xPadding;

        MinY = Math.Floor(MinY);
        MaxY = Math.Ceiling(MaxY);

        if(Data.XLabels is not null && Data.XLabels.Any())
        {
            string longestLabel = Data.XLabels.OrderByDescending(label => label.Length).First();
            int estimatedWidth = longestLabel.Length * 7;
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

        double percentUnderZero = Math.Abs(MinY) / (MaxY - MinY);
        int extraPixels = (int)(percentUnderZero * PlotHeight);
        ExtraBottomForRotatedLabels += extraPixels;
    }

    string GetPoints(IEnumerable<ChartPoint> points)
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

    IEnumerable<MarkupString> GetXLabels()
    {
        List<MarkupString> labels = new List<MarkupString>();
        List<string> customLabels = Data.XLabels?.ToList();
        bool hasCustomLabels = customLabels != null && customLabels.Any();
        int labelCount = hasCustomLabels ? customLabels.Count : (int)Math.Ceiling(MaxX);

        // Estimate spacing between each label
        double spacing = labelCount > 1 ? (double)PlotWidth / (labelCount - 1) : PlotWidth;

        for(int i = 0; i < labelCount; i++)
        {
            double percent = labelCount == 1 ? 0 : (double)i / (labelCount - 1);
            int x = MarginLeft + (int)(percent * PlotWidth);

            string label = (i + 1).ToString();
            if(hasCustomLabels)
                label = customLabels[i];

            var data = Data.Data.First();
            if(data.Values.Count() == labelCount)
            {
                List<ChartPoint> point = ChartData
                    .FirstOrDefault(p => p.Name == data.Name && p.Color == (string.IsNullOrEmpty(data.Color) ? "black" : data.Color))
                    ?.Values.ToList();

                if(point is not null && (i < point.Count))
                {
                    ChartPoint selection = GetCoordinates(point[i]);
                    x = (int)Math.Ceiling(selection.X) - AxisGap;
                }
            }

            int xLabel = x + AxisGap;

            // Estimate label width (average of 7px per character)    
            int fontSize = 12;
            int estimatedWidth = label.Length * 7;

            int yBase = Parameters.Height - MarginBottom;
            int yLabel;

            if(NeedsRotation)
            {
                double angleRad = ChartMathHelpers.CalculateRadious(Parameters.RotationAngleXLabel);
                double rotatedHeight = estimatedWidth * Math.Sin(angleRad) + fontSize * Math.Cos(angleRad);
                yLabel = yBase + AxisGap + (int)Math.Ceiling(rotatedHeight);
            }
            else
            {
                yLabel = yBase + (int)(AxisGap * 2.5);
            }

            string textSvg = NeedsRotation
                ? CreateRotatedSvgText(label, xLabel, yLabel, Parameters.RotationAngleXLabel, estimatedWidth)
                : CreateSvgText(label, xLabel, yLabel, "middle");

            string gridLine = CreateSvgLine(xLabel, MarginTop - (int)(AxisGap * 1.5), xLabel, Parameters.Height - MarginBottom + AxisGap);

            labels.Add((MarkupString)(gridLine + textSvg));
        }
        return labels;
    }


    private string CreateRotatedSvgText(string text, int x, int y, double angleDegrees, int estimatedWidth)
    {
        double angleRadians = ChartMathHelpers.CalculateRadious(angleDegrees);

        // Horizontal offset depends on the width and the cosine of the angle
        double offsetX = estimatedWidth * 0.5 * Math.Cos(angleRadians);

        // If angle is negative, we shift to the left instead of right
        int xCorrected = x + (int)Math.Round(offsetX);

        return $"<text x=\"{xCorrected}\" y=\"{y}\" text-anchor=\"end\" transform=\"rotate({angleDegrees},{xCorrected},{y})\" font-size=\"12\">{text}</text>";
    }




    IEnumerable<MarkupString> GetYLabels()
    {
        List<MarkupString> labels = new List<MarkupString>();
        List<string> customLabels = Data.YLabels?.ToList();
        int chartHeight = Parameters.Height - MarginTop - MarginBottom;
        int plotTop = MarginTop;
        int plotBottom = Parameters.Height - MarginBottom;
        int usableHeight = plotBottom - plotTop;

        if(customLabels != null && customLabels.Count > 0)
        {
            int x = MarginLeft - AxisGap;
            int count = customLabels.Count;

            for(int i = 0; i < count; i++)
            {
                string label = customLabels[i];
                // Repartimos desde 0 (abajo) hasta MaxY (arriba)
                double percent = (double)i / (count - 1);
                int y = plotBottom - (int)(percent * usableHeight);
                string textSvg = CreateSvgText(label, x, y, "end");
                string gridLine = CreateSvgLine(MarginLeft, y, Parameters.Width - MarginRight + AxisGap, y);
                labels.Add((MarkupString)(gridLine + textSvg));
            }
        }
        else
        {
            int minStepHeightPx = 10;
            int maxVisibleSteps = chartHeight / minStepHeightPx;
            int stepValue = Math.Max(1, Parameters.StepsY);
            int maxYCeiled = (int)Math.Ceiling(MaxY);
            int requiredSteps = maxYCeiled / stepValue + 1;

            if(requiredSteps > maxVisibleSteps)
            {
                stepValue = Math.Max(1, (int)Math.Ceiling((double)maxYCeiled / maxVisibleSteps));
            }

            int adjustedMaxY = (maxYCeiled + stepValue - 1) / stepValue * stepValue;

            for(int yValue = (int)MinY; yValue <= (int)MaxY; yValue += stepValue)
            {
                double percent = (yValue - MinY) / (MaxY - MinY);
                int y = plotBottom - (int)(percent * usableHeight);
                int x = MarginLeft - AxisGap;
                string label = yValue.ToString();
                string textSvg = CreateSvgText(label, x, y + 4, "end");
                string gridLine = CreateSvgLine(MarginLeft, y, Parameters.Width - MarginRight + AxisGap, y);
                labels.Add((MarkupString)(gridLine + textSvg));
            }
        }

        return labels;
    }

    private string CreateSvgText(string text, int x, int y, string anchor = "middle")
    {
        return $"<text x=\"{x}\" y=\"{y}\" text-anchor=\"{anchor}\" font-size=\"10\">{text}</text>";
    }

    private string CreateSvgLine(int x1, int y1, int x2, int y2)
    {
        return $"<line x1=\"{x1}\" y1=\"{y1}\" x2=\"{x2}\" y2=\"{y2}\" class=\"grid-line\" />";
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

    MarkupString GetSelectedPointLabelMarkup()
    {
        StringBuilder builder = new StringBuilder();
        if(SelectedPoint is not null)
        {
            if(Parameters.FormatterLabelPopup is null)
            {
                builder.Append($"<strong style='");
                builder.Append("font-size: medium;");
                builder.Append("font-weight: bold;");
                builder.Append("text-align: center;");
                builder.Append($"'>");
                builder.Append($"{SelectedPoint.Value}");
                builder.Append($"</strong>");
            }
            else
                builder.Append(Parameters.FormatterLabelPopup(SelectedPoint.Value));
        }
        return new MarkupString(builder.ToString());
    }

    MarkupString GetLeyendLabelMarkup(LineData serie)
    {
        StringBuilder builder = new StringBuilder();
        if(Parameters.LegendLabel is null)
        {
            builder.Append($"<div class=\"dot\" style=\"background-color: {serie?.Color ?? "black"};\"></div>");
            builder.Append($"<span>{serie?.Name}</span>");
        }
        else
            builder.Append(Parameters.LegendLabel(serie));
        return new MarkupString(builder.ToString());
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

    ChartPoint GetCoordinates(ChartPoint point)
    {
        (int x, int y) cooredinates = SetCoordinated(point.X, point.Y);
        return new(cooredinates.x, cooredinates.y, point.Value);
    }

    private (int X, int Y) SetCoordinated(double xValue, double yValue)
    {
        double normalizedX = (xValue - MinX) / (MaxX - MinX);
        double normalizedY = (yValue - MinY) / (MaxY - MinY);
        int x = MarginLeft + (int)(normalizedX * PlotWidth);
        int y = MarginTop + (int)((1 - normalizedY) * PlotHeight);
        return (x, y);
    }

}