namespace Catalog.API.Security;
public record SecurityDeCipherRequest(string Pass);

public class SecurityDeCipherEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {  
        app.MapPost("/security/decipher",
           async (SecurityDeCipherRequest request, ISender sender) =>
           {
               var command = request.Adapt<SecurityDeCipherCommand>();

               var result = await sender.Send(command);

               return Results.Ok(result.PassCipher);

           })
   .WithName("decipher")
   .Produces<string>(StatusCodes.Status200OK)
   .ProducesProblem(StatusCodes.Status400BadRequest)
   .WithSummary("decipher")
   .WithDescription("decipher");
    }
}

