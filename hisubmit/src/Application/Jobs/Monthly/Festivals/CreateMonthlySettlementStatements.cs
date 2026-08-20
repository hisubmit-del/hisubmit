using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Features.Settlements;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services.BackGroundJob;
using HiSubmit.Domain.Entities.Festivals;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Jobs.Monthly.Festivals;

public interface ICreateMonthlySettlementStatements
{
    Task InvokeAsync();
    Task CreatePreviousMonthStatementsAsync();
}

public sealed class CreateMonthlySettlementStatements(
    IBackGroundJobService backgroundJobService,
    IUnitOfWork<int> unitOfWork,
    IMediator mediator) : ICreateMonthlySettlementStatements
{
    public Task InvokeAsync()
    {
        backgroundJobService.AddRecurring(
            () => CreatePreviousMonthStatementsAsync(),
            CornJob.Monthly,
            0,
            "CreateMonthlyFestivalSettlementStatements");

        return Task.CompletedTask;
    }

    public async Task CreatePreviousMonthStatementsAsync()
    {
        var firstOfThisMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var periodStart = firstOfThisMonth.AddMonths(-1);
        var periodEnd = firstOfThisMonth;

        var festivalIds = await unitOfWork.Repository<Festival>()
            .Entities
            .Where(f => f.IsActive)
            .Select(f => f.Id)
            .ToListAsync(CancellationToken.None);

        foreach (var festivalId in festivalIds)
        {
            await mediator.Send(new CreateFestivalSettlementStatementRequest
            {
                FestivalId = festivalId,
                PeriodStart = periodStart,
                PeriodEnd = periodEnd
            }, CancellationToken.None);
        }
    }
}
