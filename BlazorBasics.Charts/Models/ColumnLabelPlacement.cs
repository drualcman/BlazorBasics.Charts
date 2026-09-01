namespace BlazorBasics.Charts.Models;

/// <summary>
/// Where the label of every column is drawn. Bottom and Top keep the label outside the plotting
/// area and let the chart grow to fit it, Left and Right run the label alongside its own column
/// and therefore fit as much text as the column is tall.
/// </summary>
public enum ColumnLabelPlacement
{
    Bottom,
    Top,
    Left,
    Right
}
