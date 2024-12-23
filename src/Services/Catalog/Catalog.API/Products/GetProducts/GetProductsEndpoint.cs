
using Catalog.API.Products.CreateProduct;
using Mapster;

namespace Catalog.API.Products.GetProducts;



public record GetProductsResponse(IEnumerable<Product> Products);
public class GetProductsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
         app.MapGet("/products", async (ISender sender) =>
         {
             var result = await sender.Send(new GetProductsQuery());

             var products = app.Adapt<GetProductsResponse>();

             return Results.Ok(products); 
         }).WithName("GetProducts")
            .Produces<CreateProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(statusCode: StatusCodes.Status400BadRequest)
            .WithSummary("Get Products")
            .WithDescription("Get Products");

    }
}

