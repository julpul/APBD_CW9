using Tutorial9.Model;


namespace Tutorial9.Services;


public interface IWarehouseervices
{
    Task<int> AddProduct(ProductDTO product);
    Task<int> AddProductProcedure(ProductDTO product);
}