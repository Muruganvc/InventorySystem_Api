using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.CompanyCategoryProduct.GetCompanyCategoryProductQuery;
public record GetCompanyCategoryProductQuery(int CompanyCategoryProductId)
    :IRequest<IResult<GetCompanyCategoryProductQueryResponse>>;
