namespace BlazorBasics.Charts.Models;

/// <summary>
/// The point of a line of a column with line chart that was clicked, saying which of the lines it
/// was, which the item alone cannot tell.
/// </summary>
public class ColumnWithLinePoint
{
    public ColumnWithLinePoint(ColumnDataItem item, int index, ColumnWithLineSeries series,
        string percentage)
    {
        Item = item;
        Index = index;
        Series = series;
        Percentage = percentage;
    }

    public ColumnDataItem Item { get; }

    /// <summary>Position of the item in the data, counting from zero.</summary>
    public int Index { get; }

    public ColumnWithLineSeries Series { get; }

    /// <summary>The percentage the chart writes over that point.</summary>
    public string Percentage { get; }
}
