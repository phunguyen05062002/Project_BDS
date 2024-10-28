using Microsoft.AspNetCore.Http;
using Project_BDS.Application.Helper;
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
    public class ProductService : IProductService
    {
        private readonly IBaseRepository<User> _baseUserRepository;
        private readonly IProductRepository _productRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductService(IHttpContextAccessor httpContextAccessor, IBaseRepository<User> baseUserRepository, IProductRepository productRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _baseUserRepository = baseUserRepository;
            _productRepository = productRepository;
        }

        #region Thêm sản phẩm
        public async Task<ResponseObject<DataResponseProduct>> AddProductAsync(RequestProduct requestProduct)
        {
            var (validationResponse, currentUser) = await ValidateCurrentUser<DataResponseProduct>();
            if (validationResponse != null)
            {
                return validationResponse;
            }

            var newProduct = ProductConverter.DTOToEntity(requestProduct);
            newProduct.StartSelling = DateTime.UtcNow;

            await _productRepository.AddAsync(newProduct);

            var responseProduct = ProductConverter.EntityToDTO(newProduct);

            return new ResponseObject<DataResponseProduct>
            {
                Status = StatusCodes.Status201Created,
                Message = "Thêm sản phẩm thành công.",
                Data = responseProduct
            };
        }
        #endregion

        #region Sửa sản phẩm
        public async Task<ResponseObject<DataResponseProduct>> UpdateProductAsync(int productId, RequestProduct requestProduct)
        {
            var (validationResponse, currentUser) = await ValidateCurrentUser<DataResponseProduct>();
            if (validationResponse != null)
            {
                return validationResponse;
            }

            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return new ResponseObject<DataResponseProduct>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Sản phẩm không tồn tại.",
                    Data = null
                };
            }

            ProductConverter.UpdateEntityFromDTO(product, requestProduct);
            product.UpdateTime = DateTime.UtcNow; // Cập nhật thời gian sửa

            await _productRepository.UpdateAsync(product);

            var responseProduct = ProductConverter.EntityToDTO(product);

            return new ResponseObject<DataResponseProduct>
            {
                Status = StatusCodes.Status200OK,
                Message = "Sửa sản phẩm thành công.",
                Data = responseProduct
            };
        }
        #endregion

        #region Xóa sản phẩm
        public async Task<ResponseObject<bool>> DeleteProductAsync(int productId)
        {
            var (validationResponse, currentUser) = await ValidateCurrentUser<bool>();
            if (validationResponse != null)
            {
                return validationResponse;
            }

            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Sản phẩm không tồn tại.",
                    Data = false
                };
            }

            await _productRepository.DeleteAsync(product);

            return new ResponseObject<bool>
            {
                Status = StatusCodes.Status200OK,
                Message = "Xóa sản phẩm thành công.",
                Data = true
            };
        }
        #endregion

        //#region Lấy sản phẩm theo id
        //public async Task<ResponseObject<DataResponseProduct>> GetProductByIdAsync(int productId)
        //{
        //    var product = await _productRepository.GetByIdAsync(productId);
        //    if (product == null)
        //    {
        //        return new ResponseObject<DataResponseProduct>
        //        {
        //            Status = StatusCodes.Status404NotFound,
        //            Message = "Sản phẩm không tồn tại.",
        //            Data = null
        //        };
        //    }

        //    var responseProduct = ProductConverter.EntityToDTO(product);
        //    return new ResponseObject<DataResponseProduct>
        //    {
        //        Status = StatusCodes.Status200OK,
        //        Message = "Lấy sản phẩm thành công.",
        //        Data = responseProduct
        //    };
        //}
        //#endregion

        #region Lấy sản phẩm theo id
        public async Task<ResponseObject<DataResponseProduct>> GetProductByIdAsync(int productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);

            if (product == null)
            {
                return new ResponseObject<DataResponseProduct>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Sản phẩm không tồn tại.",
                    Data = null
                };
            }

            var responseProduct = ProductConverter.EntityToDTO(product);

            return new ResponseObject<DataResponseProduct>
            {
                Status = StatusCodes.Status200OK,
                Message = "Lấy sản phẩm thành công.",
                Data = responseProduct
            };
        }
        #endregion

        #region Lấy danh sách sản phẩm

        public async Task<ResponseObject<IEnumerable<DataResponseProduct>>> GetAllProductsAsync()
        {
            var validationResponse = await ValidateUserForListAccess<IEnumerable<DataResponseProduct>>();
            if (validationResponse != null)
            {
                return validationResponse;
            }

            try
            {
                // Lấy danh sách tất cả sản phẩm
                var productsQuery = await _productRepository.GetAllAsync();

                if (productsQuery == null)
                {
                    return new ResponseObject<IEnumerable<DataResponseProduct>>
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Message = "Dữ liệu sản phẩm không hợp lệ.",
                        Data = null
                    };
                }

                // Chuyển đổi từ entity Product sang DataResponseProduct
                var products = productsQuery
                    .Select(ProductConverter.EntityToDTO)
                    .ToList();

                return new ResponseObject<IEnumerable<DataResponseProduct>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Lấy danh sách sản phẩm thành công.",
                    Data = products
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<IEnumerable<DataResponseProduct>>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = $"Đã xảy ra lỗi: {ex.Message}",
                    Data = null
                };
            }
        }

        #endregion

        #region Tìm kiếm sản phẩm theo loại
        public async Task<ResponseObject<IEnumerable<DataResponseProduct>>> SearchProductsByTypeAsync(int typeId, int pageIndex, int pageSize)
        {
            var products = await _productRepository.SearchByTypeAsync(typeId, pageIndex, pageSize);
            if (!products.Any())
            {
                return new ResponseObject<IEnumerable<DataResponseProduct>>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Không tìm thấy sản phẩm nào thuộc loại này.",
                    Data = null
                };
            }

            var responseProducts = products.Select(ProductConverter.EntityToDTO).ToList();
            return new ResponseObject<IEnumerable<DataResponseProduct>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Tìm kiếm sản phẩm theo loại thành công.",
                Data = responseProducts
            };
        }
        #endregion

        #region Tìm kiếm sản phẩm theo giá
        public async Task<ResponseObject<IEnumerable<DataResponseProduct>>> SearchProductsByPriceAsync(double minPrice, double maxPrice, int pageIndex, int pageSize)
        {
            var products = await _productRepository.SearchByPriceAsync(minPrice, maxPrice, pageIndex, pageSize);
            if (!products.Any())
            {
                return new ResponseObject<IEnumerable<DataResponseProduct>>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Không tìm thấy sản phẩm nào trong khoảng giá này.",
                    Data = null
                };
            }

            var responseProducts = products.Select(ProductConverter.EntityToDTO).ToList();
            return new ResponseObject<IEnumerable<DataResponseProduct>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Tìm kiếm sản phẩm theo giá thành công.",
                Data = responseProducts
            };
        }
        #endregion

        #region Tìm kiếm theo địa chỉ 
        public async Task<ResponseObject<IEnumerable<DataResponseProduct>>> SearchProductsByAddressAsync(string address, int pageIndex, int pageSize)
        {
            var products = await _productRepository.SearchByAddressAsync(address, pageIndex, pageSize);
            if (!products.Any())
            {
                return new ResponseObject<IEnumerable<DataResponseProduct>>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Không tìm thấy sản phẩm nào tại địa chỉ này.",
                    Data = null
                };
            }

            var responseProducts = products.Select(ProductConverter.EntityToDTO).ToList();
            return new ResponseObject<IEnumerable<DataResponseProduct>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Tìm kiếm sản phẩm theo địa chỉ thành công.",
                Data = responseProducts
            };
        }
        #endregion

        #region Tìm kiếm theo thời gian bắt đầu bán
        public async Task<ResponseObject<IEnumerable<DataResponseProduct>>> SearchProductsByStartSellingAsync(int? startYear, int? endYear, int? startMonth, int? endMonth, int pageIndex, int pageSize)
        {
            var products = await _productRepository.SearchByStartSellingAsync(startYear, endYear, startMonth, endMonth, pageIndex, pageSize);
            if (!products.Any())
            {
                return new ResponseObject<IEnumerable<DataResponseProduct>>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Không tìm thấy sản phẩm nào với thời gian bán này.",
                    Data = null
                };
            }

            var responseProducts = products.Select(ProductConverter.EntityToDTO).ToList();
            return new ResponseObject<IEnumerable<DataResponseProduct>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Tìm kiếm sản phẩm theo thời gian bán thành công.",
                Data = responseProducts
            };
        }
        #endregion

        #region Xác thực người dùng hiện tại
        private async Task<(ResponseObject<T> Response, User CurrentUser)> ValidateCurrentUser<T>()
        {
            var currentUser = _httpContextAccessor.HttpContext.User;

            if (!currentUser.Identity.IsAuthenticated)
            {
                return (new ResponseObject<T>
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Người dùng chưa được xác thực!",
                    Data = default(T)
                }, null);
            }

            var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return (new ResponseObject<T>
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Không tìm thấy thông tin UserId trong token!",
                    Data = default(T)
                }, null);
            }

            var currentUserId = int.Parse(userIdClaim.Value);
            var currentUserInfo = await _baseUserRepository.GetByIdAsync(currentUserId);
            if (currentUserInfo == null)
            {
                return (new ResponseObject<T>
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Không tìm thấy thông tin người dùng trong hệ thống!",
                    Data = default(T)
                }, null);
            }

            var currentUserRoleId = currentUserInfo.RoleId;

            // Chỉ cho phép người dùng có RoleId = 2 (Manager)
            if (currentUserRoleId != 2)
            {
                return (new ResponseObject<T>
                {
                    Status = StatusCodes.Status403Forbidden,
                    Message = "Bạn không có quyền thực hiện hành động này.",
                    Data = default(T)
                }, null);
            }

            return (null, currentUserInfo);
        }
        #endregion

        #region Xác thực người dùng hiện tại cho truy cập danh sách
        private async Task<ResponseObject<T>> ValidateUserForListAccess<T>()
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

            return null; // Cho phép tất cả người dùng có quyền truy cập
        }

        #endregion

        public async Task<Dictionary<string, double>> GetTotalPriceByTypeAsync()
        {
            return await _productRepository.GetTotalPriceByTypeAsync();
        }
    }
}
