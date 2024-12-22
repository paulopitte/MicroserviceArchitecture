using MediatR;

namespace Catalog.API.Products.CreateProduct;



public record CreateProductCommand(string name, List<string> category, string description, string imageFile, decimal price)
    : IRequest<CreateProductResult>;
public record CreateProductResult(Guid id);




internal sealed class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

