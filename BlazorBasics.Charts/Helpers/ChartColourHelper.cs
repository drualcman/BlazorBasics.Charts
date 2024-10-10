namespace BlazorBasics.Charts.Helpers;
internal class ChartColourHelper
{
    public static IEnumerable<ChartColor> InitializeColours(int totalColors, int separation)
    {
        List<ChartColor> colors = new List<ChartColor>();
        int hueIncrement = 360 / totalColors + separation;
        for (int i = 0; i < totalColors; i++)
        {
            colors.Add(new ChartColor($"hsl({CalculateHue(i, hueIncrement)}, {CalculateSaturation(i)}%, {CalculateLightness(i)}%)"));
        }
        return colors;
    }
    static int CalculateHue(int i, int hueIncrement) => i * hueIncrement % 360;
    static int CalculateSaturation(int i) => 80 + i % 2 * 20;
    static int CalculateLightness(int i) => 40 + i % 3 * 20;
    public static string InvertHexColor(string hexColor)
    {
        hexColor = hexColor.Replace("#", string.Empty);
        int red = Convert.ToInt32(hexColor.Substring(0, 2), 16);
        int green = Convert.ToInt32(hexColor.Substring(2, 2), 16);
        int blue = Convert.ToInt32(hexColor.Substring(4, 2), 16);
        red = 255 - red;
        green = 255 - green;
        blue = 255 - blue;
        return $"#{red:X2}{green:X2}{blue:X2}";
    }

    public static string RgbToHex(string rgb)
    {
        var values = rgb.Replace("rgb(", "").Replace(")", "").Split(',');
        int red = int.Parse(values[0].Trim());
        int green = int.Parse(values[1].Trim());
        int blue = int.Parse(values[2].Trim());
        return $"#{red:X2}{green:X2}{blue:X2}";
    }

    public static string HslToHex(string hsl)
    {
        var hslValues = hsl.Replace("hsl(", "")
                            .Replace(")", "")
                            .Replace("%", "")
                            .Split(',');

        double hue = double.Parse(hslValues[0]);
        if (hue < 0 || hue > 360)
        {
            Random Random = new Random();
            _ = Random.Next(0, 360);
            hue = Random.Next(0, 360);
        }
        double saturation = ConvertToFraction(hslValues[1]);
        double lightness = ConvertToFraction(hslValues[2]);
        double chroma = (1 - Math.Abs(2 * lightness - 1)) * saturation;
        double hPrime = hue / 60.0;
        double x = CalculateX(chroma, hPrime);
        double r1 = 0, g1 = 0, b1 = 0;
        if (hPrime >= 0 && hPrime < 1)
        {
            r1 = chroma; g1 = x; b1 = 0;
        }
        else if (hPrime >= 1 && hPrime < 2)
        {
            r1 = x; g1 = chroma; b1 = 0;
        }
        else if (hPrime >= 2 && hPrime < 3)
        {
            r1 = 0; g1 = chroma; b1 = x;
        }
        else if (hPrime >= 3 && hPrime < 4)
        {
            r1 = 0; g1 = x; b1 = chroma;
        }
        else if (hPrime >= 4 && hPrime < 5)
        {
            r1 = x; g1 = 0; b1 = chroma;
        }
        else if (hPrime >= 5 && hPrime < 6)
        {
            r1 = chroma; g1 = 0; b1 = x;
        }
        double m = lightness - chroma / 2;
        int r = (int)((r1 + m) * 255);
        int g = (int)((g1 + m) * 255);
        int b = (int)((b1 + m) * 255);
        r = Math.Max(0, Math.Min(255, r));
        g = Math.Max(0, Math.Min(255, g));
        b = Math.Max(0, Math.Min(255, b));
        return $"#{r:X2}{g:X2}{b:X2}";
    }
    static double ConvertToFraction(string number) => double.Parse(number) / 100.0;
    static double CalculateX(double chroma, double hPrime) => chroma * (1 - Math.Abs(hPrime % 2 - 1));
}
