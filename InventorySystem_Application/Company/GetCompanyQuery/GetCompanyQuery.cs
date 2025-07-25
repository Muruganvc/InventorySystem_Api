using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Company.GetCompanyQuery;
public record GetCompanyQuery(int Id) : IRequest<IResult<GetCompanyQueryReponse>>;
