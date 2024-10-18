﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.ProductEntities;

namespace Persistence.Data.Configuration
{
    internal class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasOne(product => product.ProductBrand)
                .WithMany()
                .HasForeignKey(product => product.BrandId);

            builder.HasOne(product => product.ProductType)
                .WithMany()
                .HasForeignKey(product => product.TypeId);

            builder.Property(product=> product.Price)
                .HasColumnType("decimal(18,3)");
        }
    }
}
