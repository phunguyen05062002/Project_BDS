using Project_BDS.Application.Payloads.RequestModels.ProductRequests;
using Project_BDS.Application.Payloads.Response_Models.DataProduct;
using Project_BDS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Application.Payloads.Mappers
{
    public class ProductConverter
    {
        public static Product DTOToEntity(RequestProduct request)
        {
            return new Product
            {
                HostName = request.HostName,
                HostPhoneNumber = request.HostPhoneNumber,
                Address = request.Address,
                Title = request.Title,
                Price = request.Price,
                TypeId = request.TypeId,
                StartSelling = request.StartSelling,
                IsActive = request.IsActive,
                // Giả sử RequestProduct chứa danh sách URL hoặc đường dẫn của ảnh
                ProductImgs = request.ProductImgs?.Select(img => new ProductImg
                {
                    LinkImg = img.LinkImg,
                    Description = img.Description
                }).ToList() ?? new List<ProductImg>()
            };
        }

        public static DataResponseProduct EntityToDTO(Product product)
        {
            return new DataResponseProduct
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price,
                HostName = product.HostName,
                HostPhoneNumber = product.HostPhoneNumber,
                Address = product.Address,
                StartSelling = product.StartSelling,
                IsActive = product.IsActive,
                TypeId = product.TypeId,
                ProductImgs = product.ProductImgs.Select(img => new DataResponseProductImg
                {
                    Id = img.Id,
                    ProductId = img.ProductId,
                    LinkImg = img.LinkImg,
                    Description = img.Description
                }).ToList()
            };
        }


        public static void UpdateEntityFromDTO(Product product, RequestProduct request)
        {
            product.HostName = request.HostName;
            product.HostPhoneNumber = request.HostPhoneNumber;
            product.Address = request.Address;
            product.Title = request.Title;
            product.Price = request.Price;
            product.TypeId = request.TypeId;
            product.StartSelling = request.StartSelling;
            product.IsActive = request.IsActive;
            // Cập nhật danh sách ảnh
            product.ProductImgs = request.ProductImgs?.Select(img => new ProductImg
            {
                LinkImg = img.LinkImg,
                Description = img.Description
            }).ToList() ?? new List<ProductImg>();
        }
    }
}
