using Microsoft.AspNetCore.Mvc;
using Tutorial9.Services;
using Tutorial9.Model;

namespace Tutorial9.Controllers;
[ApiController]
[Route("api/[controller]")]
public class WarehouseController:ControllerBase
{
    private readonly IWarehouseervices _warehouseervices;

    public WarehouseController(IWarehouseervices warehouseervices)
    {
        _warehouseervices = warehouseervices;
    }

    [HttpPost]
    public async Task<IActionResult> AddToWarehouse([FromBody] ProductDTO product)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("product data is wrong format");
        }
        try
        {
            var id = await _warehouseervices.AddProduct(product);
            return Ok(id);

        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [HttpPost("Procedure")]
    public async Task<IActionResult> AddProductToWarehouseProcedure([FromBody] ProductDTO product)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("product data is wrong format");
        }

        try
        {
            var result = await _warehouseervices.AddProductProcedure(product);
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
}