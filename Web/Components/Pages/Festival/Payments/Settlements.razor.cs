using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Hisubmit.Client.SharedModels.Enums.Payments;
using Hisubmit.Client.SharedModels.Features.Settlements.Commands;
using Hisubmit.Client.SharedModels.Features.Settlements.Queries;
using HiSubmit.Client.Infrastructure.Managers.FestivalPayments;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Web.Components.Pages.Festival.Payments;

public partial class Settlements
{
    private readonly List<FestivalSettlementStatementResponse> _statements = new();
    private DateRange _period = new(DateTime.Today.AddMonths(-1), DateTime.Today);
    private bool _loading;
    private bool _processing;
    private bool _canCreate;
    private string _error;
    private int? _adjustmentStatementId;
    private decimal _adjustmentAmount;
    private string _adjustmentReason;
    private string _adjustmentEvidence;

    protected override async Task OnInitializedAsync()
    {
        await base.CheckPermission(Permissions.FestivalPayment.CartItem);
        _canCreate = SelectedFestivalId > 0;
        await LoadStatementsAsync();
    }

    private async Task LoadStatementsAsync()
    {
        if (SelectedFestivalId <= 0)
            return;

        _loading = true;
        var result = await PaymentsManager.GetSettlementStatements(
            new GetFestivalSettlementStatementsQuery { FestivalId = SelectedFestivalId });
        _statements.Clear();
        if (result.Succeeded && result.Data is not null)
            _statements.AddRange(result.Data);
        else
            _error = string.Join(" ", result.Messages);
        _loading = false;
    }

    private async Task CreateStatementAsync()
    {
        if (_period.Start is null || _period.End is null)
            return;

        _processing = true;
        _error = null;
        var result = await PaymentsManager.CreateSettlementStatement(
            new CreateFestivalSettlementStatementCommand
            {
                FestivalId = SelectedFestivalId,
                PeriodStart = _period.Start.Value,
                PeriodEnd = _period.End.Value.AddDays(1)
            });
        if (!result.Succeeded)
            _error = string.Join(" ", result.Messages);
        await LoadStatementsAsync();
        _processing = false;
    }

    private async Task AddAdjustmentAsync(int statementId)
    {
        _error = null;
        _adjustmentStatementId = statementId;
        _adjustmentAmount = 0;
        _adjustmentReason = string.Empty;
        _adjustmentEvidence = string.Empty;
        await Task.CompletedTask;
    }

    private async Task SaveAdjustmentAsync()
    {
        if (!_adjustmentStatementId.HasValue)
            return;
        _processing = true;
        _error = null;
        var result = await PaymentsManager.AddSettlementAdjustment(
            SelectedFestivalId,
            _adjustmentStatementId.Value,
            new AddSettlementAdjustmentCommand
            {
                StatementId = _adjustmentStatementId.Value,
                Amount = _adjustmentAmount,
                Reason = _adjustmentReason,
                EvidenceUrl = _adjustmentEvidence
            });
        if (!result.Succeeded)
            _error = string.Join(" ", result.Messages);
        else
            _adjustmentStatementId = null;
        await LoadStatementsAsync();
        _processing = false;
    }

    private async Task ConfirmStatementAsync(int statementId)
    {
        _processing = true;
        _error = null;
        var result = await PaymentsManager.UpdateSettlementStatus(
            SelectedFestivalId,
            statementId,
            new UpdateSettlementStatusCommand
            {
                FestivalId = SelectedFestivalId,
                StatementId = statementId,
                Status = SettlementStatus.Confirmed,
                Note = "Confirmed by festival account."
            });
        if (!result.Succeeded)
            _error = string.Join(" ", result.Messages);
        await LoadStatementsAsync();
        _processing = false;
    }

    private string ExportUrl(int statementId, string format) =>
        $"/api/v1/FestivalPayments/{SelectedFestivalId}/SettlementStatements/{statementId}/export?format={format}";

    private static Color StatusColor(SettlementStatus status) => status switch
    {
        SettlementStatus.Paid => Color.Success,
        SettlementStatus.Confirmed => Color.Info,
        SettlementStatus.Disputed => Color.Error,
        _ => Color.Warning
    };
}
