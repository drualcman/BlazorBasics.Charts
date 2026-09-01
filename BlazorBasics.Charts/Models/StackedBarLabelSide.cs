namespace BlazorBasics.Charts.Models;

/// <summary>
/// Which side of the bar the labels are written on.
/// </summary>
public enum StackedBarLabelSide
{
    /// <summary>Left of a vertical bar, above a horizontal one.</summary>
    Before,

    /// <summary>Right of a vertical bar, below a horizontal one.</summary>
    After,

    /// <summary>
    /// Over its own segment, written in the contrasting colour of the segment. Only worth using
    /// when the segments are big enough to hold their name.
    /// </summary>
    Inside
}
