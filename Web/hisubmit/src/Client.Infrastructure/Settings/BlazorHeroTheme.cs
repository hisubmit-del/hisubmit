using System;
using MudBlazor;
using MudBlazor.Utilities;

namespace HiSubmit.Client.Infrastructure.Settings;

public class BlazorHeroTheme
{
    private static readonly Typography DefaultTypography = new ()
    {
        Default = new DefaultTypography { FontFamily = ["Open Sans"], FontWeight = "600" },
        Body1 = new Body1Typography { FontFamily = ["Open Sans"] },
        Body2 = new Body2Typography { FontFamily = ["Open Sans"] },
        H1 = new H1Typography { FontFamily = ["Open Sans"], FontSize = "2.25rem", FontWeight = "600" },
        H2 = new H2Typography { FontFamily = ["Open Sans"] ,FontSize = "1.9rem"},
        H3 = new H3Typography { FontFamily = ["Open Sans"] ,FontSize = "1.7rem",FontWeight = "600" },
        H4 = new H4Typography { FontFamily = ["Open Sans"], FontSize = "1.5rem",FontWeight = "600" },
        H5 = new H5Typography { FontFamily = ["Open Sans"], FontWeight = "600", FontSize = "1.3rem" },
        H6 = new H6Typography { FontFamily = ["Open Sans"] },
        Caption = new CaptionTypography { FontFamily = ["Open Sans"] },
        Button = new ButtonTypography()
        {
            TextTransform = "none",
            FontWeight = "600",
        },
        Subtitle1 = new Subtitle1Typography { FontSize = "17px", FontWeight = "600", FontFamily = ["Open Sans"], LineHeight = "1.6" },
        Subtitle2 = new Subtitle2Typography { FontWeight = "600" }
    };

    private static readonly LayoutProperties DefaultLayoutProperties = new ()
    {
        DefaultBorderRadius = "3px",
    };

    //[Obsolete("Obsolete")]
    public static readonly MudTheme DefaultTheme = new()
    {
        PaletteDark=new PaletteDark()
        {
            Primary="#fd0200"
        },
        PaletteLight = new PaletteLight()
        {
            Primary = "#FD0200",
            Tertiary = "#FD0200",
           // Secondary = "#FBCF5E",
            Secondary = "#FCCA6F",
            // Secondary = "#969594",
            GrayDark = "#969594",
            //Surface = "#ffffff",
             Background = "#F8F9FA",
            // Background = "#F9F7F2",
            //Background = "#aaaaaa",
            Dark = "#151928",
            Success = "#48BB78",
            Info = "#0077ff",
            AppbarBackground = "#F8F9FA",
            DrawerBackground = "#F8F9FA"
        },
            
        // Shadows = new Shadow
        // {
        //     Elevation = DefaultShadows.Shadows,
        // },
        Typography = DefaultTypography,
        LayoutProperties = DefaultLayoutProperties
    };
}

public class DefaultButton : ButtonTypography
{
    public string TextTransform { get; set; }
}


public static class DefaultShadows
{
    private const string FirstPallet = "rgba(0,30,70,0.2)";
    private const string SecondPallet = "rgba(0,30,70,0.14)";
    private const string ThirdPallet = "rgba(0,30,70,0.12)";

    public static string[] Shadows { get; set; } = new[]
    {
        "none",
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" ,
        $" 0px 2px 1px -1px rgba(145, 143, 142,.2),0px 1px 1px 0px rgba(145, 143, 142,0.14),0px 1px 3px 0px rgba(145, 143, 142,0.12)" 
    };
}

public static class ChartColors
{
    public const string Red = "#E53935";
   // public const string Dark = "#11242b";
    public const string Blue = "#00A2FF";
    public const string Yellow = "#FFC000";
    //public const string BlueGray = "#546E7A";
    public const string Green = "#00D084";
    public const string Cyan = "#00ACC1";
    public const string Purple = "#7F3DFF";

   // public static string[] ChartPlate1 = [Blue,Red,Green,Yellow];
    public static string[] ChartPlate1 = [Blue,Purple,Green,Yellow];
    public static string[] ChartPlate2 = [Blue, Purple, Green, Yellow];

    public static string[] ChartPlate3 = [Green,Blue,Yellow,Red];
    public static string[] ChartPlate4 = [Green,Blue,Yellow,Red];
    public static string[] LongChartPlate = [Blue, Purple, Green, Yellow];

    // public static string[] GenerateColor()
    // {
    //     
    // }
}