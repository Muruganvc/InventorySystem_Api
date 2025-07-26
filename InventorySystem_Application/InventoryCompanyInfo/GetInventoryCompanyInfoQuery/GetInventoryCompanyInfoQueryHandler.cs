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
    public async Task<IResult<GetInventoryCompanyInfoQueryResponse>> Handle(
    GetInventoryCompanyInfoQuery request,
    CancellationToken cancellationToken)
    {
        var companyData = await _invCompanyInfoRepository.Table
            .AsNoTracking()
            .Where(c => c.InventoryCompanyInfoId == request.InventoryCompanyInfoId)
            .Select(c => new
            {
                c.InventoryCompanyInfoId,
                c.InventoryCompanyInfoName,
                c.Description,
                c.Address,
                c.MobileNo,
                c.GstNumber,
                c.ApiVersion,
                c.UiVersion,
                c.QrCode,
                c.Email,
                c.BankName,
                c.BankBranchName,
                c.BankAccountNo,
                c.BankBranchIFSC
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (companyData == null)
            return Result<GetInventoryCompanyInfoQueryResponse>.Failure("Company not found.");

        var company = new GetInventoryCompanyInfoQueryResponse(
            companyData.InventoryCompanyInfoId,
            companyData.InventoryCompanyInfoName,
            companyData.Description,
            companyData.Address,
            companyData.MobileNo,
            companyData.GstNumber,
            companyData.ApiVersion,
            companyData.UiVersion,
            companyData.QrCode != null
                ? $"data:image/jpeg;base64,{Convert.ToBase64String(companyData.QrCode)}"
                : null,
            companyData.Email,
            companyData.BankName,
            companyData.BankBranchName,
            companyData.BankAccountNo,
            companyData.BankBranchIFSC
        );

        return Result<GetInventoryCompanyInfoQueryResponse>.Success(company);
    }
}