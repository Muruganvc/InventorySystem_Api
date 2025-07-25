using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.Company.GetCompanyQuery;
internal sealed class GetCompanyQueryHandler : IRequestHandler<GetCompanyQuery, IResult<GetCompanyQueryReponse>>
{
    private readonly IRepository<InventorySystem_Domain.Company> _companyRepository;

    public GetCompanyQueryHandler(
        IRepository<InventorySystem_Domain.Company> companyRepository)
    {
        _companyRepository = companyRepository;
    }
    public async Task<IResult<GetCompanyQueryReponse>> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.Table
            .AsNoTracking()
            .Where(c => c.CompanyId == request.Id)
            .Select(c => new GetCompanyQueryReponse(
                c.CompanyId,
                c.CompanyName,
                c.Description ?? string.Empty,
                c.IsActive,
                c.RowVersion,
                c.CreatedByUser.UserName
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return company is null
            ? Result<GetCompanyQueryReponse>.Failure("Company not found.")
            : Result<GetCompanyQueryReponse>.Success(company);
    }


}
