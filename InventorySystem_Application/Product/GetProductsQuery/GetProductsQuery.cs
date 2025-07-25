using InventorySystem_Application.Common;
using MediatR;
namespace InventorySystem_Application.Product.GetProductsQuery;
public record GetProductsQuery() 
    : IRequest<IResult<IReadOnlyList<GetProductsQueryResponse>>>;
