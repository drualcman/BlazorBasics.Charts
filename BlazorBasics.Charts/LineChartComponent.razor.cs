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

    int ViewBoxWidth => Parameters.Width;
    int ViewBoxHeight => Parameters.Height;
    int MarginLeft => MaxYLabelWidth + AxisGap;
    int MarginBottom => 20 + AxisGap;
    double PlotWidth => ViewBoxWidth - MarginLeft - MarginRight;
    double PlotHeight => ViewBoxHeight - MarginTop - MarginBottom;

    List<LineSeries> ChartData;
    int MaxYLabelWidth = 0;
    double MinX;
    double MaxX;
    double MinY;
    double MaxY;
    LineSeries SelectedSerie;
    ChartPoint SelectedPoint;

    protected override void OnParametersSet()
    {
        ChartData = new List<LineSeries>();
        List<double> allYValues = new List<double>();
        int maxValuesCount = Data.Data.Max(d => d.Values.Count());
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
                double y = j < originalCount ? double.TryParse(valueList[j], out double value) ? value : 0 : 0;

                allYValues.Add(y);
                points.Add(new ChartPoint(x, y, valueList[j].ToString()));
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
            yRange = 1;
            MaxY += 0.5;
            MinY -= 0.5;
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
        for(int i = 0; i < labelCount; i++)
        {
            double percent = labelCount == 1 ? 0 : (double)i / (labelCount - 1);
            int x = MarginLeft + (int)(percent * PlotWidth); // <-- Sin MarginBottom            
            string label = (i + 1).ToString();
            if(hasCustomLabels)
                label = customLabels[i];

            var data = Data.Data.First();
            if(data.Values.Count() == labelCount)
            {
                var point = ChartData.FirstOrDefault(p => p.Name == data.Name && p.Color == (string.IsNullOrEmpty(data.Color) ? "black" : data.Color))?.Values.ToList() ?? null;
                if(point is not null)
                {
                    var selection = GetCoordinates(point[i]);
                    x = (int)Math.Ceiling(selection.X) - AxisGap;
                }
            }

            string position = "middle";
            if(i == labelCount - 1)
            {
                position = "end";
            }

            string textSvg = CreateSvgText(label, x + AxisGap, Parameters.Height - MarginBottom + AxisGap, position);
            string gridLine = CreateSvgLine(x + AxisGap, MarginTop - AxisGap, x + AxisGap, Parameters.Height - MarginBottom);
            labels.Add((MarkupString)(gridLine + textSvg));
        }
        return labels;
    }

    IEnumerable<MarkupString> GetYLabels()
    {
        List<MarkupString> labels = new List<MarkupString>();
        List<string> customLabels = Data.YLabels?.ToList();
        int chartHeight = Parameters.Height - MarginTop - MarginBottom;
        if(customLabels != null && customLabels.Count > 0)
        {
            string longestLabel = customLabels.OrderByDescending(label => label.Length).FirstOrDefault();
            int x = MarginLeft - AxisGap;
            int count = customLabels.Count;
            for(int i = 0; i < count; i++)
            {
                string label = customLabels[i];
                double percent = (double)i / (count - 1);
                double valueY = MinY + (MaxY - MinY) * percent;
                double normalizedY = (valueY - MinY) / (MaxY - MinY);
                int y = MarginTop + (int)((1 - normalizedY) * PlotHeight) - AxisGap;
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
            for(int yValue = 0; yValue <= adjustedMaxY; yValue += stepValue)
            {
                double normalizedY = (yValue - MinY) / (MaxY - MinY);
                int y = MarginTop + (int)((1 - normalizedY) * PlotHeight);
                int x = MarginLeft - 5;
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

    ChartPoint GetCoordinates(ChartPoint point)
    {
        (int x, int y) cooredinates = SetCoordinated(point.X, point.Y);
        return new(cooredinates.x, cooredinates.y, point.Value);
    }

    MarkupString GetSelectedPointLabelMarkup()
    {
        StringBuilder builder = new StringBuilder();
        if(SelectedPoint is not null)
        {
            if(Parameters.FormaterLabelPopup is null)
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
                builder.Append(Parameters.FormaterLabelPopup(SelectedPoint.Value));
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

    private (int X, int Y) SetCoordinated(double xValue, double yValue)
    {
        double normalizedX = (xValue - MinX) / (MaxX - MinX);
        double normalizedY = (yValue - MinY) / (MaxY - MinY);
        int x = MarginLeft + (int)(normalizedX * PlotWidth);
        int y = MarginTop + (int)((1 - normalizedY) * PlotHeight);
        return (x, y);
    }

}