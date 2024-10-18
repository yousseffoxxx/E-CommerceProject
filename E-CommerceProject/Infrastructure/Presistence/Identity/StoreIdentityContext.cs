﻿using Domain.Entities.UserEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Persistence.Identity
{
    public class StoreIdentityContext : IdentityDbContext
    {
        public StoreIdentityContext(DbContextOptions options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Address>().ToTable("Addresses");
        }
    }
}
