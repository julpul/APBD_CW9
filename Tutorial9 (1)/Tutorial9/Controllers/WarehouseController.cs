using Microsoft.AspNetCore.Mvc;
using Tutorial9.Model;


namespace Tutorial9.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehouseController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public WarehouseController(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    public IActionResult Get([FromBody] ProductDTO product)
    {
        
        
        return Ok(product);
    }
    
}