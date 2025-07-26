using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Category.GetCategoriesByCompanyQuery;
public record GetCategoriesByCompanyQuery(int CompanyId)
    : IRequest<IResult<IReadOnlyList<GetCategoriesByCompanyQueryResponse>>>;
