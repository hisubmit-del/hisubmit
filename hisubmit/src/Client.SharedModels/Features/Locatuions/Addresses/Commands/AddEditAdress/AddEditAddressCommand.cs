namespace Hisubmit.Client.SharedModels.Features.Locatuions.Commands.AddEdit;

public class AddEditAddressCommand
{
    public int Id { get; set; }
    public string Text { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public int CountryId { get; set; }
    public string CountryName { get; set; }
    public int? FestivalId { get; set; }

    public int? SubmissionFestivalId { get; set; }

    public int? VenueId { get; set; }
    public int? ProjectId { get; set; }

    public override string ToString()
    {
        string addressString = " _ ";
        if (this != null && CountryId!=null)
        {
            
            addressString = $"{CountryName} , {State} , {City}";
        }
        return addressString;
    }
        
    public  string ToShortString()
    {
        string addressString = " _ ";
        if (this != null &&  CountryId!=null)
        {
            addressString = $"{CountryName} ,  {City}";
        }
        return addressString;
    }
}