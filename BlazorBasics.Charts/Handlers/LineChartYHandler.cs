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
        int x = MarginLeft - AxisGap;
        const int minStepHeightPx = 10; // Espacio mínimo entre etiquetas en píxeles

        if (customLabels != null && customLabels.Any())
        {
            int count = customLabels.Count;
            // Calcular cuántas etiquetas caben según el espacio disponible
            int maxVisibleLabels = Math.Max(1, usableHeight / minStepHeightPx);
            int labelCount = Math.Min(count, maxVisibleLabels);
            double step = count <= 1 ? 0 : (double)(count - 1) / (labelCount - 1);

            for (int i = 0; i < labelCount; i++)
            {
                int index = (int)Math.Round(i * step);
                if (index >= count)
                    index = count - 1;
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

            // Altura útil del gráfico
            usableHeight = Height - MarginTop - MarginBottom;

            // Máximo número de etiquetas que caben en el espacio vertical
            int maxVisibleLabels = Math.Max(3, usableHeight / minStepHeightPx);

            // Queremos al menos 3 etiquetas, como min/mid/max
            int labelCount = Math.Min(maxVisibleLabels, Math.Max(3, StepsY));

            // Asegurar que labelCount sea >= 3
            if (labelCount < 3)
                labelCount = 3;

            List<int> yValues = new List<int>();
            for (int i = 0; i < labelCount; i++)
            {
                double percent = labelCount <= 1 ? 0 : (double)i / (labelCount - 1);
                int yValue = (int)Math.Round(minYFloored + percent * (maxYCeiled - minYFloored));
                if (!yValues.Contains(yValue))
                    yValues.Add(yValue);
            }

            foreach (int yValue in yValues)
            {
                double percent = rangeY <= 0 ? 0 : (yValue - MinY) / rangeY;
                int y = (Height - MarginBottom) - (int)(percent * usableHeight);
                string label = yValue.ToString();
                string textSvg = SvgHelper.CreateSvgText(label, x, y + 4, "end");
                string gridLine = ShowLines ? SvgHelper.CreateSvgLine(MarginLeft, y, Width - MarginRight + AxisGap, y) : string.Empty;
                positions.Add((x, y, textSvg, gridLine));
            }
        }
    }
}