using System;
using HiSubmit.Application.Features.Festivals.Queries.GetEventCateoryById;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Application.Features.Festivals.Queries.GetAllDeadLineEventCategory;

public class GetAllDeadLineEventCategoryResponse
{
    public int Id { get; set; }
    public string  CategoryName { get; set; }
    public int EventCategoryId { get; set; }
    public int DeadLineId { get; set; }

    public GetEventCategoryByIdResponse EventCategory { get; set; } = new();
        
    public int? StudentFee { get; set; }
    public int? StandardFee { get; set; }
    public int? GoldFee { get; set; }

    public DateTime DeadLineDate { get; set; }
    public string DeadLineName { get; set; }
        
    public FeeType SelectedFeeType { get; set; }
    public  bool Nearest { get; set; }
        
        
}