﻿using Project_BDS.Application.Payloads.RequestModels.ProductRequests;
using Project_BDS.Application.Payloads.Responses;
using Project_BDS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Application.InterfaceService
{
    public interface IProductImgService
    {
        Task<ResponseObject<bool>> AddProductImageAsync(ProductImgRequest request);
        Task<ResponseObject<List<ProductImg>>> GetAllProductImagesAsync();
        Task<ResponseObject<List<ProductImg>>> GetProductImagesAsync(int productId);
        Task<ResponseObject<bool>> UpdateProductImageAsync(int imageId, ProductImgRequest request);
        Task<ResponseObject<bool>> DeleteProductImageAsync(int imageId);
    }
}
