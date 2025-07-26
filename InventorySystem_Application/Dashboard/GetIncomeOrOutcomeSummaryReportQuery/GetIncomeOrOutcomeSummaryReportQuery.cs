using InventorySystem_Application.Common;
using MediatR;
namespace InventorySystem_Application.Dashboard.GetIncomeOrOutcomeSummaryReportQuery;
public record GetIncomeOrOutcomeSummaryReportQuery(DateTime? FromDate, DateTime? EndDate)
    : IRequest<IResult<IReadOnlyList<GetIncomeOrOutcomeSummaryReportQueryResponse>>>;
