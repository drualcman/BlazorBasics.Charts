using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorBasics.Charts.Helpers;

internal static class ChartMarkupHelper
{
    /// <summary>
    /// Builds the svg text element of a label. Razor keeps "text" for itself as a tag of its own
    /// and refuses to let it carry attributes, so the only way to write a real svg text element,
    /// and hang a click on it, is building it here.
    /// </summary>
    internal static RenderFragment TextElement(ComponentBase owner, ChartText text, string style,
        Func<int, Task> click)
    {
        return builder =>
        {
            builder.OpenElement(0, "text");
            builder.AddAttribute(1, "x", SvgHelper.Number(text.X));
            builder.AddAttribute(2, "y", SvgHelper.Number(text.Y));
            builder.AddAttribute(3, "text-anchor", text.Anchor);
            builder.AddAttribute(4, "font-size", text.FontSize);

            if (!string.IsNullOrEmpty(text.Colour))
                builder.AddAttribute(5, "fill", text.Colour);

            string rotation = SvgHelper.Rotation(text);
            if (!string.IsNullOrEmpty(rotation))
                builder.AddAttribute(6, "transform", rotation);

            if (!string.IsNullOrEmpty(style))
                builder.AddAttribute(7, "style", style);

            if (click is not null)
            {
                builder.AddAttribute(8, "onclick", EventCallback.Factory.Create<MouseEventArgs>(
                    owner, () => click(text.SegmentIndex)));
            }

            builder.AddContent(9, text.Content);
            builder.CloseElement();
        };
    }
}
