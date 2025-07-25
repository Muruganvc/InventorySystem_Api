﻿using InventorySystem_Application.Common;
using InventorySystem_Domain.Common;
using MediatR;

namespace InventorySystem_Application.Company.UpdateCompnayCommand;
internal sealed class UpdateCompnayCommandHandler : IRequestHandler<UpdateCompnayCommand, IResult<bool>>
{
    private readonly IRepository<InventorySystem_Domain.Company> _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCompnayCommandHandler(IRepository<InventorySystem_Domain.Company> companyRepository, IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<IResult<bool>> Handle(UpdateCompnayCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByAsync(c => c.CompanyId == request.CompanyId);

        if (company is null)
            return Result<bool>.Failure("Selected company not found.");

        if (company.RowVersion != request.RowVersion)
            return Result<bool>.Failure("The company has been modified by another user. Please reload and try again.");

        company.Update(request.CompanyName,request.IsActive, request.Description, modifiedBy: 1); // Replace `1` with actual user ID if available

        var isSuccess = await _unitOfWork.ExecuteInTransactionAsync<bool>(async () =>
        {
            var affectedRows = await _unitOfWork.SaveAsync();
            return affectedRows > 0;
        }, cancellationToken);

        return Result<bool>.Success(isSuccess);
    }

}
