using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_BDS.Application.InterfaceService;
using Project_BDS.Application.Payloads.RequestModels.ProductRequests;

namespace Project_BDS.Api.Controllers
{
    [Route(Application.Constants.Constant.DefaultValue.DEFAULT_CONTROLLER_ROUTE)]
    [ApiController]
    public class ProductTypeController : ControllerBase
    {
        private readonly IProductTypeService _productTypeService;
        public ProductTypeController(IProductTypeService productTypeService)
        {
            _productTypeService = productTypeService;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddProductType([FromBody] Request_ProductType request)
        {
            var response = await _productTypeService.AddProductTypeAsync(request);
            if (response.Status != StatusCodes.Status200OK)
            {
                return StatusCode(response.Status, response.Message);
            }

            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateProductType(int id, [FromBody] Request_ProductType request)
        {
            var response = await _productTypeService.UpdateProductTypeAsync(id, request);
            if (response.Status != StatusCodes.Status200OK)
            {
                return StatusCode(response.Status, response.Message);
            }

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteProductType(int id)
        {
            var response = await _productTypeService.DeleteProductTypeAsync(id);
            if (response.Status != StatusCodes.Status200OK)
            {
                return StatusCode(response.Status, response.Message);
            }

            return Ok(response);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllProductTypes()
        {
            var response = await _productTypeService.GetAllProductTypesAsync();
            if (response.Status != StatusCodes.Status200OK)
            {
                return StatusCode(response.Status, response.Message);
            }

            return Ok(response);
        }
    }
}
