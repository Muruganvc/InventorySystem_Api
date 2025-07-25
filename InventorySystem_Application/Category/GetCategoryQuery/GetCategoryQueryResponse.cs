﻿namespace InventorySystem_Application.Category.GetCategoryQuery;

public record GetCategoryQueryResponse(int CategoryId,string CategoryName,
    int CompanyId, string CompanyName,string? Description, bool IsActive,
    uint RowVersion, string CreatedBy, DateTime CreatedAt);