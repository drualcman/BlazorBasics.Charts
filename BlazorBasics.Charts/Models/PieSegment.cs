namespace BlazorBasics.Charts.Models;

internal class PieSegment
{
    public PieSegment(int index, ChartSegment pie, double fillPercentage, double transparentPercentage)
    {
        Index = index;
        Pie = pie;
        FillPercentage = fillPercentage;
        TransparentPercentage = transparentPercentage;
    }

    public int Index { get; set; }
    public ChartSegment Pie { get; init; }
    public double FillPercentage { get; init; }
    public double TransparentPercentage { get; init; }
    public bool IsLargest { get; private set; }

    public void SetLargest(bool value)
    {
        IsLargest = value;
    }
}