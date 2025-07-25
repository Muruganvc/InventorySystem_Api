using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Product.GetProductQuery;
public record GetProductQuery(int ProductId) :IRequest<IResult<GetProductQueryResponse>>;
