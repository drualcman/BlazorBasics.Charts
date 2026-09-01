namespace BlazorBasics.Charts.Models;

/// <summary>
/// Direction the single bar of a stacked chart runs in, which also decides how its labels read.
/// </summary>
public enum StackedBarOrientation
{
    /// <summary>
    /// One column, stacked from the top downwards so the segments follow the order they are given
    /// in. Labels read horizontally, beside the column.
    /// </summary>
    Vertical,

    /// <summary>
    /// One bar, stacked from left to right. Labels read vertically, above or below the bar.
    /// </summary>
    Horizontal
}
