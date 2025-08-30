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
    private bool ShowLines;

    public LineChartYHandler(int axisGap, double maxY, double minY,
        int marginTop, int marginRight, int marginLeft, int marginBottom,
        int width, int height, int stepsY, IEnumerable<string> yLabels, bool showLines)
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
        ShowLines = showLines;
    }

    internal IEnumerable<MarkupString> GetYLabels()
    {
        List<(int x, int y, string label, string gridLine)> positions = new();
        CalculateLabelPositions(positions);
        return positions.Select(p => (MarkupString)(p.gridLine + p.label));
    }

    private void CalculateLabelPositions(List<(int x, int y, string label, string gridLine)> positions)
    {
        positions.Clear();
        List<string> customLabels = YLabels?.ToList();
        int chartHeight = Height - MarginTop - MarginBottom;
        int plotTop = MarginTop;
        int plotBottom = Height - MarginBottom;
        int usableHeight = plotBottom - plotTop;
        int usableHeightLocal = plotBottom - plotTop;
        int x = MarginLeft - AxisGap;
        const int minStepHeightPx = 10; // Minimum spacing between labels in pixels

        if (customLabels != null && customLabels.Any())
        {
            int count = customLabels.Count;
            int maxVisibleLabels = Math.Max(1, usableHeight / minStepHeightPx);
            int labelCount = Math.Min(count, maxVisibleLabels);
            double step = count <= 1 ? 0 : (double)(count - 1) / (labelCount - 1);

            for (int i = 0; i < labelCount; i++)
            {
                int index = (int)Math.Round(i * step);
                if (index >= count)
                {
                    index = count - 1;
                }
                string label = customLabels[index];
                double percent = count <= 1 ? 0 : (double)index / (count - 1);
                int y = plotBottom - (int)(percent * usableHeight);
                string textSvg = SvgHelper.CreateSvgText(label, x, y + 4, "end");
                string gridLine = ShowLines ? SvgHelper.CreateSvgLine(MarginLeft, y, Width - MarginRight + AxisGap, y) : string.Empty;
                positions.Add((x, y, textSvg, gridLine));
            }
        }
        else
        {
            double rangeY = MaxY - MinY;
            int requestedSteps = StepsY > 0 ? StepsY : 5;
            int maxVisibleLabels = Math.Max(3, usableHeightLocal / minStepHeightPx);

            List<int> yValues = new List<int>();

            if (requestedSteps <= 20)
            {
                // Treat StepsY as TickAmount (number of labels to show)
                int tickAmount = requestedSteps;

                if (tickAmount < 2)
                {
                    tickAmount = 2;
                }

                // Expand range so it divides evenly
                double rawRange = MaxY - MinY;
                double step = Math.Ceiling(rawRange / (tickAmount - 1));

                double adjustedMin = Math.Floor(MinY / step) * step;
                double adjustedMax = adjustedMin + step * (tickAmount - 1);

                for (int i = 0; i < tickAmount; i++)
                {
                    int val = (int)Math.Round(adjustedMin + i * step);
                    yValues.Add(val);
                }
            }
            else
            {
                // Treat StepsY as step size
                int step = requestedSteps;
                int minYFloored = (int)Math.Floor(MinY);
                int maxYCeiled = (int)Math.Ceiling(MaxY);

                int firstMultiple = (int)Math.Ceiling(minYFloored / (double)step) * step;
                for (int val = firstMultiple; val <= maxYCeiled; val += step)
                {
                    yValues.Add(val);
                }

                if (!yValues.Contains(minYFloored))
                {
                    yValues.Insert(0, minYFloored);
                }
                if (!yValues.Contains(maxYCeiled))
                {
                    yValues.Add(maxYCeiled);
                }
            }

            // Si hay más etiquetas que las que caben, reducimos manteniendo equidistancia
            if (yValues.Count > maxVisibleLabels)
            {
                List<int> reduced = new List<int>();
                int steps = maxVisibleLabels - 1;
                double minVal = yValues.First();
                double maxVal = yValues.Last();
                double step = (maxVal - minVal) / steps;

                for (int i = 0; i <= steps; i++)
                {
                    int val = (int)Math.Round(minVal + i * step);
                    reduced.Add(val);
                }

                yValues = reduced;
            }

            // Render labels
            double finalRange = yValues.Last() - yValues.First();
            for (int i = 0; i < yValues.Count; i++)
            {
                int yValue = yValues[i];
                double percent = finalRange <= 0 ? 0 : (yValue - yValues.First()) / finalRange;
                int y = (Height - MarginBottom) - (int)(percent * usableHeightLocal);

                string label = yValue.ToString();
                string textSvg = SvgHelper.CreateSvgText(label, x, y + 4, "end");
                string gridLine = ShowLines ? SvgHelper.CreateSvgLine(MarginLeft, y, Width - MarginRight + AxisGap, y) : string.Empty;
                positions.Add((x, y, textSvg, gridLine));
            }
        }
    }

}