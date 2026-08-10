using Hisubmit.Client.SharedModels.Features.Locatuions.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Enums;
using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Features.Projects.Commands.EditProjectSubmitterInformation;

public class EditProjectSubmitterInformationCommand 
{
    public int Id { get; set; }
    public string UserId { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
    public AddEditAddressCommand Address { get; set; }

    public DateTime? BirthDate { get; set; }
    public Gender Gender { get; set; }
}