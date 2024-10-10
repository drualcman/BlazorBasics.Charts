namespace BlazorBasics.Charts.Handlers;

internal class PieChartAnglesHandler(int separationOffset)
{
    private readonly int SeparationOffset = separationOffset;

    public void SetLargest(List<PieSegment> segments)
    {
        if (segments is not null && segments.Any())
        {
            var largestSegmentIndex =
                segments.IndexOf(
                    segments.Aggregate((max, current) => max.Pie.Value > current.Pie.Value ? max : current));
            segments[largestSegmentIndex].SetLargest(true);
            var selected = segments.IndexOf(segments.FirstOrDefault(s => s.Pie.IsSelected));
            if (selected >= 0)
            {
                segments[selected].SetLargest(true);
                segments[largestSegmentIndex].SetLargest(false);
            }
        }
    }

    public double CalculateOffsetX(double angle) => Math.Cos(CalculateRadious(angle)) * SeparationOffset;

    public double CalculateOffsetY(double angle) => Math.Sin(CalculateRadious(angle)) * SeparationOffset;

    private double CalculateRadious(double angle) => angle * (Math.PI / 180);

    public double DegreeToRadian(double angle) => Math.PI * angle / 180.0;
}