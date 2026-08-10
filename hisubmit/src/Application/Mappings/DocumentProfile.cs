using AutoMapper;
using HiSubmit.Application.Features.Documents.Commands.AddEdit;
using HiSubmit.Application.Features.Documents.Queries.GetById;
using HiSubmit.Domain.Entities.Misc;

namespace HiSubmit.Application.Mappings
{
    public class DocumentProfile : Profile
    {
        public DocumentProfile()
        {
            CreateMap<AddEditDocumentCommand, Document>().ReverseMap();
            CreateMap<GetDocumentByIdResponse, Document>().ReverseMap();
        }
    }
}