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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace HiSubmit.Client.Infrastructure.Routes.Festivals;

public static class FestivalEndPoint
{
    private  const string _route = "api/v1/festival/";
    private static string GetRoute(int festivalId)
    {
        return string.Format($"{_route}{festivalId}/");
    }

    public static string SpecialRequest(SpecialRequestCommand command)
    {
        var route = $"{GetRoute(command.FestivalId)}SpecialRequest";
        return route;
    }
    public static string GetAllImages(GetAllFestivalImageQuery query)
    {
        var route = $"{GetRoute(query.FestivalId)}Images";
        return route;
    }
    public static string UploadImages(AddEditFestivalImageCommand command)
    {
        var route = $"{GetRoute(command.FestivalId)}UploadImages";
        return route;
    }
    public static string ReleaseFestival(ReleaseFestivalCommand command)
    {
        var route = $"{GetRoute(command.FestivalId)}Release";
        return route;
    }

    public static string AddFestival(AddFestivalCommand command)
    {
        var route = $"{GetRoute(0)}AddFestival";
        return route;
    }
    public static string UpdateAdditionalSettings(AddEditAdditionalSettingCommand command)
    {
        var route = $"{GetRoute(command.Id)}UpdateAdditionalSetting";
        return route;
    }

    public static string AddEditDeadlineEntry(AddEditDeadLineEntryRequest request)
    {
        var route = $"{GetRoute(request.FestivalId)}AddEditDeadLineEntry";
        return route;
    }
    public static string UpdateDeadLine(AddEditFestivalDeadlineCommand command)
    {
        var route = $"{GetRoute(command.Id)}UpdateDeadLine";
        return route;
    }
    public static string AddEditVenue(AddEditFestivalVenueCommand command)
    {
        var route = $"{GetRoute(command.FestivalId)}UpdateVenue";
        return route;
    }
    public static string UpdateDetail(AddEditFestivalDetailCommand command)
    {
        var route = $"{GetRoute(command.Id)}UpdateDetail";
        return route;
    }
    public static string UpdateContact(AddEditFestivalContactCommand command)
    {
        var route = $"{GetRoute(command.Id)}UpdateContact";
        return route;
    }
    public static string AddEditEventOrganizer(AddEditEventOrginizerCommand command)
    {
        var route = $"{GetRoute(command.FestivalId)}AddOrganizer";
        return route;
    }
        
    public static string GetFestivalById(GetFestivalDetailByIdQuery query)
    {
        var route = $"{GetRoute(query.FestivalId)}getById?{QueryHelper.GetQueryString(query)}";
        return route;
    }
    public static string GetAllEventOrganizer(GetAllOrganizerQuery query)
    {
        var route = $"{GetRoute(query.FestivalId)}GetAllOrganizer?{QueryHelper.GetQueryString(query)}";
        return route;
    }
       
    public static string DeleteOrganizer(DeleteEventOrginizerCommand command)
    {
        var route = $"{GetRoute(command.FestivalId)}DeleteOrganizer?{QueryHelper.GetQueryString(command)}";
        return route;
    }
    public static string GetAllVenue(GetAllVenueQuery query)
    {
        var route = $"{GetRoute(query.FestivalId)}GetAllVenue?{QueryHelper.GetQueryString(query)}";
        return route;
    }
    public static string GetVenueById(GetVenueByIdQuery query)
    {
        var route = $"{GetRoute(query.FestivalId)}VenueDetail?{QueryHelper.GetQueryString(query)}";
        return route;
    }public static string DeleteVenue(DeleteVenueCommand command)
    {
        var route = $"{GetRoute(command.FestivalId)}DeleteVenue?{QueryHelper.GetQueryString(command)}";
        return route;
    }
    public static string GetAllDeadlineEntry(GetAllDeadlineQuery query)
    {
        var route = $"{GetRoute(query.FestivalId)}AllDeadLineEntry?{QueryHelper.GetQueryString(query)}";
        return route;
    }
    public static string GetDeadlineEntryById(GetDeadLineByIdQuery query)
    {
        var route = $"{GetRoute(query.FestivalId)}DetailDeadLine?{QueryHelper.GetQueryString(query)}";
        return route;
    }
    public static string DeleteDeadLineEntry(DeleteDeadLineEntryCommand command)
    {
        var route = $"{GetRoute(command.FestivalId)}DeleteDeadLine?{QueryHelper.GetQueryString(command)}";
        return route;
    }

    public static string GetFestivalsNames(GetFestivalNamesQuery query)
    {
        var route = $"{GetRoute(0)}GetFestivalNames?{QueryHelper.GetQueryString(query)}";
        return route;
    }

    public static string AllPeriods(GetAllFestivalPeriodsQuery query)
    {
        var route=$"{GetRoute(query.FestivalId)}AllPeriods?{QueryHelper.GetQueryString(query)}";
        return route;
    }

    public static string AllReview(GetAllReviewQuery query)
    {
        var route=$"{GetRoute(query.FestivalId.Value)}AllReview?{QueryHelper.GetQueryString(query)}";
        return route;
    }

    public static string AddReview(AddReviewCommand command)
    {
        var route = $"{GetRoute(command.FestivalId)}AddReview";
        return route;
    }
}