using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.InventoryCompanyInfo.GetInventoryCompanyInfoQuery;

internal class GetInventoryCompanyInfoQueryHandler
    : IRequestHandler<GetInventoryCompanyInfoQuery, IResult<GetInventoryCompanyInfoQueryResponse>>
{
    private readonly IRepository<InventorySystem_Domain.InventoryCompanyInfo> _invCompanyInfoRepository;

    public GetInventoryCompanyInfoQueryHandler(
        IRepository<InventorySystem_Domain.InventoryCompanyInfo> invCompanyInfoRepository)
    {
        _invCompanyInfoRepository = invCompanyInfoRepository;
    }
    public async Task<IResult<GetInventoryCompanyInfoQueryResponse>> Handle(GetInventoryCompanyInfoQuery request, CancellationToken cancellationToken)
    {
        var company = await _invCompanyInfoRepository.Table
            .AsNoTracking()
            .Where(c => c.InventoryCompanyInfoId == request.InventoryCompanyInfoId)
            .Select(c => new GetInventoryCompanyInfoQueryResponse(
                c.InventoryCompanyInfoId,
                c.InventoryCompanyInfoName,
                c.Description,
                c.Address,
                c.MobileNo,
                c.GstNumber,
                c.ApiVersion,
                c.UiVersion,
                $"data:image/jpeg;base64,{Convert.ToBase64String(c.QrCode)}",
                c.Email,
                c.BankName,
                c.BankBranchName,
                c.BankAccountNo,
                c.BankBranchIFSC
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return company is null
            ? Result<GetInventoryCompanyInfoQueryResponse>.Failure("Company not found.")
            : Result<GetInventoryCompanyInfoQueryResponse>.Success(company);
    }
}