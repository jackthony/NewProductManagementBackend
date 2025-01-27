using Microsoft.AspNetCore.Mvc;
using ProductManagement.Application.DTOs;
using ProductManagement.Application.Services;

namespace ProductManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _service;

    public ProductsController(ProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _service.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);
        if (product == null) return NotFound();

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductRequestDto productRequestDto)
    {
        var productResponse = await _service.AddAsync(productRequestDto);
        return CreatedAtAction(nameof(GetById), new { id = productResponse.Id }, productResponse);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProductRequestDto productRequestDto)
    {
        await _service.UpdateAsync(id, productRequestDto);
        return NoContent(); // Devuelve solo el código de estado 204
    }



    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
