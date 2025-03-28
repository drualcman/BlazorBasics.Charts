namespace BlazorBasics.Charts.Handlers;
internal class LineChartYHandler
{
    private readonly int AxisGap;
    private readonly double MaxY;
    private readonly double MinY;
    private readonly int MarginTop;
    private readonly int MarginRight;
    private readonly int MarginLeft;
    private readonly int MarginBottom;
    private readonly int Width;
    private readonly int Height;
    private readonly int StepsY;
    private readonly IEnumerable<string> YLabels;

    public LineChartYHandler(int axisGap, double maxY, double minY, int marginTop, int marginRight, int marginLeft, int marginBottom, int width, int height, int stepsY, IEnumerable<string> yLabels)
    {
        AxisGap = axisGap;
        MaxY = maxY;
        MinY = minY;
        MarginTop = marginTop;
        MarginRight = marginRight;
        MarginLeft = marginLeft;
        MarginBottom = marginBottom;
        Width = width;
        Height = height;
        StepsY = stepsY;
        YLabels = yLabels;
    }

    internal IEnumerable<MarkupString> GetYLabels()
    {
        List<MarkupString> labels = new List<MarkupString>();
        List<string> customLabels = YLabels?.ToList();
        int chartHeight = Height - MarginTop - MarginBottom;
        int plotTop = MarginTop;
        int plotBottom = Height - MarginBottom;
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
                string textSvg = SvgHelper.CreateSvgText(label, x, y, "end");
                string gridLine = SvgHelper.CreateSvgLine(MarginLeft, y, Width - MarginRight + AxisGap, y);
                labels.Add((MarkupString)(gridLine + textSvg));
            }
        }
        else
        {
            int minStepHeightPx = 10;
            int maxVisibleSteps = chartHeight / minStepHeightPx;
            int stepValue = Math.Max(1, StepsY);
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
                string textSvg = SvgHelper.CreateSvgText(label, x, y + 4, "end");
                string gridLine = SvgHelper.CreateSvgLine(MarginLeft, y, Width - MarginRight + AxisGap, y);
                labels.Add((MarkupString)(gridLine + textSvg));
            }
        }
        return labels;
    }
}
