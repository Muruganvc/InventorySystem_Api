using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Dashboard.GetCompanyWiseIncomeQuery;
public record GetCompanyWiseIncomeQuery()
    : IRequest<IResult<IReadOnlyList<GetCompanyWiseIncomeQueryResponse>>>;
