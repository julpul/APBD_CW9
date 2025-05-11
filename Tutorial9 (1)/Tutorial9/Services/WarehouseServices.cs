using System.Data;
using Microsoft.Data.SqlClient;
using Tutorial9.Model;


namespace Tutorial9.Services;


public class WarehouseServices(IConfiguration configuration):IWarehouseervices
{
    public async Task<int> AddProduct(ProductDTO product)
{
    if (product.Amount <= 0)
    {
        throw new Exception("Ilość musi być większa niż 0");
    }

    await using var conn = new SqlConnection(configuration.GetConnectionString("Default"));
    await using var cmd = new SqlCommand("", conn);
    
    await conn.OpenAsync();
    
    var transaction = await conn.BeginTransactionAsync();
    cmd.Transaction = transaction as SqlTransaction;

    try
    {
        cmd.CommandText = @"SELECT COUNT(*) FROM Product WHERE IdProduct = @IdProduct";
        cmd.Parameters.AddWithValue("@IdProduct", product.IdProduct);
        int c = (int)await cmd.ExecuteScalarAsync();
        if (c <= 0)
        {
            throw new Exception($"Produkt {product.IdProduct} nie istnieje");
        }
        
        cmd.CommandText = @"SELECT COUNT(*) FROM Warehouse WHERE IdWarehouse = @IdWarehouse";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@IdWarehouse", product.IdWarehouse);
        c = (int)await cmd.ExecuteScalarAsync();
        if (c <= 0)
        {
            throw new Exception($"Magazyn {product.IdWarehouse} nie istnieje");
        }
        
        cmd.CommandText = @"
            SELECT IdOrder
            FROM [Order]
            WHERE IdProduct = @IdProduct
              AND Amount = @Amount
              AND CreatedAt < @CreatedAt";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@IdProduct", product.IdProduct);
        cmd.Parameters.AddWithValue("@Amount", product.Amount);
        cmd.Parameters.AddWithValue("@CreatedAt", product.CreatedAt);
        var idOrder = await cmd.ExecuteScalarAsync();
        if (idOrder == null)
        {
            throw new Exception($"Zamówienie dla produktu o ID: {product.IdProduct} nie istnieje");
        }

        // Sprawdzenie, czy zamówienie nie zostało już zrealizowane
        cmd.CommandText = @"
            SELECT COUNT(*)
            FROM Product_Warehouse
            WHERE IdOrder = @IdOrder";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@IdOrder", idOrder);
        c = (int)await cmd.ExecuteScalarAsync();
        if (c > 0)
        {
            throw new Exception($"Zamówienie dla produktu {product.IdProduct} zostało już zrealizowane");
        }

        //Aktualizacja daty realizacji zamówienia
        cmd.CommandText = @"
            UPDATE [Order]
            SET FulfilledAt = @FulfilledAt
            WHERE IdOrder = @IdOrder";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@IdOrder", idOrder);
        cmd.Parameters.AddWithValue("@FulfilledAt", DateTime.Now);
        await cmd.ExecuteNonQueryAsync();

        //Pobranie ceny produktu
        cmd.CommandText = @"
            SELECT Price
            FROM Product
            WHERE IdProduct = @IdProduct";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@IdProduct", product.IdProduct);
        decimal price = (decimal)await cmd.ExecuteScalarAsync();

        // Wstawienie rekordu do Product_Warehouse
        cmd.CommandText = @"
            INSERT INTO Product_Warehouse(IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)
            VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() as int);";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@IdWarehouse", product.IdWarehouse);
        cmd.Parameters.AddWithValue("@IdProduct", product.IdProduct);
        cmd.Parameters.AddWithValue("@IdOrder", idOrder);
        cmd.Parameters.AddWithValue("@Amount", product.Amount);
        cmd.Parameters.AddWithValue("@Price", price * product.Amount);
        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

        int result = (int)await cmd.ExecuteScalarAsync();

        await transaction.CommitAsync();
        return result;
    }
    catch (Exception)
    {
        await transaction.RollbackAsync();
        throw;
    }
}



    public async Task<int> AddProductProcedure(ProductDTO product)
    {
        string command = "AddProductToWarehouse";

        await using SqlConnection conn = new SqlConnection(configuration.GetConnectionString("Default"));
        await using SqlCommand cmd = new SqlCommand(command, conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@IdProduct", product.IdProduct);
        cmd.Parameters.AddWithValue("@IdWarehouse", product.IdWarehouse);
        cmd.Parameters.AddWithValue("@Amount", product.Amount);
        cmd.Parameters.AddWithValue("@CreatedAt", product.CreatedAt);

        await conn.OpenAsync();
        int result = (int)await cmd.ExecuteScalarAsync();

        return result;
    }
}