﻿using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Shared;
using static System.Net.WebRequestMethods;

namespace Services.Mapping_Profiles
{
    public class PictureUrlResolver(IConfiguration configuration) : IValueResolver<Product, ProductResultDTO, string>
    {
        public string Resolve(Product source, ProductResultDTO destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(source.PictureUrl)) return string.Empty;

            return $"{configuration["BaseUrl"]}{source.PictureUrl}";
        }
    }
}