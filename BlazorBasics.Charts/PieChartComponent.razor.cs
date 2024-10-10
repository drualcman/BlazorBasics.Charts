namespace BlazorBasics.Charts;

public partial class PieChartComponent : IDisposable
{
    private PieChartAnimationHandler AnimationHandler;

    private string LabelStyle = "display: none;";
    private PieChartRenderHandler RenderHandler;
    private PieChartSegmentHandler SegmentHandler = new();
    private string Style;

    private string WrapperCss = "";
    [Parameter] public Func<Task<IReadOnlyList<ChartSegment>>> DataSource { get; set; }
    [Parameter] public PieChartParams Parameters { get; set; } = new();
    [Parameter] public RenderFragment<ChartSegment> ChildContent { get; set; }
    [Parameter] public Func<ChartSegment, string> OnHover { get; set; }
    [Parameter] public EventCallback<ChartSegment> OnClick { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; }

    public void Dispose()
    {
        AnimationHandler?.Dispose();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Attributes is not null && Attributes.TryGetValue("class", out var css)) WrapperCss = css.ToString();
        Style = $"--pie-width: {Parameters.Width.ToString(CultureInfo.InvariantCulture)}px; " +
                $"--pie-height: {Parameters.Height.ToString(CultureInfo.InvariantCulture)}px; ";
        if (Attributes is not null && Attributes.TryGetValue("style", out var style))
        {
            Style += style.ToString();
            Attributes.Remove("style");
        }

        if (RenderHandler is null)
        {
            RenderHandler = new PieChartRenderHandler(Parameters);
            var data = await DataSource();
            RenderHandler.SetPie(data);
            AnimationHandler = new PieChartAnimationHandler(Parameters.DelayTime, RenderHandler.Segments.Count);
            AnimationHandler.OnAnimation += () => InvokeAsync(StateHasChanged);
            AnimationHandler.StartAnimationSequence();
        }
    }

    public void UpdateData(IReadOnlyList<ChartSegment> data)
    {
        RenderHandler.SetPie(data);
        StateHasChanged();
    }

    private void Leave()
    {
        LabelStyle = "display:none;";
        SegmentHandler.SetLeave();
    }

    private async Task PieClick(PieSegment segment)
    {
        SegmentHandler.SelectPie(segment);
        Leave();
        if (OnClick.HasDelegate)
            await OnClick.InvokeAsync(segment.Pie);
    }

    private void ShowLabel(PieSegment segment, MouseEventArgs e)
    {
        SegmentHandler.SetHover(segment);
        LabelStyle = "display:block;";
    }

    private MarkupString ShowLabelData()
    {
        var result = "";
        if (SegmentHandler.HoverIsSelected)
        {
            if (OnHover is not null)
                result = OnHover.Invoke(SegmentHandler.HoveredChart);
            else
                result =
                    $"{SegmentHandler.HoveredChart.Name} {SegmentHandler.HoveredChart.Value.ToString("F2", CultureInfo.InvariantCulture)}%";
        }

        return new MarkupString(result);
    }

    private string GetTransformStyle(PieSegment segment)
    {
        var middleAngle = RenderHandler.CalculateMiddleAngle(segment);
        StringBuilder result = new();
        if (SegmentHandler.ShouldTransform(segment))
            result.Append(
                $"transform: translate({RenderHandler.GetHorizontal(middleAngle).ToString(CultureInfo.InvariantCulture)}px, {RenderHandler.GetVertial(middleAngle).ToString(CultureInfo.InvariantCulture)}px);");
        return result.ToString();
    }
}