using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Category.GetCategoriesQuery;

public record GetCategoriesQuery(): 
    IRequest<IResult<IReadOnlyList<GetCategoriesQueryResponse>>>;

