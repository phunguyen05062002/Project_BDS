using Microsoft.AspNetCore.Http;
using Project_BDS.Application.InterfaceService;
using Project_BDS.Application.Payloads.RequestModels.ProductRequests;
using Project_BDS.Application.Payloads.Responses;
using Project_BDS.Domain.Entities;
using Project_BDS.Domain.InterfaceRepostories;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project_BDS.Application.ImplementService
{
    public class ProductImgService : IProductImgService
    {
        private readonly IProductImgRepository _productImgRepository;
        private readonly IBaseRepository<User> _baseUserRepository;
        private readonly IProductRepository _productRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductImgService(IProductImgRepository productImgRepository, IBaseRepository<User> baseUserRepository, IHttpContextAccessor httpContextAccessor, IProductRepository productRepository)
        {
            _productImgRepository = productImgRepository;
            _baseUserRepository = baseUserRepository;
            _httpContextAccessor = httpContextAccessor;
            _productRepository = productRepository;
        }

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

        public async Task<ResponseObject<bool>> AddProductImageAsync(ProductImgRequest request)
        {
            var validationResponse = await ValidateCurrentUser<bool>();
            if (validationResponse.Response != null)
            {
                return validationResponse.Response;
            }

            // Kiểm tra xem ProductId có tồn tại không
            var productExists = await _productRepository.ExistsAsync(request.ProductId);
            if (!productExists)
            {
                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Sản phẩm không tồn tại.",
                    Data = false
                };
            }

            try
            {
                var productImg = new ProductImg
                {
                    ProductId = request.ProductId,
                    LinkImg = request.LinkImg,
                    Description = request.Description
                };

                await _productImgRepository.AddAsync(productImg);

                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Thêm ảnh sản phẩm thành công.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết nếu cần
                var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = $"Có lỗi xảy ra: {errorMessage}",
                    Data = false
                };
            }
        }

        public async Task<ResponseObject<List<ProductImg>>> GetProductImagesAsync(int productId)
        {
            var validationResponse = await ValidateCurrentUser<List<ProductImg>>();
            if (validationResponse.Response != null)
            {
                return validationResponse.Response;
            }

            try
            {
                var images = await _productImgRepository.GetByProductIdAsync(productId);
                return new ResponseObject<List<ProductImg>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Lấy danh sách ảnh sản phẩm thành công.",
                    Data = images
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<List<ProductImg>>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = $"Có lỗi xảy ra: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<List<ProductImg>>> GetAllProductImagesAsync()
        {
            var validationResponse = await ValidateCurrentUser<List<ProductImg>>();
            if (validationResponse.Response != null)
            {
                return validationResponse.Response;
            }

            try
            {
                var images = await _productImgRepository.GetAllAsync();
                return new ResponseObject<List<ProductImg>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Lấy tất cả ảnh sản phẩm thành công.",
                    Data = images
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<List<ProductImg>>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = $"Có lỗi xảy ra: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ResponseObject<bool>> UpdateProductImageAsync(int imageId, ProductImgRequest request)
        {
            var validationResponse = await ValidateCurrentUser<bool>();
            if (validationResponse.Response != null)
            {
                return validationResponse.Response;
            }

            try
            {
                var productImg = await _productImgRepository.GetByIdAsync(imageId);
                if (productImg == null)
                {
                    return new ResponseObject<bool>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Không tìm thấy ảnh sản phẩm.",
                        Data = false
                    };
                }

                productImg.LinkImg = request.LinkImg;
                productImg.Description = request.Description;

                await _productImgRepository.UpdateAsync(productImg);

                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Cập nhật ảnh sản phẩm thành công.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = $"Có lỗi xảy ra: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<ResponseObject<bool>> DeleteProductImageAsync(int imageId)
        {
            var validationResponse = await ValidateCurrentUser<bool>();
            if (validationResponse.Response != null)
            {
                return validationResponse.Response;
            }

            try
            {
                var productImg = await _productImgRepository.GetByIdAsync(imageId);
                if (productImg == null)
                {
                    return new ResponseObject<bool>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Không tìm thấy ảnh sản phẩm.",
                        Data = false
                    };
                }

                await _productImgRepository.DeleteAsync(productImg);

                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Xóa ảnh sản phẩm thành công.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ResponseObject<bool>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = $"Có lỗi xảy ra: {ex.Message}",
                    Data = false
                };
            }
        }
    }
}
