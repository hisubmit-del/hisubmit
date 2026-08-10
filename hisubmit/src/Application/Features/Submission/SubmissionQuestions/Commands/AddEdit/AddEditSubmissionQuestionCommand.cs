using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Submission.SubmissionQuestions.Commands.AddEdit
{
    public class AddEditSubmissionQuestionCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Questiontype Questiontype { get; set; }
        public List<UpdateDropDownCheckBoxOption> Options { get; set; }
        public int? FestivalId { get; set; }
        public bool ApplyforAllCategory { get; set; }
        public List<int> EventCategoriesId { get; set; }
        public int? JudgingId { get; set; }
        public AddEditSubmissionQuestionCommand()
        {
            EventCategoriesId = new List<int>();
            Options = new List<UpdateDropDownCheckBoxOption>();
        }
    }
    internal class AddEditSubmissionQuestionCommandHandler : IRequestHandler<AddEditSubmissionQuestionCommand, Result<int>>
    {
        private readonly IStringLocalizer<AddEditSubmissionQuestionCommandHandler> _stringLocalizer;
        private IMapper _mapper { get; set; }
        private IUnitOfWork<int> _unitOfWork;
        public AddEditSubmissionQuestionCommandHandler(
            IStringLocalizer<AddEditSubmissionQuestionCommandHandler> stringLocalizer,
            IMapper mapper, IUnitOfWork<int> unitOfWork)
        {
            _stringLocalizer = stringLocalizer;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(AddEditSubmissionQuestionCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
            {
                var subQuestion = _mapper.Map<SubmissionQuestion>(request);
                var categoriesId = new List<int>();
                if (!request.ApplyforAllCategory)
                {
                    categoriesId = request.EventCategoriesId;                   
                }
                else
                {
                    var allcatsId =await _unitOfWork.Repository<EventCategory>().Entities.Select(p => p.Id).ToListAsync();
                    categoriesId = allcatsId;
                }
                foreach (var catId in request.EventCategoriesId)
                {
                    subQuestion.SubmissionQuestionEventCategories
                        .Add(new SubmissionQuestionEventCategory()
                        {
                            EventCategoryId = catId
                        });
                }
                await _unitOfWork.Repository<SubmissionQuestion>().AddAsync(subQuestion);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllsubmissionQuestion);
                return await Result<int>.SuccessAsync(subQuestion.Id, _stringLocalizer["Question Added"]);
            }
            else
            {
                var dbSubQuestion =await _unitOfWork.Repository<SubmissionQuestion>().GetByIdAsync(request.Id);
                if (dbSubQuestion != null)
                {
                    await UpdateCategories(request.EventCategoriesId, request.Id);
                    await UpdateOptions(request.Options, request.Id);
                    var updatedSubQuestion = _mapper.Map(request, dbSubQuestion);
                    await _unitOfWork.Repository<SubmissionQuestion>().UpdateAsync(updatedSubQuestion);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllsubmissionQuestion);
                    return await Result<int>.SuccessAsync(updatedSubQuestion.Id, _stringLocalizer["Question updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_stringLocalizer["Question not Found"]);
                }
            }
        }

        private async Task UpdateCategories(List<int> catsId, int subQuestionId)
        {
            var deadLineCats = await _unitOfWork.Repository<SubmissionQuestionEventCategory>()
                .Entities.Where(p => p.SubmissionQuestionId == subQuestionId)
                .ToListAsync();
            var deletedCats = deadLineCats.Where(deadlneCat => !catsId.Any(id => id == deadlneCat.Id))
                .ToList();
            var addedCats = catsId.Where(id => !deadLineCats.Any(deadLine => deadLine.Id == id))
                .ToList();
            if (deletedCats != null)
            {
                foreach (var item in deletedCats)
                {
                    await _unitOfWork.Repository<SubmissionQuestionEventCategory>().DeleteAsync(item);
                }
            }
            if (addedCats != null)
            {
                foreach (var item in addedCats)
                {
                    await _unitOfWork.Repository<SubmissionQuestionEventCategory>().AddAsync(new SubmissionQuestionEventCategory()
                    {
                        EventCategoryId = item,
                        SubmissionQuestionId = subQuestionId
                    });
                }
            }
        }
        private async Task UpdateOptions(List<UpdateDropDownCheckBoxOption> clientOptions, int subQuestionId)
        {
            var dbOptions = await _unitOfWork.Repository<DropDownOptionCheckBoxItem>()
                .Entities.Where(p => p.QuestionId == subQuestionId)
                .ToListAsync();
            var deletedOptions = dbOptions.Where(dbOption => !clientOptions.Any(clientoption => clientoption.Id == dbOption.Id))
                .ToList();
            var addedOption = clientOptions.Where(option => option.Id == 0)
                .ToList();
            var updatedOptions = dbOptions.Where(dbOption => clientOptions.Any(clientoption => clientoption.Id == dbOption.Id))
                .ToList();

            if (deletedOptions != null)
            {
                foreach (var item in deletedOptions)
                {
                    await _unitOfWork.Repository<DropDownOptionCheckBoxItem>().DeleteAsync(item);
                }
            }
            if (addedOption != null)
            {
                foreach (var item in addedOption)
                {
                    await _unitOfWork.Repository<DropDownOptionCheckBoxItem>().AddAsync(new DropDownOptionCheckBoxItem()
                    {
                        QuestionId = subQuestionId,
                        Title = item.Title,
                    });
                }
            }
            if (updatedOptions != null)
            {
                foreach (var item in updatedOptions)
                {
                    await _unitOfWork.Repository<DropDownOptionCheckBoxItem>().UpdateAsync(item);
                }
            }
        }

    }
}
