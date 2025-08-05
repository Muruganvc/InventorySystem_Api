using InventorySystem_Application.Common;
using MediatR;

namespace InventorySystem_Application.Company.BulkCompanyCommand;
public record BulkCompanyEntry(
   string CompanyName,
   string CategoryName,
   string ProductCategory
);
public record BulkCompanyCommand(List<BulkCompanyEntry> BulkCompanyCommands) : IRequest<IResult<bool>>;