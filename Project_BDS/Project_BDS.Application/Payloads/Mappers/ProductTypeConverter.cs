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
    public class ProductTypeConverter
    {
        public static DataResponse_ProductType EntityToDTO(ProductType entity)
        {
            return new DataResponse_ProductType
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name
            };
        }

        public static ProductType DTOToEntity(Request_ProductType dto)
        {
            return new ProductType
            {
                Code = dto.Code,
                Name = dto.Name
            };
        }
    }
}
