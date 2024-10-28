using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_BDS.Application.ImplementService;
using Project_BDS.Application.InterfaceService;
using Project_BDS.Application.Payloads.RequestModels.ProductRequests;
using Project_BDS.Application.Payloads.Response_Models.DataProduct;
using Project_BDS.Application.Payloads.Responses;

namespace Project_BDS.Api.Controllers
{
    [Route(Application.Constants.Constant.DefaultValue.DEFAULT_CONTROLLER_ROUTE)]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }
        // Thêm sản phẩm
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ResponseObject<DataResponseProduct>>> AddProduct([FromBody] RequestProduct requestProduct)
        {
            var response = await _productService.AddProductAsync(requestProduct);
            return StatusCode(response.Status, response);
        }

        // Sửa sản phẩm
        [HttpPut("{productId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ResponseObject<DataResponseProduct>>> UpdateProduct(int productId, [FromBody] RequestProduct requestProduct)
        {
            var response = await _productService.UpdateProductAsync(productId, requestProduct);
            return StatusCode(response.Status, response);
        }

        // Xóa sản phẩm
        [HttpDelete("{productId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ResponseObject<bool>>> DeleteProduct(int productId)
        {
            var response = await _productService.DeleteProductAsync(productId);
            return StatusCode(response.Status, response);
        }

        // Lấy danh sách sản phẩm
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllProducts()
        {
            var response = await _productService.GetAllProductsAsync();
            return StatusCode(response.Status, response);
        }

        // Lấy sản phẩm theo ID
        [HttpGet("{productId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ResponseObject<DataResponseProduct>>> GetProductById(int productId)
        {
            var response = await _productService.GetProductByIdAsync(productId); 
            return StatusCode(response.Status, response);
        }
        [HttpGet("searchByType")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SearchProductsByType(int typeId, int pageIndex = 1, int pageSize = 10)
        {
            var response = await _productService.SearchProductsByTypeAsync(typeId, pageIndex, pageSize);
            return StatusCode(response.Status, response);
        }

        [HttpGet("searchByPrice")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SearchProductsByPrice(double minPrice, double maxPrice, int pageIndex = 1, int pageSize = 10)
        {
            var response = await _productService.SearchProductsByPriceAsync(minPrice, maxPrice, pageIndex, pageSize);
            return StatusCode(response.Status, response);
        }

        [HttpGet("searchByAddress")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SearchProductsByAddress(string address, int pageIndex = 1, int pageSize = 10)
        {
            var response = await _productService.SearchProductsByAddressAsync(address, pageIndex, pageSize);
            return StatusCode(response.Status, response);
        }

        [HttpGet("searchProductsByStartSelling")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SearchProductsByStartSelling(int? startYear = null, int? endYear = null, int? startMonth = null, int? endMonth = null, int pageIndex = 1, int pageSize = 10)
        {
            var response = await _productService.SearchProductsByStartSellingAsync(startYear, endYear, startMonth, endMonth, pageIndex, pageSize);
            return StatusCode(response.Status, response);
        }
       
        [HttpGet("GetTotalPriceByType")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetTotalPriceByType()
        {
            var result = await _productService.GetTotalPriceByTypeAsync();
            return Ok(result);
        }
    }
}
