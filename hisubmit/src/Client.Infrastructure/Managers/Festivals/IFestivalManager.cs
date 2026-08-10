using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEdiitEventOrginizer;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditAdditinalSettings;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditDeadLineEntry;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalContact;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalDeadlines;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalVenue;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.CreateFestival;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.DeleteDeadLineEntry;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.DeleteEventOrginizer;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.DeleteVenue;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllDeadLine;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllOrginizer;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllVenue;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDeadLineById;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetVenueById;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalImages;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddFestival;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.ReleaseFestival;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.SpecialRequest;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllFestivalPeriods;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllImages;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetFestivalNames;
using Hisubmit.Client.SharedModels.Features.Reviews.Commands;
using Hisubmit.Client.SharedModels.Features.Reviews.Queries;

namespace HiSubmit.Client.Infrastructure.Managers.Festivals
{
    public interface IFestivalManager : ITransientManager
    {
        Task<IResult> SpecialRequest(SpecialRequestCommand command);
        Task<IResult<int>> SaveDetailAsync(AddEditFestivalDetailCommand request);
        Task<IResult<GetFestivalDetailResponse>> GetFestivalDetailAsync(GetFestivalDetailByIdQuery query);
        Task<IResult<List<GetAllEventOrganizerResponse>>> GetAllOrganizerAsync(GetAllOrganizerQuery query);
        Task<IResult<int>> SaveOrginizerAsync(AddEditEventOrginizerCommand request);
        Task<IResult<int>> DeleteOrginizer(DeleteEventOrginizerCommand command);
        Task<IResult<int>> SaveContactAsync(AddEditFestivalContactCommand command);
        Task<IResult<int?>> SaveVenueAsync(AddEditFestivalVenueCommand command);
        Task<PaginatedResult<GetAllVenueResponse>> GetAllVenueAsync(GetAllVenueQuery query);
        Task<IResult<int>> DeleteVenueAsync(DeleteVenueCommand command);
        Task<IResult<GetVenueByIdResponse>> GetVenueById(GetVenueByIdQuery query);
        Task<IResult<int>> SaveDeadLineAsync(AddEditFestivalDeadlineCommand command);
        Task<IResult<GetDeadLineByIdResponse>> AddEditDeadLineEntry(AddEditDeadLineEntryRequest request);
        Task<IResult<GetDeadLineByIdResponse>> GetDeadlineEntryDetail(GetDeadLineByIdQuery query);
        Task<IResult<List<GetAllDeadLineResponse>>> GetAllDeadlineEntry(GetAllDeadlineQuery query);
        Task<IResult<int>> DeleteDeadLineEntry(DeleteDeadLineEntryCommand command);
        Task<IResult<int>> SaveAdditionalSetting(AddEditAdditionalSettingCommand command);
        Task<IResult<int>> AddFestival(AddFestivalCommand command);
        Task<IResult> Release(ReleaseFestivalCommand command);
        Task<PaginatedResult<GetFestivalNamesResponse>> GetFestivalNames(GetFestivalNamesQuery query);
        Task<IResult> UploadImages(AddEditFestivalImageCommand command);
        Task<PaginatedResult<GetAllFestivalImageResponse>> GetAllImages(GetAllFestivalImageQuery query);

        Task<IResult<GetAllFestivalPeriodsResponse>> GetAllFestivalPeriods(GetAllFestivalPeriodsQuery query);
        Task<PaginatedResult<GetAllReviewResponse>> GetAllReview(GetAllReviewQuery query);
    }
}
