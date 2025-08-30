namespace BlazorBasics.Charts.Models;

public class LineChartPointOptions(
    bool visibleAllPoints = false,
    bool visibleMaxPoint = true,
    bool visibleMinPoint = true
)
{
    public bool VisibleAllPoints => visibleAllPoints;
    public bool VisibleMaxPoint => visibleMaxPoint;
    public bool VisibleMinPoint => visibleMinPoint;
}
