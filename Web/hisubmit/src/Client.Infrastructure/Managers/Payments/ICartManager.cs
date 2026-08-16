using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Collections.Generic;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.Infrastructure.Extensions;
using Hisubmit.Client.SharedModels.Features.Payments.Queries;
using Hisubmit.Client.SharedModels.Features.Payments.Commands;
using Hisubmit.Client.SharedModels.Features.Users.Commands.SpecialFee;

namespace HiSubmit.Client.Infrastructure.Managers.Payments
{
    public interface ICartManager:ITransientManager
    {
        Task<PaginatedResult<GetAllCartsResponse>> GetAll(GetAllCartsFilterDto filterDto);
        Task<IResult<List<GetCartItemResponse>>> GetItems(GetUserOpenCartItemQuery query);
        Task<IResult<int>> PaidCart(PaidCartRequest request);
        Task<IResult<CheckPaymentResponse>> CheckAndPaidCart(PaidCartRequest request);
        Task<IResult> AddSpecialAccountToCard(SpecialFeeCommand command);
        Task<IResult<GetSiteCommissionResponse>> GetSpecialAccountFee();
        Task<IResult> DeleteItem(DeleteCartItemCommand command);
        Task<IResult<DownloadCartFactorResponse>> DownloadFactor(DownloadCartFactorRequest request);
        Task<IResult> PaidZero(PaidZeroCartRequest request);
        Task<PaginatedResult<GetCartItemResponse>> CalculateDiscountCodes(CalculateDiscountCodesRequest request);


    }

    public class CartManager(HttpClient httpClient) : ICartManager
    {
        private readonly BaseEndPoint _endPoint = new("api/v1/cart");

        public async Task<PaginatedResult<GetAllCartsResponse>> GetAll(GetAllCartsFilterDto filterDto)
        {
            var response = await httpClient.GetAsync(_endPoint.GenerateUrl("GetAll",filterDto));
            return await response.ToPaginatedResult<GetAllCartsResponse>();
        }
        
        public async Task<IResult<DownloadCartFactorResponse>> 
            DownloadFactor(DownloadCartFactorRequest request)
        {
            var response = await httpClient.PostAsJsonAsync
                (_endPoint.GenerateUrl("DownloadCartFactor"),request);
            
            return await response.ToResult<DownloadCartFactorResponse>();
        }

        public async Task<IResult> PaidZero(PaidZeroCartRequest request)
        {
            var response = await httpClient.GetAsync(_endPoint.GenerateUrl("PaidZeroCart", request));
            return await response.ToResult();
        }

        public async Task<PaginatedResult<GetCartItemResponse>> CalculateDiscountCodes(CalculateDiscountCodesRequest request)
        {
            var response = await httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("CalculateDiscountCode"), request);
            return await response.ToPaginatedResult<GetCartItemResponse>();
        }

        public async Task<IResult<List<GetCartItemResponse>>> GetItems(GetUserOpenCartItemQuery query)
        {
            var response = await httpClient.GetAsync(_endPoint.GenerateUrl("GetItems", query));
            return await response.ToResult<List<GetCartItemResponse>>();
        }
        public async Task<IResult<int>> PaidCart(PaidCartRequest request)
        {
            var response = await httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("PaidCart"),request);
            return await response.ToResult<int>();
        }

        public async Task<IResult<CheckPaymentResponse>> CheckAndPaidCart(PaidCartRequest request)
        {
            var response = await httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("CheckPaidCart"),request);
            return await response.ToResult<CheckPaymentResponse>();
        }

        public async Task<IResult> AddSpecialAccountToCard(SpecialFeeCommand command)
        {
            var response = await httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("SpecialAccountAddToCard"),command);
            return await response.ToResult<int>();
        }

        public async Task<IResult<GetSiteCommissionResponse>> GetSpecialAccountFee()
        {
            var response = await httpClient.GetAsync(_endPoint.GenerateUrl("SpecialAccountFee"));
            return await response.ToResult<GetSiteCommissionResponse>();
        }

        public async Task<IResult> DeleteItem(DeleteCartItemCommand command)
        {
            var response = await httpClient.DeleteAsync(_endPoint.GenerateUrl("DeleteItem",command));
            return await response.ToResult();
        }
    }
}