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
            int maxYCeiled = (int)Math.Ceiling(MaxY);
            int minYFloored = (int)Math.Floor(MinY);

            // Expand slightly to avoid values sticking to edges
            int padding = (int)Math.Ceiling(rangeY * 0.05);
            if (padding == 0)
            {
                padding = 1;
            }
            minYFloored -= padding;
            maxYCeiled += padding;

            int maxVisibleLabels = Math.Max(3, usableHeightLocal / minStepHeightPx);
            int requestedSteps = StepsY > 0 ? StepsY : 5;

            List<int> yValues = new List<int>();

            if (requestedSteps <= 20)
            {
                int segments = requestedSteps;
                for (int i = 0; i <= segments; i++)
                {
                    double ratio = segments == 0 ? 0 : (double)i / segments;
                    double raw = minYFloored + ratio * (maxYCeiled - minYFloored);
                    int yVal = (int)Math.Round(raw);
                    if (!yValues.Contains(yVal))
                    {
                        yValues.Add(yVal);
                    }
                }
            }
            else
            {
                int step = requestedSteps;
                int firstMultiple = (int)Math.Ceiling(minYFloored / (double)step) * step;

                if (minYFloored < firstMultiple)
                {
                    if (!yValues.Contains(minYFloored))
                    {
                        yValues.Add(minYFloored);
                    }
                }

                for (int val = firstMultiple; val <= maxYCeiled; val += step)
                {
                    if (!yValues.Contains(val))
                    {
                        yValues.Add(val);
                    }
                }

                if (!yValues.Contains(maxYCeiled))
                {
                    yValues.Add(maxYCeiled);
                }
            }

            // Ensure min and max
            if (!yValues.Contains(minYFloored))
            {
                yValues.Insert(0, minYFloored);
            }
            if (!yValues.Contains(maxYCeiled))
            {
                yValues.Add(maxYCeiled);
            }
            yValues.Sort();

            // Try to keep odd count (better centering)
            if (yValues.Count % 2 == 0)
            {
                int stepSize = yValues.Count > 1 ? yValues[1] - yValues[0] : 1;
                yValues.Add(maxYCeiled + stepSize);
            }

            // Reduce evenly if too many
            if (yValues.Count > maxVisibleLabels)
            {
                int selectionCount = maxVisibleLabels;
                List<int> reduced = new List<int>();

                for (int k = 0; k < selectionCount; k++)
                {
                    double idxD = (double)k * (yValues.Count - 1) / (double)(selectionCount - 1);
                    int idx = (int)Math.Round(idxD);
                    if (idx < 0)
                    {
                        idx = 0;
                    }
                    if (idx > yValues.Count - 1)
                    {
                        idx = yValues.Count - 1;
                    }
                    int val = yValues[idx];
                    if (!reduced.Contains(val))
                    {
                        reduced.Add(val);
                    }
                }

                if (!reduced.Contains(yValues[0]))
                {
                    reduced.Insert(0, yValues[0]);
                }
                if (!reduced.Contains(yValues[yValues.Count - 1]))
                {
                    reduced.Add(yValues[yValues.Count - 1]);
                }
                reduced.Sort();
                yValues = reduced;
            }

            // Render labels
            for (int i = 0; i < yValues.Count; i++)
            {
                int yValue = yValues[i];
                double percent = rangeY <= 0 ? 0 : (yValue - MinY) / rangeY;
                int y = (Height - MarginBottom) - (int)(percent * usableHeightLocal);
                string label = yValue.ToString();
                string textSvg = SvgHelper.CreateSvgText(label, x, y + 4, "end");
                string gridLine = ShowLines ? SvgHelper.CreateSvgLine(MarginLeft, y, Width - MarginRight + AxisGap, y) : string.Empty;
                positions.Add((x, y, textSvg, gridLine));
            }
        }
    }

}