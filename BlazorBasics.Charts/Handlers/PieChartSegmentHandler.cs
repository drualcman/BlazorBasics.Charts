namespace BlazorBasics.Charts.Handlers;

internal class PieChartSegmentHandler(bool SeparateBiggerByDefault)
{
    public PieSegment HoveredPie { get; private set; }
    public PieSegment SelectedPie { get; private set; }

    public bool HoverIsSelected => HoveredPie is not null;

    public ChartSegment HoveredChart => HoveredPie?.Pie ?? new ChartSegment();

    public void SetLeave()
    {
        if(SelectedPie is null)
            HoveredPie = null;
        else
            HoveredPie = SelectedPie;
    }

    public void SelectPie(PieSegment segment)
    {
        if(SelectedPie is not null && SelectedPie == segment)
            SelectedPie = null;
        else
            SelectedPie = segment;
    }

    public bool IsActive(PieSegment segment)
    {
        bool result;
        if(HoveredPie is not null)
            result = HoveredPie == segment;
        else
            result = false;
        return result;
    }

    public bool IsLargest(PieSegment segment) =>
        SeparateBiggerByDefault && (HoveredPie is null && segment.IsLargest);

    public bool ShouldTransform(PieSegment segment) =>
        IsActive(segment) || IsLargest(segment);

    public void SetHover(PieSegment segment) =>
        HoveredPie = segment;
}