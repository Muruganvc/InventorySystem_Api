using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Company.GetCompaniesQuery;
public record GetCompaniesQuery(bool IsAllActiveCompany) : IRequest<IResult<IReadOnlyList<GetCompaniesQueryResponse>>>;
