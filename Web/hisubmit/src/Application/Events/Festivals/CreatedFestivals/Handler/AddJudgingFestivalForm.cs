using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Enums;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Constants.Application;

namespace HiSubmit.Application.Events.Festivals.Handler;

public class AddJudgingFestivalForm(IUnitOfWork<int> unitOfWork) : INotificationHandler<CreatedFestival>
{
    public async Task Handle(CreatedFestival notification, CancellationToken cancellationToken)
    {
        await AddFilmJudgingForm(notification.Id);
        await AddScriptJudgingForm(notification.Id);
        await AddPhotographyJudgingForm(notification.Id);
        await AddMusicJudgingForm(notification.Id);
        await unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllFestivalCacheKey);
    }

    private async Task AddFilmJudgingForm(int festivalId)
    {
        var judging = new Judging()
        {
            FestivalId = festivalId,
            JudgingButtons = new List<JudgingButton>()
            {
                new(){Name="Pass"},
                new(){Name="Recommend"},
                new(){Name="Award Worthy"},
                new(){Name="Maybe"},
            },
            JudgingFileds = new List<JudgingFiled>()
            {
                new(){Name="Originality / Creativity"},
                new(){Name="Direction"},
                new(){Name="Writing"},
                new(){Name="Cinematography"},
                new(){Name="Performances"},
                new(){Name="Production Value"},
                new(){Name="Pacing"},
                new(){Name="Structure"},
                new(){Name="Sound / Music"},

            },
            ProjectType = ProjectType.Film
        };
        await unitOfWork.Repository<Judging>().AddAsync(judging);
    }

    private async Task AddScriptJudgingForm(int festivalId)
    {
        var judging = new Judging()
        {
            FestivalId = festivalId,
            JudgingButtons = new List<JudgingButton>()
            {
                new(){Name="Pass"},
                new(){Name="Recommend"},
                new(){Name="Award Worthy"},
                new(){Name="Maybe"},
            },
            JudgingFileds = new List<JudgingFiled>()
            {

                new(){Name="Concept/Originality"},
                new(){Name="Structure"},
                new(){Name="Plot"},
                new(){Name="Pacing"},
                new(){Name="Characters"},
                new(){Name="Dialogue"},
                new(){Name="Average"},
            },
            ProjectType = ProjectType.Script_ScreenWriting
        };
        await unitOfWork.Repository<Judging>().AddAsync(judging);
    }

    private async Task AddMusicJudgingForm(int festivalId)
    {
        var judging = new Judging()
        {
            FestivalId = festivalId,
            JudgingButtons = new List<JudgingButton>()
            {
                new(){Name="Pass"},
                new(){Name="Recommend"},
                new(){Name="Award Worthy"},
                new(){Name="Maybe"},
            },
            JudgingFileds = new List<JudgingFiled>()
            {
                new(){Name="Originality / Creativity"},
                new(){Name="Lyrics / Writing"},
                new(){Name="Melody"},
                new(){Name="Performance"},
                new(){Name="Overall Quality"},
                new(){Name="Average"},
            },
            ProjectType = ProjectType.Music
        };
        await unitOfWork.Repository<Judging>().AddAsync(judging);
    }

    private async Task AddPhotographyJudgingForm(int festivalId)
    {
        var judging = new Judging()
        {
            FestivalId = festivalId,
            JudgingButtons = new List<JudgingButton>()
            {
                new(){Name="Pass"},
                new(){Name="Recommend"},
                new(){Name="Award Worthy"},
                new(){Name="Maybe"},
            },
            JudgingFileds = new List<JudgingFiled>()
            {
                new(){Name="Creativity"},
                new(){Name="Composition"},
                new(){Name="Direction"},
                new(){Name="Overall Quality"},
                new(){Name="Average"},

            },
            ProjectType = ProjectType.Photography
        };
        await unitOfWork.Repository<Judging>().AddAsync(judging);
    }
}