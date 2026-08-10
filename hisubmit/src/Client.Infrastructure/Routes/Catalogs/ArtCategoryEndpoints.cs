namespace HiSubmit.Client.Infrastructure.Routes.Catalogs
{
    public static class ArtCategoriesEndPoints
    {
        public static string ExportFiltered(string searchString)
        {
            return $"{Export}?searchString={searchString}";
        }

        public static string Export = "api/v1/artCategory/export";

        public static string GetAll = "api/v1/artCategory";
        public static string Delete = "api/v1/artCategory";
        public static string Save = "api/v1/artCategory";
        public static string GetCount = "api/v1/artCategory/count";
    }
}