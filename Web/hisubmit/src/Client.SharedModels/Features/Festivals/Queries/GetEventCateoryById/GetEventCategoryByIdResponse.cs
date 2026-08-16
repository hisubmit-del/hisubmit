using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditEventCategory;
using Hisubmit.Client.SharedModels.Enums;
using System.Collections.Generic;

namespace Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetEventCateoryById
{
    public class GetEventCategoryByIdResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int FestivalId { get; set; }

        //RunTime
        public ProjectType? ProjectType { get; set; }
        public RuntimeType? RuntimeType { get; set; }
        public int FirstRunTimeValue { get; set; }
        public int? SecoundRunTimeValue { get; set; }

        public bool RequirePassword { get; set; }
        public string Password { get; set; }
        public bool StudentProject { get; set; }

        //Locations
        public LocationType? LocationType { get; set; }
        public int? CountryId { get; set; }
        public string CityOrStateName { get; set; }

        public List<UpdateDeadlineCategoryonFee> DeadLineCategories { get; set; }
        
        public List<int> CountriesId { get; set; }
        public List<string> CountriesName { get; set; }
    }
}
