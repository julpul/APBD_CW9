using Tutorial9.Model;

namespace Tutorial9.Services;

public interface IProduct_WarehouseService
{
    Task<int> addProduct_Warehouse(ProductDTO product);
    
    
}