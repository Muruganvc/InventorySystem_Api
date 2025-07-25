using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.InventoryCompanyInfo.GetInventoryCompanyInfoQuery;
public record GetInventoryCompanyInfoQuery(int InventoryCompanyInfoId) :
    IRequest<IResult<GetInventoryCompanyInfoQueryResponse>>;
