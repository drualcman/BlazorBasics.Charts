namespace BlazorBasics.Charts.Models;

public class ChartColor
{
    public string Background { get; set; }
    public string Foreground { get; set; }

    public ChartColor(string background)
    {
        Background = background;
        Foreground = GetContrastingColor(background);
    }

    public ChartColor(string background, string foreground)
    {
        Background = background;
        Foreground = foreground;
    }

    private string GetContrastingColor(string color)
    {
        string hexColour;
        if (color.StartsWith("#"))
            hexColour = ChartColourHelper.InvertHexColor(color);
        else if (color.StartsWith("rgb"))
            hexColour = ChartColourHelper.InvertHexColor(ChartColourHelper.RgbToHex(color));
        else if (color.StartsWith("hsl"))
            hexColour = ChartColourHelper.InvertHexColor(ChartColourHelper.HslToHex(color));
        else
            hexColour = color;
        return hexColour;
    }
}
