namespace Hisubmit.Client.SharedModels.Features.Settlements.Queries;

public class ExportFestivalSettlementQuery
{
    public int FestivalId { get; set; }
    public int StatementId { get; set; }
    public string Format { get; set; } = "excel";
}
