namespace BlazorBasics.Charts.Models;

public class ChartSegment
{
    public string Name { get; set; }
    public double Value { get; set; }
    public bool IsSelected { get; set; }
    public string ChartColor { get; set; }
    public string LabelColor { get; set; }
    public Func<ChartSegment, string> SetTitleTopic { get; set; }
    internal string ShowTitle()
    {
        return SetTitleTopic is not null ? SetTitleTopic(this) : $"{this.Name}: {this.Value}";
    }
}