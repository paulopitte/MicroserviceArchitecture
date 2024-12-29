

using Mapster;

namespace Catalog.API.Security;


public record SecurityCipherRequest(string Pass);
 

public class SecurityEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/security/cipher", 
            async (SecurityCipherRequest request, ISender sender) =>
        {
            var command = request.Adapt<SecurityCipherCommand>();

            var result = await sender.Send(command);      

            return Results.Ok(result.PassCipher);

        })
    .WithName("cipher")
    .Produces<string>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .WithSummary("cipher")
    .WithDescription("cipher");
    }
}

