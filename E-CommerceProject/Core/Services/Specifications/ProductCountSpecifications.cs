﻿using Domain.Contracts;
using Domain.Entities;
using Shared;

namespace Services.Specifications
{
    internal class ProductCountSpecifications : Specifications<Product>
    {
        public ProductCountSpecifications(ProductSpecificationsParameters parameters)
            : base(product =>
            (!parameters.BrandId.HasValue || product.BrandId == parameters.BrandId.Value) &&
            (!parameters.TypeId.HasValue || product.TypeId == parameters.TypeId.Value))
        {

        }
    }
}