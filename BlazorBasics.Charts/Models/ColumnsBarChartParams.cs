namespace BlazorBasics.Charts.Models;

public class ColumnsBarChartParams
{
    public const double VERTICAL_LABEL_ANGLE = -90;

    public ColumnsBarChartParams(
        string backgroundColour = "#D3D3D3",
        int thickness = 20,
        int dimension = 100,
        bool showValues = false,
        IEnumerable<ChartColor> chartColours = null,
        int gap = 5,
        int maxWidth = 600,
        bool rotatedLabels = false,
        double labelRotationAngle = VERTICAL_LABEL_ANGLE,
        int labelFontSize = 12,
        ColumnLabelPlacement? labelPlacement = null,
        ColumnFillReference fillReference = ColumnFillReference.HighestValue)
    {
        BackgroundColour = backgroundColour;
        Thickness = thickness;
        Dimension = dimension;
        MaxWidth = maxWidth;
        ShowValues = showValues;
        Gap = gap;
        ChartColors = new(chartColours ?? ChartColourHelper
            .InitializeColours(256, 30));
        MaxWidth = maxWidth;
        RotatedLabels = rotatedLabels;
        LabelRotationAngle = labelRotationAngle;
        LabelFontSize = labelFontSize;
        LabelPlacement = labelPlacement;
        FillReference = fillReference;
    }

    public string BackgroundColour { get; init; }
    public int Thickness { get; init; }
    public int Dimension { get; init; }
    public int MaxWidth { get; init; }
    public int Gap { get; init; }
    public bool ShowValues { get; init; }

    /// <summary>
    /// Turns the column labels from horizontal to <see cref="LabelRotationAngle"/>. Together with
    /// <see cref="LabelPlacement"/> it decides whether the chart grows to fit the labels or the
    /// labels run alongside their own column.
    /// </summary>
    public bool RotatedLabels { get; init; }

    /// <summary>
    /// Rotation applied to the column labels when <see cref="RotatedLabels"/> is on. Negative angles
    /// read bottom to top, positive angles read top to bottom, and -90 is fully vertical.
    /// </summary>
    public double LabelRotationAngle { get; init; }

    public int LabelFontSize { get; init; }

    /// <summary>
    /// Where every label is drawn. Left null so that each chart keeps the placement it has always
    /// had: the column chart puts them under the columns, or beside them once they are rotated,
    /// and the bar chart puts them at the end of every bar.
    /// </summary>
    public ColumnLabelPlacement? LabelPlacement { get; init; }

    /// <summary>
    /// What a completely filled column represents. See <see cref="ColumnFillReference"/>.
    /// </summary>
    public ColumnFillReference FillReference { get; init; }

    public List<ChartColor> ChartColors { get; set; }
    public int MaxColours => ChartColors.Count;
}
