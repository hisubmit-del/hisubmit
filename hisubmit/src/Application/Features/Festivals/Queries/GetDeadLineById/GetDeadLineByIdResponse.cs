using System;
using System.Collections.Generic;

namespace HiSubmit.Application.Features.Festivals.Queries.GetDeadLineById
{
    public class GetDeadLineByIdResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool ApplyToAllCategory { get; set; }
        public List<int> CategoriesId { get; set; }
        public int FestivalId { get; set; }
     
    }
}
