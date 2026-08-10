using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Features.Brands.Commands.AddEdit
{
    public  class AddEditArtCatgoryRequest 
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }
}