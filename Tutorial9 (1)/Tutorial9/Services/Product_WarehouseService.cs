using Microsoft.Data.SqlClient;
using Tutorial9.Model;

namespace Tutorial9.Services;

public class Product_WarehouseService : IProduct_WarehouseService
{
    private readonly IConfiguration _configuration;

    public Product_WarehouseService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<int> addProduct_Warehouse(ProductDTO product)
    {
        using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var command = new SqlCommand();
        await connection.OpenAsync();
        
        var transaction = connection.BeginTransaction();
        command.Transaction = transaction;
        try
        {
            command.CommandText = @"SELECT COUNT(*) FROM Product where ProductID=@ProductID";
            command.Parameters.AddWithValue(@"@ProductID",product.IdProduct);            
            
            int count = (int)await command.ExecuteScalarAsync();
            if (count <= 0)
            {
                throw new Exception("Product not found");
            }
            
            
            
            
            return 0;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<ProductDTO>> addProduct_Warehouse(ProductDTO product)
    {
        List<ProductDTO> products = new List<ProductDTO>();
        
        using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var command = new SqlCommand();
        {
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();
            command.Transaction = transaction;
            
            command.CommandText = @"SELECT * FROM Product";

            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    products.Add(new ProductDTO
                    {
                        IdProduct = reader.GetInt32(reader.GetOrdinal("IdProduct")),
                        IdWarehouse = reader.GetInt32(reader.GetOrdinal("IdWarehouse")),
                        Amount = reader.GetInt32(reader.GetOrdinal("Amount")),
                        DateTime = reader.GetDateTime(reader.GetOrdinal("DateTime")),
                    });
                }
            }
        }
        
    }
        
}