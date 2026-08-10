using System;

namespace HiSubmit.Application.Features.Festivals.Queries.GetAllDeadLine
{
    public class GetAllDeadLineResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool? ApplyToAllCategory { get; set; }
    }
}
