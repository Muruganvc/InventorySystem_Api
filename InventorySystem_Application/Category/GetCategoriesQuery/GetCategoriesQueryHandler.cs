using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.Category.GetCategoriesQuery;
internal sealed class GetCategoriesQueryHandler
     : IRequestHandler<GetCategoriesQuery, IResult<IReadOnlyList<GetCategoriesQueryResponse>>>
{
    private readonly IRepository<InventorySystem_Domain.Category> _categoryRepository;

    public GetCategoriesQueryHandler(
        IRepository<InventorySystem_Domain.Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    public async Task<IResult<IReadOnlyList<GetCategoriesQueryResponse>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var companyCategories = await _categoryRepository.Table
            .AsNoTracking()
            .Select(c => new GetCategoriesQueryResponse(
                c.CategoryId, c.CategoryName,
                c.Company.CompanyId, c.Company.CompanyName,
                c.Description, c.IsActive, c.RowVersion,
                c.CreatedByUser.UserName,
                c.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<GetCategoriesQueryResponse>>.Success(companyCategories);
    }
}
