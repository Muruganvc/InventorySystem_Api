using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Category.GetCategoryQuery;
public record GetCategoryQuery(int CategoryId): IRequest<IResult<GetCategoryQueryResponse>>;
