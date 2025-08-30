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

        if(customLabels != null && customLabels.Any())
        {
            int count = customLabels.Count;
            // Calcular cuántas etiquetas caben según el espacio disponible
            int maxVisibleLabels = Math.Max(1, usableHeight / minStepHeightPx);
            int labelCount = Math.Min(count, maxVisibleLabels);
            double step = count <= 1 ? 0 : (double)(count - 1) / (labelCount - 1);

            for(int i = 0; i < labelCount; i++)
            {
                int index = (int)Math.Round(i * step);
                if(index >= count)
                    index = count - 1;
                string label = customLabels[index];
                double percent = count <= 1 ? 0 : (double)index / (count - 1);
                int y = plotBottom - (int)(percent * usableHeight);
                string textSvg = SvgHelper.CreateSvgText(label, x, y + 4, "end");
                string gridLine = SvgHelper.CreateSvgLine(MarginLeft, y, Width - MarginRight + AxisGap, y);
                positions.Add((x, y, textSvg, gridLine));
            }
        }
        else
        {
            double rangeY = MaxY - MinY;
            int maxYCeiled = (int)Math.Ceiling(MaxY);
            int minYFloored = (int)Math.Floor(MinY);
            int maxVisibleLabels = Math.Max(1, usableHeight / minStepHeightPx);
            int stepValue = Math.Max(1, StepsY);

            // Calcular el stepValue para maximizar el número de etiquetas sin exceder maxVisibleLabels
            if(rangeY > 0)
            {
                stepValue = Math.Max(1, (int)Math.Ceiling(rangeY / (maxVisibleLabels - 1)));
            }
            else
            {
                stepValue = 1; // Evitar división por cero
            }

            int requiredSteps = (int)((maxYCeiled - minYFloored) / stepValue) + 1;

            // Ajustar stepValue si hay menos de 2 etiquetas para asegurar al menos 2
            if(requiredSteps < 2)
            {
                stepValue = (int)Math.Ceiling(rangeY);
                requiredSteps = 2;
            }

            for(int i = 0; i < requiredSteps; i++)
            {
                int yValue = minYFloored + i * stepValue;
                if(yValue > maxYCeiled)
                    break;
                double percent = rangeY <= 0 ? 0 : (yValue - MinY) / rangeY;
                int y = plotBottom - (int)(percent * usableHeight);
                string label = yValue.ToString();
                string textSvg = SvgHelper.CreateSvgText(label, x, y + 4, "end");
                string gridLine = SvgHelper.CreateSvgLine(MarginLeft, y, Width - MarginRight + AxisGap, y);
                positions.Add((x, y, textSvg, gridLine));
            }
        }
    }
}