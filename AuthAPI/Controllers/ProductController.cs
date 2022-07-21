using Auth.Core.DTOs;
using Auth.Core.Model;
using Auth.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : CustomBaseController
    {
        private readonly IServiceGeneric<Product, ProductDto> _productService;

        public ProductController(IServiceGeneric<Product, ProductDto> productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct()
        {
            return ActionResultInstance(await _productService.GetAllAsync());
        }
        [HttpPost]
        public async Task<IActionResult> SaveProduct(ProductDto product)
        {
            return ActionResultInstance(await _productService.AddAsync(product));
        }
        [HttpPut]
        public async Task<IActionResult> UpdateProduct(ProductDto product)
        {
            return ActionResultInstance(await _productService.Update(product, product.Id));
        }
        //api/prodct/2
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            return ActionResultInstance(await _productService.Remove(id));
        }
    }
}
