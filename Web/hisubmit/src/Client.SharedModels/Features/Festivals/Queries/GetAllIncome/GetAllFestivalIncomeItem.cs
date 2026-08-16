using Hisubmit.Client.SharedModels.Enums;
using System;

namespace Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllIncome;

public class GetAllFestivalIncomeItem
{
    public double Price { get; set; }
    public string Title { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public DateTime PaidDate { get; set; }
    public CartItemType IncomItemType { get; set; }
}

