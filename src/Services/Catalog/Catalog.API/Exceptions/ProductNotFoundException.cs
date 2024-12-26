namespace Catalog.API.Exceptions;
public class ProductNotFoundException : Exception
{
    public ProductNotFoundException() : base("Ops!! Product not found!")    {    }

}

