using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Festivals;
using System.Collections.Generic;
using System.ComponentModel;

namespace HiSubmit.Domain.Entities.Catalog;

public class ArtCategory : AuditableEntity<int>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<FestivalArtCategory> FestivalArtCategories { get; set; }
    // public decimal Tax { get; set; }    
}