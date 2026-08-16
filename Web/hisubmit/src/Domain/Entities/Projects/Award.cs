using HiSubmit.Domain.Contracts;
using System;

namespace HiSubmit.Domain.Entities.Projects
{
    public class Award:AuditableEntity<int>
    {
        public string Title { get; set; }
        public string Location { get; set; }
        public string AwardsWon { get; set; }
        public DateTime Date { get; set; }

        public string ImageUrl { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
