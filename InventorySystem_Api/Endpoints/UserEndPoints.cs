using InventorySystem_Api.Request;
using InventorySystem_Application.Customer.GetCustomerQuery;
using InventorySystem_Application.InventoryCompanyInfo.CreateInventoryCompanyInfoCommand;
using InventorySystem_Application.InventoryCompanyInfo.GetInventoryCompanyInfoQuery;
using InventorySystem_Application.InventoryCompanyInfo.UpdateInventoryCompanyInfoCommand;
using InventorySystem_Application.MenuItem.AddOrRemoveUserMenuItemCommand;
using InventorySystem_Application.MenuItem.GetUserMenuQuery;
using InventorySystem_Application.Users.ActiveOrInActiveUserCommand;
using InventorySystem_Application.Users.AddOrRemoveUserRoleCommand;
using InventorySystem_Application.Users.CreateUserCommand;
using InventorySystem_Application.Users.ForgetPasswordCommand;
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
    /// <summary>
    /// Maps the user-related API endpoints.
    /// </summary>
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        // GET: /users - Retrieve all users
        app.MapGet("/users", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetUsersQuery());
            return Results.Ok(result);
        })
        .WithName("GetAllUsers")
        .WithOpenApi()
        .Produces(200);

        app.MapGet("/user/{userId}", async (int userId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetUserQuery(userId));
            return Results.Ok(result);
        })
            .WithName("GetUser")
            .WithOpenApi()
            .Produces(200);

        // POST: /user - Create a new user
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
        }).RequireAuthorization("SuperAdminOnly")
        .WithName("CreateUser")
        .WithOpenApi()
        .Produces(200)
        .Produces(400);


        app.MapPost("/login", async ([FromBody] LoginRequest request, IMediator mediator) =>
        {
            var command = new LoginCommand(request.UserName, request.Password);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        }).WithMetadata(new AllowAnonymousAttribute())
         .WithName("Login")
         .WithOpenApi()
         .Produces(200)
         .Produces(400);

        // PUT: /user/{userId} - Update user details (including optional image)
        app.MapPut("/user/{userId}", async (HttpRequest request, int userId, IMediator mediator) =>
        {
            var form = await request.ReadFormAsync();

            string firstName = form["FirstName"]!;
            string lastName = form["LastName"]!;
            string email = form["Email"]!;
            string mobileNo = form["MobileNo"]!;
            IFormFile? imageFile = form.Files.GetFile("Image");

            byte[]? imageData = null;
            if (imageFile is { Length: > 0 })
            {
                using var ms = new MemoryStream();
                await imageFile.CopyToAsync(ms);
                imageData = ms.ToArray();
            }

            var command = new UpdateUserCommand(
                userId,
                firstName,
                lastName,
                email,
                imageData,
                mobileNo
            );

            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("UpdateUser")
        .WithOpenApi()
        .Produces(200)
        .Produces(400);

        // PUT: /user/{userId}/status/{isActive} - Set user active/inactive
        app.MapPut("/user/{userId}/status/{isActive}", async (int userId, bool isActive, IMediator mediator) =>
        {
            var command = new ActiveOrInActiveUserCommand(userId, isActive);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("UpdateUserStatus")
        .WithOpenApi()
        .Produces(200)
        .Produces(400);

        // PUT: /user/{userId}/role/{roleId} - Add or remove a user role
        app.MapPut("/user/{userId}/role/{roleId}", async (int userId, int roleId, IMediator mediator) =>
        {
            var command = new AddOrRemoveUserRoleCommand(userId, roleId);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("AddOrRemoveUserRole")
        .WithOpenApi()
        .Produces(200)
        .Produces(400);


        app.MapPost("/inventory-company-info", async (HttpRequest request, IMediator mediator) =>
        {
            var form = await request.ReadFormAsync();

            var inventoryCompanyInfoName = form["inventoryCompanyInfoName"];
            string? description = form["description"];
            var address = form["address"];
            var email = form["email"];
            var mobileNo = form["mobileNo"];
            var gstNumber = form["gstNumber"];
            var apiVersion = form["apiVersion"];
            var uiVersion = form["uiVersion"];
            var bankName = form["bankName"];
            var bankBranchName = form["bankBranchName"];
            var bankAccountNo = form["bankAccountNo"];
            var bankBranchIFSC = form["bankBranchIFSC"];

            IFormFile? qrCodeFile = form.Files["QrCode"];
            byte[]? qrCodeData = null;

            if (qrCodeFile is { Length: > 0 })
            {
                using var ms = new MemoryStream();
                await qrCodeFile.CopyToAsync(ms);
                qrCodeData = ms.ToArray();
            }

            var command = new CreateInventoryCompanyInfoCommand(
                inventoryCompanyInfoName!, description!, address!, mobileNo!, gstNumber!, apiVersion!, uiVersion!, qrCodeData!,
                email!, bankName!, bankBranchName!, bankAccountNo!, bankBranchIFSC!
            );
            var result = await mediator.Send(command);
            return Results.Ok(result);
        });

        app.MapPut("/inventory-company-info/{invCompanyInfoId}", async (
                HttpRequest request,
                int invCompanyInfoId,
                IMediator mediator) =>
        {
            var form = await request.ReadFormAsync();
            string? inventoryCompanyInfoName = form["inventoryCompanyInfoName"];
            string? description = form["description"];
            string? address = form["address"];
            string? email = form["email"];
            string? mobileNo = form["mobileNo"];
            string? gstNumber = form["gstNumber"];
            string? apiVersion = form["apiVersion"];
            string? uiVersion = form["uiVersion"];
            string? bankName = form["bankName"];
            string? bankBranchName = form["bankBranchName"];
            string? bankAccountNo = form["bankAccountNo"];
            string? bankBranchIFSC = form["bankBranchIFSC"];

            // Handle file upload (QRCode)
            byte[]? qrCodeData = null;
            var qrCodeFile = form.Files["QrCode"];
            if (qrCodeFile is { Length: > 0 })
            {
                using var ms = new MemoryStream();
                await qrCodeFile.CopyToAsync(ms);
                qrCodeData = ms.ToArray();
            }

            // Construct command
            var command = new UpdateInventoryCompanyInfoCommand(invCompanyInfoId, inventoryCompanyInfoName!, description!, address!,
                mobileNo!, gstNumber!, apiVersion!, uiVersion!, qrCodeData!, email!, bankName!,
                bankBranchName!, bankAccountNo!, bankBranchIFSC!);

            var result = await mediator.Send(command);
            return Results.Ok(result);
        });

        app.MapGet("/inventory-company-info/{invCompanyInfoId}", async (int invCompanyInfoId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetInventoryCompanyInfoQuery(invCompanyInfoId));
            return Results.Ok(result);
        })
        .WithName("GetInvCompanyInfo")
        .WithOpenApi()
        .Produces(200);

        app.MapPut("/change-password/{userId}", async (
                int userId,
                [FromBody] ChangePasswordRequest request,
                IMediator mediator) =>
        {
            var command = new PasswordChangeCommand(userId, request.CurrentPassword, request.PasswordHash);

            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
            .WithName("Changepassword")
            .WithOpenApi()
            .Produces(200)
            .Produces(400);

        app.MapPut("/forget-password/{userId}/mobile/{mobileNo}", async (
        int userId,string mobileNo,
        IMediator mediator) =>
        {
            var command = new ForgetPasswordCommand(userId,mobileNo,"");
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
            .WithName("fortgetpassword")
            .WithOpenApi()
            .Produces(200)
            .Produces(400);

        app.MapPut("/user-menu/{userId}/menu/{menuId}", async (
              int userId, int menuId,
              IMediator mediator) =>
        {
            var command = new AddOrRemoveUserMenuItemCommand(userId, menuId);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
          .WithName("usermenu")
          .WithOpenApi()
          .Produces(200)
          .Produces(400);

        app.MapGet("/menu/{userId}", async (int userId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetUserMenuQuery(userId));
            return Results.Ok(result);
        })
       .WithName("GetUserMenu")
       .WithOpenApi()
       .Produces(200);

        app.MapGet("/customers", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetCustomerQuery());
            return Results.Ok(result);
        })
        .WithName("GetCustomers")
        .WithOpenApi()
        .Produces(200);

        return app;
    }
}