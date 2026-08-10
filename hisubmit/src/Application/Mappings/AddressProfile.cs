using AutoMapper;
using HiSubmit.Application.Features.Locatuions.Commands.AddEdit;
using HiSubmit.Domain.Entities.Locations;

namespace HiSubmit.Application.Mappings
{
    public class AddressProfile : Profile
    {
        public AddressProfile()
        {
            CreateMap<AddEditAddressCommand, Address>().ReverseMap()
                .ForMember(p=>p.CountryName,map=>map.MapFrom(src=>TakeCountryNameIfExist(src)));
        }

        private static string TakeCountryNameIfExist(Address address)
        {
            string countryName = string.Empty;
            if(address.Country != null)
            {
                countryName = address.Country.Name;
            }
            return countryName;
        }
    }

}