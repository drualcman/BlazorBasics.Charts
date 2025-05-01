namespace BlazorBasics.Charts.Models;

public class ChartDoubleSegment : ChartSegment
{
    public double PrimaryPercentage { get; set; }
    public double SecondaryPercentage { get; set; }
    public string PrimaryChartColor { get; set; }
    public string SecondaryChartColor { get; set; }

    public double GetPercentage(double maxTotal)
    {
        if(maxTotal == 0)
            return 0;
        return Math.Round(Value / maxTotal) * 100;
    }
}