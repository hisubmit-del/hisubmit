using Hisubmit.Client.SharedModels.Features.Brands.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Features.Brands.Queries.GetAll;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes.Catalogs;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Managers.Catalog.Brand
{
    public class ArtCategoryManager : IArtCategoryManager
    {
        private readonly HttpClient _httpClient;

        public ArtCategoryManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<string>> ExportToExcelAsync(string searchString = "")
        {
            var response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
                ? ArtCategoriesEndPoints.Export
                : ArtCategoriesEndPoints.ExportFiltered(searchString));
            return await response.ToResult<string>();
        }

        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{ArtCategoriesEndPoints.Delete}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<IResult<List<GetAllArtCategoryResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(ArtCategoriesEndPoints.GetAll);
            return await response.ToResult<List<GetAllArtCategoryResponse>>();
        }

        public async Task<IResult<int>> SaveAsync(AddEditArtCatgoryRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ArtCategoriesEndPoints.Save, request);
            return await response.ToResult<int>();
        }
    }
}