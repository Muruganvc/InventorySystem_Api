using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.Company.GetCompaniesQuery;

internal sealed class GetCompaniesQueryHandler : IRequestHandler<GetCompaniesQuery, IResult<IReadOnlyList<GetCompaniesQueryResponse>>>
{
    private readonly IRepository<InventorySystem_Domain.Company> _companyRepository;

    public GetCompaniesQueryHandler(
        IRepository<InventorySystem_Domain.Company> companyRepository)
    {
        _companyRepository = companyRepository;
    }
    public async Task<IResult<IReadOnlyList<GetCompaniesQueryResponse>>> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        var companies = await _companyRepository.Table
            .AsNoTracking()
           .Where(c => request.IsAllActiveCompany || c.IsActive)
            .Select(c => new GetCompaniesQueryResponse(
                c.CompanyId,
                c.CompanyName,
                c.Description ?? string.Empty,
                c.IsActive,
                c.RowVersion,c.CreatedAt,
                c.CreatedByUser.UserName
            )).ToListAsync(cancellationToken);

        return Result<IReadOnlyList<GetCompaniesQueryResponse>>.Success(companies);
    }

}
