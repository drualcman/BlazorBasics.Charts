using Timer = System.Timers.Timer;

namespace BlazorBasics.Charts.Handlers;

internal class PieChartAnimationHandler : IDisposable
{
    private readonly double DelayTime;
    private readonly int TotalSegments;

    private Timer AnimationTimer;

    public PieChartAnimationHandler(double delayTime, int totalSegments)
    {
        DelayTime = delayTime;
        TotalSegments = totalSegments;
        for (var i = 0; i < totalSegments; i++) SegmentStyles.Add(i, $"opacity: {(delayTime > 0 ? 0 : 1)};");
    }

    public Dictionary<int, string> SegmentStyles { get; } = new();

    public void Dispose()
    {
        if (AnimationTimer is not null)
        {
            AnimationTimer.Stop();
            AnimationTimer.Dispose();
        }
    }

    public event Action OnAnimation;

    public void StartAnimationSequence()
    {
        if (OnAnimation is not null && DelayTime > 0)
        {
            var currentSegment = 0;
            AnimationTimer = new Timer(DelayTime);
            AnimationTimer.Elapsed += (sender, args) =>
            {
                if (currentSegment < TotalSegments)
                {
                    SegmentStyles[currentSegment] = "opacity: 1; transition: opacity 1s ease-out;";
                    currentSegment++;
                    OnAnimation.Invoke();
                }
                else
                {
                    AnimationTimer.Stop();
                    AnimationTimer.Dispose();
                }
            };
            AnimationTimer.Start();
        }
        else
        {
            for (var i = 0; i < TotalSegments; i++) SegmentStyles[i] = "opacity: 1; transition: opacity 1s ease-out;";
        }
    }
}