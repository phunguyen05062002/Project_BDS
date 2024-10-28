using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_BDS.Application.ImplementService;
using Project_BDS.Application.InterfaceService;
using Project_BDS.Application.Payloads.RequestModels.ProductRequests;

namespace Project_BDS.Api.Controllers
{
    [Route(Application.Constants.Constant.DefaultValue.DEFAULT_CONTROLLER_ROUTE)]
    [ApiController]
    public class ProductImgController : ControllerBase
    {
        private readonly IProductImgService _productImgService;

        public ProductImgController(IProductImgService productImgService)
        {
            _productImgService = productImgService;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddProductImage([FromBody] ProductImgRequest request)
        {
            var response = await _productImgService.AddProductImageAsync(request);
            if (response.Status == StatusCodes.Status200OK)
                return Ok(response);
            return StatusCode(response.Status, response);
        }

        [HttpGet("{productId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetProductImages(int productId)
        {
            var response = await _productImgService.GetProductImagesAsync(productId);
            return StatusCode(response.Status, response);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllProductImages()
        {
            var response = await _productImgService.GetAllProductImagesAsync();
            return StatusCode(response.Status, response);
        }

        [HttpPut("{imageId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateProductImage(int imageId, [FromBody] ProductImgRequest request)
        {
            var response = await _productImgService.UpdateProductImageAsync(imageId, request);
            return StatusCode(response.Status, response);
        }

        [HttpDelete("{imageId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteProductImage(int imageId)
        {
            var response = await _productImgService.DeleteProductImageAsync(imageId);
            return StatusCode(response.Status, response);
        }

    }
}
