using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.CompanyCategoryProduct.GetCompanyCategoryProductsQuery;
public record GetCompanyCategoryProductsQuery():
    IRequest<IResult<IReadOnlyList<GetCompanyCategoryProductsQueryResponse>>>;

