using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TicketSystem.Business.IServices;
using TicketSystem.Business.Services;
using TicketSystem.Common.CustomAttributes;
using TicketSystem.DataAccess.DTOs;
using TicketSystem.DataAccess.Models;

namespace TicketSystemWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetProducts")]
        public async Task<IActionResult> GetProducts()
        {
            var response = await _productService.GetAllProductsAsync();
            _logger.LogDebug($"ProductController-GetProducts Request=None / Response={JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }

        [HttpGet]
        [Route("GetProduct/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var response = await _productService.GetProductByIdAsync(id);
            _logger.LogDebug($"ProductController-GetProduct Request=ProductId:{id} / Response={JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }

        [HttpPost]
        [CustomAuthorizeAttribute([UserType.Manager])]
        [Route("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] PostProductDto productDto)
        {
            var response = await _productService.CreateProductAsync(productDto);
            _logger.LogDebug($"ProductController-CreateProduct Request={JsonConvert.SerializeObject(productDto)} / Response={JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }

        [HttpPost]
        [CustomAuthorizeAttribute([UserType.Manager])]
        [Route("UpdateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct([FromBody] PostProductDto productDto , int id)
        {
            var response = await _productService.UpdateProductAsync(productDto, id);
            _logger.LogDebug($"ProductController-UpdateProduct Request={JsonConvert.SerializeObject(productDto)} / Response={JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }

        [HttpPost]
        [CustomAuthorizeAttribute([UserType.Manager])]
        [Route("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var response = await _productService.DeleteProductAsync(id);
            _logger.LogDebug($"ProductController-DeleteProduct Request=ProductId:{id} / Response={JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }
        [HttpGet]
        [Route("GetProductLookup")]
        public async Task<IActionResult> GetProductLookup()
        {
            var response = await _productService.ProducsLookUp();
            _logger.LogDebug($"ProductController-GetProductLookup Request=None / Response={JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }

    }
}
