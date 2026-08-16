using System.Linq;

namespace HiSubmit.Client.Infrastructure.Routes
{
    public static class ProductsEndpoints
    {

        public static string GetAllPaged = $"api/v1/products";

        //    (int? festivalId,int pageNumber, int pageSize, string searchString, string[] orderBy)
        //{
        //    var url = $"api/v1/products/{festivalId}/getAll?pageNumber={pageNumber}&pageSize={pageSize}&searchString={searchString}&orderBy=";
        //    if (orderBy?.Any() == true)
        //    {
        //        foreach (var orderByPart in orderBy)
        //        {
        //            url += $"{orderByPart},";
        //        }
        //        url = url[..^1]; // loose training ,
        //    }
        //    return url;
        //}

        public static string GetCount = "api/v1/products/count";

        public static string GetProductImage(int productId,int festivalId)
        {
            return $"api/v1/products/{festivalId}/image/{productId}";
        }

        public static string ExportFiltered(string searchString,int festivalId)
        {
            return $"{Export(festivalId)}?searchString={searchString}";
        }

      


        public static string Save(int festivalId)
        {
            return $"api/v1/products/{festivalId}/update";
        }
        
        public static string Get(int festivalId)
        {
            return $"api/v1/products/{festivalId}/get";
        }

        public static string Delete(int festivalId)
        {
            return $"api/v1/products/{festivalId}/delete";
        }

        public static string Export(int festivalId)
        {
            return $"api/v1/products/{festivalId}/export";
        }
        public static string ChangePassword = "api/identity/account/changepassword";
        public static string UpdateProfile = "api/identity/account/updateprofile";
    }
}