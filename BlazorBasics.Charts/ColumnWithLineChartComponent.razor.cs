namespace BlazorBasics.Charts;

public partial class ColumnWithLineChartComponent
{
    [Parameter] public ColumnWithLineChartData Data { get; set; }
    [Parameter] public ColumnWithLineChartParams Parameters { get; set; } = new();

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; }

    [Parameter] public EventCallback<ColumnDataItem> OnItemClick { get; set; }

    const int Margin = 15;
    private string Style;
    private string WrapperCss = "";
    private ColumnDataItem SelectedItem;
    private ColumnBar SelectedPoint;
    bool IsShowingPrimaryValues;
    bool IsShowingSecondValues;

    private int ChartHeight
    {
        get
        {
            var contentHeight = Parameters.Height / 2;
            return Math.Max(Parameters.MinBarHeight, contentHeight);
        }
    }
    private int CalculatedWidth => (Data.Data.Count() * (Parameters.BarWidth + Parameters.Spacing)) + Margin;
    private int CalculatedHeight => ChartHeight + (Margin * 3);
    private string ViewBox => $"0 0 {CalculatedWidth} {CalculatedHeight}";

    // Listas internas para el renderizado
    private List<LinePoints> GrandTotalPoints = new();
    private List<LinePoints> PrimaryPercentagePoints = new();
    private List<LinePoints> SecondaryPercentagePoints = new();
    private List<ColumnBar> ColumnsPrimary = new();
    private List<ColumnBar> ColumnsSecondary = new();
    private List<ColumnBar> BigTotalPoints = new();
    private List<ColumnBar> PrimaryPoints = new();
    private List<ColumnBar> SecondaryPoints = new();
    private List<ChartLabels> BigTotalPercentageLabels = new();
    private List<ChartLabels> BottomLabels = new();
    private List<ChartLabels> PrimaryPercentageLabels = new();
    private List<ChartLabels> SecondaryPercentageLabels = new();

    protected override void OnParametersSet()
    {
        InitializeStyles();
        CalculateChartData();
    }

    private void InitializeStyles()
    {
        if(Attributes?.TryGetValue("class", out var css) == true)
            WrapperCss = css.ToString();

        Style = $"--char-height: {Parameters.Height}px; " +
                $"--primary-color: {Parameters.PrimaryColor}; " +
                $"--secondary-color: {Parameters.SecondaryColor}; " +
                $"--grand-total-color: {Parameters.GrandTotalLineColor}; " +
                $"--primary-percentage-color: {Parameters.PrimaryPercentageLineColor};" +
                $"--secondary-percentage-color: {Parameters.SecondaryPercentageLineColor};";

        if(Attributes?.TryGetValue("style", out var style) == true)
        {
            Style += style.ToString();
            Attributes.Remove("style");
        }
    }

    private void CalculateChartData()
    {
        ClearCollections();

        var maxColumnTotal = Data.Data.Max(x => x.Value);
        var grandTotal = Data.Data.Sum(x => x.Value);

        for(var i = 0; i < Data.Data.Count(); i++)
        {
            var item = Data.Data.ElementAt(i);
            CalculateColumn(item, i, maxColumnTotal, grandTotal);
        }
    }

    private void CalculateColumn(ColumnDataItem item, int index, decimal maxColumnTotal, decimal grandTotal)
    {
        var primaryPercentage = item.Value > 0 ? (item.PrimaryValue * 100.0m / item.Value) : 0;
        var secondaryPercentage = item.Value > 0 ? (item.SecondaryValue * 100.0M / item.Value) : 0;
        var columnHeight = (double)(item.Value * ChartHeight / maxColumnTotal);
        var grandTotalPercentage = (double)(item.Value * 100.0m / grandTotal);

        var x = Margin + (index * (Parameters.BarWidth + Parameters.Spacing));
        var yBase = Margin + ChartHeight;

        // Barras
        var primaryHeight = (double)((decimal)columnHeight * primaryPercentage / 100.0m);
        ColumnsPrimary.Add(new(x, yBase - columnHeight, Parameters.BarWidth, (int)primaryHeight, Parameters.PrimaryColor));
        ColumnsSecondary.Add(new(x, yBase - columnHeight + primaryHeight, Parameters.BarWidth, (int)(columnHeight - primaryHeight), Parameters.SecondaryColor));

        // Puntos
        var pointX = x + (Parameters.BarWidth / 2);
        var grandTotalPointY = yBase - (ChartHeight * grandTotalPercentage / 100);
        GrandTotalPoints.Add(new(pointX, grandTotalPointY));

        var primaryPercentagePointY = yBase - columnHeight + (columnHeight * (100 - (double)primaryPercentage) / 100);
        PrimaryPercentagePoints.Add(new(pointX, primaryPercentagePointY));

        var secondaryPercentagePointY = yBase - columnHeight + (columnHeight * (100 - (double)secondaryPercentage) / 100);
        SecondaryPercentagePoints.Add(new(pointX, secondaryPercentagePointY));

        // Etiquetas
        BigTotalPoints.Add(new(pointX, grandTotalPointY, 0, 0, Parameters.GrandTotalLineColor));
        PrimaryPoints.Add(new(pointX, primaryPercentagePointY, 0, 0, Parameters.PrimaryPercentageLineColor));
        SecondaryPoints.Add(new(pointX, secondaryPercentagePointY, 0, 0, Parameters.SecondaryPercentageLineColor));

        BigTotalPercentageLabels.Add(new($"{(int)grandTotalPercentage}%", pointX, (int)(grandTotalPointY) - 10));
        PrimaryPercentageLabels.Add(new($"{(int)primaryPercentage}%", pointX, (int)(primaryPercentagePointY) - 10));
        SecondaryPercentageLabels.Add(new($"{(int)secondaryPercentage}%", pointX, (int)(secondaryPercentagePointY) - 10));
        BottomLabels.Add(new(item.Label, (int)(x + (Parameters.BarWidth / 2)), (int)(ChartHeight + Margin + 20)));
    }

    private void ClearCollections()
    {
        GrandTotalPoints.Clear();
        PrimaryPercentagePoints.Clear();
        SecondaryPercentagePoints.Clear();
        ColumnsPrimary.Clear();
        ColumnsSecondary.Clear();
        PrimaryPoints.Clear();
        BottomLabels.Clear();
        PrimaryPercentageLabels.Clear();
        SecondaryPercentageLabels.Clear();
        BigTotalPercentageLabels.Clear();
    }

    private async Task OnColumnClick(ColumnDataItem item)
    {
        HideLabels();
        SelectedPoint = null;
        SelectedItem = item;
        await OnItemClick.InvokeAsync(item);
    }

    private async Task OnPointClick(ColumnDataItem item, ColumnBar point)
    {
        HideLabels();
        SelectedPoint = point;
        SelectedItem = item;
        await OnItemClick.InvokeAsync(SelectedItem);
    }

    void ShowPrimaryValues()
    {
        HideLabels();
        IsShowingPrimaryValues = true;
        IsShowingSecondValues = false;
    }
    void ShowSecondValues()
    {
        HideLabels();
        IsShowingPrimaryValues = false;
        IsShowingSecondValues = true;
    }

    void HideLabels()
    {
        SelectedPoint = null;
        SelectedItem = null;
        IsShowingPrimaryValues = false;
        IsShowingSecondValues = false;
    }
}

// Clases internas
internal class LinePoints
{
    public double X { get; }
    public double Y { get; }

    public LinePoints(double x, double y)
    {
        X = x;
        Y = y;
    }
}

internal class ColumnBar
{
    public double X { get; }
    public double Y { get; }
    public int Width { get; }
    public int Height { get; }
    public string Color { get; }

    public ColumnBar(double x, double y, int width, int height, string color)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Color = color;
    }
}

internal class ChartLabels
{
    public string Label { get; }
    public int X { get; }
    public int Y { get; }

    public ChartLabels(string label, int x, int y)
    {
        Label = label;
        X = x;
        Y = y;
    }
}