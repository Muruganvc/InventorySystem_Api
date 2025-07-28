using InventorySystem_Api.Request;
using InventorySystem_Application.Common;
using InventorySystem_Application.Customer.GetCustomerQuery;
using InventorySystem_Application.InventoryCompanyInfo.CreateInventoryCompanyInfoCommand;
using InventorySystem_Application.InventoryCompanyInfo.GetInventoryCompanyInfoQuery;
using InventorySystem_Application.InventoryCompanyInfo.UpdateInventoryCompanyInfoCommand;
using InventorySystem_Application.MenuItem.AddOrRemoveUserMenuItemCommand;
using InventorySystem_Application.MenuItem.GetMenusQuery;
using InventorySystem_Application.MenuItem.GetUserMenuQuery;
using InventorySystem_Application.Users.ActiveOrInActiveUserCommand;
using InventorySystem_Application.Users.AddOrRemoveUserRoleCommand;
using InventorySystem_Application.Users.CreateUserCommand;
using InventorySystem_Application.Users.ForgetPasswordCommand;
using InventorySystem_Application.Users.GetAllRoles;
using InventorySystem_Application.Users.GetRoleByUserQuery;
using InventorySystem_Application.Users.GetUserQuery;
using InventorySystem_Application.Users.GetUsersQuery;
using InventorySystem_Application.Users.LoginCommand;
using InventorySystem_Application.Users.PasswordChangeCommand;
using InventorySystem_Application.Users.UpdateUserCommand;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem_Api.Endpoints;

public static class UserEndPoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/users", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetUsersQuery());
            return Results.Ok(result);
        })
        .WithName("GetAllUsers")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get all users";
            operation.Description = "Retrieves a list of all registered users.";
            return operation;
        })
        .Produces<IResult<IReadOnlyList<GetUsersQueryResponse>>>(StatusCodes.Status200OK);

        app.MapGet("/user/{userId}", async (int userId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetUserQuery(userId));
            return Results.Ok(result);
        })
        .WithName("GetUser")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get user by ID";
            operation.Description = "Retrieves user details using the user ID.";
            return operation;
        })
        .Produces<IResult<GetUserQueryResponse>>(StatusCodes.Status200OK);

        app.MapPost("/user", async ([FromBody] CreateUserRequest request, IMediator mediator) =>
        {
            var command = new CreateUserCommand(
                request.FirstName,
                request.LastName,
                request.UserName,
                "Welcome2627",
                request.Email,
                request.MobileNo
            );
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .RequireAuthorization("SuperAdminOnly")
        .WithName("CreateUser")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Create new user";
            operation.Description = "Creates a user account with default password. Super Admin only.";
            return operation;
        })
        .Produces<IResult<int>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/user/{userId}", async (HttpRequest request, int userId, IMediator mediator) =>
        {
            var form = await request.ReadFormAsync();

            var command = new UpdateUserCommand(
                userId,
                form["FirstName"]!,
                form["LastName"]!,
                form["Email"]!,
                form.Files.GetFile("Image") is { Length: > 0 } file
                    ? await GetBytes(file)
                    : null,
                form["MobileNo"]!
            );

            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("UpdateUser")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Update user details";
            operation.Description = "Updates a user’s information and profile image.";
            return operation;
        })
        .Produces<IResult<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/user/{userId}/status/{isActive}", async (int userId, bool isActive, IMediator mediator) =>
        {
            var result = await mediator.Send(new ActiveOrInActiveUserCommand(userId, isActive));
            return Results.Ok(result);
        })
        .WithName("UpdateUserStatus")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Change user active status";
            operation.Description = "Activates or deactivates a user.";
            return operation;
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/user/{userId}/role/{roleId}", async (int userId, int roleId, IMediator mediator) =>
        {
            var result = await mediator.Send(new AddOrRemoveUserRoleCommand(userId, roleId));
            return Results.Ok(result);
        })
        .WithName("AddOrRemoveUserRole")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Assign/remove role";
            operation.Description = "Adds or removes a role for a given user.";
            return operation;
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        app.MapPost("/login", async ([FromBody] LoginRequest request, IMediator mediator) =>
        {
            var result = await mediator.Send(new LoginCommand(request.UserName, request.Password));
            return Results.Ok(result);
        })
        .WithMetadata(new AllowAnonymousAttribute())
        .WithName("Login")
        .WithOpenApi(operation =>
        {
            operation.Summary = "User login";
            operation.Description = "Authenticates user and returns JWT and role information.";
            return operation;
        })
        .Produces<IResult<LoginCommandResponse>>(StatusCodes.Status200OK);

        app.MapPut("/change-password/{userId}", async (int userId, [FromBody] ChangePasswordRequest request, IMediator mediator) =>
        {
            var result = await mediator.Send(new PasswordChangeCommand(userId, request.CurrentPassword, request.PasswordHash));
            return Results.Ok(result);
        })
        .WithName("Changepassword")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Change password";
            operation.Description = "Allows a user to change their password.";
            return operation;
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/forget-password/{userId}/mobile/{mobileNo}", async (int userId, string mobileNo, IMediator mediator) =>
        {
            var result = await mediator.Send(new ForgetPasswordCommand(userId, mobileNo, ""));
            return Results.Ok(result);
        })
        .WithName("ForgetPassword")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Forget password";
            operation.Description = "Resets a user's password based on user ID and mobile number.";
            return operation;
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/user-menu/{userId}/menu/{menuId}", async (int userId, int menuId, IMediator mediator) =>
        {
            var result = await mediator.Send(new AddOrRemoveUserMenuItemCommand(userId, menuId));
            return Results.Ok(result);
        })
        .WithName("UserMenuAccessToggle")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Toggle menu access";
            operation.Description = "Assigns or removes a menu item from a user.";
            return operation;
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        app.MapGet("/menu/{userId}", async (int userId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetUserMenuQuery(userId));
            return Results.Ok(result);
        })
        .WithName("GetUserMenu")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get menus for user";
            operation.Description = "Returns all menu items assigned to the specified user.";
            return operation;
        })
        .Produces<IResult<IReadOnlyList<GetMenuItemQueryResponse>>>(StatusCodes.Status200OK);

        app.MapGet("/menus", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetMenusQuery());
            return Results.Ok(result);
        })
        .WithName("GetAllMenus")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get all menus";
            operation.Description = "Returns all available system menus.";
            return operation;
        })
        .Produces<IResult<IReadOnlyList<GetMenusQueryResponse>>>(StatusCodes.Status200OK);

        app.MapGet("/roles", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetRolesQuery());
            return Results.Ok(result);
        })
        .WithName("GetAllRoles")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get all roles";
            operation.Description = "Retrieves all user roles in the system.";
            return operation;
        })
        .Produces<IResult<IReadOnlyList<GetAllRolesResponse>>>(StatusCodes.Status200OK);

        app.MapGet("/user-roles/{userId:int}", async (int userId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetRoleByUserQuery(userId));
            return Results.Ok(result);
        })
        .WithName("GetUserRolesByUserId")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get roles assigned to user";
            operation.Description = "Returns a list of roles assigned to the given user ID.";
            return operation;
        })
        .Produces<IResult<IReadOnlyList<GetRoleByUserQueryResponse>>>(StatusCodes.Status200OK);

        app.MapGet("/customers", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetCustomerQuery());
            return Results.Ok(result);
        })
        .WithName("GetCustomers")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get all customers";
            operation.Description = "Fetches a list of all registered customers.";
            return operation;
        })
        .Produces<IResult<IReadOnlyList<GetCustomerQueryResponse>>>(StatusCodes.Status200OK);

        app.MapPost("/inventory-company-info", async (HttpRequest request, IMediator mediator) =>
        {
            var form = await request.ReadFormAsync();

            var qrCode = form.Files["QrCode"];
            byte[]? qrCodeData = qrCode is { Length: > 0 } ? await GetBytes(qrCode) : null;

            var command = new CreateInventoryCompanyInfoCommand(
                form["inventoryCompanyInfoName"]!, form["description"]!, form["address"]!, form["mobileNo"]!,
                form["gstNumber"]!, form["apiVersion"]!, form["uiVersion"]!, qrCodeData!,
                form["email"]!, form["bankName"]!, form["bankBranchName"]!,
                form["bankAccountNo"]!, form["bankBranchIFSC"]!
            );

            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithOpenApi(operation =>
        {
            operation.Summary = "Create inventory company info";
            operation.Description = "Registers a new inventory company with its details and QR code.";
            return operation;
        })
        .Produces<IResult<int>>(StatusCodes.Status200OK);

        app.MapPut("/inventory-company-info/{invCompanyInfoId}", async (HttpRequest request, int invCompanyInfoId, IMediator mediator) =>
        {
            var form = await request.ReadFormAsync();

            var qrCode = form.Files["QrCode"];
            byte[]? qrCodeData = qrCode is { Length: > 0 } ? await GetBytes(qrCode) : null;

            var command = new UpdateInventoryCompanyInfoCommand(
                invCompanyInfoId, form["inventoryCompanyInfoName"]!, form["description"]!, form["address"]!,
                form["mobileNo"]!, form["gstNumber"]!, form["apiVersion"]!, form["uiVersion"]!, qrCodeData!,
                form["email"]!, form["bankName"]!, form["bankBranchName"]!, form["bankAccountNo"]!, form["bankBranchIFSC"]!
            );

            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithOpenApi(operation =>
        {
            operation.Summary = "Update inventory company info";
            operation.Description = "Modifies details of an existing inventory company.";
            return operation;
        })
        .Produces<IResult<bool>>(StatusCodes.Status200OK);

        app.MapGet("/inventory-company-info/{invCompanyInfoId}", async (int invCompanyInfoId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetInventoryCompanyInfoQuery(invCompanyInfoId));
            return Results.Ok(result);
        })
        .WithName("GetInvCompanyInfo")
        .WithOpenApi(operation =>
        {
            operation.Summary = "Get inventory company info";
            operation.Description = "Retrieves company details for a given company ID.";
            return operation;
        })
        .Produces<IResult<GetInventoryCompanyInfoQueryResponse>>(StatusCodes.Status200OK);

        return app;
    }

    private static async Task<byte[]> GetBytes(IFormFile file)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        return ms.ToArray();
    }
}
