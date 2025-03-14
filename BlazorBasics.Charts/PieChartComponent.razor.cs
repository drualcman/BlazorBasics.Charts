namespace BlazorBasics.Charts;

public partial class PieChartComponent : IDisposable
{
    private PieChartAnimationHandler AnimationHandler;

    private string LabelStyle = "display: none;";
    private string BiggestStyle = "display: block;";
    private PieChartRenderHandler RenderHandler;
    private PieChartSegmentHandler SegmentHandler;
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
        if(Attributes is not null && Attributes.TryGetValue("class", out var css))
            WrapperCss = css.ToString();
        Style = $"--pie-width: {Parameters.Width.ToString(CultureInfo.InvariantCulture)}px; " +
                $"--pie-height: {Parameters.Height.ToString(CultureInfo.InvariantCulture)}px; ";
        if(Attributes is not null && Attributes.TryGetValue("style", out var style))
        {
            Style += style.ToString();
            Attributes.Remove("style");
        }

        if(RenderHandler is null)
        {
            RenderHandler = new PieChartRenderHandler(Parameters);
            var data = await DataSource();
            RenderHandler.SetPie(data);
            AnimationHandler = new PieChartAnimationHandler(Parameters.DelayTime, RenderHandler.Segments.Count);
            AnimationHandler.OnAnimation += () => InvokeAsync(StateHasChanged);
            AnimationHandler.StartAnimationSequence();
        }
        SegmentHandler = new(Parameters.SeparateBiggerByDefault);
    }

    public void UpdateData(IReadOnlyList<ChartSegment> data)
    {
        RenderHandler.SetPie(data);
        StateHasChanged();
    }

    private void Leave()
    {
        if(SegmentHandler.SelectedPie is null)
        {
            LabelStyle = "display:none;";
            BiggestStyle = "display:block;";
            SegmentHandler.SetLeave();
        }
    }

    private async Task PieClick(PieSegment segment)
    {
        SegmentHandler.SelectPie(segment);
        if(SegmentHandler.SelectedPie is not null)
            ShowLabel(segment);
        else
            Leave();
        if(OnClick.HasDelegate)
            await OnClick.InvokeAsync(segment.Pie);
    }

    private void ShowLabel(PieSegment segment, MouseEventArgs e)
    {
        SegmentHandler.SetHover(segment);
        ShowLabel(segment);
    }

    private void ShowLabel(PieSegment segment)
    {
        LabelStyle = $"display:block; {GetPosition(segment)}";
        BiggestStyle = "display:none;";
    }

    private MarkupString ShowLabelData()
    {
        MarkupString result = new();
        if(SegmentHandler.HoverIsSelected)
        {
            if(OnHover is not null)
                result = new MarkupString(OnHover.Invoke(SegmentHandler.HoveredChart));
            else
                result = ShowLabelData(SegmentHandler.HoveredChart);
        }
        return result;
    }

    private MarkupString ShowLabelData(ChartSegment segment)
    {
        var content = "";
        if(segment is not null)
        {
            content = segment.SetTitleTopic is not null ? segment.ShowTitle() :
                $"{segment.Value.ToString("F2", CultureInfo.InvariantCulture)}%";
        }
        return new MarkupString(content);
    }

    private string GetPosition(PieSegment segment)
    {
        string content = string.Empty;

        if(segment is not null)
        {
            double middleAngle = RenderHandler.CalculateMiddleAngle(segment);
            double radiusOffset = (Parameters.Width / 2.0) * Parameters.CenterTextSeparationPercentage;

            double centerX = Parameters.Width / 2.0;
            double centerY = Parameters.Height / 2.0;

            double labelX = centerX + RenderHandler.GetHorizontal(middleAngle, radiusOffset);
            double labelY = centerY + RenderHandler.GetVertical(middleAngle, radiusOffset);

            content = $@"left:{((int)Math.Ceiling(labelX)).ToString(CultureInfo.InvariantCulture)}px; top: {((int)Math.Ceiling(labelY)).ToString(CultureInfo.InvariantCulture)}px;";
        }
        return content;
    }

    private string GetTransformStyle(PieSegment segment)
    {
        var middleAngle = RenderHandler.CalculateMiddleAngle(segment);
        StringBuilder result = new();
        if(SegmentHandler.ShouldTransform(segment))
            result.Append(
                $"transform: translate({RenderHandler.GetHorizontal(middleAngle).ToString(CultureInfo.InvariantCulture)}px, {RenderHandler.GetVertial(middleAngle).ToString(CultureInfo.InvariantCulture)}px);");
        return result.ToString();
    }
}