namespace BlazorBasics.Charts.Handlers;
internal class LineChartYHandler
{
    public double MaxY { get; private set; }
    public double MinY { get; private set; }

    private readonly int AxisGap;
    private readonly int MarginTop;
    private readonly int MarginRight;
    private readonly int MarginLeft;
    private readonly int MarginBottom;
    private readonly int Width;
    private readonly int Height;
    private readonly int StepsY;
    private readonly IEnumerable<string> YLabels;
    private bool ShowLines;
    private readonly CultureInfo ParsingCulture;

    public LineChartYHandler(int axisGap, double maxY, double minY,
        int marginTop, int marginRight, int marginLeft, int marginBottom,
        int width, int height, int stepsY, IEnumerable<string> yLabels, bool showLines,
        CultureInfo parsingCulture)
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
        ParsingCulture = parsingCulture;
    }

    internal IEnumerable<MarkupString> GetYLabels()
    {
        List<(int x, int y, string label, string gridLine)> positions = new List<(int x, int y, string label, string gridLine)>();
        CalculateLabelPositions(positions);
        return positions.Select(p => (MarkupString)(p.gridLine + p.label));
    }

    private void CalculateLabelPositions(List<(int x, int y, string label, string gridLine)> positions)
    {
        positions.Clear();
        List<string> customLabels = YLabels != null ? YLabels.ToList() : null;
        int chartHeight = Height - MarginTop - MarginBottom;
        int plotTop = MarginTop;
        int plotBottom = Height - MarginBottom;
        int usableHeight = plotBottom - plotTop;
        int usableHeightLocal = usableHeight;
        int x = MarginLeft - AxisGap;
        const int minStepHeightPx = 10; // Minimum spacing between labels in pixels

        if (customLabels is not null && customLabels.Any())
        {
            // Use provided custom labels and map them evenly
            int count = customLabels.Count;
            int maxVisibleLabels = Math.Max(1, usableHeight / minStepHeightPx);
            int labelCount = Math.Min(count, maxVisibleLabels);
            double stepIndex = count <= 1 ? 0.0 : (double)(count - 1) / (labelCount - 1);

            List<double> yValues = new List<double>();

            for (int i = 0; i < labelCount; i++)
            {
                int index = (int)Math.Round(i * stepIndex);
                if (index >= count)
                {
                    index = count - 1;
                }

                string label = customLabels[index];
                double percent = count <= 1 ? 0.0 : (double)index / (count - 1);
                int y = plotBottom - (int)Math.Round(percent * usableHeight);
                string textSvg = SvgHelper.CreateSvgText(label, x, y + 4, "end");
                string gridLine = ShowLines ? SvgHelper.CreateSvgLine(MarginLeft, y, Width - MarginRight + AxisGap, y) : string.Empty;
                positions.Add((x, y, textSvg, gridLine));

                double yValue = double.TryParse(customLabels[index], NumberStyles.Any, ParsingCulture, out double value) ? value : i;
                yValues.Add(yValue);
            }

            if (yValues.Count == 0)
            {
                yValues.Add(0);
                yValues.Add(1);
            }

            // Expose a consistent numeric mapping for custom labels:
            MinY = yValues.Min();
            MaxY = yValues.Max();
        }
        else
        {
            int requestedSteps = StepsY > 0 ? StepsY : 5;
            int maxVisibleLabels = Math.Max(3, usableHeightLocal / minStepHeightPx);

            List<double> yValues = new List<double>();

            if (requestedSteps <= 20)
            {
                // Margen arriba/abajo (5%)
                double minAdjusted = MinY - 0.01;
                double maxAdjusted = MaxY + 0.01;

                // Evitamos negativos innecesarios si todos los datos son >= 0
                if (minAdjusted < 0 && MinY >= 0)
                {
                    minAdjusted = 0;
                }

                // Recalculamos rango
                double rawRange = maxAdjusted - minAdjusted;

                // Generamos ticks uniformes
                double step = rawRange / (requestedSteps - 1);

                for (int i = 0; i < requestedSteps; i++)
                {
                    double val = minAdjusted + i * step;
                    yValues.Add(val);
                }

                MinY = yValues.First();
                MaxY = yValues.Last();
            }
            else
            {
                // Treat StepsY as step size (integer)
                int step = StepsY;
                int minYFloored = (int)Math.Floor(MinY);
                int maxYCeiled = (int)Math.Ceiling(MaxY);

                int firstMultiple = (int)Math.Ceiling(minYFloored / (double)step) * step;
                for (int val = firstMultiple; val <= maxYCeiled; val += step)
                {
                    yValues.Add((double)val);
                }

                if (!yValues.Contains((double)minYFloored))
                {
                    yValues.Insert(0, (double)minYFloored);
                }
                if (!yValues.Contains((double)maxYCeiled))
                {
                    yValues.Add((double)maxYCeiled);
                }

                MinY = Math.Min(MinY, yValues.First());
                MaxY = Math.Max(MaxY, yValues.Last());
            }

            // If too many labels for vertical space, reduce evenly
            if (yValues.Count > maxVisibleLabels)
            {
                List<double> reduced = new List<double>();
                int steps = maxVisibleLabels - 1;
                double minVal = yValues.First();
                double maxVal = yValues.Last();
                double step = (maxVal - minVal) / steps;

                for (int i = 0; i <= steps; i++)
                {
                    double val = minVal + i * step;
                    reduced.Add(val);
                }

                yValues = reduced;
                MinY = Math.Min(MinY, yValues.First());
                MaxY = Math.Max(MaxY, yValues.Last());
            }

            // Render labels — use InvariantCulture and round pixel placement
            double finalRange = yValues.Last() - yValues.First();
            double firstValue = yValues.First();

            for (int i = 0; i < yValues.Count; i++)
            {
                double yValue = yValues[i];
                double percent = finalRange <= 0.0 ? 0.0 : (yValue - firstValue) / finalRange;
                int y = (Height - MarginBottom) - (int)Math.Round(percent * usableHeightLocal);

                // Format label with up to two decimals, culture-invariant
                string label = yValue.ToString("0.##", ParsingCulture);
                string textSvg = SvgHelper.CreateSvgText(label, x, y + 4, "end");
                string gridLine = ShowLines ? SvgHelper.CreateSvgLine(MarginLeft, y, Width - MarginRight + AxisGap, y) : string.Empty;
                positions.Add((x, y, textSvg, gridLine));
            }
        }
    }
}
