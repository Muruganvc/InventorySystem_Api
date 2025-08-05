using InventorySystem_Application.Common;
using InventorySystem_Domain;
using InventorySystem_Domain.Common;
using MediatR;

namespace InventorySystem_Application.Company.BulkCompanyCommand;
internal sealed class BulkCompanyCommandHandler : IRequestHandler<BulkCompanyCommand, IResult<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<InventorySystem_Domain.Company> _companyRepository;
    private readonly IRepository<InventorySystem_Domain.Category> _categoryRepository;
    private readonly IRepository<ProductCategory> _productCategoryRepository;
    private readonly IUserInfo _userInfo;

    public BulkCompanyCommandHandler(
        IUnitOfWork unitOfWork,
        IRepository<InventorySystem_Domain.Company> companyRepository,
        IRepository<InventorySystem_Domain.Category> categoryRepository,
        IRepository<ProductCategory> productCategoryRepository,
        IUserInfo userInfo)
    {
        _unitOfWork = unitOfWork;
        _companyRepository = companyRepository;
        _categoryRepository = categoryRepository;
        _productCategoryRepository = productCategoryRepository;
        _userInfo = userInfo;
    }

    public async Task<IResult<bool>> Handle(BulkCompanyCommand request, CancellationToken cancellationToken)
    {
        bool isSuccess = false;

        var companyId = await _unitOfWork.ExecuteInTransactionAsync<bool>(async () =>
        {
            foreach (var item in request.BulkCompanyCommands)
            {
                // Add or get Company
                var existingCompany = await _companyRepository.GetByAsync(c => c.CompanyName == item.CompanyName);
                int companyId;
                if (existingCompany == null)
                {
                    var company = InventorySystem_Domain.Company.Create(item.CompanyName, _userInfo.UserId, false, string.Empty);
                    await _companyRepository.AddAsync(company);
                    await _unitOfWork.SaveAsync();
                    companyId = company.CompanyId;
                }
                else
                {
                    companyId = existingCompany.CompanyId;
                }

                // Add or get Category
                var existingCategory = await _categoryRepository.GetByAsync(c =>
                    c.CategoryName == item.CategoryName && c.CompanyId == companyId);
                int categoryId;

                if (existingCategory == null)
                {
                    var category = InventorySystem_Domain.Category.Create(item.CategoryName, companyId, _userInfo.UserId, string.Empty, false);

                    await _categoryRepository.AddAsync(category);
                    await _unitOfWork.SaveAsync();
                    categoryId = category.CategoryId;
                }
                else
                {
                    categoryId = existingCategory.CategoryId;
                }

                // Add ProductCategory if not exists
                var existingProductCategory = await _productCategoryRepository.GetByAsync(pc =>
                    pc.ProductCategoryName == item.ProductCategory && pc.CategoryId == categoryId);

                if (existingProductCategory == null)
                {
                    var productCategory = ProductCategory.Create(item.ProductCategory, categoryId, _userInfo.UserId, string.Empty, false);
                    await _productCategoryRepository.AddAsync(productCategory);
                    isSuccess = await _unitOfWork.SaveAsync() > 0;
                }
            }
            return true;
        }, cancellationToken);
        return Result<bool>.Success(isSuccess);
    }
}
