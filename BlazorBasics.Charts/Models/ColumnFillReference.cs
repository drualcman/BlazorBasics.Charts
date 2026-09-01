namespace BlazorBasics.Charts.Models;

/// <summary>
/// What a full column means, which is what decides how much of each column gets filled.
/// </summary>
public enum ColumnFillReference
{
    /// <summary>
    /// A full column is the highest value of the chart, so the biggest column is always full and
    /// the rest are read against it. Good to compare values with each other.
    /// </summary>
    HighestValue,

    /// <summary>
    /// A full column is the sum of every value of the chart, so a value worth a fifth of the total
    /// fills a fifth of its column. Good to read each value as its share of the whole.
    /// </summary>
    TotalOfAllValues
}
