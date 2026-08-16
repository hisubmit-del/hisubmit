using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Locations;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Locatuions.Commands.AddEdit
{
    public class AddEditAddressCommand : IRequest<Result<int>>
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
            if (this != null)
            {
                addressString = $"{CountryName} , {State} , {City}";
            }
            return addressString;
        }
        
        public  string ToShortString()
        {
            string addressString = " _ ";
            if (this != null)
            {
                addressString = $"{CountryName} ,  {City}";
            }
            return addressString;
        }
    }
    public class AddEditAddressCommandHandler : IRequestHandler<AddEditAddressCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<AddEditAddressCommandHandler> _stringLocalizer;
        public AddEditAddressCommandHandler(IMapper mapper, IUnitOfWork<int> unitOfWork, IStringLocalizer<AddEditAddressCommandHandler> stringLocalizer)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _stringLocalizer = stringLocalizer;
        }

        public async Task<Result<int>> Handle(AddEditAddressCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
            {
                var dbAddress =await GetDbAddress(request);
                if(dbAddress != null)
                {
                    var updatedAddress = _mapper.Map(request, dbAddress);
                    await _unitOfWork.Repository<Address>().UpdateAsync(updatedAddress);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllAddressCachKey);
                    return await Result<int>.SuccessAsync(updatedAddress.Id, _stringLocalizer["Address Updated"]);
                }
                else
                {
                    var address = _mapper.Map<Address>(request);
                    await _unitOfWork.Repository<Address>().AddAsync(address);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllAddressCachKey);
                    return await Result<int>.SuccessAsync(address.Id, _stringLocalizer["Address Saved"]);
                }
               
            }
            else
            {
                var address = await _unitOfWork.Repository<Address>().GetByIdAsync(request.Id);
                if (address != null)
                {
                    await _unitOfWork.Repository<Address>().UpdateAsync(address);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllAddressCachKey);
                    return await Result<int>.SuccessAsync(address.Id, _stringLocalizer["Address Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_stringLocalizer["Address Not Found!"]);
                }
            }
        }

        private async Task<Address> GetDbAddress(AddEditAddressCommand commmand)
        {
            Address address = null;
            if (commmand.FestivalId != null)
            {
                address = await _unitOfWork.Repository<Address>()
                       .Entities.Where(p => p.FestivalId == commmand.FestivalId)
                       .FirstOrDefaultAsync();
            }
            else if (commmand.VenueId != null)
            {
                address = await _unitOfWork.Repository<Address>()
                    .Entities.Where(p => p.VenueId == commmand.VenueId)
                    .FirstOrDefaultAsync();
            }
            else if (commmand.SubmissionFestivalId != null)
            {
                address = await _unitOfWork.Repository<Address>()
                    .Entities.Where(p => p.SubmissionFestivalId == commmand.SubmissionFestivalId)
                    .FirstOrDefaultAsync();
            }

            return address;
        }
    }
}
