using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Company.GetCompaniesQuery;
public record GetCompaniesQuery(): IRequest<IResult<IReadOnlyList<GetCompaniesQueryResponse>>>;
