using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Content;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.Comments.Commands;

public class AddCommentCommand:IRequest<IResult>
{
    public string Text { get; set; }
    public string Title { get; set; }
    public bool ShowInSite { get; set; }
    public CommentType Type { get; set; }
    public bool ShowFestival { get; set; }

    public int? FestivalId { get; set; }

    public int? ParentId { get; set; }
}

public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, IResult>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<AddCommentCommand> _localize;

    public AddCommentCommandHandler(
        IMapper mapper, IUnitOfWork<int> unitOfWork, IStringLocalizer<AddCommentCommand> localize)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _localize = localize;
    }
    public  async Task<IResult> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = _mapper.Map<Comment>(request);
        await _unitOfWork.Repository<Comment>().AddAsync(comment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync("Your Comment SAved");
    }
}