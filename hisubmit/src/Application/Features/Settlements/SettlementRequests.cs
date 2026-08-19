using System.Collections.Generic;
using MediatR;
using Hisubmit.Client.SharedModels.Features.Settlements.Commands;
using Hisubmit.Client.SharedModels.Features.Settlements.Queries;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Application.Features.Settlements;

public sealed class GetFestivalSettlementStatementsRequest
    : GetFestivalSettlementStatementsQuery,
      IRequest<IResult<List<FestivalSettlementStatementResponse>>>
{
}

public sealed class CreateFestivalSettlementStatementRequest
    : CreateFestivalSettlementStatementCommand, IRequest<IResult>
{
}

public sealed class AddSettlementAdjustmentRequest
    : AddSettlementAdjustmentCommand, IRequest<IResult>
{
}

public sealed class UpdateSettlementStatusRequest
    : UpdateSettlementStatusCommand, IRequest<IResult>
{
}

public sealed class ExportFestivalSettlementRequest
    : ExportFestivalSettlementQuery,
      IRequest<IResult<SettlementFileResponse>>
{
}
