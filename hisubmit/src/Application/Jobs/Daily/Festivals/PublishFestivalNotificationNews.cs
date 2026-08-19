using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services.BackGroundJob;
using HiSubmit.Domain.Entities.Content;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Entities.Submitter;
using HiSubmit.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Jobs.Daily.Festivals;

public interface IPublishFestivalNotificationNews
{
    Task InvokeAsync();
    Task PublishDueFestivalNewsAsync();
}

/// <summary>
/// Publishes one public festival news item after the festival notification date.
/// The title contains the notification date so the operation is idempotent
/// without adding a schema column or duplicating an existing news record.
/// </summary>
public sealed class PublishFestivalNotificationNews(
    IBackGroundJobService backgroundJobService,
    IUnitOfWork<int> unitOfWork) : IPublishFestivalNotificationNews
{
    public Task InvokeAsync()
    {
        backgroundJobService.AddRecurring(
            () => PublishDueFestivalNewsAsync(),
            CornJob.Daily,
            0,
            "PublishFestivalNotificationNews");

        return Task.CompletedTask;
    }

    public async Task PublishDueFestivalNewsAsync()
    {
        var now = DateTime.Now;
        var festivals = await unitOfWork.Repository<Festival>()
            .Entities
            .Where(f => f.Public &&
                        f.EnableAutomaticSelectionNews &&
                        f.NotificationDate.HasValue &&
                        f.NotificationDate.Value <= now)
            .ToListAsync(CancellationToken.None);

        foreach (var festival in festivals)
        {
            var title = $"{festival.Name} - Official Selection {festival.NotificationDate:yyyy-MM-dd}";
            var alreadyPublished = await unitOfWork.Repository<New>()
                .Entities
                .AnyAsync(n => n.FestivalId == festival.Id && n.Title == title,
                    CancellationToken.None);

            if (alreadyPublished)
                continue;

            var selectedProjects = await unitOfWork.Repository<Submit>()
                .Entities
                .Where(s => s.FestivalId == festival.Id &&
                            s.SubmitStatus != SubmitStatus.Disqualified &&
                            s.SubmitStatus != SubmitStatus.Withdrawn &&
                            (s.JudgingStatus == JudgingStatus.Selected ||
                             s.JudgingStatus == JudgingStatus.AwardWinner ||
                             s.JudgingStatus == JudgingStatus.Finalist ||
                             s.JudgingStatus == JudgingStatus.SemiFinalist ||
                             s.JudgingStatus == JudgingStatus.QuarterFinalist ||
                             s.JudgingStatus == JudgingStatus.Nominee ||
                             s.JudgingStatus == JudgingStatus.HonorableMention))
                .Include(s => s.Project)
                .OrderBy(s => s.Project.Title)
                .ToListAsync(CancellationToken.None);

            // Do not publish an empty "official selection" announcement. The
            // daily job will retry once the festival publishes selections.
            if (selectedProjects.Count == 0)
                continue;

            var description = new StringBuilder();
            description.Append("<p>");
            description.Append(WebUtility.HtmlEncode(
                string.IsNullOrWhiteSpace(festival.Description)
                    ? $"The official selection of {festival.Name} is now available."
                    : festival.Description));
            description.Append("</p><h3>Selected works</h3><ul>");

            foreach (var submit in selectedProjects)
            {
                var project = submit.Project;
                var synopsis = project?.EnglishBriefSynopsis ?? project?.OriginalBriefSynopsis;
                description.Append("<li><strong>");
                description.Append(WebUtility.HtmlEncode(project?.Title ?? "Untitled work"));
                description.Append("</strong>");
                if (!string.IsNullOrWhiteSpace(synopsis))
                {
                    description.Append(" - ");
                    description.Append(WebUtility.HtmlEncode(synopsis));
                }
                description.Append("</li>");
            }

            description.Append("</ul>");

            await unitOfWork.Repository<New>().AddAsync(new New
            {
                FestivalId = festival.Id,
                Title = title,
                ShortDescription = $"Official selection from {festival.Name}.",
                Description = description.ToString(),
                BannerUrl = festival.LogoURL,
                IsEnable = true
            });

            await unitOfWork.SaveChangesAsync(CancellationToken.None);
        }
    }
}
