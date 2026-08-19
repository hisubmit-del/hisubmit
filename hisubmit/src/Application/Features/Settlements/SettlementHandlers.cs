using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.PdfConverter;
using HiSubmit.Application.Interfaces.Services;
using Hisubmit.Client.SharedModels.Features.Settlements.Commands;
using Hisubmit.Client.SharedModels.Features.Settlements.Queries;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Domain.Entities.Festivals.Tickets;
using HiSubmit.Domain.Enums;
using HiSubmit.Domain.Enums.Payments;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.Settlements;

public sealed class GetFestivalSettlementStatementsQueryHandler
    : IRequestHandler<GetFestivalSettlementStatementsRequest, IResult<List<FestivalSettlementStatementResponse>>>
{
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetFestivalSettlementStatementsQueryHandler(IUnitOfWork<int> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult<List<FestivalSettlementStatementResponse>>> Handle(
        GetFestivalSettlementStatementsRequest request,
        CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<FestivalSettlementStatement>()
            .Entities
            .AsNoTracking()
            .Include(x => x.Adjustments)
            .Include(x => x.AdvertisingInvoices)
            .Where(x => x.FestivalId == request.FestivalId);

        if (request.PeriodStart.HasValue)
            query = query.Where(x => x.PeriodStart >= request.PeriodStart.Value);
        if (request.PeriodEnd.HasValue)
            query = query.Where(x => x.PeriodEnd <= request.PeriodEnd.Value);

        var statements = await query
            .OrderByDescending(x => x.PeriodStart)
            .ToListAsync(cancellationToken);

        return await Result<List<FestivalSettlementStatementResponse>>.SuccessAsync(
            statements.Select(Map).ToList());
    }

    internal static FestivalSettlementStatementResponse Map(FestivalSettlementStatement statement)
    {
        return new FestivalSettlementStatementResponse
        {
            Id = statement.Id,
            FestivalId = statement.FestivalId,
            PeriodStart = statement.PeriodStart,
            PeriodEnd = statement.PeriodEnd,
            GrossIncome = statement.GrossIncome,
            SiteCharges = statement.SiteCharges,
            AdvertisingCharges = statement.AdvertisingCharges,
            PaymentsToFestival = statement.PaymentsToFestival,
            NetAmount = statement.NetAmount +
                        statement.Adjustments.Sum(x => x.Amount),
            Status = (Hisubmit.Client.SharedModels.Enums.Payments.SettlementStatus)statement.Status,
            DisputeReason = statement.DisputeReason,
            ApprovalNote = statement.ApprovalNote,
            PaymentReference = statement.PaymentReference,
            ConfirmedOn = statement.ConfirmedOn,
            PaidOn = statement.PaidOn,
            Adjustments = statement.Adjustments.Select(x => new SettlementAdjustmentResponse
            {
                Id = x.Id,
                Amount = x.Amount,
                Reason = x.Reason,
                EvidenceUrl = x.EvidenceUrl
            }).ToList(),
            AdvertisingInvoices = statement.AdvertisingInvoices.Select(x => new AdvertisingInvoiceResponse
            {
                Id = x.Id,
                InvoiceNumber = x.InvoiceNumber,
                Amount = x.Amount,
                Description = x.Description,
                IssuedOn = x.IssuedOn,
                DueOn = x.DueOn,
                PaidOn = x.PaidOn,
                PaymentReference = x.PaymentReference
            }).ToList()
        };
    }
}

public sealed class CreateFestivalSettlementStatementCommandHandler
    : IRequestHandler<CreateFestivalSettlementStatementRequest, IResult>
{
    private readonly IUnitOfWork<int> _unitOfWork;

    public CreateFestivalSettlementStatementCommandHandler(IUnitOfWork<int> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult> Handle(
        CreateFestivalSettlementStatementRequest request,
        CancellationToken cancellationToken)
    {
        if (request.PeriodEnd <= request.PeriodStart)
            return await Result.FailAsync("Settlement period end must be after period start.");

        var existing = await _unitOfWork.Repository<FestivalSettlementStatement>()
            .Entities
            .FirstOrDefaultAsync(x => x.FestivalId == request.FestivalId &&
                                      x.PeriodStart == request.PeriodStart &&
                                      x.PeriodEnd == request.PeriodEnd,
                cancellationToken);
        if (existing is not null)
            return await Result.FailAsync("A settlement statement already exists for this period.");

        var start = request.PeriodStart;
        var end = request.PeriodEnd;
        var cartItems = _unitOfWork.Repository<CarTItem>().Entities
            .Where(x => x.Cart.Paid &&
                        x.Cart.CartDate >= start &&
                        x.Cart.CartDate < end);

        var submissionIncome = await cartItems
            .Where(x => x.CartItemType == CartItemType.Submit &&
                        x.Submit.FestivalId == request.FestivalId)
            .SumAsync(x => x.PriceAfterDiscount ?? x.Price, cancellationToken);

        var serviceCharges = await cartItems
            .Where(x => x.CartItemType == CartItemType.ServiceFee &&
                        x.Submit.FestivalId == request.FestivalId)
            .SumAsync(x => x.PriceAfterDiscount ?? x.Price, cancellationToken);

        var productIncome = await cartItems
            .Where(x => x.CartItemType == CartItemType.Product &&
                        x.ProductSold.Status == ProductSoldStatus.Paid &&
                        x.ProductSold.Product.FestivalId == request.FestivalId)
            .SumAsync(x => x.ProductSold.ShareFestivalIncome, cancellationToken);

        var ticketIncome = await cartItems
            .Where(x => (x.CartItemType == CartItemType.Ticket ||
                         x.CartItemType == CartItemType.Badge) &&
                        x.SoldTicket.SoldTicketStatus == SoldTicketStatus.Paid &&
                        x.SoldTicket.Ticket.Venue.FestivalId == request.FestivalId)
            .SumAsync(x => x.SoldTicket.ShareFestivalIncome, cancellationToken);

        var paidToFestival = await _unitOfWork.Repository<FestivalPaymentItem>()
            .Entities
            .Where(x => x.FestivalId == request.FestivalId &&
                        x.PaidDate >= start &&
                        x.PaidDate < end)
            .SumAsync(x => x.Amount, cancellationToken);

        var advertisingCharges = await _unitOfWork.Repository<AdvertisingInvoice>()
            .Entities
            .Where(x => x.FestivalId == request.FestivalId &&
                        x.IssuedOn >= start &&
                        x.IssuedOn < end)
            .SumAsync(x => x.Amount, cancellationToken);

        var gross = (decimal)(submissionIncome + productIncome + ticketIncome);
        var statement = new FestivalSettlementStatement
        {
            FestivalId = request.FestivalId,
            PeriodStart = start,
            PeriodEnd = end,
            GrossIncome = gross,
            SiteCharges = (decimal)serviceCharges,
            AdvertisingCharges = advertisingCharges,
            PaymentsToFestival = (decimal)paidToFestival,
            NetAmount = gross - (decimal)serviceCharges -
                        advertisingCharges - (decimal)paidToFestival,
            Status = SettlementStatus.Pending
        };

        await _unitOfWork.Repository<FestivalSettlementStatement>().AddAsync(statement);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync("Settlement statement created.");
    }
}

public sealed class AddSettlementAdjustmentCommandHandler
    : IRequestHandler<AddSettlementAdjustmentRequest, IResult>
{
    private readonly IUnitOfWork<int> _unitOfWork;

    public AddSettlementAdjustmentCommandHandler(IUnitOfWork<int> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult> Handle(
        AddSettlementAdjustmentRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Amount == 0 || string.IsNullOrWhiteSpace(request.Reason))
            return await Result.FailAsync("Adjustment amount and reason are required.");

        var statement = await _unitOfWork.Repository<FestivalSettlementStatement>()
            .GetByIdAsync(request.StatementId);
        if (statement is null)
            return await Result.FailAsync("Settlement statement was not found.");
        if (statement.FestivalId != request.FestivalId)
            return await Result.FailAsync("This settlement statement does not belong to the selected festival.");
        if (statement.Status is SettlementStatus.Paid or SettlementStatus.Confirmed)
            return await Result.FailAsync("A confirmed or paid statement cannot be adjusted.");

        await _unitOfWork.Repository<SettlementAdjustment>().AddAsync(new SettlementAdjustment
        {
            FestivalSettlementStatementId = request.StatementId,
            Amount = request.Amount,
            Reason = request.Reason.Trim(),
            EvidenceUrl = request.EvidenceUrl?.Trim()
        });
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync("Adjustment added.");
    }
}

public sealed class UpdateSettlementStatusCommandHandler
    : IRequestHandler<UpdateSettlementStatusRequest, IResult>
{
    private readonly IUnitOfWork<int> _unitOfWork;

    public UpdateSettlementStatusCommandHandler(IUnitOfWork<int> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult> Handle(
        UpdateSettlementStatusRequest request,
        CancellationToken cancellationToken)
    {
        var statement = await _unitOfWork.Repository<FestivalSettlementStatement>()
            .GetByIdAsync(request.StatementId);
        if (statement is null)
            return await Result.FailAsync("Settlement statement was not found.");
        if (statement.FestivalId != request.FestivalId)
            return await Result.FailAsync("This settlement statement does not belong to the selected festival.");

        statement.Status = (SettlementStatus)request.Status;
        statement.ApprovalNote = request.Note?.Trim();
        statement.PaymentReference = request.PaymentReference?.Trim();
        if (statement.Status == SettlementStatus.Confirmed)
            statement.ConfirmedOn = DateTime.UtcNow;
        if (statement.Status == SettlementStatus.Paid)
            statement.PaidOn = DateTime.UtcNow;

        await _unitOfWork.Repository<FestivalSettlementStatement>().UpdateAsync(statement);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync("Settlement status updated.");
    }
}

public sealed class ExportFestivalSettlementRequestHandler
    : IRequestHandler<ExportFestivalSettlementRequest, IResult<SettlementFileResponse>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IExcelService _excelService;
    private readonly IPdfGenerator _pdfGenerator;

    public ExportFestivalSettlementRequestHandler(
        IUnitOfWork<int> unitOfWork,
        IExcelService excelService,
        IPdfGenerator pdfGenerator)
    {
        _unitOfWork = unitOfWork;
        _excelService = excelService;
        _pdfGenerator = pdfGenerator;
    }

    public async Task<IResult<SettlementFileResponse>> Handle(
        ExportFestivalSettlementRequest request,
        CancellationToken cancellationToken)
    {
        var statement = await _unitOfWork.Repository<FestivalSettlementStatement>()
            .Entities
            .Include(x => x.Adjustments)
            .Include(x => x.AdvertisingInvoices)
            .FirstOrDefaultAsync(x => x.Id == request.StatementId &&
                                      x.FestivalId == request.FestivalId,
                cancellationToken);
        if (statement is null)
            return await Result<SettlementFileResponse>.FailAsync("Settlement statement was not found.");

        var rows = new[]
        {
            new SettlementExportRow
            {
                Period = $"{statement.PeriodStart:yyyy-MM-dd} - {statement.PeriodEnd:yyyy-MM-dd}",
                GrossIncome = statement.GrossIncome,
                SiteCharges = statement.SiteCharges,
                AdvertisingCharges = statement.AdvertisingCharges,
                PaymentsToFestival = statement.PaymentsToFestival,
                Adjustments = statement.Adjustments.Sum(x => x.Amount),
                NetAmount = statement.NetAmount + statement.Adjustments.Sum(x => x.Amount),
                Status = statement.Status.ToString()
            }
        };

        if (string.Equals(request.Format, "pdf", StringComparison.OrdinalIgnoreCase))
        {
            var row = rows[0];
            var html = $$"""
                <html><head><meta charset="utf-8"><style>
                body{font-family:Arial;color:#172033} table{width:100%;border-collapse:collapse}
                th,td{border:1px solid #d5dbe5;padding:10px;text-align:left}
                th{background:#e9eef5}
                </style></head><body>
                <h1>HiSubmit Settlement Statement</h1>
                <p>Festival ID: {{statement.FestivalId}}</p>
                <p>Period: {{WebUtility.HtmlEncode(row.Period)}}</p>
                <table><tr><th>Item</th><th>Amount</th></tr>
                <tr><td>Gross income</td><td>{{row.GrossIncome:C2}}</td></tr>
                <tr><td>Site charges</td><td>{{row.SiteCharges:C2}}</td></tr>
                <tr><td>Advertising charges</td><td>{{row.AdvertisingCharges:C2}}</td></tr>
                <tr><td>Payments to festival</td><td>{{row.PaymentsToFestival:C2}}</td></tr>
                <tr><td>Adjustments</td><td>{{row.Adjustments:C2}}</td></tr>
                <tr><th>Net amount</th><th>{{row.NetAmount:C2}}</th></tr>
                </table><p>Status: {{WebUtility.HtmlEncode(row.Status)}}</p>
                </body></html>
                """;
            var pdf = await _pdfGenerator.GenerateFile(new PdfGeneratorRequest
            {
                Content = html,
                DocTitle = $"settlement-{statement.Id}.pdf"
            });
            return await Result<SettlementFileResponse>.SuccessAsync(new SettlementFileResponse
            {
                File = pdf,
                MimeType = "application/pdf",
                FileName = $"settlement-{statement.Id}.pdf"
            });
        }

        var base64 = await _excelService.ExportAsync(rows,
            new Dictionary<string, Func<SettlementExportRow, object>>
            {
                ["Period"] = x => x.Period,
                ["Gross income"] = x => x.GrossIncome,
                ["Site charges"] = x => x.SiteCharges,
                ["Advertising charges"] = x => x.AdvertisingCharges,
                ["Payments to festival"] = x => x.PaymentsToFestival,
                ["Adjustments"] = x => x.Adjustments,
                ["Net amount"] = x => x.NetAmount,
                ["Status"] = x => x.Status
            }, "Settlement");
        return await Result<SettlementFileResponse>.SuccessAsync(new SettlementFileResponse
        {
            File = Convert.FromBase64String(base64),
            MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            FileName = $"settlement-{statement.Id}.xlsx"
        });
    }

    private sealed class SettlementExportRow
    {
        public string Period { get; set; }
        public decimal GrossIncome { get; set; }
        public decimal SiteCharges { get; set; }
        public decimal AdvertisingCharges { get; set; }
        public decimal PaymentsToFestival { get; set; }
        public decimal Adjustments { get; set; }
        public decimal NetAmount { get; set; }
        public string Status { get; set; }
    }
}

public sealed class CreateAdvertisingInvoiceCommandHandler
    : IRequestHandler<CreateAdvertisingInvoiceRequest, IResult>
{
    private readonly IUnitOfWork<int> _unitOfWork;

    public CreateAdvertisingInvoiceCommandHandler(IUnitOfWork<int> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult> Handle(
        CreateAdvertisingInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        if (request.FestivalId <= 0 || request.Amount <= 0 ||
            string.IsNullOrWhiteSpace(request.InvoiceNumber))
            return await Result.FailAsync("Festival, invoice number and a positive amount are required.");

        if (request.StatementId.HasValue)
        {
            var statement = await _unitOfWork.Repository<FestivalSettlementStatement>()
                .Entities
                .FirstOrDefaultAsync(x => x.Id == request.StatementId.Value, cancellationToken);
            if (statement is null || statement.FestivalId != request.FestivalId)
                return await Result.FailAsync("The selected settlement statement is invalid.");
        }

        var invoice = new AdvertisingInvoice
        {
            FestivalId = request.FestivalId,
            AdvertiseRequestId = request.AdvertiseRequestId,
            FestivalSettlementStatementId = request.StatementId,
            InvoiceNumber = request.InvoiceNumber.Trim(),
            Amount = request.Amount,
            Description = request.Description?.Trim(),
            IssuedOn = request.IssuedOn == default ? DateTime.UtcNow : request.IssuedOn,
            DueOn = request.DueOn
        };

        await _unitOfWork.Repository<AdvertisingInvoice>().AddAsync(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync("Advertising invoice created.");
    }
}
