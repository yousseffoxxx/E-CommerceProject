﻿using AutoMapper;
using Domain.Contracts;
using Domain.Entities.BasketEntities;
using Domain.Exceptions;
using Services.Abstractions;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BasketService(IBasketRepository basketRepository , IMapper mapper)
        : IBasketService
    {
        public async Task<bool> DeleteBasketAsync(string id)
            => await basketRepository.DeleteBasketAsync(id);

        public async Task<BasketDTO?> GetBasketAsync(string id)
        {
            var basket = await basketRepository.GetBasketAsync(id);

            return basket is null ? throw new BasketNotFoundException(id) 
                : mapper.Map<BasketDTO>(basket);
        }

        public async Task<BasketDTO?> UpdateBasketAsync(BasketDTO basket)
        {
            var customerBasket = mapper.Map<CustomerBasket>(basket);

            var updatedBasket = await basketRepository.UpdateBasketAsync(customerBasket);
            
            return updatedBasket is null ? 
                throw new Exception("Can't Update Basket Now")
                : mapper.Map<BasketDTO>(updatedBasket);
        }
    }
}