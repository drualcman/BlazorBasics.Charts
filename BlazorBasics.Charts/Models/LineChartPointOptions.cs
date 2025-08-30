namespace BlazorBasics.Charts.Models;

public class LineChartPointOptions(
    bool visibleAllPoints = false,
    bool visibleMaxPoint = true,
    bool visibleMinPoint = true,
    bool visibleMaxPointLine = false,
    bool visibleMinPointLine = false
)
{
    public bool VisibleAllPoints => visibleAllPoints;
    public bool VisibleMaxPoint => visibleMaxPoint;
    public bool VisibleMinPoint => visibleMinPoint;
    public bool VisibleMaxPointLine => visibleMaxPointLine;
    public bool VisibleMinPointLine => visibleMinPointLine;
}
