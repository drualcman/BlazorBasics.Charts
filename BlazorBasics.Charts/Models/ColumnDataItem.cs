namespace BlazorBasics.Charts.Models;

public class ColumnDataItem
{
    public string Label { get; set; }
    public decimal PrimaryValue { get; set; }
    public decimal SecondaryValue { get; set; }
    public decimal Value => PrimaryValue + SecondaryValue;

    public ColumnDataItem(string label, decimal primaryValue, decimal secondaryValue)
    {
        Label = label;
        PrimaryValue = primaryValue;
        SecondaryValue = secondaryValue;
    }
}
