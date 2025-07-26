using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Dashboard.GetTotalProductQuery;
public record  GetTotalProductQuery() 
    : IRequest<IResult<GetTotalProductQueryResponse>>;

