﻿using Project_BDS.Application.Payloads.Response_Models.DataProduct;
using Project_BDS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Application.Payloads.Mappers
{
    public class ProductImgConverter
    {
        public static DataResponseProductImg EntityToDTO(ProductImg productImg)
        {
            return new DataResponseProductImg
            {
                Id = productImg.Id,
                ProductId = productImg.ProductId,
                LinkImg = productImg.LinkImg,
                Description = productImg.Description
            };
        }
    }
}