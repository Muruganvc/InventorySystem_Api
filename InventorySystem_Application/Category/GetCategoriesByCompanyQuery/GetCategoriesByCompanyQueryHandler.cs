using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.Category.GetCategoriesByCompanyQuery;
internal sealed class GetCategoriesByCompanyQueryHandler
    : IRequestHandler<GetCategoriesByCompanyQuery, IResult<IReadOnlyList<GetCategoriesByCompanyQueryResponse>>>
{
    private readonly IRepository<InventorySystem_Domain.Category> _categoryRepository;
    public GetCategoriesByCompanyQueryHandler(
        IRepository<InventorySystem_Domain.Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    public async Task<IResult<IReadOnlyList<GetCategoriesByCompanyQueryResponse>>> Handle(
    GetCategoriesByCompanyQuery request,
    CancellationToken cancellationToken)
    {
        var companyWiseCategories = await _categoryRepository.Table
            .AsNoTracking()
            .Where(c => c.CompanyId == request.CompanyId)
            .Select(c => new GetCategoriesByCompanyQueryResponse(
                c.CategoryId,
                c.CategoryName,
                c.Company.CompanyId,
                c.Company.CompanyName,
                c.Description,
                c.IsActive,
                c.RowVersion,
                c.CreatedByUser.UserName,
                c.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<GetCategoriesByCompanyQueryResponse>>.Success(companyWiseCategories);
    }

}
