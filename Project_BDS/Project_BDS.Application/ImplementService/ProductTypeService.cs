using Microsoft.AspNetCore.Http;
using Project_BDS.Application.InterfaceService;
using Project_BDS.Application.Payloads.Mappers;
using Project_BDS.Application.Payloads.RequestModels.ProductRequests;
using Project_BDS.Application.Payloads.Response_Models.DataProduct;
using Project_BDS.Application.Payloads.Responses;
using Project_BDS.Domain.Entities;
using Project_BDS.Domain.InterfaceRepostories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Application.ImplementService
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly IProductTypeRepository _productTypeRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBaseRepository<User> _baseUserRepository;

        public ProductTypeService(IProductTypeRepository productTypeRepository, IHttpContextAccessor httpContextAccessor, IBaseRepository<User> baseUserRepository)
        {
            _productTypeRepository = productTypeRepository;
            _httpContextAccessor = httpContextAccessor;
            _baseUserRepository = baseUserRepository;
        }

        private async Task<ResponseObject<T>> ValidateCurrentUser<T>()
        {
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                return new ResponseObject<T>
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Người dùng chưa được xác thực!",
                    Data = default(T)
                };
            }

            var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return new ResponseObject<T>
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Không tìm thấy thông tin UserId trong token!",
                    Data = default(T)
                };
            }

            var currentUserId = int.Parse(userIdClaim.Value);
            var currentUserInfo = await _baseUserRepository.GetByIdAsync(currentUserId);
            if (currentUserInfo == null)
            {
                return new ResponseObject<T>
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Không tìm thấy thông tin người dùng trong hệ thống!",
                    Data = default(T)
                };
            }

            var currentUserRoleId = currentUserInfo.RoleId;

            if (currentUserRoleId != 2)
            {
                return new ResponseObject<T>
                {
                    Status = StatusCodes.Status403Forbidden,
                    Message = "Bạn không có quyền truy cập vào chức năng này.",
                    Data = default(T)
                };
            }

            return null;
        }

        public async Task<ResponseObject<DataResponse_ProductType>> AddProductTypeAsync(Request_ProductType request)
        {
            var validationResponse = await ValidateCurrentUser<DataResponse_ProductType>();
            if (validationResponse != null)
            {
                return validationResponse;
            }

            var productType = ProductTypeConverter.DTOToEntity(request);
            await _productTypeRepository.AddAsync(productType);
            await _productTypeRepository.SaveChangesAsync();

            var responseProductType = ProductTypeConverter.EntityToDTO(productType);

            return new ResponseObject<DataResponse_ProductType>
            {
                Status = StatusCodes.Status200OK,
                Message = "Thêm loại sản phẩm thành công!",
                Data = responseProductType
            };
        }

        public async Task<ResponseObject<DataResponse_ProductType>> UpdateProductTypeAsync(int id, Request_ProductType request)
        {
            var validationResponse = await ValidateCurrentUser<DataResponse_ProductType>();
            if (validationResponse != null)
            {
                return validationResponse;
            }

            var productType = await _productTypeRepository.GetByIdAsync(id);
            if (productType == null)
            {
                return new ResponseObject<DataResponse_ProductType>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Không tìm thấy loại sản phẩm!",
                    Data = null
                };
            }

            productType.Code = request.Code;
            productType.Name = request.Name;

            await _productTypeRepository.UpdateAsync(productType);
            await _productTypeRepository.SaveChangesAsync();

            var responseProductType = ProductTypeConverter.EntityToDTO(productType);

            return new ResponseObject<DataResponse_ProductType>
            {
                Status = StatusCodes.Status200OK,
                Message = "Cập nhật loại sản phẩm thành công!",
                Data = responseProductType
            };
        }

        public async Task<ResponseObject<bool>> DeleteProductTypeAsync(int id)
        {
            var validationResponse = await ValidateCurrentUser<bool>();
            if (validationResponse != null)
            {
                return validationResponse;
            }

            var productType = await _productTypeRepository.GetByIdAsync(id);
            if (productType == null)
            {
                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Không tìm thấy loại sản phẩm!",
                    Data = false
                };
            }

            await _productTypeRepository.DeleteAsync(productType);
            await _productTypeRepository.SaveChangesAsync();

            return new ResponseObject<bool>
            {
                Status = StatusCodes.Status200OK,
                Message = "Xóa loại sản phẩm thành công!",
                Data = true
            };
        }

        public async Task<ResponseObject<IEnumerable<DataResponse_ProductType>>> GetAllProductTypesAsync()
        {
            var validationResponse = await ValidateCurrentUser<IEnumerable<DataResponse_ProductType>>();
            if (validationResponse != null)
            {
                return validationResponse;
            }

            var productTypes = await _productTypeRepository.GetAllAsync();
            var responseProductTypes = productTypes.Select(ProductTypeConverter.EntityToDTO);

            return new ResponseObject<IEnumerable<DataResponse_ProductType>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Lấy danh sách loại sản phẩm thành công!",
                Data = responseProductTypes
            };
        }
    }
}
