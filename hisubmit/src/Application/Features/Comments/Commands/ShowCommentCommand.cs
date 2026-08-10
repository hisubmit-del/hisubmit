using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Content;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.Comments.Commands;

public class ShowCommentCommand:IRequest<IResult>
{
    public int  CommentId { get; set; }
    public bool ShowInSite { get; set; }
    public bool ShowFestival { get; set; }
    
}

public class ShowCommentCommandHandler : IRequestHandler<ShowCommentCommand, IResult>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<AddCommentCommand> _localize;

    public ShowCommentCommandHandler(
        IMapper mapper, IUnitOfWork<int> unitOfWork, IStringLocalizer<AddCommentCommand> localize)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _localize = localize;
    }
    public async Task<IResult> Handle(ShowCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _unitOfWork.Repository<Comment>().GetByIdAsync(request.CommentId);
        if (comment == null)
            return await Result.FailAsync(_localize["comment not found"]);
        comment.ShowFestival = request.ShowFestival;
        comment.ShowInSite = request.ShowInSite;
        await _unitOfWork.Repository<Comment>().UpdateAsync(comment);
        return await Result.SuccessAsync("Comment Updated");
    }
}