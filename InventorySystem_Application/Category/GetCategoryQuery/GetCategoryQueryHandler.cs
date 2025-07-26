using InventorySystem_Application.Common;
using InventorySystem_Application.Company.GetCompanyQuery;
using InventorySystem_Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem_Application.Category.GetCategoryQuery;
internal sealed class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery,
    IResult<GetCategoryQueryResponse>>
{
    private readonly IRepository<InventorySystem_Domain.Category> _categoryRepository;

    public GetCategoryQueryHandler(
        IRepository<InventorySystem_Domain.Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    public async Task<IResult<GetCategoryQueryResponse>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
    {
        var companyCategory = await _categoryRepository.Table
           .AsNoTracking()
           .Where(c => c.CategoryId == request.CategoryId)
           .Select(c => new GetCategoryQueryResponse(
               c.CategoryId, c.CategoryName,
               c.Company.CompanyId,c.Company.CompanyName,
               c.Description,c.IsActive,c.RowVersion,
               c.CreatedByUser.UserName,
               c.CreatedAt
           ))
           .FirstOrDefaultAsync(cancellationToken);

        return companyCategory is null
           ? Result<GetCategoryQueryResponse>.Failure("Company category not found.")
           : Result<GetCategoryQueryResponse>.Success(companyCategory);
    }
}
