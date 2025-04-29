namespace BlazorBasics.Charts.Models;

public class ChartDoubleSegment
{
    public string Name { get; set; }
    public double PrimaryValue { get; set; }
    public double SecondaryValue { get; set; }
    public bool IsSelected { get; set; }
    public string PrimaryChartColor { get; set; }
    public string SecondaryChartColor { get; set; }
    public string LabelColor { get; set; }
    public Func<ChartDoubleSegment, string> SetTitleTopic { get; set; }
    internal string ShowTitle()
    {
        return SetTitleTopic is not null ? SetTitleTopic(this) : $"{this.Name}: {this.PrimaryValue}/{this.SecondaryValue}";
    }

    public double GetTotal()
    {
        return PrimaryValue + SecondaryValue;
    }

    public double GetPercentage(double maxTotal)
    {
        if(maxTotal == 0)
            return 0;
        return Math.Round((GetTotal() / maxTotal) * 100);
    }
}