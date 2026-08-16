using HiSubmit.Application.Features.Festivals.Commands.AddEditEventCategory;
using System.Collections.Generic;

namespace HiSubmit.Application.Features.Festivals.Queries.GetAllEventCategory
{
    public class GetAllEventCategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<UpdateDeadlineCategoryonFee> DeadLineCategories { get; set; }

        public GetAllEventCategoryResponse()
        {
            DeadLineCategories = new List<UpdateDeadlineCategoryonFee>();
        }
    }
}
