namespace Hisubmit.Client.SharedModels.Contracts.Permission;

public static class ApplicationClaimTypes
{
    public const string Permission = "Permission";
    public const string FestivalId = "FestivalId";
    public const string SelectedFestival = "SelectedFestivalId";
    public const string AdminLoginFestival = "AdminLoginFestival";
    public static string FestivalRole { get; } = "FestivalRole";

    public static string FestivalPermission { get; } = "FestivalPermission";
}