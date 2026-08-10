using HiSubmit.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSubmit.Domain.Entities.Festivals
{
    public class DeadLine:AuditableEntity<int>
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool ApplyToAllCategory { get; set; }

        [ForeignKey(nameof(Festival))]
        public int FestivalId { get; set; }
        public Festival Festival { get; set; }

        public List<DeadlineEventCategory> DeadlineEventCategories { get; set; }
    }
}

