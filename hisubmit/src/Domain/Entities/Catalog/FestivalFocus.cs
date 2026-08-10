using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Festivals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Domain.Entities.Catalog
{
    public class FestivalFocus: AuditableEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<FestivalFestivalFocus> FestivalFestivalFoci { get; set; }        
    }
}
