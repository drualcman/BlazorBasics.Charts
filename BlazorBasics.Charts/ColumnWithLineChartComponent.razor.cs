namespace BlazorBasics.Charts;

public partial class ColumnWithLineChartComponent
{
    [Parameter] public List<ChartDataItem> Data { get; set; } = new();

    // Constantes para el layout
    const int margin = 15;
    int ChartHeight => (int)(barWidth * 6);
    const int barWidth = 75;
    const int spacing = 15;

    // Calcular valores
    int MaxColumnTotal => Data.Max(x => x.Value);
    int GrandTotal => Data.Sum(x => x.Value);

    List<LinePoints> GrandTotalPoints = new List<LinePoints>();
    List<LinePoints> PrimaryPercentagePoints = new List<LinePoints>();
    List<LinePoints> SecondaryPercentagePoints = new List<LinePoints>();
    List<ColumnBar> ColumnsPrimary = new List<ColumnBar>();
    List<ColumnBar> ColumnsSecondary = new List<ColumnBar>();
    List<ColumnBar> PrimaryPoints = new List<ColumnBar>();
    List<ColumnBar> SecondaryPoints = new List<ColumnBar>();
    List<ChartLabels> BottomLabels = new List<ChartLabels>();
    List<ChartLabels> BigTotalPercentageLabels = new List<ChartLabels>();
    List<ChartLabels> PrimaryPercentageLabels = new List<ChartLabels>();
    List<ChartLabels> SecondaryPercentageLabels = new List<ChartLabels>();
    //int ChartHeight => Math.Max((int)(ColumnsPrimary.Max(c => c.Y) + ColumnsSecondary.Max(c => c.Y)), chartHeight);
    int ChartWidth => (Data.Count * (barWidth + spacing) + margin);

    private string ViewBox => $"0 0 {CalculatedWidth} {CalculatedHeight}";
    private int CalculatedWidth => (Data.Count * (barWidth + spacing)) + margin;
    private int CalculatedHeight => ChartHeight + (margin * 3);

    protected override void OnParametersSet()
    {
        for(var i = 0; i < Data.Count; i++)
        {
            var item = Data[i];
            var columnTotal = item.Value;
            var primaryPercentage = columnTotal > 0 ? (item.PrimaryValue * 100.0 / columnTotal) : 0;
            var secondaryPercentage = columnTotal > 0 ? (item.SecondaryValue * 100.0 / columnTotal) : 0;
            var columnHeight = columnTotal * ChartHeight / MaxColumnTotal;
            var grandTotalPercentage = columnTotal * 100.0 / GrandTotal;

            // Posiciones
            var x = margin + (i * (barWidth + spacing));
            var yBase = margin + ChartHeight;

            // Barra primaria
            var primaryHeight = (int)(columnHeight * primaryPercentage / 100);
            ColumnsPrimary.Add(new(x, yBase - columnHeight, barWidth, primaryHeight, "#4e79a7"));

            // Barra de secundaria
            ColumnsSecondary.Add(new(x, yBase - columnHeight + primaryHeight, barWidth, columnHeight - primaryHeight, "#f28e2b"));

            // Punto del porcentaje del total general
            var pointX = x + (barWidth / 2);
            var grandTotalPointY = yBase - (ChartHeight * grandTotalPercentage / 100);
            GrandTotalPoints.Add(new(pointX, grandTotalPointY));

            // Punto del primario
            var primaryPercentagePointY = yBase - columnHeight + (columnHeight * (100 - primaryPercentage) / 100);
            PrimaryPercentagePoints.Add(new(pointX, primaryPercentagePointY));

            // Punto del secundario
            var secondaryPercentagePointY = yBase - columnHeight + (columnHeight * (100 - secondaryPercentage) / 100);
            SecondaryPercentagePoints.Add(new(pointX, secondaryPercentagePointY));


            PrimaryPoints.Add(new(pointX, grandTotalPointY, 0, 0, "#e15759"));
            SecondaryPoints.Add(new(pointX, primaryPercentagePointY, 0, 0, "#59a84b"));

            BigTotalPercentageLabels.Add(new($"{(int)grandTotalPercentage}%", pointX, (int)(grandTotalPointY)));
            PrimaryPercentageLabels.Add(new($"{(int)primaryPercentage}%", pointX, (int)(primaryPercentagePointY)));
            SecondaryPercentageLabels.Add(new($"{(int)secondaryPercentage}%", pointX, (int)(secondaryPercentagePointY)));
            BottomLabels.Add(new(item.Label, (int)(x + (barWidth / 2)), (int)(ChartHeight + margin + 20)));
        }
    }
}

public class LinePoints
{
    public double X { get; set; }
    public double Y { get; set; }

    public LinePoints(double x, double y)
    {
        X = x;
        Y = y;
    }
}
public class ColumnBar
{
    public double X { get; set; }
    public double Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string Color { get; set; }

    public ColumnBar(double x, double y, int width, int height, string color)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Color = color;
    }
}

public class ChartDataItem
{
    public string Label { get; set; } = string.Empty;
    public int PrimaryValue { get; set; }
    public int SecondaryValue { get; set; }
    public int Value => PrimaryValue + SecondaryValue;
}

public class ChartLabels
{
    public string Label { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public ChartLabels(string label, int x, int y)
    {
        Label = label;
        X = x;
        Y = y;
    }
}