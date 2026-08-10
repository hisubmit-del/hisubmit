using AutoMapper;
using HiSubmit.Application.Features.DocumentTypes.Commands.AddEdit;
using HiSubmit.Application.Features.DocumentTypes.Queries.GetAll;
using HiSubmit.Application.Features.DocumentTypes.Queries.GetById;
using HiSubmit.Domain.Entities.Misc;

namespace HiSubmit.Application.Mappings
{
    public class DocumentTypeProfile : Profile
    {
        public DocumentTypeProfile()
        {
            CreateMap<AddEditDocumentTypeCommand, DocumentType>().ReverseMap();
            CreateMap<GetDocumentTypeByIdResponse, DocumentType>().ReverseMap();
            CreateMap<GetAllDocumentTypesResponse, DocumentType>().ReverseMap();
        }
    }
}